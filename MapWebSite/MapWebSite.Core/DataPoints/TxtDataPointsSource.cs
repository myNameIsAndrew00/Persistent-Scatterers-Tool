using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapWebSite.Model;

namespace MapWebSite.Core.DataPoints
{
    using HeaderData = Tuple<DateTime, float, float>;
    

    public class TxtDataPointsSource : IDataPointsSource
    {
        readonly int headerUnusedLinesCount  = 10;

        public string HeaderFile { get; set; } = null;

        public string DisplacementsFile { get; set; } = null;

        public PointsDataSet CreateDataSet(string datasetName)
        {
            if (HeaderFile == null || DisplacementsFile == null) return null;

            //List<Point> points = new List<Point>(0);

            ConcurrentBag<Point> points = new ConcurrentBag<Point>();
            PointsDataSet pointsDataSet = new PointsDataSet() { Name = datasetName, Points = points };

            try
            {
                HeaderData[] headerData = parseHeaderData();
                string[] displacementsTextLines = File.ReadAllLines(DisplacementsFile);
 
                Parallel.ForEach(displacementsTextLines, (dataLine) =>
                {
                    IDictionary<string, float> lineInfo = generateDictionary(dataLine, out float[] lineDisplacements);
                    points.Add(generatePoint(lineInfo, lineDisplacements, headerData));
                });
                                 
            }
            catch(Exception exception)
            {
                //TODO: parse exception here
                pointsDataSet = null;
            }
            return pointsDataSet;
        }




        #region Private

        private Point generatePoint(IDictionary<string, float> lineInfo, float[] lineDisplacements, HeaderData[] headerData)
        {
            Point point = new Point()
            {
                Displacements = new List<Displacement>(),
                Number       = Convert.ToInt32(lineInfo["PointNumber"]),
                DeformationRate              = lineInfo["DeformationRate"],
                EastingProjectionCoordinate  = lineInfo["EastingProjection"],
                EstimatedDeformationRate     = lineInfo["EstimatedDeformation"],
                EstimatedHeight              = lineInfo["EstimatedHeight"],
                Height                       = lineInfo["Height"],
                NorthingProjectionCoordinate = lineInfo["NorthingProjection"],
                ReferenceImageX              = lineInfo["ReferenceImageX"],
                ReferenceImageY              = lineInfo["ReferenceImageY"],
                StandardDeviation            = lineInfo["StandardDeviation"],
                Observations = null // TODO:add information to observations                
            };

            for (int index = 0; index < lineDisplacements.Length; index++)
                point.Displacements.Add(new Displacement()
                {   
                    Date = headerData[ index ].Item1,
                    JD = headerData[ index].Item2,
                    DaysFromReference = headerData[ index ].Item3,
                    Value = lineDisplacements[ index ]
                });

            return point;
        }

        private IDictionary<string, float> generateDictionary(string dataLine, out float[] displacements)
        {

            IDictionary<string, float> result = new ConcurrentDictionary<string, float>();
          
            var tokens = dataLine.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            float[] localDisplacements = new float[tokens.Length - headerUnusedLinesCount];

            Task[] tasks = new Task[11];
            //define assignments tasks to be realised in parallel
            tasks[0] = Task.Run(() => result["PointNumber"] = float.Parse(tokens[0]));
            tasks[1] = Task.Run(() => result["ReferenceImageX"] = float.Parse(tokens[1]));
            tasks[2] = Task.Run(() => result["ReferenceImageY"] = float.Parse(tokens[2]));
            tasks[3] = Task.Run(() => result["EastingProjection"] = float.Parse(tokens[3]));
            tasks[4] = Task.Run(() => result["NorthingProjection"] = float.Parse(tokens[4]));
            tasks[5] = Task.Run(() => result["Height"] = float.Parse(tokens[5]));
            tasks[6] = Task.Run(() => result["DeformationRate"] = float.Parse(tokens[6]));
            tasks[7] = Task.Run(() => result["StandardDeviation"] = float.Parse(tokens[7]));
            tasks[8] = Task.Run(() => result["EstimatedHeight"] = float.Parse(tokens[8]));
            tasks[9] = Task.Run(() => result["EstimatedDeformation"] = float.Parse(tokens[9]));

            tasks[10] = Task.Run(() =>{
                                for (int index = headerUnusedLinesCount; index < tokens.Length; index++)
                                          localDisplacements[index - headerUnusedLinesCount] = float.Parse(tokens[index]);
                                });
            Task.WaitAll(tasks);

            displacements = localDisplacements;
            return result;
        }

        private HeaderData[] parseHeaderData()
        {
            List<HeaderData> result = new List<HeaderData>();

            using (StreamReader streamReader = new StreamReader(HeaderFile))
            {
                //first 10 lines of the file are useles
                for (int i = 0; i < headerUnusedLinesCount; i++)
                    streamReader.ReadLine();

                string dataLine = streamReader.ReadLine();

                while (!string.IsNullOrEmpty(dataLine))
                {
                    /*parse the header file*/
                    var tokens = dataLine.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                  
                    var dateInfo = tokens[1].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var jDInfo = tokens[2].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var dayReferenceInfo = tokens[3].Trim();

                    result.Add(new HeaderData(
                        new DateTime(Convert.ToInt32(dateInfo[0]), Convert.ToInt32(dateInfo[1]), Convert.ToInt32(dateInfo[2])),
                        float.Parse(jDInfo[0]),
                        float.Parse(dayReferenceInfo)
                        ));

                    dataLine = streamReader.ReadLine();
                }
            }

            return result.ToArray();
        }


        #endregion
    }
}
