using Moq;
using NUnit.Framework;
using ProjekatProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectTest.Testovi
{
    public class DeviceTests
    {
        //Metoda za dodavanje uredjaja u listu iz program.cs
        private static bool AddDeviceToList(List<Device> d, Server server, CreateMeasure cm, SendMeasureToServerOn5Minutes sm,
            IConsoleReader consoleReader)
        {
            try
            {
                Console.Write("Unesi ID: ");
                int devID = int.Parse(consoleReader.ReadLine());
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
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            return true;

        }


        //TESTIRANJE za unos ID-jeva uredjaja
        public void AddDeviceToList_ShouldAddDeviceToList(List<Device> d,Server server,CreateMeasure cm,SendMeasureToServerOn5Minutes sm)
        {

            var mockConsoleReader = new Mock<IConsoleReader>();
            mockConsoleReader.Setup(cr => cr.ReadLine()).Returns("ff"); // Simulacija unosa sa konzole

            var yourClassInstance = new DeviceTests(); 

            var result = DeviceTests.AddDeviceToList(d, server, cm, sm, mockConsoleReader.Object);

            if(result == false )
            {
                Console.WriteLine("Greska klijent ne sme da unese slova za ID uredjaja");
            }

            mockConsoleReader.Setup(cr => cr.ReadLine()).Returns("2"); // Simulacija unosa sa konzole
            result = DeviceTests.AddDeviceToList(d, server, cm, sm, mockConsoleReader.Object);

            mockConsoleReader.Setup(cr => cr.ReadLine()).Returns("2"); // Simulacija unosa sa konzole
            result = DeviceTests.AddDeviceToList(d, server, cm, sm, mockConsoleReader.Object);

            if(result == false )
            {
                Console.WriteLine("Klijent ne sme da unese dva ista ID-ja");
            }           

            Console.WriteLine("Uspesno su prosli testovi za unos id uredjaj");


        }

        // TESTIRANJE za kreiranje merenja
        public void Test_CreateNewMeasure()
        {

            Device device = new Device(0);
            CreateMeasure createMeasure = new CreateMeasure();

            CreateMeasure.CreateNewMeasure(new Tuple<Device>(device));

            Assert.That(device.Measurements.Count, Is.EqualTo(2)); // Očekujemo da će biti zabeleženo tačno 2 merenja.
            Console.WriteLine("Uspesno kreirano merenje");
        }

        // TESTIRANJE za zapisivanje merenja
        public void Test_RecordMeasurement()
        {

            Device device = new Device(1);

            device.RecordMeasurement(new Random());

            Assert.That(device.Measurements.Count, Is.EqualTo(1));
            Console.WriteLine("Uspesno izmereno merenje");
        }

    }
}
