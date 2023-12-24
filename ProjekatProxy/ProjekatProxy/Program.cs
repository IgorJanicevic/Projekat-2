using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
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
            //SendMeasureToServerDTO sendMeasureToServerDTO = new SendMeasureToServerDTO();

            // Kreiranje instanci uređaja
            Device device1 = new Device(1);
            Device device2 = new Device(2);

            List<Device> devices = new List<Device>();
            devices.Add(device1);
            devices.Add(device2);


            //Kreiranje merenja svaki minut, za svaki uredjaj
            CreateMeasure cm = new CreateMeasure();
            cm.Create(devices);



            //Slanje svih merenja sa svih uredjaja koji se ne nalaze na serveru
            SendMeasureToServerOn5Minutes sm= new SendMeasureToServerOn5Minutes();
            sm.SendMeasure(server, devices);
           
            

            /*
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
            */





            Console.WriteLine("Aplikacija radi. Pritisnite Enter da završite.");
            Console.ReadLine();

            sm.Dispose();
            cm.Dispose();
        }


        private static void AddDeviceToList(List<Device> d)
        {
            Console.WriteLine("Unesi ID: ");
            int devID= int.Parse(Console.ReadLine());

            d.Add(new Device(devID));
        }
    }
}
