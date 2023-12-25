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

        private void LastUpdatedValueID(int devID)
        {
            throw new NotImplementedException();
        }

        private void LastUpdatedValueAllID()
        {
            throw new NotImplementedException();
        }

        private void AllAnalogData()
        {
            throw new NotImplementedException();
        }

        private void AllDigitalData()
        {
            throw new NotImplementedException();
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
