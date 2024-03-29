﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProjekatProxy
{
    public class SendMeasureToServerOn5Minutes
    {
        private Timer timer1;
        public static DateTime lastChange;

        //Metoda koja vraca vreme kada je poslednje merenje poslato serveru
        public DateTime lastUpdate()
        {
            return lastChange;
        }
        public void SendMeasure(Server server, Device devices)
        {
            int intervalZaSlanjeMerenja = int.Parse(ConfigurationManager.AppSettings["IntervalZaSlanjeMerenja"]);
            timer1 = new Timer(SendMeasureOn5Minutes, new Tuple<Server, Device>(server, devices), 0, intervalZaSlanjeMerenja); // 5 minuta u milisekundama                             
        }

        public void Dispose()
        {
            if (timer1 != null)
            timer1.Dispose();
        }

        private static void SendMeasureOn5Minutes(object state)
        {
            //Console.WriteLine($"Pozvana je metoda u: {DateTime.Now}");


            var arguments = (Tuple<Server, Device>)state;
            Server server = arguments.Item1;
            Device device = arguments.Item2;

            lastChange= DateTime.Now; //Vreme poslednje izmene

            SendMeasureToServerDTO sendMeasureToServerDTO = new SendMeasureToServerDTO();

            
                sendMeasureToServerDTO.SlanjeMerenja(server, device);
                device.Measurements.Clear();
            


        }
    }
}
