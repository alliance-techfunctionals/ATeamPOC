using System;
using System.Collections.Generic;
using System.Text;
using XsdToObjectTreeLibrary.Batch;
using Xunit;

namespace XUnitTestProject.Batch
{
    public class BatchTester
   {
        [Fact]
        public void BatchMethod()
        {

            long newFileSize = XmlBatchHelper.GetBatchMaxElementCount(5000, 521);
        }
   }
}
