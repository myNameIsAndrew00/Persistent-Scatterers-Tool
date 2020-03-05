using MapWebSite.Core.Database;
using MapWebSite.Core.DataPoints;
using MapWebSite.Model;
using MapWebSite.Repository;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Timers;

namespace MapWebSite.DataPointsParserService
{
    /// <summary>
    /// Represents a parser service which process csv files to create points in cassandra database. 
    /// Files are uploaded by users in a directory configured in app.config.
    /// The datasets header data exists in the database with the status 'Uploaded'.
    /// After the points are inserted in database, the status of the dataset is changed in 'Generated'.
    /// Any parsing exception will change the dataset status in 'Generate Fail'
    /// </summary>
    public partial class Parser : ServiceBase
    {

        private readonly IUserRepository userRepository = null;

        private readonly IDataPointsRepository dataPointsRepository = null;

        private readonly System.Timers.Timer timer = new System.Timers.Timer();

        private static object actionLock = new object();

        public Parser()
        {
            InitializeComponent();
            CassandraDataPointsRepository.Initialise();

            this.userRepository = new SQLUserRepository();
            this.dataPointsRepository = CassandraDataPointsRepository.Instance;

            if (!EventLog.Exists(ConfigurationManager.AppSettings["LogEntryName"]))
                EventLog.CreateEventSource(
                                ConfigurationManager.AppSettings["LogSourceName"],
                                ConfigurationManager.AppSettings["LogEntryName"]);

        }

        protected override void OnStart(string[] args)
        {

            int timerPeriod = Convert.ToInt32(ConfigurationManager.AppSettings["ServiceTimer"]) * 1000;

            logData("Service starting", EventLogEntryType.Information);
            logData($"Timer is set to {timerPeriod} miliseconds", EventLogEntryType.SuccessAudit);
            timer.Interval = timerPeriod;
            timer.Elapsed += ParserAction;
            timer.Enabled = true;
        }


        #region Private

        private void logData(string message, EventLogEntryType infoType)
        {
            using (EventLog eventLog = new EventLog(ConfigurationManager.AppSettings["LogEntryName"]))
            {
                eventLog.Source = ConfigurationManager.AppSettings["LogSourceName"];
                eventLog.WriteEntry(message, infoType);
            }
        }

        public void ParserAction(object sender, ElapsedEventArgs e)
        {
            lock (actionLock)
            {
                logData("Starting parsing method.", EventLogEntryType.Information);

                var userDirectories = Directory.GetDirectories(
                                      ConfigurationManager.AppSettings["PointsDatasetsCheckpointFolder"]);
                foreach (var directory in userDirectories)
                {
                    string username = Path.GetFileName(directory);
                    logData($"Starting to parse points for user {username}", EventLogEntryType.Information);

                    var unprocessedDirectories = Directory.GetDirectories(directory);

                    foreach (var unprocessedDirectory in unprocessedDirectories)
                    {
                        logData($"Starting to process the directory {unprocessedDirectory}", EventLogEntryType.Information);
                        try
                        {

                            processFile(Path.Combine(unprocessedDirectory,
                                                     ConfigurationManager.AppSettings["DataPointsSourceFileName"]),
                                        username,
                                        Path.GetFileName(unprocessedDirectory));

                            string processedDirectory = Path.Combine(ConfigurationManager.AppSettings["PointsDatasetsParsedFolder"],
                                                                     username,
                                                                     Path.GetFileName(unprocessedDirectory));

                            if (!Directory.Exists(Path.GetDirectoryName(processedDirectory)))
                                Directory.CreateDirectory(Path.GetDirectoryName(processedDirectory));

                            Directory.Move(unprocessedDirectory,
                                       processedDirectory);

                            logData($"Successufuly parsed directory {unprocessedDirectory}", EventLogEntryType.Information);
                        }
                        catch (Exception exception)
                        {
                            logData(
                                "Failed to parse: " + unprocessedDirectory + "\n" +
                                exception.Message + "\n" + exception.StackTrace, EventLogEntryType.Error);
                        }
                    }

                }
            }

        }

        private void processFile(string fileName, string username, string datasetName)
        {
            IDataPointsSource pointsSource = new TxtDataPointsSource();

            int datasetId = this.userRepository.GetDatasetID(username, datasetName);
            if (datasetId == -1) throw new Exception($"Failed to find / create dataset with the name {datasetName} for user {username} ");

            (pointsSource as TxtDataPointsSource).HeaderFile = ConfigurationManager.AppSettings["DataSetsHeaderFile"];
            (pointsSource as TxtDataPointsSource).DisplacementsFile = fileName;
            (pointsSource as TxtDataPointsSource).LatitudeZone = 'T'; //TODO: modify here. This can be read from database
            (pointsSource as TxtDataPointsSource).Zone = 35;          //TODO: modify here. This can be read from database

            decimal minimumLatitude = 90,
                    minimumLongitude = 180,
                    maximumLatitude = -90,
                    maximumLongitude = -180,
                    minimumHeight = decimal.MaxValue,
                    maximumHeight = decimal.MinValue,
                    minimumStdDev = decimal.MaxValue,
                    maximumStdDev = decimal.MinValue,
                    minimumDefRate = decimal.MaxValue,
                    maximumDefRate = decimal.MinValue;
            int sectionIndex = 1;

            ///The source file must be looped, because a full read in memory of the file could throw a OutOfMemoryException.
            ///This means that the file will be read and parsed in chunks and multiple datasets with the same ID will be created 
            ///and stored in repository.
            foreach (var dataset in pointsSource.CreateDataSet(datasetName, CoordinateSystem.Default))
            {

                if (dataset == null)
                {
                    logData("Failed to read the dataset", EventLogEntryType.Error);
                    this.userRepository.UpdateDatasetStatus(datasetName, DatasetStatus.GenerateFail, username);
                    //todo: cleanup the repository?
                    return;
                }
                dataset.ID = datasetId;

                IDataPointsRegionsSource regionSource = new PowerOfTwoRegionsSource();
                regionSource.GenerateRegions(dataset, sectionIndex++);

                logData($"Starting to parse an entry of {dataset.Points.Count()} points count", EventLogEntryType.Information);

                Task<bool> insertTask = this.dataPointsRepository.InsertPointsDataset(dataset);

                insertTask.Wait();

                logData("Finished to insert the current dataset", EventLogEntryType.Information);

                if (sectionIndex == 21)
                {

                }
                if (insertTask.IsFaulted || !insertTask.Result) throw new Exception("Failed to insert data in the database");

                minimumLatitude = Math.Min(minimumLatitude, dataset.MinimumLatitude ?? 90);
                maximumLatitude = Math.Max(maximumLatitude, dataset.MaximumLatitude ?? -90);
                minimumLongitude = Math.Min(minimumLongitude, dataset.MinimumLongitude ?? 180);
                maximumLongitude = Math.Max(maximumLongitude, dataset.MaximumLongitude ?? -180);

                minimumHeight = Math.Min(minimumHeight, dataset.MinimumHeight ?? minimumHeight);
                maximumHeight = Math.Max(maximumHeight, dataset.MaximumHeight ?? maximumHeight);
                minimumStdDev = Math.Min(minimumStdDev, dataset.MinimumStdDev ?? minimumStdDev);
                maximumStdDev = Math.Max(maximumStdDev, dataset.MaximumStdDev ?? maximumStdDev);
                minimumDefRate = Math.Min(minimumDefRate, dataset.MinimumDeformationRate ?? minimumDefRate);
                maximumDefRate = Math.Max(maximumDefRate, dataset.MaximumDeformationRate ?? maximumDefRate); ;

                //update the status of the dataset which has been processed
            }

            this.userRepository.UpdateDatasetLimits(datasetName,
                                                      username,
                                                      minimumLatitude,
                                                      minimumLongitude,
                                                      maximumLatitude,
                                                      maximumLongitude);

            this.userRepository.UpdateDatasetRepresentationLimits(datasetName,
                                                    username,
                                                    minimumHeight, 
                                                    maximumHeight, 
                                                    minimumDefRate, 
                                                    maximumDefRate,
                                                    minimumStdDev, 
                                                    maximumStdDev);

            this.userRepository.UpdateDatasetStatus(datasetName, DatasetStatus.Generated, username);




        }


        #endregion


    }
}
