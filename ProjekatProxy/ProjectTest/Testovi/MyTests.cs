using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Reflection;
using ProjekatProxy;
using System.Net.Sockets;
using System.IO;
using NUnit.Framework.Legacy;

namespace ProjectTest
{
    // Dummy klasa Server za potrebe testiranja
    public class DummyServer : Server
    {
        public DummyServer() : base(8080) { }
    }

    public class MyTests
    {
        //DEVICE

        // TESTIRANJE CreateNewMeasure iz CreateMeasurement
        public void Test_CreateNewMeasure()
        {
            // Arrange
            Device device = new Device(0);
            CreateMeasure createMeasure = new CreateMeasure();

            // Act
            CreateMeasure.CreateNewMeasure(new Tuple<Device>(device));

            // Assert
            Assert.That(device.Measurements.Count, Is.EqualTo(2)); // Očekujemo da će biti zabeleženo tačno 2 merenja.
            Console.WriteLine("Uspesno kreirano merenje");
        }

        // TESTIRANJE RecorcMeasurement iz Device.cs
        public void Test_RecordMeasurement()
        {
            // Arrange
            Device device = new Device(1);

            // Act
            device.RecordMeasurement(new Random());

            // Assert
            Assert.That(device.Measurements.Count, Is.EqualTo(1));
            // Možete dodati dodatne asertacije kako biste proverili tačnost snimljenog merenja.
            // Na primer: Assert.That(device.Measurements[0].Value, Is.GreaterThan(0));
            Console.WriteLine("Uspesno izmereno merenje");
        }
        private Server server;

        
        //SERVER


        [SetUp]
        public void Setup_Server()
        {
            // Inicijalizujemo Server pre svakog testa
            server = new Server(8080);
        }

        // TESTIRANJE AcceptMessageFromProxy iz Server
        public void Test_AcceptMessageFromProxy()
        {
            // Arrange
            server.AcceptProxy();

            // Act
            string result = server.AcceptMessageFromProxy(0);

            // Assert
            //Assert.IsNotNull(result);
        }

        // TESTIRANJE AcceptProxy iz Server
        public void Test_AcceptProxy()
        {
            // Act
            server.AcceptProxy();

            // Assert
            //Assert.IsNotNull(server.tcpClient);
        }

        // TESTIRANJE LogEvent iz Server
        public void Test_LogEvent(Server s)
        {
            // Arrange
            string message = "Test log message";

            // Act
            server.LogEvent(message);

            // Assert
            // Postavite relativnu putanju u odnosu na folder projekta
            string relativePath = Path.Combine("BazaPodataka.txt");

            // Dobijte apsolutnu putanju od relativne
            string absolutePath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);

            string logContent = File.ReadAllText(absolutePath);
            //Assert.IsTrue(logContent.Contains(message));
        }


        //CLIENT

        private Client client;

        [SetUp]
        public void Setup_Client()
        {
            // Inicijalizujemo Client pre svakog testa
            client = new Client("TestClient");
        }

        // TESTIRANJE SencMessage iz Client
        public void Test_SendMessage()
        {
            try
            {
                // Arrange
                using (var sw = new StringWriter())
                {
                    Console.SetOut(sw);

                    // Act
                    client.SendMessage();

                    // Assert
                    string expected = "[Client]"; // Očekujemo da će ispis na konzoli sadržavati "[Client]"
                    StringAssert.Contains(expected, sw.ToString());
                }
            }catch(FormatException)
            {
                Console.WriteLine("Uneli ste pogresan format ID-ja");
            }catch(Exception e)
            {
                Console.WriteLine("Desila se greska pri slanju poruke");
            }
        }

        // TESTIRANJE AcceptDataFromProxy iz Client
        public void Test_AcceptDataFromProxy()
        {
            // Arrange
            var measurementList = new List<Measurement>
            {
                new Measurement (1, true, 3.14, DateTime.Now),
                new Measurement (1, true, 3.14, DateTime.Now)
            };

            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                // Act
                client.AcceptDataFromProxy();

                // Assert
                foreach (var measurement in measurementList)
                {
                    string expected = measurement.ToString(); // Očekujemo da će ispis na konzoli sadržavati string reprezentaciju svakog merenja
                    StringAssert.Contains(expected, sw.ToString());
                }
            }
        }

    }
}
