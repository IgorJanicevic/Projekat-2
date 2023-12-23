using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjekatProxy
{
    public class Device : IDevice
    {
        public int UniqueID { get; private set; }
        public List<Measurement> Measurements { get; private set; }

        public Device(int id)
        {
            UniqueID = id;
            Measurements = new List<Measurement>();
        }

        public void RecordMeasurement()
        {
            Random random = new Random();
            double value = random.NextDouble() * 100;
            bool isAnalog = random.Next(2) == 0;
            int UniqueID = random.Next(100, 1000);
            Measurement measurement = new Measurement(UniqueID, isAnalog, value);
            Measurements.Add(measurement);

            // Pozovi metodu za slanje merenja
            //SendMeasurementToServer(measurement);
        }


        public void SendMeasurementToServer()
        {
            // Implementiraj logiku za slanje merenja na server          
        }
        
        public string GetMeasurementType()
        {
            // Povrati tip merenja poslednjeg merenja
            if (Measurements.Count > 0)
            {
                return Measurements[Measurements.Count - 1].IsAnalog ? "Analog" : "Digital";

            }
            return "Unknown";
        }




    }
}