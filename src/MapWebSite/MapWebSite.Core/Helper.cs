using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MapWebSite.Core
{
    public static class Helper
    {
        /// <summary>
        /// Use this method to convert a color in format HSL
        /// /summary>
        /// <param name="hue">The hue, a double between 0 and 240</param>
        /// <param name="saturation">A value between 0 and 1</param>
        /// <param name="luminance">A value between 0 and 1</param>
        /// <returns></returns>
        public static Color ConvertHSLToRGB(double hue, double saturation, double luminance)
        {

            Func<double, double, double, double> HueToRgb = (v1, v2, vH) =>
               {
                   if (vH < 0)
                       vH += 1;

                   if (vH > 1)
                       vH -= 1;

                   if ( (6 * vH) < 1)
                       return (v1 + (v2 - v1) * 6 * vH);

                   if ( (2 * vH) < 1)
                       return v2;

                   if ( (3 * vH) < 2)
                       return (v1 + (v2 - v1) * ((2.0f / 3) - vH) * 6);

                   return v1;
               };

            byte r = 0, g = 0, b = 0;
            
            if (saturation == 0)
            {
                r = g = b = (byte)(luminance * 255);
            }
            else
            {
                double v1, v2;
                hue /= 360;

                v2 = (luminance < 0.5) ? (luminance * (1 + saturation)) : ((luminance + saturation) - (luminance * saturation));
                v1 = 2 * luminance - v2;

                r = (byte)(255 * HueToRgb(v1, v2, hue + (1.0f / 3)));
                g = (byte)(255 * HueToRgb(v1, v2, hue));
                b = (byte)(255 * HueToRgb(v1, v2, hue - (1.0f / 3)));
            }

            return Color.FromArgb(r, g, b);
        }

        
        /// <summary>
        /// Use this method to a get fraction of a number
        /// </summary>
        /// <param name="percent">The percentage representing a ratio expressed as a fraction of 100 </param>
        /// <param name="totalCount">Number of total units from which will be retrieved the fraction</param>
        /// <returns>Amount of items representing a fraction of totalCount based on percent</returns>
        public static int GetFraction(decimal percent, int totalAmount)
        {
            if (percent < 0m || percent > 100m) throw new ArgumentException("The percent must be a value between 0 and 100");

            return (int)((decimal)totalAmount * percent / 100m);
        }

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

        /// <summary>
        /// This class provide a method to convert geographic coordinates
        /// </summary>
        public class UTMConverter
        {
            private readonly decimal PI = 3.1415926535897931m;

            private readonly decimal k0 = 0.9996m;

            private readonly decimal e = 0.0818191913108701855043250591190036654364021844723924m;

            private readonly decimal ePow2div4 = 0.001673595016691193816151969727945394843836788900874815961m;

            private readonly decimal ePow4div64 = 0.00000070023006997339932702041564050782509749197746139619143m;

            private readonly decimal ePow6 = 0.000000001171901555644807060528396418770459079272577296244862421875m;

            private readonly decimal a = 6378137;

            private readonly decimal e1sq = 0.006739497m;

            private decimal[] phi = new decimal[2];

            private decimal[] fact = new decimal[4];


            public Tuple<decimal, decimal> ToLatLong(int zone, char latitudeZone, decimal easting, decimal northing)
            {
                if (getHemisphere(latitudeZone) == 'S') northing = 10000000 - northing;

                this.setVariables(easting, northing);

                decimal latitude = 180m * (phi[0] - fact[0] * (fact[1] + fact[2] + fact[3])) / PI;

                decimal longitude = (zone > 0 ? 6m * zone - 183.0m : 3.0m) - phi[1];

                if (getHemisphere(latitudeZone) == 'S') latitude = -latitude;

                return new Tuple<decimal, decimal>(latitude, longitude);
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

            private void setVariables(decimal easting, decimal northing)
            {

                Action setFirstPhi = delegate ()
                {
                    decimal arc = northing / k0;
                    decimal mu = (decimal)arc / (a * (1
                                        - ePow2div4
                                        - 3 * ePow4div64
                                        - 5 * ePow6));
                    decimal ei = (1 - (decimal)Math.Sqrt((double)(1 - e.Pow(2))))
                                    / (decimal)(1 + Math.Pow((double)(1 - e.Pow(2)), 0.5));

                    decimal ca = 3 * ei / 2 - 27 * ei.Pow(3) / 32;
                    decimal cb = 21 * ei.Pow(2) / 16 - 55 * ei.Pow(4) / 32;
                    decimal cc = 151 * ei.Pow(3) / 96;
                    decimal cd = 1097 * ei.Pow(4) / 512;

                    phi[0] = ((decimal)mu
                                    + ca * (decimal)Math.Sin((double)(2 * mu))
                                    + cb * (decimal)Math.Sin((double)(4 * mu))
                                    + cc * (decimal)Math.Sin((double)(6 * mu))
                                    + cd * (decimal)Math.Sin((double)(8 * mu)));
                };

                Action setFact = delegate ()
                {
                    decimal n0 = a / (decimal)Math.Pow((1 - Math.Pow(((double)e * Math.Sin((double)phi[0])), 2)), 0.5);

                    decimal r0 = (a * (1 - e * e) / (decimal)Math.Pow((1 - Math.Pow(((double)e * Math.Sin((double)phi[0])), 2)), 1.5));

                    fact[0] = (n0 * (decimal)Math.Tan((double)phi[0]) / r0);

                    decimal dd0 = (decimal)(500000 - (double)easting) / (n0 * k0);

                    fact[1] = dd0.Pow(2) / 2;

                    decimal t0 = ((decimal)Math.Tan((double)phi[0])).Pow(2);
                    decimal Q0 = e1sq * ((decimal)Math.Cos((double)phi[0])).Pow(2);

                    fact[2] = (5 + 3 * t0 + 10 * Q0 - 4 * Q0 * Q0 - 9 * e1sq) * dd0.Pow(4) / 24;

                    fact[3] = (61 + 90 * t0 + 298 * Q0 + 45 * t0 * t0 - 252 * e1sq - 3 * Q0
                        * Q0)
                        * dd0.Pow(6) / 720;

                    decimal lof1 = (500000 - easting) / (n0 * k0);
                    decimal lof2 = (1 + 2 * t0 + Q0) * dd0.Pow(3) / 6;

                    decimal lof3 = (5 - 2 * Q0 + 28 * t0 - 3 * Q0.Pow(2) + 8 * e1sq + 24 * t0.Pow(2))
                                 * dd0.Pow(5) / 120;
                    decimal _a2 = (lof1 - lof2 + lof3) / (decimal)Math.Cos((double)phi[0]);

                    phi[1] = _a2 * 180 / PI;
                };

                setFirstPhi();
                setFact();
            }
        }

    }
}
