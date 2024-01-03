using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace ProjekatProxy
{
    public class Server : IServer
    {
        public List<Measurement> dataStore; // Za čuvanje podataka o merenjima
        private DateTime lastChange; //Poslednja izmena
        private OrdersForServer orders= new OrdersForServer();

        private ServerListenClient slp = new ServerListenClient();

        private TcpListener tcpListener; // Za slusanje zahteva od proxy
        private TcpClient tcpClient; // 


        public Server(int port)
        {
            
            dataStore = new List<Measurement>();
            tcpListener = new TcpListener(IPAddress.Any, port);
            tcpListener.Start();
            Console.WriteLine("Server listening on port " + port);

           
        }

        //Metoda za prihvatanje zahteva od proxy
        public void AcceptProxy()
        {
           
            tcpClient= slp.AcceptClient(tcpListener);                      
        }

     
        //Metoda za prijema zahteva od proxy
        public string AcceptMessageFromProxy(int brOperacije)
        {            
            string option = slp.StartReading(tcpClient);
            int devID= int.Parse(option);
            Measurement tempForOne = null;
            

            switch (brOperacije)
            {
                case 0:
                    slp.SendMessageToServer(lastChange.ToString(),tcpClient);  // Vreme poslednje izmene
                    return null;
                case 1:
                   dataStore= AllDataFromID(devID); // Svi podaci odadbranog ID-ja
                    break;
                case 2:
                    tempForOne= LastUpdatedValueID(devID); // Poslednja azurirana vrednost odredjenog ID-ja  
                    if(tempForOne!=null)
                    dataStore.Add(tempForOne);
                    break;
                case 3:
                    dataStore= LastUpdatedValueAllID(); // Poslednje azurirane vrednosti svih uredjaja
                    break;
                case 4:
                    dataStore= AllAnalogData(); // Sva analogna merenja
                    break;
                case 5:
                    dataStore= AllDigitalData(); // Sva digitalna merenja
                    break;

            }
            //Provera da li smo vratili nesto
            if (dataStore == null)
            {
                Console.WriteLine("Ne postoji nijedan uredjaj");
                return null;
            }
            //Provera za slanje
            SandList(dataStore);
           
            dataStore.Clear();

            return option;            
        }

        //Metoda za slanje Liste merenja Proxy-ju
        private void SandList(List<Measurement> lista)
        {
            slp.SandList(lista, tcpClient);
        }

        //Metoda za dobijanje svih podataka odredjenog uredjaja
        private List<Measurement> AllDataFromID(int devID)
        {
            Dictionary<int,List<Measurement>> dataRet= new Dictionary<int,List<Measurement>>(); //Proba da se salje recnik
     
            List<Measurement> data = orders.AllDataFromID(devID);
            dataRet.Add(devID, data);

            return data;            
        }

        //Metoda za dobijanje posledje azuriranje vrednosti odredjenog id-ja
        private Measurement LastUpdatedValueID(int devID)
        {
            Dictionary<int, List<Measurement>> dataRet = new Dictionary<int, List<Measurement>>();

            List<Measurement> measurements = orders.AllDataFromID(devID);
            //Provera da li postoji
            if (measurements.Count() == 0) {
                Console.WriteLine("\nUneli ste uredjaj sa nepostojecim ID-jem");
                return null;
            }

            Measurement first = measurements[0];

            for (int i = 1; i < measurements.Count; i++)
            {
                if (first.Timestamp.CompareTo(measurements[i].Timestamp) < 0)
                {
                    first = measurements[i];
                }
            }
            //Dodavanje jednog
            measurements.Clear();
            measurements.Add(first);
            dataRet.Add(devID,measurements);      

            return first;
        }

        //Metoda koja kupuje sve poslednje azuriranje vrednosti svakog ID-ja
        public List<Measurement> LastUpdatedValueAllID()
        {
            //Lista svih id u bazi podataka
            List<int> devIDs= orders.GetAllDeviceID();
            if (devIDs == null)
                return null;

            List<Measurement> measurements= new List<Measurement>(); 

            Dictionary<int,Measurement> data = new Dictionary<int,Measurement>();

            //Prolazimo kroz svaki device i uzimamo njegovu poslednju azuriranu vrednost
            foreach (int id in devIDs)
            {
                Measurement m = this.LastUpdatedValueID(id);
                measurements.Add(m); 
                data.Add(id, m); //dodajemo je u recnik zajedno sa idijem uredja
            }

            //Provera
            if (measurements.Count == 0) {
                Console.WriteLine("Trenutno ne postoji nijedan uredjaj");
                return null;
            }
            return measurements;
            
        }

        //Sva analogna merenja
        private List<Measurement> AllAnalogData()
        {
            //Svi uredjaji
            List<int> devIDs = orders.GetAllDeviceID();
            //Provera da li postoje uredjaji
            if (devIDs == null)
                return null;
            List<Measurement> analogAll= new List<Measurement>();
            foreach(int id in devIDs)
            {
                //Sva merenja odredjenog uredjaja
                List<Measurement> temp= orders.AllDataFromID(id);
                foreach(Measurement m in temp)
                {
                    if(m.IsAnalog== true) analogAll.Add(m); //Ovde dodajemo ako je analogno merenje
                }
            }
            //Provera
            if (analogAll.Count == 0)
            {
                Console.WriteLine("Trenutno ne postoji nijedan uredjaj");
                return null;
            }

            return analogAll;

        }

        //Metoda za sva digitalna merenja
        private List<Measurement> AllDigitalData()
        {
            List<int> devIDs = orders.GetAllDeviceID();
            //Provera da li postoje uredjaji
            if (devIDs == null)
                return null;

            List<Measurement> digitalAll = new List<Measurement>();
            foreach (int id in devIDs)
            {
                //Sva merenja odredjenog uredjaja
                List<Measurement> temp = orders.AllDataFromID(id);
                foreach (Measurement m in temp)
                {                 
                    if (m.IsAnalog==false) digitalAll.Add(m); //Ovde dodajemo ako je analogno merenje
                }
            }

            //Provera
            if (digitalAll.Count == 0)
            {
                Console.WriteLine("Trenutno ne postoji nijedan uredjaj");
                return null;
            }

            return digitalAll;
        }



        // Metoda za upis podataka o merenjima
        public void WriteData(Measurement measurement)
        {
            dataStore.Add(measurement);
            LogEvent($"{measurement}");
            lastChange = DateTime.Now;
        }              

        // Metoda za logovanje događaja
        public void LogEvent(string message)
        {
       
            string filePath = "C:\\Users\\HomePC\\Documents\\GitHub\\Projekat-2\\ProjekatProxy\\ProjekatProxy\\Server\\BazaPodataka.txt";

           
                File.AppendAllText(filePath, message + "\n");
            lastChange = DateTime.Now;


        }

    }
}
