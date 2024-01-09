using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Eventing.Reader;
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
        private DateTime lastTime; //Poslednji poslat zahtev serveru
        private int currentDevID; //

        // Za konekciju sa Serverom
        private TcpClient tcpClient;

        // Za konekciju sa Klijentima

        private ServerListenClient slc= new ServerListenClient();
        private TcpListener ListenerForClients;
        private Dictionary<string,TcpClient> tcpClients= new Dictionary<string, TcpClient>();
        public TcpClient tcpTemp;


        public Proxy(Server s)
        {
            localDataStore = new Dictionary<int, List<Measurement>>();

            
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
            currentDevID = int.Parse(option);

            SendMeasureToServerOn5Minutes smts = new SendMeasureToServerOn5Minutes();
            DateTime tempForLastUpdate = smts.lastUpdate();

            if (HasLocalCopy(currentDevID, tempForLastUpdate, br))
            {              
                return null;
            }



          //Blok koji salje nazad poruku
            slc.SendMessageToServer(option, tcpClient);
            lastTime = DateTime.Now;
                   

            return option;
               
        }

        //Prihvatanje podataka sa servera
        public void AcceptDataFromServer()
        {
           dataFromServer= slc.AcceptDataFromServer(tcpClient);
           
            if (dataFromServer!=null)
           UpdateLocalCopy(currentDevID, dataFromServer);             
        }

        //Metoda za slanje podatak nazad klijetnu
        public void SendDataToClient()
        {
            slc.SandList(dataFromServer,tcpTemp);
            LogEvent($"Proxy sent data to client.");
        }



        // Privatna metoda za proveru lokalne kopije podataka
        private bool HasLocalCopy(int deviceID, DateTime lastAccessTime,int br)
        {
            int rezultatPoredjenja = DateTime.Compare(lastTime,lastAccessTime);
            //Console.WriteLine($"POSLEDNJE UPTDATE: {lastAccessTime}\nPOSLENJI PROSTUP SERVERU: {lastTime}");
            if (localDataStore.Count==0)
                return false;
            if (rezultatPoredjenja<0)
            {
                switch (br)
                {
                    case 1:
                        if (localDataStore.ContainsKey(deviceID)) {
                            slc.SandList(localDataStore[deviceID], tcpTemp);
                            return true;
                        }
                        return false;
                    case 2:
                        return LastUpdated(deviceID);
                    case 3:
                        return LastUpdated(0);                      
                    case 4:
                        return DigitOrAnalog(0);
                    case 5:
                        return DigitOrAnalog(1);
                }

            }
            return false;
                   
        }

    
        // Privatna metoda za ažuriranje lokalne kopije podataka
        private void UpdateLocalCopy(int deviceID, List<Measurement> serverData)
        {
            if (deviceID == 0)
                return;

            if (localDataStore.ContainsKey(deviceID))
            {
                //localDataStore[deviceID] = serverData;
                localDataStore.Remove(deviceID);
                localDataStore.Add(deviceID, serverData);
            }   
            else
            {
                localDataStore.Add(deviceID, serverData);
            }
           // Console.WriteLine($"Local copy updated for Device ID {deviceID}");
            LogEvent($"Local copy updated for Device ID {deviceID}.");
        }

        // Metoda za logovanje događaja
        private void LogEvent(string message)
        {
            //Console.WriteLine($"[Proxy] {DateTime.Now}: {message}");
            // Postavite relativnu putanju u odnosu na folder projekta
            string relativePath = Path.Combine("LokalnaKopija.txt");

            // Dobijte apsolutnu putanju od relativne
            string absolutePath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);

            File.AppendAllText(absolutePath, message + "\n");
        }
        

        //Proverava loklalnu kopiju za poslednje azuiranje vrednosti
        private bool LastUpdated(int br)
        {
            List<Measurement> lisRet= new List<Measurement>();
            if(br == 0){ //Za sve
                foreach (int devID in localDataStore.Keys)
                {
                    Console.WriteLine(devID);
                    if (localDataStore[devID] == null)
                        continue;
                    List<Measurement> temp = localDataStore[devID];
                    Measurement first = temp[0];
                    foreach (Measurement m in temp)
                    {
                        if (first.Timestamp.CompareTo(m.Timestamp) < 0)
                        {
                            first = m;
                        }
                    }
                    lisRet.Add(first);
                }
                slc.SandList(lisRet, tcpTemp);
                lisRet.Clear();
                SaveShutdownTime(DateTime.Now); // Pristupili smo lokalnoj kopiji
                return true;

            }
            else //Za jedan ID 2. tacka
            {
                if (localDataStore.ContainsKey(br))
                {
                    List<Measurement> temp = localDataStore[br];
                    Measurement first = temp[0];
                    foreach (Measurement m in temp)
                    {
                        if (first.Timestamp.CompareTo(m.Timestamp) < 0)
                        {
                            first = m;
                        }
                    }
                    lisRet.Add(first);
                    slc.SandList(lisRet, tcpTemp);
                    lisRet.Clear();
                    SaveShutdownTime(DateTime.Now); // Pristupili smo lokalnoj kopiji
                    return true;
                }
                return false;
            }

        }

        //Proverava sve lokalne kopije digitalni i analognih merenja za vracanje klijentu
        private bool DigitOrAnalog(int v)
        {
            List<Measurement> retVal= new List<Measurement>();
            if (v == 0)
            {
                foreach(int dev in localDataStore.Keys)
                {
                    List<Measurement> LocalList= localDataStore[dev];
                    foreach (Measurement m in LocalList)
                    {
                        if (m.IsAnalog)
                            retVal.Add(m);
                    }
                }
                slc.SandList(retVal, tcpTemp);
                SaveShutdownTime(DateTime.Now); // Pristupili smo lokalnoj kopiji
                return true;
            }
            else
            {
                foreach (int dev in localDataStore.Keys)
                {
                    List<Measurement> LocalList = localDataStore[dev];
                    foreach (Measurement m in LocalList)
                    {
                        if (!m.IsAnalog)
                            retVal.Add(m);
                    }
                }
                slc.SandList(retVal, tcpTemp);
                SaveShutdownTime(DateTime.Now); // Pristupili smo lokalnoj kopiji
                return true;
            }
            return false;
        }
        

        //Upisavanje kada se poslednji put pristuplio lokalnoj kopiji
        public void SaveShutdownTime(DateTime time)
        {
            // Postavite relativnu putanju u odnosu na folder projekta
            string relativePath = Path.Combine("shutdown.txt");

            // Dobijte apsolutnu putanju od relativne
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);

            File.WriteAllText(filePath, time.ToString());
        }

        //Provera starosti pristupa lokalnoj kopiji
        public void CheckLocalCopyAge()
        {
            // Postavite relativnu putanju u odnosu na folder projekta
            string relativePath = Path.Combine("shutdown.txt");

            // Dobijte apsolutnu putanju od relativne
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);
            // Provera da li fajl postoji
            if (File.Exists(filePath))
            {
                // Čitanje vremena poslednjeg pristupa fajlu
                string dateString = File.ReadAllText(filePath);
                string format = "dd/MM/yyyy HH:mm:ss";
                DateTime.TryParseExact(dateString, format, null, System.Globalization.DateTimeStyles.None, out DateTime lastAccessTime);

                // Provera da li je prošlo više od 24 sata
                int intervalStarosti = int.Parse(ConfigurationManager.AppSettings["IntervalStarostiLokalneKopije"]);

                if (DateTime.Now - lastAccessTime > TimeSpan.FromHours(intervalStarosti))
                {
                    // Fajl je stariji od 24 sata, možete izvršiti odgovarajuće akcije
                    Console.WriteLine("Lokalna kopija je starija od 24 sata.");
                    localDataStore.Clear();
                }
                
            }
            else
            {
                
            }
        }


    }
}
