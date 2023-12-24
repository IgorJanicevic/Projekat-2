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
            
          //Kreiranje instanci Servera, Proxy-ja i Klijenta
            Server server = new Server(8080);
            Proxy proxy = new Proxy(server, TimeSpan.FromHours(24)); // Postavljamo vreme isteka lokalnih kopija na 24 sata
            server.AcceptProxy();
            server.AcceptMessageFromProxy();


            Client client = new Client();
            proxy.ProxyAcceptClient();          
            proxy.AcceptClientMessage();

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("**************************************************************************************");

            //Lista uredjaja
            List<Device> devices = new List<Device>();
            
          //string za izbor u hendleru
            string temp1;

          //Kreiranje merenja svaki minut, za svaki uredjaj
            CreateMeasure cm = new CreateMeasure();          

          //Slanje svih merenja sa svih uredjaja koji se ne nalaze na serveru
            SendMeasureToServerOn5Minutes sm = new SendMeasureToServerOn5Minutes();



            do
            {

                Console.WriteLine("Izaberi opciju: ");
                Console.WriteLine("1 - Kreiraj novi device");
                Console.WriteLine("2 - Klijentski pod..");
                Console.WriteLine("X - Izlaz");

                temp1 = Console.ReadLine();

                switch(temp1)
                {
                    case "1":
                        AddDeviceToList(devices,server,cm,sm);
                        break;
                    
                }
             

            } while (!temp1.ToUpper().Equals("X"));

            

            /*
            //Kreiranje merenja svaki minut, za svaki uredjaj
            CreateMeasure cm = new CreateMeasure();
            cm.Create(devices);

            //Slanje svih merenja sa svih uredjaja koji se ne nalaze na serveru
            SendMeasureToServerOn5Minutes sm= new SendMeasureToServerOn5Minutes();
            sm.SendMeasure(server, devices);*/
           
            

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
  
            cm.Dispose();
            sm.Dispose();

            

            
        }

        

        private static bool AddDeviceToList(List<Device> d,Server server,CreateMeasure cm,SendMeasureToServerOn5Minutes sm)
        {
            try
            {
                Console.WriteLine("Unesi ID: ");
                int devID = int.Parse(Console.ReadLine());
                Device tempDev = new Device(devID);
               
                foreach(Device dev in d)
                {
                    if (dev.UniqueID == devID)
                    {
                        Console.WriteLine("Vec postoji uredjaj sa tom ID oznakom");
                        return false;
                    }
                }
                
                d.Add(tempDev);


                //Kreiranje merenja svaki minut, za svaki uredjaj
                cm.Create(tempDev);

                //Slanje svih merenja sa svih uredjaja koji se ne nalaze na serveru
                sm.SendMeasure(server, tempDev);

            }catch(FormatException ex)
            {
                Console.WriteLine(ex.Message);
            }catch(Exception ex){
                Console.WriteLine(ex.Message);
            }

            return true;

        }
    }
}
