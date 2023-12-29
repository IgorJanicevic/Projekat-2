using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ProjekatProxy
{
    public class Proxy : IProxy
    {
        private readonly Dictionary<int, List<Measurement>> localDataStore; // Lokalno čuvanje podataka
        private List<Measurement> dataFromServer=new List<Measurement>(); //Za cuvanje pomocna lista
        private readonly TimeSpan dataExpirationTime; // Vreme nakon kojeg će lokalna kopija podataka biti obrisana
        //private readonly string MessageFromClient; // Videcemo da li je potrebno
        
        // Za konekciju sa Serverom
        private TcpClient tcpClient;

        // Za konekciju sa Klijentima

        private ServerListenClient slc= new ServerListenClient();
        private TcpListener ListenerForClients;
        private Dictionary<string,TcpClient> tcpClients= new Dictionary<string, TcpClient>();
        public TcpClient tcpTemp;


        public Proxy(Server s, TimeSpan dataExpirationTime)
        {
            localDataStore = new Dictionary<int, List<Measurement>>();
            this.dataExpirationTime = dataExpirationTime;

            
            try
            {
                //Konekcija sa Serverom na pocetku
                tcpClient = new TcpClient("127.0.0.1", 8080);
               
               
            }catch(Exception ex) {
                Console.WriteLine(ex.Message + "PROXY");
            }
            Console.WriteLine("Proxy connected to server");           
          

            // Slusanje klijenata
            ListenerForClients = new TcpListener(IPAddress.Any, 5000);
            ListenerForClients.Start();
            Console.WriteLine("Proxy listening on port " + 5000);
        }

        //Metoda za prihvatanje klijenata
        public void ProxyAcceptClient(string name)
        {
            // Čekaj na konekciju od proxy-ja
            tcpTemp = slc.AcceptClient(ListenerForClients);
            tcpClients.Add(name,tcpTemp);
        }

        //Za prihvatanje poruke od klijenta i salje
        public string AcceptClientMessage(string name,int br)
        {
            //Blok koji prihvata poruku
            string option=null;
            if(tcpClients.ContainsKey(name))
            {
                tcpTemp= tcpClients[name];
            }
            option = slc.StartReading(tcpTemp);

            //Blok koji proverava lokalne podatke
            int trazeni = int.Parse(option);          
       


          //Blok koji salje nazad poruku
            slc.SendMessageToServer(option, tcpClient);
          //this.SendMessage(option);         

            return option;
               
        }

        //Prihvatanje podataka sa servera
        public void AcceptDataFromServer()
        {
           dataFromServer= slc.AcceptDataFromServer(tcpClient);
        }

        //Metoda za slanje podatak nazad klijetnu
        public void SendDataToClient()
        {
            slc.SandList(dataFromServer,tcpTemp);
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
        private void UpdateLocalCopy(int deviceID, List<Measurement> serverData)
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
