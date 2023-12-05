using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjekatProxy
{
    internal class Proxy
    {
        private readonly Dictionary<int, List<double>> localDataStore; // Lokalno čuvanje podataka
        private readonly Server server; // Reference na server
        private readonly TimeSpan dataExpirationTime; // Vreme nakon kojeg će lokalna kopija podataka biti obrisana

        public Proxy(Server server, TimeSpan dataExpirationTime)
        {
            localDataStore = new Dictionary<int, List<double>>();
            this.server = server;
            this.dataExpirationTime = dataExpirationTime;
        }

        // Metoda za obradu zahteva klijenta
        public List<double> ProcessClientRequest(int deviceID, DateTime lastAccessTime)
        {
            if (HasLocalCopy(deviceID, lastAccessTime))
            {
                LogEvent($"Local copy exists for Device ID {deviceID}. Retrieving locally.");
                return localDataStore[deviceID];
            }

            LogEvent($"Local copy not found or outdated for Device ID {deviceID}. Requesting data from server.");
            var serverData = server.GetData();

            // Ažuriranje lokalne kopije podataka
            UpdateLocalCopy(deviceID, serverData);

            return serverData;
        }

        // Privatna metoda za proveru lokalne kopije podataka
        private bool HasLocalCopy(int deviceID, DateTime lastAccessTime)
        {
            return localDataStore.ContainsKey(deviceID) &&
                   (DateTime.Now - lastAccessTime) < dataExpirationTime;
        }

        // Privatna metoda za ažuriranje lokalne kopije podataka
        private void UpdateLocalCopy(int deviceID, List<double> serverData)
        {
            if (localDataStore.ContainsKey(deviceID))
            {
                localDataStore[deviceID] = serverData;
            }
            else
            {
                localDataStore.Add(deviceID, serverData);
            }

            LogEvent($"Local copy updated for Device ID {deviceID}.");
        }

        // Metoda za logovanje događaja
        private void LogEvent(string message)
        {
            Console.WriteLine($"[Proxy] {DateTime.Now}: {message}");
        }

    }
}
