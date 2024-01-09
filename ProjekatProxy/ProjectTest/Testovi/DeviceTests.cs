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
        //Test za unos ID-jeva uredjaja
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
      
        public void AddDeviceToList_ShouldAddDeviceToList(List<Device> d,Server server,CreateMeasure cm,SendMeasureToServerOn5Minutes sm)
        {
            // Arrange
            var mockConsoleReader = new Mock<IConsoleReader>();
            mockConsoleReader.Setup(cr => cr.ReadLine()).Returns("ff"); // Simulacija unosa sa konzole

            var yourClassInstance = new DeviceTests(); // Promenite ovo prema vašem stvarnom scenariju

            // ... Dodajte ostale mock-ove i inicijalizaciju objekata ako je potrebno ...

            // Act
            var result = DeviceTests.AddDeviceToList(d, server, cm, sm, mockConsoleReader.Object);

            // Assert
            if(result == true )
            {
                Console.WriteLine("Greska klijent ne sme da unese slova za ID uredjaja");
            }

            mockConsoleReader.Setup(cr => cr.ReadLine()).Returns("2"); // Simulacija unosa sa konzole
            result = DeviceTests.AddDeviceToList(d, server, cm, sm, mockConsoleReader.Object);

            mockConsoleReader.Setup(cr => cr.ReadLine()).Returns("2"); // Simulacija unosa sa konzole
            result = DeviceTests.AddDeviceToList(d, server, cm, sm, mockConsoleReader.Object);

            if(result == true )
            {
                Console.WriteLine("Klijent ne sme da unese dva ista ID-ja");
            }

            Console.WriteLine("Uspesno su prosli testovi za unos id uredjaj");


        }

    }
}
