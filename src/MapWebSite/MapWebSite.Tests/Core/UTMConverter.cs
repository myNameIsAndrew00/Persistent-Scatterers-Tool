using MapWebSite.Core;
using MapWebSite.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapWebSite.Types; 

namespace MapWebSite.Tests.Core
{
    [TestClass]
    public class UTMConverter
    {

        [TestMethod]
        public void UTMConverterTest()
        {
            Helper.UTMConverter converter = new Helper.UTMConverter();
            var result = converter.ToLatLong(35, 'T', 632353.875m, 4919262.500m);
        }

        [TestMethod]
        public void ConvertPoints()
        {
            Helper.UTMConverter converter = new Helper.UTMConverter();
        

            Tuple<decimal, decimal>[] validResult = new Tuple<decimal, decimal>[]
            {
                new Tuple<decimal, decimal>(44.143143m, 28.652490m), //invalid
                new Tuple<decimal, decimal>(43.531231m,31.4124m),       //0
                new Tuple<decimal, decimal>(-3.531231m,12.412414m),     //1
                new Tuple<decimal, decimal>(61.35918m,34.312415m),      //2
                new Tuple<decimal, decimal>(0.0m,0.0m),                 //3
                new Tuple<decimal, decimal>(-10.31231m, -50.313213m),   //4
                new Tuple<decimal, decimal>(180.0m, 180.0m),            //5
                new Tuple<decimal, decimal>(90.0m,90.0m),               //6
                new Tuple<decimal, decimal>(-41.3828m, 101.28485m),     //7
                new Tuple<decimal, decimal>(77.3828m, -100.28485m),     //8
                new Tuple<decimal, decimal>(-88.3828m, -160.28485m),    //9
            };

            Tuple<decimal, decimal>[] converterResult = new Tuple<decimal, decimal>[validResult.Length];

            converterResult[0] = converter.ToLatLong(35, 'T', 632308.875m, 4895179.500m);
            converterResult[1] = converter.ToLatLong(36, 'T', 371717.98m, 4821034.09m);
            converterResult[2] = converter.ToLatLong(33, 'M', 212511.84m, 9609287.87m);
            converterResult[3] = converter.ToLatLong(36, 'V', 570176.83m, 6803500.96m);
            converterResult[4] = converter.ToLatLong(31, 'N', 166021.44m, 0.00m);
            converterResult[5] = converter.ToLatLong(22, 'L', 575197.48m, 8859976.41m);
            converterResult[6] = converter.ToLatLong(1,  'Z', 833978.56m, 19995929.89m);
            converterResult[7] = converter.ToLatLong(46, 'Z', 500000.00m, 9997964.94m);
            converterResult[8] = converter.ToLatLong(47, 'G', 691049.80m, 5416228.06m);
            converterResult[9] = converter.ToLatLong(14, 'X', 468672.17m, 8589834.08m);
            converterResult[10] = converter.ToLatLong(4, 'Z', 495951.87m, 182548.45m);

            bool validAssert = true;

            for (int index = 0; index < validResult.Length; index++)
                validAssert = validAssert & (validResult[index] == converterResult[index]);
            
            Assert.IsTrue(validAssert);
        }
         
    }
}
