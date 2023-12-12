using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjekatProxy
{

    public class Measurement
    {
        public bool IsAnalog { get; private set; }
        public double Value { get; private set; }
        public int DeviceID { get; private set; }
        public DateTime Timestamp { get; private set; }

        public Measurement(int deviceID, bool isAnalog, double value)
        {
            DeviceID = deviceID;
            IsAnalog = isAnalog;
            Value = value;
            Timestamp = DateTime.Now;
        }
    }





    internal class Device
    {
        public int UniqueID { get; private set; }
        public List<Measurement> Measurements { get; private set; }

        public Device(int uniqueID)
        {
            UniqueID = uniqueID;
            Measurements = new List<Measurement>();
        }

        public void RecordMeasurement(double value, bool isAnalog)
        {        
            Measurement measurement = new Measurement(UniqueID, isAnalog, value);
            Measurements.Add(measurement);

            // Pozovi metodu za slanje merenja
            SendMeasurementToServer(measurement);
        }

        private void SendMeasurementToServer(Measurement measurement)
        {
            // Implementiraj logiku za slanje merenja na server
            Console.WriteLine($"Device {UniqueID}: Measurement sent to server. Value: {measurement.Value}, Timestamp: {measurement.Timestamp}");
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