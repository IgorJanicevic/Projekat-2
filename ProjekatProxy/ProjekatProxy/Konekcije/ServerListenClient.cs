using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ProjekatProxy
{
    public class ServerListenClient
    {
        private TcpClient tcpClient;

        //Metoda za prihvatanje zahteva od strane proxy
        public TcpClient AcceptClient(TcpListener tcpListener)
        {

            // Čekaj na konekciju od proxy-ja
            tcpClient = tcpListener.AcceptTcpClient();
            Console.WriteLine("Client connected to server");            

            return tcpClient;

        }

        //Metoda za citanje poruke od strane proxy
        public string StartReading(TcpClient tcpClient)
        {
            NetworkStream networkStream = tcpClient.GetStream();
            byte[] buffer = new byte[4096];
            int bytesRead;


            bytesRead = networkStream.Read(buffer, 0, buffer.Length);
            if (bytesRead > 0) {
                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine("Received message from proxy: " + message);
                return message;

                // Ovde možete implementirati logiku za obradu poruke od proxy-ja
            }

            return null;

        }
    }
}
