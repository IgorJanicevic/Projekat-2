using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjekatProxy
{
    public class Measurement
    {

        public int DeviceID { get; private set; }
        public bool IsAnalog { get; private set; }
        public double Value { get; private set; }
        public DateTime Timestamp { get; private set; }

        public Measurement(int deviceID, bool isAnalog, double value)
        {
            DeviceID = deviceID;
            IsAnalog = isAnalog;
            Value = value;
            Timestamp = DateTime.Now;
        }
    }
}
