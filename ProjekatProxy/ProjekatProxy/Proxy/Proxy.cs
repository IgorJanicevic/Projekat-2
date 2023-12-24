using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ProjekatProxy
{
    internal class Proxy : IProxy
    {
        private readonly Dictionary<int, List<double>> localDataStore; // Lokalno čuvanje podataka
        private readonly Server server; // Reference na server
        private readonly TimeSpan dataExpirationTime; // Vreme nakon kojeg će lokalna kopija podataka biti obrisana
        private readonly string MessageFromClient;
        
        // Za konekciju sa Serverom
        private TcpClient tcpClient;

        // Za konekciju sa Klijentima

        private ServerListenClient slc= new ServerListenClient();
        private TcpListener ListenerForClients;
        private List<TcpClient> tcpClients= new List<TcpClient>();
        private TcpClient tcpTemp;


        public Proxy(Server s, TimeSpan dataExpirationTime)
        {
            localDataStore = new Dictionary<int, List<double>>();
            this.server = s;
            this.dataExpirationTime = dataExpirationTime;
            MessageFromClient = "Succes..";

            
            try
            {
                //Konekcija sa Serverom na pocetku
                tcpClient = new TcpClient("127.0.0.1", 8080);
                tcpClients.Add(tcpClient);
               
            }catch(Exception ex) {
                Console.WriteLine(ex.Message + "PROXY");
            }
            Console.WriteLine("Connected to server");

            // Ovde možete implementirati logiku za slanje poruka serveru

            SendMessage(MessageFromClient);

            // Slusanje klijenata
            ListenerForClients = new TcpListener(IPAddress.Any, 5000);
            ListenerForClients.Start();
            Console.WriteLine("Proxy listening on port " + 5000);
        }

        public void ProxyAcceptClient()
        {
            // Čekaj na konekciju od proxy-ja
            tcpTemp = slc.AcceptClient(ListenerForClients);
            tcpClients.Add(tcpTemp);
        }

        //Za prihvatanje poruke od klijenta
        public string AcceptClientMessage()
        {
           string option= slc.StartReading(tcpTemp);
           return option;
        }

        // Metoda za obradu zahteva klijenta
        // Potrebno je da se promeni return value
        /*public List<double> ProcessClientRequest(int deviceID, DateTime lastAccessTime)
        {
            if (HasLocalCopy(deviceID, lastAccessTime))
            {
                LogEvent($"Local copy exists for Device ID {deviceID}. Retrieving locally.");
                return localDataStore[deviceID];
            }

            LogEvent($"Local copy not found or outdated for Device ID {deviceID}. Requesting data from server.");
            var serverData = server.GetDataFromProxy();

            // Ažuriranje lokalne kopije podataka
            UpdateLocalCopy(deviceID, serverData);

            return serverData;
        }*/

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

        // Meotda za za slanje poruke
        private void SendMessage(string message)
        {
            try
            {
                NetworkStream networkStream = tcpClient.GetStream();
                byte[] buffer = Encoding.ASCII.GetBytes(message);
                networkStream.Write(buffer, 0, buffer.Length);
                //Console.WriteLine("Sent message to server: " + message);
            }catch (Exception e)
            {
                Console.WriteLine(e.Message +"PROXY MESSAGE");
            }
        }

    }
}
