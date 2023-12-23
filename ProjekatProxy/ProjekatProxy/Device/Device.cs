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
            try
            {
                Random random = new Random();
                double vv = random.NextDouble() * 100;
                double value= Math.Round(vv,2);
                bool isAnalog = random.Next(2) == 0;
                int UniqueID = random.Next(1, 100);
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
                Console.WriteLine("GRESKA PRI MERENJu");
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