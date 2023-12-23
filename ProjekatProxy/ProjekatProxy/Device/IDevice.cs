using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjekatProxy
{
    public interface IDevice
    {
        void RecordMeasurement();
        void SendMeasurementToServer();
    }
}
