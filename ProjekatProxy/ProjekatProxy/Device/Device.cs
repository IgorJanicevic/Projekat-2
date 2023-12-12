using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjekatProxy
{
    public class Device : ISending
    {
        public int UniqueID { get; private set; }
        public List<Measurement> Measurements { get; private set; }

        public Device(int uniqueID)
        {
            UniqueID = uniqueID;
            Measurements = new List<Measurement>();
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