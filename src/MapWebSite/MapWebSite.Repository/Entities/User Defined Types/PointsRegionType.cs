using MapWebSite.Model;
using MapWebSite.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapWebSite.Repository.Entities
{
    [UserDefinedType]
    internal class PointsRegionType
    {

        [UserDefinedTypeColumn("dataset_id")]
        public int dataset_id { get; set; }

        [UserDefinedTypeColumn("row")]
        public int row { get; set; }

        [UserDefinedTypeColumn("column")]
        public int column { get; set; }

        [UserDefinedTypeColumn("points")]
        public IEnumerable<BasePointType> points { get; set; }

        public static IEnumerable<PointsRegionType> GetRegions(PointsRegionsLevel pointsRegionsModel, int datasetId)
        {
            ConcurrentBag<PointsRegionType> result = new ConcurrentBag<PointsRegionType>();

            Parallel.ForEach(pointsRegionsModel.Regions, regionModel =>
            {
                var region = new PointsRegionType()
                {
                    row = regionModel.Row,
                    column = regionModel.Column,
                    dataset_id = datasetId,
                    points = new ConcurrentBag<BasePointType>()                    
                };

                var pointsBag = region.points as ConcurrentBag<BasePointType>;

                Parallel.ForEach(regionModel.Points, pointModel =>
               {
                   pointsBag.Add((BasePointType)pointModel);
               });

                result.Add(region);
            });

            return result;
        }
    }
}
