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
            Console.WriteLine("Connected to Proxy");

            // Ovde možete implementirati logiku za slanje poruka serveru

            
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
            foreach(Measurement measurement in measurList)
            {
                Console.WriteLine(measurement);
            }
        }



        // Metoda za dobavljanje svih podataka odabranog ID-ja xD
        public List<double> GetDataByDeviceID(int deviceID)
        {
            LogEvent($"Data requested for Device ID: {deviceID}");
            return receivedData.Where(data => data == deviceID).ToList();
        }

        // Metoda za dobavljanje poslednje ažurirane vrednosti odabranog ID-ja            
        public double GetLastUpdatedValueByDeviceID(int deviceID)
        {
            LogEvent($"Last updated value requested for Device ID: {deviceID}");
            return receivedData.LastOrDefault(data => data == deviceID);
        }

        // Metoda za dobavljanje poslednje ažurirane vrednosti svakog uređaja
        public Dictionary<int, double> GetLastUpdatedValuesForEachDevice()
        {
            LogEvent("Last updated values requested for each device");
            var lastUpdatedValues = new Dictionary<int, double>();

            foreach (var deviceID in receivedData.Select(d => (int)d).Distinct())
            {
                var lastValue = receivedData.LastOrDefault(data => (int)data == deviceID);
                lastUpdatedValues.Add(deviceID, lastValue);
            }

            return lastUpdatedValues;
        }

        // Metoda za dobavljanje svih podataka analognih merenja
        public List<double> GetAllAnalogData()
        {
            LogEvent("Analog data requested");
            return receivedData.Where(data => IsAnalogMeasurement(data)).ToList();
        }

        // Metoda za dobavljanje svih podataka digitalnih merenja
        public List<double> GetAllDigitalData()
        {
            LogEvent("Digital data requested");
            return receivedData.Where(data => !IsAnalogMeasurement(data)).ToList();
        }

        // Metoda za proveru da li je merenje analogno
        private bool IsAnalogMeasurement(double data)
        {
            // Implementiraj logiku prepoznavanja analogne vrednosti
            // U ovom primeru, pretpostavljamo da su vrednosti veće od 100 analogne
            return data > 100;
        }

        // Metoda za simulaciju dobijanja podataka od servera
        public void ReceiveDataFromProxy(List<double> data)
        {
            receivedData.AddRange(data);
            LogEvent("Data received from server");
        }

        

        // Metoda za logovanje događaja
        private void LogEvent(string message)
        {
            Console.WriteLine($"[Client] {DateTime.Now}: {message}");
        }


    }
}
