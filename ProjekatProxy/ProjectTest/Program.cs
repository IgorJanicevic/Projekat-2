using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

            // Provera starosti lokalne kopije
            int intervalZaProveruLokalneKopije = int.Parse(ConfigurationManager.AppSettings["IntervalZaProveruLokalneKopije"]);
            System.Threading.Timer timer = new System.Threading.Timer(LocalCopy, proxy, 0, intervalZaProveruLokalneKopije);

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

                Console.WriteLine("\nIzaberi opciju: ");
                Console.WriteLine("1 - Kreiraj novi device");
                Console.WriteLine("2 - Klijentski meni");
                Console.WriteLine("X - Izlaz\n");

                temp1 = Console.ReadLine();

                switch (temp1)
                {
                    case "1":
                        AddDeviceToList(devices, server, cm, sm);
                        break;
                    case "2":
                        ClientMenu(proxy, server);
                        break;

                }


            } while (!temp1.ToUpper().Equals("X"));




            Console.WriteLine("Aplikacija radi. Pritisnite Enter da završite.");
            Console.ReadLine();

            proxy.SaveShutdownTime(DateTime.Now);
            cm.Dispose();
            sm.Dispose();




        }



        private static void ClientMenu(Proxy pp, Server server)
        {
            ch.Handler(pp, server);
        }

        //Dodavanje uredjaja u listu kao i kreiranje njegovih merenja i slanje na serveru
        private static bool AddDeviceToList(List<Device> d, Server server, CreateMeasure cm, SendMeasureToServerOn5Minutes sm)
        {
            try
            {
                Console.Write("Unesi ID: ");
                int devID = int.Parse(Console.ReadLine());
                Device tempDev = new Device(devID);

                foreach (Device dev in d)
                {
                    if (dev.UniqueID == devID)
                    {
                        Console.WriteLine("Vec postoji uredjaj sa tom ID oznakom");
                        return false;
                    }
                }

                d.Add(tempDev);
                Console.WriteLine("Uspesno ste kreirali uredjaj");

                //Kreiranje merenja svaki minut, za svaki uredjaj
                cm.Create(tempDev);
                Thread.Sleep(200); // Da slucajno ne dodje do preplitanja kreiranja merenja i slanja 

                //Slanje svih merenja sa svih uredjaja koji se ne nalaze na serveru
                sm.SendMeasure(server, tempDev);

            }
            catch (FormatException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return true;

        }

        //Metoda za proveru starosti lokalne kopije
        private static void LocalCopy(object state)
        {

            Proxy proxy = (Proxy)state;
            proxy.CheckLocalCopyAge();
        }
    }
}
