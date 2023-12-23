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
            
           

           
            // Kreiranje timera koji će pozivati metodu svakih 5 minuta
            Timer timer = new Timer(SendMeasureOn5Minutes, new Tuple<Server, List<Device>>(server, devices), 0, 1 * 60 * 1000); // 5 minuta u milisekundama                    
            Timer timer1 = new Timer(CreateNewMeasure, new Tuple<List<Device>>(devices),0, 1 * 60 * 300);
           
            

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

           

            timer.Dispose();
            timer1.Dispose();
        }

        private static void SendMeasureOn5Minutes(object state)
        {
            // Ovde možete pozvati željenu metodu koja treba da se izvršava svakih 5 minuta
            Console.WriteLine($"Pozvana je metoda u: {DateTime.Now}");


            var arguments = (Tuple<Server, List<Device>>)state;
            Server server = arguments.Item1;
            List<Device> devices = arguments.Item2;



            SendMeasureToServerDTO sendMeasureToServerDTO = new SendMeasureToServerDTO();

            foreach(Device device in devices)
            {
                sendMeasureToServerDTO.SlanjeMerenja(server, device);
               // device.Measurements.Clear();
            }


        }
        private static void CreateNewMeasure(object state)
        {
            Console.WriteLine($"Merenje je izv u: {DateTime.Now}");

            var arguments = (Tuple<List<Device>>)state;
            List<Device> devs = arguments.Item1;
            Random random = new Random();

            foreach(Device dev in devs)
            {
                dev.Measurements.Clear();
                for(int i = 0; i < 10; i++)
                {
                    dev.RecordMeasurement(random);
                }
            }
        }
        private static void AddDeviceToList(List<Device> d)
        {
            Console.WriteLine("Unesi ID: ");
            int devID= int.Parse(Console.ReadLine());

            d.Add(new Device(devID));
        }
    }
}
