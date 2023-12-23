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

        private Random random = new Random();

        public Device(int id)
        {
            UniqueID = id;
            Measurements = new List<Measurement>();
        }

        public void RecordMeasurement(Random r)
        {
            try
            {
                
                double vv = r.NextDouble() * 100;
                double value= Math.Round(vv,2);
                bool isAnalog = r.Next(2) == 0;
                int UniqueID = r.Next(1, 100);
                //Measurement measurement = new Measurement(UniqueID, isAnalog, value);

                int tmp = 0;
                foreach (Measurement m in Measurements)
                {
                    if (m.DeviceID == UniqueID)
                    {
                        tmp = 1;
                        m.UpdateValues(value, isAnalog);
                        break;
                    }
                }
                if (tmp==0)
                {
                    Measurement measurement = new Measurement(UniqueID, isAnalog, value);
                    Measurements.Add(measurement);
                }
                tmp = 0;
                

            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("GRESKA PRI MERENJU");
            }

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