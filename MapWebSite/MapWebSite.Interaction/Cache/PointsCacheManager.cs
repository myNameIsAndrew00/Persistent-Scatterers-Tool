using MapWebSite.Model;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Cache flow:
    ///     -user asks for a set of points starting from <startLatitude><startLongitude> to <endLatitude><endLongitude>
    ///     -find the regions which contains the points
    ///     -if the regions are contained in cache with the status 'Created' then return the datapoints
    ///     -if the regions are not contained in cache, request them, and create an entry in cache with status 'Creating'
    ///     -after the points are created, the cache entry is updated and the status changes from 'Creating' to 'Created'
    /// </summary>
    internal class PointsCacheManager
    {
        const decimal latitudeSide = 0.05m;
        const decimal longitudeSide = 0.10m;

        internal static void Write(Tuple<decimal, decimal> from, Tuple<decimal, decimal> to, int datasetId, IEnumerable<BasicPoint> dataPoints)
        {
            string key = string.Format("{0}_{1}_{2}_{3}_{4}", from.Item1, from.Item2, to.Item1, to.Item2, datasetId);

            MemoryCache.Default.Set(key,
                new CacheEntry
                {
                    Status = EntryStatus.Created,
                    Value = dataPoints
                },
                DateTimeOffset.MaxValue);
        }


        /// <summary>
        /// Get the points which are contained in a interval. If the requested points are not in cache, they will not be returned
        /// </summary>
        /// <param name="from">Starting position for requested interval</param>
        /// <param name="to">End position for requested interval</param>
        /// <param name="datasetId">Id of the dataset which contains the points</param>
        /// <param name="coordinates">A parameter which contains which region was not found in the cache</param>
        /// <returns></returns>
        internal static IEnumerable<BasicPoint> Get(Tuple<decimal, decimal> from, Tuple<decimal, decimal> to, int datasetId, out List<Coordinates> coordinates)
        {         
            List<string> cacheInfo = getCacheKeys(from, to, datasetId, out List<Coordinates> requiredPoints);

            var result = new List<BasicPoint>();

            foreach (var cacheKey in cacheInfo)
            {
                CacheEntry cacheValue = null;
                int attempts = 10;
                do //busyWaiting
                {
                    attempts--;
                    if (cacheValue != null) Task.Delay(25);
                    cacheValue = (MemoryCache.Default.Get(cacheKey)) as CacheEntry;                    
                }
                while ((MemoryCache.Default.Get(cacheKey) as CacheEntry).Status == EntryStatus.Creating
                    && attempts >= 0 );

                if (cacheValue.Value == null)                   
                    continue; 

                result.AddRange((cacheValue.Value as IEnumerable<BasicPoint>)?.Where
                    (item => item.Latitude >= from.Item1 &&
                             item.Longitude >= from.Item2 &&
                             item.Latitude <= to.Item1 &&
                             item.Longitude <= to.Item2));
            }

            coordinates = requiredPoints.Count > 0 ? requiredPoints : null;
            return result;
        }


        #region Private

        private static List<string> getCacheKeys(Tuple<decimal, decimal> from, Tuple<decimal, decimal> to, int datasetId, out List<Coordinates> requiredPoints)
        {
            getCacheSearhingLimits(from,
                                   to,
                                   out decimal fromLat,
                                   out decimal fromLong,
                                   out decimal toLat,
                                   out decimal toLong);
        
            List<string> result = new List<string>();

            requiredPoints = new List<Coordinates>();

            for (decimal i = fromLat; i <= toLat; i += latitudeSide)
                for (decimal j = fromLong; j <= toLong; j += longitudeSide)
                {
                    if (
                        //TODO: check if region exists
                        !(i >= 44m &&
                        i <= 45m &&
                        j >= 28 &&
                        j <= 30m))
                        continue;

                    string key = string.Format("{0}_{1}_{2}_{3}_{4}", i, j, i + latitudeSide, j + longitudeSide, datasetId);
                    var cacheValue = MemoryCache.Default.Get(key);

                    //if the key was not found as entry in cache, request the points and create an entry
                    //which is identified as 'creating'
                    if (cacheValue == null )
                    {
                        requiredPoints.Add(new Coordinates(
                                                new Tuple<decimal, decimal>(i, j),
                                                new Tuple<decimal, decimal>(i + latitudeSide, j + longitudeSide)));
                        MemoryCache.Default.Add(key,
                                new CacheEntry()
                                {
                                    Status = EntryStatus.Creating,
                                    Value = null
                                },
                                DateTimeOffset.MaxValue);
                    }
                    else result.Add(key);
                }

            return result;
        }

        private static void getCacheSearhingLimits(
            Tuple<decimal,decimal> from,
            Tuple<decimal,decimal> to,
            out decimal fromLat, 
            out decimal fromLong, 
            out decimal toLat,
            out decimal toLong)
        {
            int latSideInt = decimal.ToInt32(latitudeSide * 100);
            int longSideInt = decimal.ToInt32(longitudeSide * 100);

            int fromLatInt = decimal.ToInt32(from.Item1 * 100);
            int fromLongInt = decimal.ToInt32(from.Item2 * 100);
            int toLatInt = decimal.ToInt32(to.Item1 * 100);
            int toLongInt = decimal.ToInt32(to.Item1 * 100);

            fromLat = Convert.ToDecimal((fromLatInt - fromLatInt % latSideInt)) / 100;
            fromLong = Convert.ToDecimal((fromLongInt - fromLongInt % longSideInt)) / 100;
            toLat = Convert.ToDecimal((toLatInt + (latSideInt - toLatInt % latSideInt))) / 100;
            toLong = decimal.ToInt32((toLongInt + (longSideInt - toLongInt % longSideInt))) / 100;

        }

        #endregion
    }
}
