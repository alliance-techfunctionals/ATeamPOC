using System;
using System.Collections.Generic;
using System.Text;

namespace XsdToObjectTreeLibrary.Batch
{
    public class XmlBatchHelper
    {
        public static int GetBatchMaxElementCount(long fileSizeInBytes, int elementCount)
        {
            var number = Getdividenumber(fileSizeInBytes);
            return elementCount / number;
        }      

        private static int Getdividenumber(long fileSizeInByte)
        {
            long y0=0, y1=0;
            long x0=0, x1=0;
            Dictionary<long, long> dataPoints = new Dictionary<long, long>();
            dataPointsAdd(ref dataPoints);           
            foreach(var findmaxkey in dataPoints)
            {
                if (findmaxkey.Key > fileSizeInByte)
                {
                    x1 = findmaxkey.Key;
                    y1 = findmaxkey.Value;
                    break;
                }
                else if (findmaxkey.Key == fileSizeInByte)
                {
                    return (int)findmaxkey.Value;
                }
                else
                {
                    x0 = findmaxkey.Key;
                    y0 = findmaxkey.Value;
                }
            }
            if(x0 == 820000 && x1==0)
            {
                return 240;
            }
            decimal y =(y0 * (fileSizeInByte - x1) / (decimal)(x0 - x1) + y1 * (fileSizeInByte - x0) / (decimal)(x1 - x0));
            return (int)Math.Round(y,0,MidpointRounding.AwayFromZero);          
        }
        private static void dataPointsAdd(ref Dictionary<long, long> dataPoints)
        {
            dataPoints.Add(500, 1);
            dataPoints.Add(1024, 2);
            dataPoints.Add(4096, 4);
            dataPoints.Add(16384, 8);
            dataPoints.Add(32768, 12);
            dataPoints.Add(65536, 20);
            dataPoints.Add(98304, 28);
            dataPoints.Add(196608, 56);
            dataPoints.Add(294912, 84);
            dataPoints.Add(393216, 112);
            dataPoints.Add(589824, 168);
            dataPoints.Add(820000, 200);
        }
    }
}
