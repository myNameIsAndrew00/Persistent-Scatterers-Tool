using MapWebSite.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace MapWebSite.Interaction
{
    using Coordinates = Tuple<
                Tuple<decimal, decimal>,
                Tuple<decimal, decimal>>
                ;
    /// <summary>
    /// A cache which manages the points
    /// </summary>
    internal class PointsCacheManager
    {
        internal static void Write(Tuple<decimal,decimal> from, Tuple<decimal,decimal> to, int datasetId, IEnumerable<BasicPoint> dataPoints)
        {
            string key = string.Format("{0}_{1}_{2}_{3}_{4}", from.Item1, from.Item2, to.Item1, to.Item2, datasetId);

            MemoryCache.Default.Add(key, dataPoints, DateTimeOffset.MaxValue);
        }


        internal static IEnumerable<BasicPoint> Get(Tuple<decimal, decimal> from, Tuple<decimal, decimal> to, int datasetId, out List<Coordinates> coordinates)
        {
            //get the area which contains the points
            //search in cache
            //if incomplete, return

            object cacheInfo = getCacheKeys(from, to, datasetId, out bool required);
            if(required)
            {
                coordinates = (List<Coordinates>)cacheInfo;
                return null;
            }

            var cacheKeys = (List<string>)cacheInfo;
            var result = new List<BasicPoint>();

            foreach (var cacheKey in cacheKeys)
            {
                result.AddRange((MemoryCache.Default.Get(cacheKey) as List<BasicPoint>).Where
                    (item => item.Latitude >= from.Item1 && 
                             item.Longitude >= from.Item2 &&
                             item.Latitude <= to.Item1 &&
                             item.Longitude <= to.Item2));
            }

            coordinates = null;
            return result;
        }


        #region Private

        private static object getCacheKeys(Tuple<decimal, decimal> from, Tuple<decimal, decimal> to, int datasetId, out bool required)
        {
            int fromLat = Convert.ToInt32(from.Item1);
            int fromLong = Convert.ToInt32(from.Item2);
            int toLat = Convert.ToInt32(to.Item1);
            int toLong = Convert.ToInt32(to.Item2);

            List<string> result = new List<string>();

            var requiredPoints = new List<Coordinates>();

            for (decimal i = fromLat; i < toLat; i += 0.5m)
                for (decimal j = fromLong; j < toLong; j += 0.5m)
                {
                    string key = string.Format("{0}_{1}_{2}_{3}_{4}", i, j, i + 0.5m, j + 0.5m, datasetId);
                    if (MemoryCache.Default.Contains(key))
                        result.Add(key);
                    else requiredPoints.Add(new Coordinates(
                                                new Tuple<decimal, decimal>(i,j),
                                                new Tuple<decimal, decimal>(i + 0.5m, j + 0.5m)));
                }

            required = requiredPoints.Count > 0;
            if (required) return requiredPoints;

            return result;                               
        }


        #endregion
    }
}
