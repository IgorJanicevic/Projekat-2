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
        }

        //PROXY

        // TESTIRANJE ProxyAcceptClient iz Proxy - proveriti zbog TCP-a ako moze bolje
        public void Test_ProxyAcceptClient()
        {
            // Arrange
            var proxy = new Proxy(new DummyServer(), TimeSpan.FromMinutes(5));

            // Act
            proxy.ProxyAcceptClient("Client1");

            // Assert
            var tcpClientsField = typeof(Proxy).GetField("tcpClients", BindingFlags.NonPublic | BindingFlags.Instance);
            var tcpClientsValue = (Dictionary<string, TcpClient>)tcpClientsField.GetValue(proxy);

            Assert.That(tcpClientsValue.ContainsKey("Client1"), Is.True);
        }

        // TESTIRANJE AcceptClientMessage iz Proxy
        public void Test_AcceptClientMessage()
        {
            // Arrange
            var proxy = new Proxy(new DummyServer(), TimeSpan.FromMinutes(5));
            proxy.ProxyAcceptClient("Client1");

            // Act
            var result = proxy.AcceptClientMessage("Client1", 1);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        // TESTIRANJE AcceptDataFromServer iz Proxy - proveri zbog private dataFromServer
        public void Test_AcceptDataFromServer()
        {
            // Arrange
            var proxy = new Proxy(new DummyServer(), TimeSpan.FromMinutes(5));

            // Act
            proxy.AcceptDataFromServer();

            // Assert
            var dataFromServerField = typeof(Proxy).GetField("dataFromServer", BindingFlags.NonPublic | BindingFlags.Instance);
            var dataFromServerValue = (List<Measurement>)dataFromServerField.GetValue(proxy);

            Assert.That(dataFromServerValue.Count, Is.GreaterThan(0));
            // Dodajte dodatne asertacije kako biste proverili tačnost lokalno sačuvanih podataka.
        }

        // TESTIRANJE SendDataToClient iz Proxy
        public void Test_SendDataToClient()
        {
            // Arrange
            var proxy = new Proxy(new DummyServer(), TimeSpan.FromMinutes(5));
            proxy.ProxyAcceptClient("Client1");
            proxy.AcceptDataFromServer();

            // Act
            proxy.SendDataToClient();

            // Assert
            // Dodajte asertacije kako biste proverili da li su podaci uspešno poslati klijentu.
        }

        //SERVER

        private Server server;

        [SetUp]
        public void Setup_Server()
        {
            // Inicijalizujemo Server pre svakog testa
            server = new Server(8080);
        }

        /**
        public void test_WriteData()
        {
            // Arrange
            Measurement measurement = new Measurement
            {
                // Postavite potrebne vrednosti za merenje
            };            

            // Act
            server.WriteData(measurement);

            // Assert
            Assert.Contains(measurement, server.dataStore);
        }
        */

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
        public void Test_LogEvent()
        {
            // Arrange
            string message = "Test log message";

            // Act
            server.LogEvent(message);

            // Assert
            string filePath = "C:\\Users\\HomePC\\Documents\\GitHub\\Projekat-2\\ProjekatProxy\\ProjekatProxy\\Server\\BazaPodataka.txt";
            string logContent = File.ReadAllText(filePath);
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
