using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using ProjectTest.Testovi;
using ProjekatProxy;

namespace ProjectTest
{
    internal class Program
    {
        static ClientHandler ch = new ClientHandler(); //Klijentski hender zbog liste klijenata

        static void Main(string[] args)
        {
            Console.WriteLine("--------------HEADER--------------");
            //Kreiranje instanci Servera, Proxy-ja i Klijenta
            Server server = new Server(8080);
            Proxy proxy = new Proxy(server); // Postavljamo vreme isteka lokalnih kopija na 24 sata
            server.AcceptProxy();
            Console.WriteLine("----------------------------------");

            //Lista uredjaja
            List<Device> devices = new List<Device>();


            //Kreiranje merenja svaki minut, za svaki uredjaj
            CreateMeasure cm = new CreateMeasure();

            //Slanje svih merenja sa svih uredjaja koji se ne nalaze na serveru
            SendMeasureToServerOn5Minutes sm = new SendMeasureToServerOn5Minutes();


            //TESTOVI

            DeviceTests deviceTests = new DeviceTests();
            deviceTests.AddDeviceToList_ShouldAddDeviceToList(devices, server, cm, sm);


            MyTests myTests = new MyTests();
            myTests.Test_CreateNewMeasure();
            myTests.Test_RecordMeasurement();
            myTests.Setup_Client();
            




            Console.WriteLine("\nAplikacija radi. Pritisnite Enter da završite.");
            Console.ReadLine();

            proxy.SaveShutdownTime(DateTime.Now);
            cm.Dispose();
            sm.Dispose();




        }
       
    }
}
