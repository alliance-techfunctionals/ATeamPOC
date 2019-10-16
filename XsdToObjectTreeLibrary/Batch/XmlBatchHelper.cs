using System;
using System.Collections.Generic;
using System.Text;

namespace XsdToObjectTreeLibrary.Batch
{
    public class XmlBatchHelper
    {

        static Dictionary<long, int> dataPoints = new Dictionary<long, int>
        {
            {500,1},
            {1024,2},
            {4096,4 },
            {16384, 8},
            {32768, 12},
            {65536, 20},
            {98304, 28},
            {196608, 56},
            {294912, 84},
            {393216, 112},
            {589824, 168},
            {820000, 200}
        };


        public static int GetBatchMaxElementCount(long fileSizeInBytes, int elementCount)
        {
            var number = retrieveDivisor(fileSizeInBytes);
            return elementCount / number;
        }      

        private static int retrieveDivisor(long fileSizeInBytes)
        { 
            long y0 = 0, y1 = 0;
            long x0 = 0, x1 = 0;
            foreach (var adjacentpoint in dataPoints)
            {
                if (adjacentpoint.Key > fileSizeInBytes)
                {
                    x1 = adjacentpoint.Key;
                    y1 = adjacentpoint.Value;
                    break;
                }
                else
                {
                    x0 = adjacentpoint.Key;
                    y0 = adjacentpoint.Value;
                }
            }
            if (x1 == 0)
            {
                return 240;
            }
            decimal y = (y0 * (fileSizeInBytes - x1) / (decimal)(x0 - x1) + y1 * (fileSizeInBytes - x0) / (decimal)(x1 - x0));
            return (int)Math.Round(y, 0, MidpointRounding.AwayFromZero) == 0 ? 1 : (int)Math.Round(y, 0, MidpointRounding.AwayFromZero);
        }      
    }
}
