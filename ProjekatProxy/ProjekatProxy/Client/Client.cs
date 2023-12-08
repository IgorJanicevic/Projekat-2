using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjekatProxy
{
    internal class Client
    {
        private readonly List<double> receivedData; // Lista za čuvanje primljenih podataka

        public Client()
        {
            receivedData = new List<double>();
        }

        // Metoda za dobavljanje svih podataka odabranog ID-ja
        public List<double> GetDataByDeviceID(int deviceID)
        {
            LogEvent($"Data requested for Device ID: {deviceID}");
            return receivedData.Where(data => data == deviceID).ToList();
        }

        // Metoda za dobavljanje poslednje ažurirane vrednosti odabranog ID-ja
        /*
         
         */
        public double GetLastUpdatedValueByDeviceID(int deviceID)
        {
            LogEvent($"Last updated value requested for Device ID: {deviceID}");
            return receivedData.LastOrDefault(data => data == deviceID);
        }

        // Metoda za dobavljanje poslednje ažurirane vrednosti svakog uređaja
        public Dictionary<int, double> GetLastUpdatedValuesForEachDevice()
        {
            LogEvent("Last updated values requested for each device");
            var lastUpdatedValues = new Dictionary<int, double>();

            foreach (var deviceID in receivedData.Select(d => (int)d).Distinct())
            {
                var lastValue = receivedData.LastOrDefault(data => (int)data == deviceID);
                lastUpdatedValues.Add(deviceID, lastValue);
            }

            return lastUpdatedValues;
        }

        // Metoda za dobavljanje svih podataka analognih merenja
        public List<double> GetAllAnalogData()
        {
            LogEvent("Analog data requested");
            return receivedData.Where(data => IsAnalogMeasurement(data)).ToList();
        }

        // Metoda za dobavljanje svih podataka digitalnih merenja
        public List<double> GetAllDigitalData()
        {
            LogEvent("Digital data requested");
            return receivedData.Where(data => !IsAnalogMeasurement(data)).ToList();
        }

        // Metoda za proveru da li je merenje analogno
        private bool IsAnalogMeasurement(double data)
        {
            // Implementiraj logiku prepoznavanja analogne vrednosti
            // U ovom primeru, pretpostavljamo da su vrednosti veće od 100 analogne
            return data > 100;
        }

        // Metoda za simulaciju dobijanja podataka od servera
        public void ReceiveDataFromServer(List<double> data)
        {
            receivedData.AddRange(data);
            LogEvent("Data received from server");
        }

        // Metoda za logovanje događaja
        private void LogEvent(string message)
        {
            Console.WriteLine($"[Client] {DateTime.Now}: {message}");
        }


    }
}
