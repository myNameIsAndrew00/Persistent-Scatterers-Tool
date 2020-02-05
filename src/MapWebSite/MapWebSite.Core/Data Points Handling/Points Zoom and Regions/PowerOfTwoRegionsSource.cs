using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapWebSite.Model;

namespace MapWebSite.Core.DataPoints
{
    /// <summary>
    /// This class can generate regions of points based on existing data points set.
    /// The algorithm used is based on dividing the world map ( (-180) - (180) degrees longitude, 
    ///                                                         (-90) - (90) degrees latitude )
    /// on rectangles with the height equals with 180 / 2^zoomLevel and width equals with 360 / 2^zoomLevel                                                         
    /// </summary>
    public class PowerOfTwoRegionsSource : IDataPointsRegionsSource
    {
        private struct Coordinate
        {
            public decimal Latitude;
            public decimal Longitude;

            public int RowIndex;
            public int ColumnIndex;

            public Coordinate(decimal latitude, decimal longitude)
            {
                Latitude = latitude;
                Longitude = longitude;
                RowIndex = 0;
                ColumnIndex = 0;
            }

        }
         

        private Coordinate bottomLeftCorner;

        private Coordinate topRightCorner;



        public bool GenerateRegions(PointsDataSet pointsDataset, int sectionIndex)
        {
            try
            {
                pointsDataset.PointsRegions = this.generateRegions(pointsDataset.Points, sectionIndex);
                return true;
            }
            catch
            {
                return false;
            }

        }

        public IEnumerable<PointsRegionsLevel> GenerateRegions(IEnumerable<PointBase> dataPoints, int sectionIndex = 0)
        {
            return this.generateRegions(dataPoints, sectionIndex);
        }


        public Tuple<decimal, decimal> MapCoordinateToRegionCorner(decimal latitude, decimal longitude, int zoomLevel)
        {
            Coordinate coordinate = this.mapToRegionCorner(new Coordinate(latitude, longitude), zoomLevel);

            return new Tuple<decimal, decimal>(coordinate.Latitude, coordinate.Longitude);
        }

        public Tuple<int, int> GetRegionIndexes(decimal latitude, decimal longitude, int zoomLevel)
        {
            Coordinate coordinate = this.mapToRegionCorner(new Coordinate(latitude, longitude), zoomLevel);

            return new Tuple<int, int>(coordinate.RowIndex, coordinate.ColumnIndex);
        }

        #region Private

        IEnumerable<PointsRegionsLevel> generateRegions(IEnumerable<PointBase> dataPoints, int sectionIndex)
        {

            List<PointsRegionsLevel> result = new List<PointsRegionsLevel>();


            bottomLeftCorner = new Coordinate(
                latitude: (dataPoints.Min(point => point.Latitude) - 0.00001m),
                longitude: (dataPoints.Min(point => point.Longitude) - 0.00001m));

            topRightCorner = new Coordinate(
                latitude: (dataPoints.Max(point => point.Latitude) - 0.00001m),
                longitude: (dataPoints.Max(point => point.Longitude) - 0.00001m));

            dataPoints = dataPoints.OrderBy(item => item.Latitude).ThenBy(item => item.Longitude);

            //get the base regions (zoom level 20)
            PointsRegionsLevel baseRegion = generateBaseRegions(dataPoints, sectionIndex);

            result.Add(baseRegion);

            for (int i = 19; i >= 3; i--)
                result.Add(generateIntermediateRegions(baseRegion, i));

            return result;
        }

        PointsRegionsLevel generateBaseRegions(IEnumerable<PointBase> sortedDataPoints, int sectionIndex)
        {
            List<PointsRegion> regions = new List<PointsRegion>();


            PointBase[] sortedPoints = sortedDataPoints.ToArray();

            List<PointBase> pointsList = new List<PointBase>();

            //get the size of the rectangle when zoom level is 20
            Coordinate rectangleSize = getRectangleSize(20);
            Coordinate bottomLeftCorner = this.mapToRegionCorner(this.bottomLeftCorner, 20);
            Coordinate topRightCorner = this.mapToRegionCorner(this.topRightCorner, 20);

            Coordinate coordinateIndex = this.mapToRegionCorner(this.bottomLeftCorner, 20);
              
            int sortedPointsIndex = 0;

            
            while (coordinateIndex.Latitude < topRightCorner.Latitude)
            {
                SortedList<decimal, PointBase> sameLatitudePoints = new SortedList<decimal, PointBase>(new DuplicateKeyComparer<decimal>());

                while (sortedPointsIndex < sortedPoints.Length && sortedPoints[sortedPointsIndex].Latitude < coordinateIndex.Latitude + rectangleSize.Latitude)
                {
                    sameLatitudePoints.Add(sortedPoints[sortedPointsIndex].Longitude, sortedPoints[sortedPointsIndex]);
                    sortedPointsIndex++;
                }

                while (coordinateIndex.Longitude < topRightCorner.Longitude)
                {
                    var areaPoints = sameLatitudePoints.TakeWhile(item => item.Key < coordinateIndex.Longitude + rectangleSize.Longitude);
                    if (areaPoints.Count() > 0)
                    {
                        regions.Add(new PointsRegion()
                        {
                            Points = areaPoints.Select(item => item.Value).ToList(),
                            Column = coordinateIndex.ColumnIndex, 
                            Row = coordinateIndex.RowIndex   
                        });
                    }

                    int areaPointsCount = areaPoints?.Count() ?? 0;
                    for (int deleteCounter = 0; deleteCounter < areaPointsCount; deleteCounter++)
                        sameLatitudePoints.RemoveAt(0);
                    if (sameLatitudePoints.Count == 0 || sortedPointsIndex == sortedPoints.Length) break;

                    coordinateIndex = this.mapToRegionCorner(new Coordinate(sameLatitudePoints.First().Value.Latitude, sameLatitudePoints.First().Value.Longitude), 20);
 
                }

                if (sortedPointsIndex == sortedPoints.Length) break;

                coordinateIndex = this.mapToRegionCorner(new Coordinate(sortedPoints[sortedPointsIndex].Latitude, 
                                                                        this.bottomLeftCorner.Longitude),
                                                             20);                 
            }


            return new PointsRegionsLevel()
            {
                Regions = regions,
                Section = sectionIndex,
                ZoomLevel = 20
            };
        }

        private PointsRegionsLevel generateIntermediateRegions(PointsRegionsLevel baseRegions, int zoomLevel)
        {
            Dictionary<Tuple<int,int>, PointsRegion> pointsRegions = new Dictionary<Tuple<int, int>, PointsRegion>();

            int maxRegionPointsCount = this.getRegionPointsMaxCount(zoomLevel);

            foreach(var baseRegion in baseRegions.Regions)
            {                    
                var baseListPoints = baseRegion.Points as List<PointBase>;
                                
                var key = new Tuple<int, int>(baseRegion.Row / (1 << (20 - zoomLevel)),
                                              baseRegion.Column / (1 << ( 20 - zoomLevel)) );

                if (!pointsRegions.ContainsKey(key)) pointsRegions.Add(key, 
                    new PointsRegion() { 
                        Points = new List<PointBase>(),
                        Row = key.Item1,
                        Column = key.Item2,
                    });

                (pointsRegions[key].Points as List<PointBase>).AddRange(baseListPoints);
            }

            Parallel.ForEach(pointsRegions.Values, pointsRegion => {
                int count = pointsRegion.Points.Count();

                if (count <= maxRegionPointsCount) return;

                // exceed = (count - this.maxRegionPointsCount);
                Random random = new Random();
               

                var points = pointsRegion.Points as List<PointBase>;

                while (pointsRegion.Points.Count() > maxRegionPointsCount)
                {
                    int index = random.Next(0, pointsRegion.Points.Count() - 1);
                    points.RemoveAt(index);                    
                } 
                    
            });

            return new PointsRegionsLevel()
            {
                ZoomLevel = zoomLevel,
                Regions = pointsRegions.Values
            };
        }

        private Coordinate getRectangleSize(int zoomLevel)
        { 
            return new Coordinate(180m / (1 << zoomLevel), 360m / (1 << zoomLevel));
        }

        private Coordinate mapToRegionCorner(Coordinate coordinate, int zoomLevel)
        {          
            Coordinate rectangleSize = getRectangleSize(zoomLevel);

            var indexes = getRegionIndexes(coordinate, rectangleSize, zoomLevel);
            
            return new Coordinate()
            {
                Latitude = 90 - indexes.Item1 * rectangleSize.Latitude,
                Longitude = indexes.Item2 * rectangleSize.Longitude - 180,
                RowIndex = indexes.Item1,
                ColumnIndex = indexes.Item2
            };
        }

        private Tuple<int, int> getRegionIndexes(Coordinate coordinate, Coordinate rectangleSize, int zoomLevel)
        {
            if (zoomLevel == 0) return new Tuple<int, int>(0, 0);

            int row = (1 << zoomLevel) - (int)Math.Floor((coordinate.Latitude + 90) / rectangleSize.Latitude + 0.1m) - 1;
            int column = (int)Math.Floor((coordinate.Longitude - 0.00000000001m  + 180) / rectangleSize.Longitude);

            return new Tuple<int, int>( row < 0 ? 0 : row, column < 0 ? 0 : column);
        }

        private int getRegionPointsMaxCount(int zoomLevel)
        {
            switch (zoomLevel)
            {
                case 19: 
                case 18:
                case 17: 
                case 16: return 500;
                case 15: return 400;
                case 14: return 350;
                case 13: return 250;
                case 12: return 150;
                case 11: return 100;
                case 10: return 70;
                case 9: return 60;
                case 8: return 50;
                case 6: return 40;
                case 5: return 30;
                case 4: return 20;
                case 3: return 10;
                case 2: return 5;
                default : return 1;
            }
        }


        #endregion
    }

}
