using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjekatProxy
{
    public interface IClient
    {
        void ReceiveDataFromProxy(List<double> data);
        //void LogEvent(string message);
        List<double> GetAllDigitalData();
        List<double> GetAllAnalogData();
        Dictionary<int, double> GetLastUpdatedValuesForEachDevice();
        double GetLastUpdatedValueByDeviceID(int deviceID);
        List<double> GetDataByDeviceID(int deviceID);

    }
}
