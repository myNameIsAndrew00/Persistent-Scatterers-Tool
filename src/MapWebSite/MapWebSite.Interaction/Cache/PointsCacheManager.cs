using MapWebSite.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace MapWebSite.Domain
{
    using Coordinates = Tuple<
                Tuple<decimal, decimal>,
                Tuple<decimal, decimal>>;

    using PointsZonePair = Tuple<IEnumerable<PointBase>, string, bool>;
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

        internal static string GenerateKey(decimal fromLatitude, decimal fromLongitude, decimal toLatitude, decimal toLongitude, int datasetId)
                => string.Format("{0:0.00}_{1:0.00}_{2:0.00}_{3:0.00}_{4}", fromLatitude, fromLongitude,toLatitude, toLongitude, datasetId);


        /// <summary>
        /// Update an existing entry in the cache
        /// </summary>
        /// <param name="from">Start position (Latitude and longitude) of the area which contains data points</param>
        /// <param name="to">End position (Latitude and longitude) of the area which contains data points</param>
        /// <param name="datasetId">Id of the dataset which contains the points</param>
        /// <param name="dataPoints">Datapoints which must be cached</param>
        /// <returns>A string which represents the key generated for the cached zone</returns>
        internal static string Write(Tuple<decimal, decimal> from, Tuple<decimal, decimal> to, int datasetId, IEnumerable<PointBase> dataPoints)
        {
            string key = GenerateKey(from.Item1, from.Item2, to.Item1, to.Item2, datasetId);
            MemoryCache.Default.Set(key,
                new CacheEntry
                {
                    Status = EntryStatus.Created,
                    Value = dataPoints
                },
                DateTimeOffset.MaxValue);
            return key;
        }

        /// <summary>
        /// Create a new empty entry in the cache
        /// </summary>
        /// <param name="from">Start position (Latitude and longitude) of the area which contains data points</param>
        /// <param name="to">End position (Latitude and longitude) of the area which contains data points</param>
        /// <param name="datasetId">Id of the dataset which contains the points</param>
        internal static void Create(Tuple<decimal, decimal> from, Tuple<decimal, decimal> to, int datasetId)
        {
            string key = GenerateKey(from.Item1, from.Item2, to.Item1, to.Item2, datasetId);
            
            MemoryCache.Default.Add(key,
                             new CacheEntry()
                             {
                                 Status = EntryStatus.Creating,
                                 Value = null
                             },
                             DateTimeOffset.MaxValue);
        }

        /// <summary>
        /// Remove a entry from the cache
        /// </summary>
        /// <param name="from">Start position (Latitude and longitude) of the area which contains data points</param>
        /// <param name="to">End position (Latitude and longitude) of the area which contains data points</param>
        /// <param name="datasetId">Id of the dataset which contains the points</param>
        internal static void Remove(Tuple<decimal, decimal> from, Tuple<decimal, decimal> to, int datasetId)
        {
            string key = GenerateKey(from.Item1, from.Item2, to.Item1, to.Item2, datasetId);
            MemoryCache.Default.Remove(key);
        }


        /// <summary>
        /// Get the points which are contained in a interval and the key of the region in which points are contained.
        /// If the requested points are not in cache, they will not be returned
        /// </summary>
        /// <param name="from">Starting position for requested interval</param>
        /// <param name="to">End position for requested interval</param>
        /// <param name="datasetId">Id of the dataset which contains the points</param>
        /// <param name="coordinates">A parameter which contains which region was not found in the cache</param>
        /// <returns></returns>
        internal static IEnumerable<PointsZonePair> Get(Tuple<decimal, decimal> from, 
                                                        Tuple<decimal, decimal> to, 
                                                        int datasetId,
                                                        Dictionary<string,int> regionsPointsCount,
                                                        out List<Coordinates> coordinates)
        {         
            List<string> cacheInfo = getCacheKeys(from, to, datasetId, out List<Coordinates> requiredPoints);

            var result = new List<PointsZonePair>();

            foreach (var cacheKey in cacheInfo)
            {
                CacheEntry cacheEntry = null;
                int attempts = 10;
                do //busyWaiting
                {
                    attempts--;
                    if (cacheEntry != null) Task.Delay(25);
                    cacheEntry = (MemoryCache.Default.Get(cacheKey)) as CacheEntry;                    
                }
                while ((MemoryCache.Default.Get(cacheKey) as CacheEntry)?.Status == EntryStatus.Creating
                    && attempts >= 0 );

                var cacheEntryValue = cacheEntry?.Value as IEnumerable<PointBase>;


                //if the entry is not valid or if the points where already fully requested, ignore the entry
                if (cacheEntryValue == null || 
                   (regionsPointsCount.Keys.Contains(cacheKey) && cacheEntryValue.Count() == regionsPointsCount[cacheKey]))                  
                    continue; 

                IEnumerable<PointBase> points = (cacheEntryValue).Where
                    (item => item.Latitude >= from.Item1 &&
                             item.Longitude >= from.Item2 &&
                             item.Latitude <= to.Item1 &&
                             item.Longitude <= to.Item2);

                result.Add(new PointsZonePair(points, cacheKey, points.Count() == cacheEntryValue.Count()));
            }

            coordinates = requiredPoints.Count > 0 ? requiredPoints : null;
            return result;
        }


        #region Private

        private static List<string> getCacheKeys(Tuple<decimal, decimal> from, Tuple<decimal, decimal> to, int datasetId, out List<Coordinates> requiredPoints)
        {
            getCacheSearchingLimits(from,
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
                        //TODO: check if region exists. A new entry must be added in database
                        !(i >= 44m &&
                        i <= 45m &&
                        j >= 28 &&
                        j <= 30m))
                        continue;

                    string key = GenerateKey(i, j, i + latitudeSide, j + longitudeSide, datasetId);
                    var cacheValue = MemoryCache.Default.Get(key);

                    //if the key was not found as entry in cache, request the points and create an entry
                    //which is identified as 'creating'
                    if (cacheValue == null )
                    {
                        var startCoordinates = new Tuple<decimal, decimal>(i, j);
                        var endCoordinates = new Tuple<decimal, decimal>(i + latitudeSide, j + longitudeSide);
                        requiredPoints.Add(new Coordinates( startCoordinates, endCoordinates));

                        PointsCacheManager.Create(startCoordinates, endCoordinates, datasetId);
                    }
                    else result.Add(key);
                }

            return result;
        }

        private static void getCacheSearchingLimits(
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
            toLong = Convert.ToDecimal((toLongInt + (longSideInt - toLongInt % longSideInt))) / 100;

        }

        #endregion
    }
}
