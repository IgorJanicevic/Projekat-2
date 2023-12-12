using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjekatProxy
{

    class Device
    {
        public int UniqueID { get; private set; }
        public List<Measurement> Measurements { get; private set; }
        public Device(int uniqueID)
        {
            UniqueID = uniqueID;
            Measurements = new List<Measurement>();
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

        private void SendMeasurementToServer(Measurement measurement)
        {
            // Implementiraj logiku za slanje merenja na server
            Console.WriteLine($"Device {UniqueID}: Measurement sent to server. Value: {measurement.Value}, Timestamp: {measurement.Timestamp}");
            foreach (Measurement m in Measurements)
            {
                
            }

           
        }




    }
}