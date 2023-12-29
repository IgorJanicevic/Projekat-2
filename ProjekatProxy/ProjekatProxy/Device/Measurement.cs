using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjekatProxy
{
    [Serializable] //Zbog slanja
    public class Measurement
    {
        public int DeviceID { get; private set; }
        public bool IsAnalog { get; private set; }
        public double Value { get; private set; }
        public DateTime Timestamp { get; private set; }

        public Measurement(int deviceID, bool isAnalog, double value, DateTime timestamp) : this(deviceID, isAnalog, value)
        {
            Timestamp = timestamp;
        }

        public Measurement(int deviceID, bool isAnalog, double value)
        {
            DeviceID = deviceID;
            IsAnalog = isAnalog;
            Value = value;
            Timestamp = DateTime.Now;
        }

        public void UpdateValues(double val,bool analog)
        {
            Timestamp = DateTime.Now;
            Value   = val;
            IsAnalog |= analog;

        }

        public override string ToString()
        {
            string Analog;
            if (IsAnalog)
                Analog = "Analog";
            else
                Analog = "Digital";
            return String.Format("{0},{1},{2},{3}",DeviceID,Value,Analog,Timestamp);
        }
    }
}
