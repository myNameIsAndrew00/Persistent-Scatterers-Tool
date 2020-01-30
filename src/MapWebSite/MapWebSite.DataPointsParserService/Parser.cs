﻿using MapWebSite.Core.Database;
using MapWebSite.Core.DataPoints;
using MapWebSite.Model;
using MapWebSite.Repository;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
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
            if (datasetId == -1) throw new Exception($"Failed to find create / dataset with the name {datasetName} for user {username} ");

            (pointsSource as TxtDataPointsSource).HeaderFile = ConfigurationManager.AppSettings["DataSetsHeaderFile"];
            (pointsSource as TxtDataPointsSource).DisplacementsFile = fileName;
            (pointsSource as TxtDataPointsSource).LatitudeZone = 'T'; //TODO: modify here. This can be read from database
            (pointsSource as TxtDataPointsSource).Zone = 35;          //TODO: modify here. This can be read from database

            PointsDataSet dataset = pointsSource.CreateDataSet(datasetName);

            if (dataset == null)
            {
                this.userRepository.UpdateDatasetStatus(datasetName, DatasetStatus.GenerateFail, username);
                return;
            }
            dataset.ID = datasetId;

            //get the id of the dataset which has been processed

            //UPDATE:zoom is not required for now
            //IDataPointsZoomLevelsGenerator zoomGenerator = new SquareMeanPZGenerator();
            //PointsDataSet[] zoomedDataSets = zoomGenerator.CreateDataSetsZoomSets(dataset, 3, 19);

            IDataPointsRegionsSource regionSource = new PowerOfTwoRegionsSource();
            regionSource.GenerateRegions(dataset);

            Task insertTask = this.dataPointsRepository.InsertPointsDataset(dataset);

            insertTask.Wait();

            //update the status of the dataset which has been processed
            this.userRepository.UpdateDatasetStatus(dataset.Name, DatasetStatus.Generated, username);
        }


        #endregion
    }
}
