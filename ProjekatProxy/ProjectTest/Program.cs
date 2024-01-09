using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjekatProxy;

namespace ProjectTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MyTests myTests = new MyTests();

            myTests.Test_CreateNewMeasure();
            myTests.Test_RecordMeasurement();
            myTests.Test_ProxyAcceptClient();
            myTests.Test_AcceptClientMessage();
        }
    }
}
