using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjekatProxy
{
    internal class Device
    {  
    
                // Atributi klase
        public int UniqueID { get; private set; }
        public bool IsAnalog { get; private set; }
        public double MeasurementValue { get; private set; }
        public DateTime Timestamp { get; private set; }

        // Konstruktor klase sa dodatnim parametrom za vreme
        public Device(int uniqueID, bool isAnalog, DateTime initialTime)
        {
            UniqueID = uniqueID;
            IsAnalog = isAnalog;
            SetInitialTime(initialTime);
        }

        // Metoda za postavljanje inicijalnog vremena
        private void SetInitialTime(DateTime initialTime)
        {
            Timestamp = initialTime;
        }

        // Metoda za upis podataka o merenjima
        public void RecordMeasurement(double value)
        {
            MeasurementValue = value;
            Timestamp = DateTime.Now;

            // Pozovi metodu za slanje merenja
            SendMeasurement();
        }

        // Privatna metoda za slanje merenja
        private void SendMeasurement()
        {
            // Logika za slanje merenja na server
            Console.WriteLine($"Device {UniqueID}: Measurement sent to server. Value: {MeasurementValue}, Timestamp: {Timestamp}");
        }

        // Metoda za proveru tipa merenja
        public string GetMeasurementType()
        {
            return IsAnalog ? "Analog" : "Digital";
        }


    }
}
