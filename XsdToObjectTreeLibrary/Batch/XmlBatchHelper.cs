using System;
using System.Collections.Generic;
using System.Text;

namespace XsdToObjectTreeLibrary.Batch
{
    public static class XmlBatchHelper
    {
        internal static int GetBatchMaxElementCount(long fileSizeInBytes, int elementCount)
        {
            if (fileSizeInBytes <= 500 * 1024)
            {
                return elementCount;
            }
            else if (fileSizeInBytes <= 1024 * 1024)
            {
                return elementCount / 2;
            }
            else if (fileSizeInBytes <= 4096 * 1024)
            {
                return elementCount / 4;
            }
            else if (fileSizeInBytes <= 16384 * 1024)
            {
                return elementCount / 8;
            }
            else if (fileSizeInBytes <= 32768 * 1024)
            {
                return elementCount / 12;
            }
            else if (fileSizeInBytes <= 65536 * 1024)
            {
                return elementCount / 20;
            }
            else if (fileSizeInBytes <= 98304 * 1024)
            {
                return elementCount / 28;
            }
            else if (fileSizeInBytes <= 196608 * 1024)
            {
                return elementCount / 56;
            }
            else if (fileSizeInBytes <= 294912 * 1024)
            {
                return elementCount / 84;
            }
            else if (fileSizeInBytes <= 393216 * 1024)
            {
                return elementCount / 112;
            }
            else if (fileSizeInBytes <= 589824 * 1024)
            {
                return elementCount / 168;
            }
            else if (fileSizeInBytes <= 820000 * 1024)
            {
                return elementCount / 200;
            }
            else
            {
                return elementCount / 240;
            }
        }
    }
}
