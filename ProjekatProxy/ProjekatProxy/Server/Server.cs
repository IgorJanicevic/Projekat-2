using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace ProjekatProxy
{
    public class Server : IServer
    {
       // public List<Measurement> dataStore; // Za čuvanje podataka o merenjima

        private ServerListenClient slp = new ServerListenClient();

        private TcpListener tcpListener; // Za slusanje zahteva od proxy
        private TcpClient tcpClient; // 


        public Server(int port)
        {
            //dataStore = new List<Measurement>();

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
        public string AcceptMessageFromProxy()
        {
            //Console.WriteLine("OKER P");
            string option = slp.StartReading(tcpClient);
            //Console.WriteLine("GGGGGGGGG");
            return option;
                       
        }











        // Metoda za upis podataka o merenjima
        public void WriteData(Measurement measurement)
        {
            //dataStore.Add(measurement);
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
