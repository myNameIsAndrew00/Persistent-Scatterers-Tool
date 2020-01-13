using MapWebSite.Core.Database;
using MapWebSite.Core.DataPoints;
using MapWebSite.Model;
using MapWebSite.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace MapWebSite.DataPointsParserService
{
    /// <summary>
    /// Represents a parser service which process csv files to create points in cassandra database. 
    /// Files are uploaded by users in a directory configured in app.config.
    /// The datasets header data exists in the database with the status 'Uploaded'.
    /// After the points are inserted in database, the status of the dataset is changed in 'Generated'.
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

            this.userRepository = new SQLUserRepository();
            this.dataPointsRepository = CassandraDataPointsRepository.Instance;

            if (!EventLog.Exists(ConfigurationManager.AppSettings["LogEntryName"]))
                EventLog.CreateEventSource(
                                ConfigurationManager.AppSettings["LogSourceName"],
                                ConfigurationManager.AppSettings["LogEntryName"]);

        }

        protected override void OnStart(string[] args)
        {
            logData("Service starting", EventLogEntryType.Information);

            timer.Interval = Convert.ToInt32(ConfigurationManager.AppSettings["ServiceTimer"]) * 1000;
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
                var userDirectories = Directory.GetDirectories(
                                      ConfigurationManager.AppSettings["PointsDatasetsCheckpointFolder"]);
                foreach (var directory in userDirectories)
                {
                    string username = Path.GetFileName(directory);
                    logData($"Starting to parse points for user {username}", EventLogEntryType.Information);

                    var unprocessedDirectories = Directory.GetDirectories(directory);

                    foreach (var unprocessedDirectory in unprocessedDirectories)
                    {
                        try
                        {
                             
                            processFile(Path.Combine(unprocessedDirectory,
                                                     ConfigurationManager.AppSettings["DataPointsSourceFileName"]),
                                        username,
                                        Path.GetDirectoryName(unprocessedDirectory));
                            
                            string processedDirectory = Path.Combine(ConfigurationManager.AppSettings["PointsDatasetsParsedFolder"], 
                                                                     username,
                                                                     Path.GetFileName(unprocessedDirectory));
 

                            Directory.Move(unprocessedDirectory,
                                           processedDirectory);
                        }
                        catch (Exception exception)
                        {
                            logData(exception.Message, EventLogEntryType.Error);                            
                        }
                    }

                }
            }

        }

        private void processFile(string fileName, string username, string datasetName)
        {
            IDataPointsSource pointsSource = new TxtDataPointsSource();

            (pointsSource as TxtDataPointsSource).HeaderFile = ConfigurationManager.AppSettings["DataSetsHeaderFile"];
            (pointsSource as TxtDataPointsSource).DisplacementsFile = fileName;
            (pointsSource as TxtDataPointsSource).LatitudeZone = 'T'; //TODO: modify here. This can be read from database
            (pointsSource as TxtDataPointsSource).Zone = 35;          //TODO: modify here. This can be read from database

            PointsDataSet dataset = pointsSource.CreateDataSet(datasetName);

            //get the id of the dataset which has been processed
            dataset.ID = this.userRepository.GetDatasetID(username, dataset.Name);
            if (dataset.ID == -1) throw new Exception($"Failed to find create / dataset with the name {dataset.Name} for user {username} ");

            IDataPointsZoomLevelsGenerator zoomGenerator = new SquareMeanPZGenerator();
            PointsDataSet[] zoomedDataSets = zoomGenerator.CreateDataSetsZoomSets(dataset, 3, 19);

            Task insertTask = this.dataPointsRepository.InsertPointsDatasets(dataset, zoomedDataSets);

            insertTask.Wait();

            //update the status of the dataset which has been processed
            this.userRepository.UpdateDatasetStatus(dataset.Name, DatasetStatus.Generated, username);
        }


        #endregion
    }
}
