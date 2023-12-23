using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ProjekatProxy
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Kreiranje instanci Servera, Proxy-ja i Klijenta
            Server server = new Server();
            Proxy proxy = new Proxy(server, TimeSpan.FromHours(24)); // Postavljamo vreme isteka lokalnih kopija na 24 sata
            Client client = new Client();

            // Kreiranje instanci uređaja
            Device device1 = new Device(1);
            Device device2 = new Device(2);
            
            for(int i = 0;i < 5; i++)
            {
                device1.RecordMeasurement();
              
            }
            
            

            SendMeasureToServerDTO sendMeasureToServerDTO = new SendMeasureToServerDTO();
            sendMeasureToServerDTO.SlanjeMerenja(server,device1);

            // Simulacija slanja merenja sa uređaja  POTREBNA JE IZMENA
           // device1.RecordMeasurement(75.0, isAnalog: true);
          //  device2.RecordMeasurement(120.0, isAnalog: false);

            // Simulacija zahteva klijenta preko proxy-ja
            var lastAccessTime = DateTime.Now.AddMinutes(-10); // Poslednji pristup pre 10 minuta
            var requestData1 = proxy.ProcessClientRequest(device1.UniqueID, lastAccessTime);
            var requestData2 = proxy.ProcessClientRequest(device2.UniqueID, lastAccessTime);

            // Simulacija primanja podataka od servera od strane klijenta
            client.ReceiveDataFromServer(requestData1);
            client.ReceiveDataFromServer(requestData2);

            // Prikaz rezultata
            Console.WriteLine($"Data received by client for Device {device1.UniqueID}:");
            foreach (var data in client.GetAllAnalogData())
            {
                Console.WriteLine($"- {data}");
            }

            Console.WriteLine($"Data received by client for Device {device2.UniqueID}:");
            foreach (var data in client.GetAllDigitalData())
            {
                Console.WriteLine($"- {data}");
            }
        }
    }
}
