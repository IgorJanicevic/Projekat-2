using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace ProjekatProxy
{
    public class Server : IServer
    {
        public List<Measurement> dataStore; // Za čuvanje podataka o merenjima
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


            switch (brOperacije)
            {
                case 1:
                    AllDataFromID(devID); // Svi podaci odadbranog ID-ja
                    break;
                case 2:
                    LastUpdatedValueID(devID); // Poslednja azurirana vrednost odredjenog ID-ja
                    break;
                case 3:
                    LastUpdatedValueAllID(); // Poslednje azurirane vrednosti svih uredjaja
                    break;
                case 4:
                    AllAnalogData(); // Sva analogna merenja
                    break;
                case 5:
                    AllDigitalData(); // Sva digitalna merenja
                    break;

            }

            return option;            
        }


        private void AllDataFromID(int devID)
        {
            List<Measurement> data=orders.AllDataFromID(devID);
            
            
        }

        private Measurement LastUpdatedValueID(int devID)
        {
            List<Measurement> measurements = orders.AllDataFromID(devID);
            Measurement first = measurements[0];

            for (int i = 1; i < measurements.Count; i++)
            {
                if (first.Timestamp.CompareTo(measurements[i].Timestamp) < 0)
                {
                    first = measurements[i];
                }
            }

            Console.WriteLine(first);
            return first;
        }

        //Metoda koja kupuje sve poslednje azuriranje vrednosti svakog ID-ja
        public void LastUpdatedValueAllID()
        {
            //Lista svih id u bazi podataka
            List<int> devIDs= orders.GetAllDeviceID();
        
            List<Measurement> measurements= new List<Measurement>(); 

            Dictionary<int,Measurement> data = new Dictionary<int,Measurement>();

            //Prolazimo kroz svaki device i uzimamo njegovu poslednju azuriranu vrednost
            foreach (int id in devIDs)
            {
                Measurement m = this.LastUpdatedValueID(id);
                measurements.Add(m); 
                data.Add(id, m); //dodajemo je u recnik zajedno sa idijem uredja
            }
            
                foreach (var item in data)
                    Console.WriteLine("ID " + item.Key + ":\t" + item.Value);
            
        }

        //Sva analogna merenja
        private void AllAnalogData()
        {
            //Svi uredjaji
            List<int> devIDs = orders.GetAllDeviceID();
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
            foreach(var item in analogAll)
                Console.WriteLine(item);
        }

        //Metoda za sva digitalna merenja
        private void AllDigitalData()
        {
            List<int> devIDs = orders.GetAllDeviceID();
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
            foreach (var item in digitalAll)
                Console.WriteLine(item);
        }













        // Metoda za upis podataka o merenjima
        public void WriteData(Measurement measurement)
        {
            dataStore.Add(measurement);
            LogEvent($"{measurement}");
        }              

        // Metoda za logovanje događaja
        public void LogEvent(string message)
        {
       
            string filePath = "C:\\Users\\HomePC\\Documents\\GitHub\\Projekat-2\\ProjekatProxy\\ProjekatProxy\\Server\\BazaPodataka.txt";

           
                File.AppendAllText(filePath, message + "\n");
            

        }       

    }
}
