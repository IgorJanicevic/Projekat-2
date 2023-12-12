using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

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
            //Console.WriteLine($"[Server] {DateTime.Now}: {message}");
            while (true)
            {
                string path = @"C: \Users\PC User\Documents\GitHub\Projekat - 2\ProjekatProxy\ProjekatProxy\Server\Ispis.txt";
                string contents = "tekst";

                File.WriteAllText(path, contents);
            }
            
        }

    }
}
