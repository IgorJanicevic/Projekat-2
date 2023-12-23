using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace ProjekatProxy
{
    public class Server : IServer
    {
        private readonly List<Measurement> dataStore; // Za čuvanje podataka o merenjima
        //private string databasePath = "measurement_database.txt";

        public Server()
        {
            dataStore = new List<Measurement>();
        }

        // Metoda za upis podataka o merenjima
        public void WriteData(Measurement measurement)
        {
            dataStore.Add(measurement);
            LogEvent($"Data written: {measurement}");
        }

        // Metoda za dobavljanje podataka od proxy servera
        public List<double> GetDataFromProxy()
        {
            throw new NotImplementedException();

        }


        // Metoda za logovanje događaja
        public void LogEvent(string message)
        {
            //Console.WriteLine($"[Server] {DateTime.Now}: {message}");
            while (true)
            {
                string filePath = "C:\\Users\\HomePC\\Documents\\GitHub\\Projekat-2\\ProjekatProxy\\ProjekatProxy\\Server\\BazaPodataka.txt";
                //string contents = "tekst";

                File.WriteAllText(filePath, message);
            }
            
        }

    }
}
