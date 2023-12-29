using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
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

        //Metoda za citanje poruke 
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

        //Metoda za slanje poruke serveru sa odredjenom naredbom  
        public void SendMessageToServer(string message,TcpClient client)
        {
            try
            {
                NetworkStream networkStream = client.GetStream();
                byte[] buffer = Encoding.ASCII.GetBytes(message);
                networkStream.Write(buffer, 0, buffer.Length);
                Console.WriteLine("Sent message to server: " + message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "PROXY MESSAGE");
            }
        }

        //Metoda za slanje poruka sa unosom preko konzole
        public void SendMessage(TcpClient tcpClient)
        {
            try
            {
                Console.Write("Unesi poruku koju zelis da posaljes proxy: ");
                string message = Console.ReadLine();
                int temp = int.Parse(message);
                NetworkStream networkStream = tcpClient.GetStream();
                byte[] buffer = Encoding.ASCII.GetBytes(message);
                networkStream.Write(buffer, 0, buffer.Length);
                Console.WriteLine("Sent message to server: " + message);

            }catch (Exception ) 
            {
                Console.WriteLine("Doslo je do greske, ne postoji nijedan klijent!");
            }

        }




        //Metoda za slanje liste podataka sa servera proxy-ju
        public void SandList(List<Measurement> lista, TcpClient tcpClient)
        {
            // Kreirajte memoriju za serijalizaciju
            MemoryStream memoryStream = new MemoryStream();

            // Koristite BinaryFormatter za serijalizaciju liste
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(memoryStream, lista);

            // Dohvatite bajtove iz memorije
            byte[] serializedData = memoryStream.ToArray();

            // Šaljete podatke na proxy
            NetworkStream stream = tcpClient.GetStream();
            stream.Write(serializedData, 0, serializedData.Length);
        }


        //Metoda za prihvatanje liste podataka od strane servera
        public List<Measurement> AcceptDataFromServer(TcpClient tcpClient)
        {
            // Čitate podatke sa servera
            NetworkStream stream = tcpClient.GetStream();
            BinaryFormatter formatter = new BinaryFormatter();

            // Dohvatite bajtove sa mreže
            byte[] receivedData = new byte[4096]; // Prilagodite veličinu bafera prema vašim potrebama
            int bytesRead = stream.Read(receivedData, 0, receivedData.Length);

            // Kreirajte memoriju za deserijalizaciju
            MemoryStream memoryStream = new MemoryStream(receivedData, 0, bytesRead);

            // Deserijalizujete podatke u listu
            List<Measurement> receivedMeasurements = (List<Measurement>)formatter.Deserialize(memoryStream);
       

            if(receivedMeasurements.Count== 0) {
                Console.WriteLine("\nUneli ste uredjaj sa nepostojecim ID-jem!");
            }

            return receivedMeasurements;
        }


    }
}
