using System;
using System.Collections.Generic;
using System.Linq;
using MapWebSite.Model;
using MapWebSite.Core;
using System.Threading.Tasks;

namespace MapWebSite.Core.DataPoints
{
     
    /// <summary>
    /// Use this class to generate zoom levels points for a certain DataSet of Points
    /// This class create points dividing provided points in certains squares of fixed dimension
    /// </summary>
    public class SquareMeanPZGenerator : IDataPointsZoomLevelsGenerator
    {
        private struct Coordinate
        {
            public decimal Latitude;
            public decimal Longitude;

            public Coordinate(decimal latitude, decimal longitude)
            {
                Latitude = latitude;
                Longitude = longitude;
            }

        }

        private Coordinate bottomLeftCorner;

        private Coordinate topRightCorner;


        public PointsDataSet[] CreateDataSetsZoomSets(PointsDataSet originalDataSet, int minZoomLevel, int maxZoomLevel)
        {

            PointsDataSet[] result = new PointsDataSet[maxZoomLevel - minZoomLevel + 1];

            bottomLeftCorner = new Coordinate(
                latitude: (originalDataSet.Points.Min(point => point.Latitude) - 0.00001m),
                longitude: (originalDataSet.Points.Min(point => point.Longitude) - 0.00001m));

            topRightCorner = new Coordinate(
                latitude: (originalDataSet.Points.Max(point => point.Latitude) - 0.00001m),
                longitude: (originalDataSet.Points.Max(point => point.Longitude) - 0.00001m));

            originalDataSet.Points = originalDataSet.Points.OrderBy(item => item.Latitude).ThenBy(item => item.Longitude);

            Parallel.ForEach(result, (dataSet, state, index) =>
                result[ index ] = createDataset(originalDataSet, index + minZoomLevel)
            );
            
            return result;             
        }



        #region Private

        /// <summary>
        /// This algorithm create a dataset with reduced number of points, depending of the zoom Level.<br></br>
        /// Algorithm:<br></br> 
        ///            -1.sort the original dataset acording to its latitude.<br></br>
        ///            -2.take chunks of points which are contained in an side of rectangle of a specific dimension starting at a specific point *see notes to understand this<br></br>
        ///            -3.sort the chukn acording to points longitude<br></br>
        ///            -4.repeat the step 2, this time slice the rectangle in squares<br></br>
        ///            -5.take the chunk from 4, and build a point base on it. Add the point into the result
        /// </summary>
        /// <param name="sortedPointsDataset"></param>
        /// <param name="zoomLevel"></param>
        /// <returns></returns>
        private PointsDataSet createDataset(PointsDataSet sortedPointsDataset, long zoomLevel)
        {
            PointsDataSet result = new PointsDataSet()
            {
                ID = sortedPointsDataset.ID,
                Name = sortedPointsDataset.Name,
                ZoomLevel = zoomLevel
            };

            var number = 0;
            Point[] sortedPoints = sortedPointsDataset.Points.ToArray();
            List<Point> pointsList = new List<Point>();
            
            decimal squareSide = getSquareSide(zoomLevel);      
            decimal latitudeIndex = bottomLeftCorner.Latitude;
            int sortedPointsIndex = 0;


            while(latitudeIndex < topRightCorner.Latitude)
            {
                SortedList<decimal, Point> sameLatitudePoints = new SortedList<decimal, Point>(new DuplicateKeyComparer<decimal>());

                while ( sortedPointsIndex < sortedPoints.Length && sortedPoints[sortedPointsIndex].Latitude <= latitudeIndex + squareSide) {
                    sameLatitudePoints.Add(sortedPoints[sortedPointsIndex].Longitude, sortedPoints[sortedPointsIndex]);
                    sortedPointsIndex++;
                }

                decimal longitudeIndex = bottomLeftCorner.Longitude;
                while (longitudeIndex < topRightCorner.Longitude) {
                    var areaPoints = sameLatitudePoints.TakeWhile(item => item.Key < longitudeIndex);
                    if(areaPoints.Count() > 0)
                        pointsList.Add(new Point()
                        {
                            DeformationRate = areaPoints.Average(item => item.Value.DeformationRate),
                            Displacements = null,
                            EstimatedDeformationRate = areaPoints.Average(item => item.Value.EstimatedDeformationRate),
                            EstimatedHeight = areaPoints.Average(item => item.Value.EstimatedHeight),
                            Height = areaPoints.Average(item => item.Value.Height),
                            Latitude = latitudeIndex + squareSide / 2,
                            Longitude = longitudeIndex + squareSide / 2,
                            Number = number++,
                            StandardDeviation = areaPoints.Average(item => item.Value.StandardDeviation),
                            Observations = "Generated with mean square algorithm"
                        });
                    int areaPointsCount = areaPoints?.Count() ?? 0;
                    for(int deleteCounter = 0; deleteCounter < areaPointsCount; deleteCounter++)
                        sameLatitudePoints.RemoveAt(0);
                    if (sameLatitudePoints.Count == 0 || sortedPointsIndex == sortedPoints.Length) break;

                    longitudeIndex = squareSide * ((int)(sameLatitudePoints.First().Key / squareSide)) + squareSide;
                }

                if (sortedPointsIndex == sortedPoints.Length) break;
                        
                latitudeIndex = squareSide * ((int)(sortedPoints[sortedPointsIndex].Latitude / squareSide)) + squareSide;


            }
 

            result.Points = pointsList;                     
            return result;
        }


        private decimal getSquareSide(long zoomLevel)
        { 
            switch (zoomLevel)
            {
                case 19:
                    return 0.0002m;
                case 18:
                    return 0.0009m;
                case 17:
                    return 0.0025m;
                case 16:
                    return 0.0036m;
                case 15:
                    return 0.0064m;
                case 14:
                    return 0.0081m;
                case 13:
                    return 0.0121m;
                case 12:
                    return 0.0169m;
                case 11:
                    return 0.0225m;
                case 10:
                    return 0.0289m;
                case 9:
                    return 0.0361m;
                case 8:
                    return 0.0441m;
                case 7:
                    return 0.0529m;
                case 6:
                    return 0.0625m;
                case 5:
                    return 0.0729m;
                case 4:
                    return 0.0841m;
                case 3:
                    return 0.0961m;
                default:
                    return 1m;
            } 
        }

        #endregion
    }
}
