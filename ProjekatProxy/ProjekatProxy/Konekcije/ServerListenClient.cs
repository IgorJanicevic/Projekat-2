﻿using System;
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
            tcpClient = tcpListener.AcceptTcpClient();            

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
                return message;
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
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "PROXY MESSAGE");
            }
        }

        //Metoda za slanje poruka sa unosom preko konzole
        public void SendMessage(TcpClient tcpClient)
        {
            string message="";

            Console.Write("Unesi ID: ");
            message = Console.ReadLine();
            int temp = int.Parse(message);

            NetworkStream networkStream = tcpClient.GetStream();
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            networkStream.Write(buffer, 0, buffer.Length);
        }




        //Metoda za slanje liste podataka sa servera proxy-ju
        public void SandList(List<Measurement> lista, TcpClient tcpClient)
        {
            // Kreirajte memoriju za serijalizaciju
            MemoryStream memoryStream = new MemoryStream();

            // Koriscenje BinaryFormatter za serijalizaciju liste
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(memoryStream, lista);

            // Hvatanje bajtova iz memorije
            byte[] serializedData = memoryStream.ToArray();

            // Slanje podataka na proxy
            NetworkStream stream = tcpClient.GetStream();
            stream.Write(serializedData, 0, serializedData.Length);
        }


        //Metoda za prihvatanje liste podataka od strane servera
        public List<Measurement> AcceptDataFromServer(TcpClient tcpClient)
        {
            // Čitanje podataka sa servera
            NetworkStream stream = tcpClient.GetStream();
            BinaryFormatter formatter = new BinaryFormatter();

            // Hvatanje bajtova iz mreze
            byte[] receivedData = new byte[4096]; // Prilagodite veličinu bafera prema vašim potrebama
            int bytesRead = stream.Read(receivedData, 0, receivedData.Length);

            // Kreiranje memorije za deserijalizaciju
            MemoryStream memoryStream = new MemoryStream(receivedData, 0, bytesRead);

            // Deserijalizacija podataka u listu
            List<Measurement> receivedMeasurements = (List<Measurement>)formatter.Deserialize(memoryStream);

            if(receivedMeasurements.Count== 0) {
                Console.WriteLine("\nUneli ste uredjaj sa nepostojecim ID-jem!");
                return null;
            }

            return receivedMeasurements;
        }


    }
}
