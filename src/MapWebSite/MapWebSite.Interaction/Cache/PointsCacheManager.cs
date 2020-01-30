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
    using Region = Tuple<int, int, int>;

    using PointsZonePair = Tuple<IEnumerable<PointBase>, string, bool>;
   
    /// <summary>
    /// A cache which manages the points
    /// Cache flow:
    ///     -user asks for a set of points starting from a row and column (based on zoom level)
    ///     -find the regions which contains the points
    ///     -if the regions are contained in cache with the status 'Created' then return the datapoints
    ///     -if the regions are not contained in cache, request them, and create an entry in cache with status 'Creating'
    ///     -after the points are created, the cache entry is updated and the status changes from 'Creating' to 'Created'
    /// </summary>
    internal class PointsCacheManager
    {
        const decimal latitudeSide = 0.05m;
        const decimal longitudeSide = 0.10m;

        internal static string GenerateKey(int row, int column, int zoomLevel, int datasetId)
                => string.Format("{0}_{1}_{2}_{3}", row, column, zoomLevel, datasetId);


        /// <summary>
        /// Update an existing entry in the cache
        /// </summary>
        /// <param name="row">The row of the area which contains data points</param>
        /// <param name="column">The column of the area which contains data points</param>
        /// <param name="datasetId">Id of the dataset which contains the points</param>
        /// <param name="dataPoints">Datapoints which must be cached</param>
        /// <returns>A string which represents the key generated for the cached zone</returns>
        internal static string Write(int row, int column, int zoomLevel, int datasetId, IEnumerable<PointBase> dataPoints)
        {
            string key = GenerateKey(row, column, zoomLevel, datasetId);
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
        /// <param name="row">The row of the area which contains data points</param>
        /// <param name="column">The column of the area which contains data points</param>
        /// <param name="datasetId">Id of the dataset which contains the points</param>
        internal static void Create(int row, int column, int zoomLevel, int datasetId)
        {
            string key = GenerateKey(row, column, zoomLevel, datasetId);

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
        /// <param name="row">The row of the area which contains data points</param>
        /// <param name="column">The column of the area which contains data points</param>
        /// <param name="datasetId">Id of the dataset which contains the points</param>
        internal static void Remove(int row, int column, int zoomLevel, int datasetId)
        {
            string key = GenerateKey(row, column, zoomLevel, datasetId);
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
        internal static IEnumerable<PointsZonePair> Get(Tuple<int, int> from, 
                                                        Tuple<int, int> to, 
                                                        int zoomLevel,
                                                        int datasetId,
                                                        Dictionary<string,int> regionsPointsCount,
                                                        out List<Region> coordinates)
        {         
            List<string> cacheInfo = getCacheKeys(from, to, zoomLevel, datasetId,  out List<Region> requiredPoints);

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
                 
                result.Add(new PointsZonePair(cacheEntryValue, cacheKey, false));
            }

            coordinates = requiredPoints.Count > 0 ? requiredPoints : null;
            return result;
        }


        #region Private

        private static List<string> getCacheKeys(Tuple<int, int> from, Tuple<int, int> to, int zoomLevel, int datasetId, out List<Region> requiredPoints)
        {
        
            List<string> result = new List<string>();

            requiredPoints = new List<Region>();

            for (int i = from.Item1; i <= to.Item1; i += 1)
                for (int j = from.Item2; j <= to.Item2; j += 1)
                {
                  
                    string key = GenerateKey(i, j, zoomLevel, datasetId);
                    var cacheValue = MemoryCache.Default.Get(key);

                    //if the key was not found as entry in cache, request the points and create an entry
                    //which is identified as 'creating'
                    if (cacheValue == null )
                    {
                        
                        requiredPoints.Add(new Region(i, j, zoomLevel));

                        PointsCacheManager.Create(i, j, zoomLevel, datasetId);
                    }
                    else result.Add(key);
                }

            return result;
        } 

        #endregion
    }
}
