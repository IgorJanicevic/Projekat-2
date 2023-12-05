using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjekatProxy
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Kreiranje instanci klase Server, Proxy i Client
            var server = new Server();
            var proxy = new Proxy(server, TimeSpan.FromHours(24)); // Postavljamo vreme isteka lokalnih kopija na 24 sata
            var client = new Client();

            // Kreiranje instance klase Device
            var deviceID = 1;
            Device device = new Device(deviceID, isAnalog: true, initialTime: DateTime.Now);

            // Simulacija slanja merenja sa uređaja na server
            device.RecordMeasurement(50.0);

            // Simulacija zahteva klijenta preko proxy-ja
            var lastAccessTime = DateTime.Now.AddMinutes(-10); // Poslednji pristup pre 10 minuta
            var requestData = proxy.ProcessClientRequest(deviceID, lastAccessTime);

            // Simulacija primanja podataka od servera od strane klijenta
            client.ReceiveDataFromServer(requestData);

            // Prikaz rezultata
            Console.WriteLine("Data received by client:");
            foreach (var data in client.GetAllAnalogData())
            {
                Console.WriteLine($"- {data}");
            }

          

        }
    }
}
