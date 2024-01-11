using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ProjekatProxy
{
    public class Client : IClient
    {
        public readonly string Name;
        private readonly List<double> receivedData; // Lista za čuvanje primljenih podataka
        private List<Measurement> measurList; // Lista za čuvanje primljenih podataka
        private TcpClient tcpClient; // Za konekciju sa Proxy-jem
        
        private ServerListenClient slc= new ServerListenClient();


        public Client(string name)
        {
            Name = name;
            receivedData = new List<double>();

            try
            {

                tcpClient = new TcpClient("127.0.0.1", 5000);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "Client not connected on proxy");
            }
        }

        //Metoda za slanje poruke proxy-ju
        public void SendMessage()
        {
            slc.SendMessage(tcpClient);        
        }

        //Metoda za slanje vec unapred definisane poruke //ZA IZBOR OPERACIJE..
        public void SandMessage(string message)
        {
            slc.SendMessageToServer(message, tcpClient);
           
        }

        //Prihvatanje podataka od strane proxy-ja
        public void AcceptDataFromProxy()
        {
           measurList = new List<Measurement>();
           measurList= slc.AcceptDataFromServer(tcpClient);
            if (measurList != null)
            {
                foreach (Measurement measurement in measurList)
                {
                    Console.WriteLine(measurement);
                }
            }
            else
            {
                Console.WriteLine("Izabrali ste praznu opciju");
            }
        }


        // Metoda za logovanje događaja
        private void LogEvent(string message)
        {
            Console.WriteLine($"[Client] {DateTime.Now}: {message}");
        }


    }
}
