using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjekatProxy
{
    internal class Recording
    {
        public void RecordMeasurement(Device device)
        {

            Random random = new Random();
            double value = random.NextDouble() * 100;
            bool isAnalog = random.Next(2) == 0;
            int UniqueID = random.Next(100, 1000);
            Measurement measurement = new Measurement(UniqueID, isAnalog, value);
            device.Measurements.Add(measurement);

            // Pozovi metodu za slanje merenja
            //SendMeasurementToServer(measurement);
        }

    }
}
