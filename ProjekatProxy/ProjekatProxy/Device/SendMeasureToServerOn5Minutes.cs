using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProjekatProxy
{
    public class SendMeasureToServerOn5Minutes
    {
        private Timer timer1;
        public void SendMeasure(Server server, List<Device> devices)
        {
            timer1 = new Timer(SendMeasureOn5Minutes, new Tuple<Server, List<Device>>(server, devices), 0, 1 * 60 * 1000); // 5 minuta u milisekundama                             
        }

        public void Dispose()
        {
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

            foreach (Device device in devices)
            {
                sendMeasureToServerDTO.SlanjeMerenja(server, device);
                device.Measurements.Clear();
            }


        }
    }
}
