using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjekatProxy
{
    internal class Server
    {
        private readonly List<double> dataStore; // Za čuvanje podataka o merenjima

        public Server()
        {
            dataStore = new List<double>();
        }

        // Metoda za upis podataka o merenjima
        public void WriteData(double measurement)
        {
            dataStore.Add(measurement);
            LogEvent($"Data written: {measurement}");
        }

        // Metoda za dobavljanje podataka od proxy servera
        public List<double> GetData()
        {
            LogEvent("Data requested from proxy server");
            return dataStore;
        }

        // Metoda za logovanje događaja
        private void LogEvent(string message)
        {
            Console.WriteLine($"[Server] {DateTime.Now}: {message}");
        }

    }
}
