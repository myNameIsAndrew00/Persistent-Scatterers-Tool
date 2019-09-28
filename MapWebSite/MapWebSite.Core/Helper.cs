using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MapWebSite.Core
{
    public static class Helper
    {
   
        public static byte[] GenerateRandomBytes(int bytesCount)
        {
           if (bytesCount == 0) return null;

           byte[] result = new byte[bytesCount];
           
            using (var randomProvider = new RNGCryptoServiceProvider())
                randomProvider.GetNonZeroBytes(result);

            return result;
        }

        public static byte[] HashData(byte[] Data, byte[] Salt)
        {
            if (Data?.Length == 0) return null;

            byte[] hashInput = new byte[Data.Length + Salt?.Length ?? 0];
            byte[] digest = null;

            //concatenate data and salt
            for (int index = 0; index < Data.Length; index++)
                hashInput[index] = Data[index];
            for (int index = 0; index < Salt?.Length; index++)
                hashInput[index + Data.Length] = Salt[index];

            using (SHA256 sha256Instance = SHA256.Create())
            {
                sha256Instance.ComputeHash(hashInput, 0, Data.Length + Salt.Length);
                digest = sha256Instance.Hash;             
            }

            return digest;
        }

        public class UTMConverter
        {
            private readonly double k0 = 0.9996;
             
            private readonly double e = 0.0818191913108701855043250591190036654364021844723924;

            private readonly double ePow2div4 = 0.001673595016691193816151969727945394843836788900874815961;

            private readonly double ePow4div64 = 0.00000070023006997339932702041564050782509749197746139619143;

            private readonly double ePow6 = 0.000000001171901555644807060528396418770459079272577296244862421875;

            private readonly double a = 6378137;

            private readonly double e1sq = 0.006739497;

            private double[] phi = new double[2];

            private double[] fact = new double[4];


            public Tuple<decimal, decimal> ToLatLong(int zone, char latitudeZone, double easting, double northing)
            {
                if (getHemisphere(latitudeZone) == 'S') northing = 10000000 - northing;

                this.setVariables(easting, northing);

                double latitude = 180 * (phi[0] - fact[0] * (fact[1] + fact[2] + fact[3])) / Math.PI;

                double longitude = (zone > 0 ? 6 * zone - 183.0 : 3.0) - phi[1];

                if (getHemisphere(latitudeZone) == 'S') latitude = -latitude;

                return new Tuple<decimal, decimal>(Convert.ToDecimal(latitude), Convert.ToDecimal(longitude));
            }


            private char getHemisphere(char latitudeZone)
            {
                switch (latitudeZone)
                {
                    case 'A':
                    case 'C':
                    case 'D':
                    case 'E':
                    case 'F':
                    case 'G':
                    case 'H':
                    case 'J':
                    case 'K':
                    case 'L':
                        return 'S';
                    default:
                        return 'N';
                }
            }

            private void setVariables(double easting, double northing)
            {

                Action setFirstPhi = delegate ()
                {
                    double arc = northing / k0;
                    double mu = arc / (a * (1
                                        - ePow2div4
                                        - 3 * ePow4div64
                                        - 5 * ePow6));
                    double ei = (1 - Math.Pow((1 - e * e), 0.5))
                                    / (1 + Math.Pow((1 - e * e), 0.5));

                    double ca = 3 * ei / 2 - 27 * Math.Pow(ei, 3) / 32;
                    double cb = 21 * Math.Pow(ei, 2) / 16 - 55 * Math.Pow(ei, 4) / 32;
                    double cc = 151 * Math.Pow(ei, 3) / 96;
                    double cd = 1097 * Math.Pow(ei, 4) / 512;

                    phi[0] =           (mu 
                                    + ca * Math.Sin(2 * mu) 
                                    + cb * Math.Sin(4 * mu) 
                                    + cc * Math.Sin(6 * mu) 
                                    + cd * Math.Sin(8 * mu));
                };

                Action setFact = delegate ()
                {
                    double n0 = a / Math.Pow((1 - Math.Pow((e * Math.Sin(phi[0])), 2)), 0.5);

                    double r0 = a * (1 - e * e) / Math.Pow((1 - Math.Pow((e * Math.Sin(phi[0])), 2)), 1.5);

                    fact[0] = (n0 * Math.Tan(phi[0]) / r0);

                    double dd0 = (500000 - easting) / (n0 * k0);

                    fact[1] = dd0 * dd0 / 2;

                    double t0 = Math.Pow(Math.Tan(phi[0]), 2);
                    double Q0 = e1sq * Math.Pow(Math.Cos(phi[0]), 2);

                    fact[2] = (5 + 3 * t0 + 10 * Q0 - 4 * Q0 * Q0 - 9 * e1sq) * Math.Pow(dd0, 4) / 24;

                    fact[3] = (61 + 90 * t0 + 298 * Q0 + 45 * t0 * t0 - 252 * e1sq - 3 * Q0
                        * Q0)
                        * Math.Pow(dd0, 6) / 720;

                    double lof1 = (500000 - easting) / (n0 * k0);
                    double lof2 = (1 + 2 * t0 + Q0) * Math.Pow(dd0, 3) / 6.0;
                    double lof3 = (5 - 2 * Q0 + 28 * t0 - 3 * Math.Pow(Q0, 2) + 8 * e1sq + 24 * Math.Pow(t0, 2))
                        * Math.Pow(dd0, 5) / 120;
                    double _a2 = (lof1 - lof2 + lof3) / Math.Cos(phi[0]);

                    phi[1] = _a2 * 180 / Math.PI;
                };
                              
                setFirstPhi();
                setFact();
            }
        }

    }
}
