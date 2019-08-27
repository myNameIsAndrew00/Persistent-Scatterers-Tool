using MapWebSite.Core;
using MapWebSite.Types;
using Newtonsoft.Json;
using System;
 

namespace MapWebSite.Repository.Entities
{
    [UserDefinedType] 
    internal class PointDisplacementType
    {
            
        public DateTimeOffset date { get; set; }
         
        public decimal jd { get; set; }
         
        public decimal days_from_reference { get; set; }
         
        public decimal value { get; set; }
    }
}
