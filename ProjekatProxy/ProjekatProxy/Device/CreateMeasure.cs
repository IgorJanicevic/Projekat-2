using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProjekatProxy
{
    public class CreateMeasure
    {        
        private Timer timer;

        

        public void Create(List<Device> dev)
        {
            try
            {
                timer = new Timer(CreateNewMeasure, new Tuple<List<Device>>(dev), 0, 1 * 60 * 300);
            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }

        public void Dispose()
        {
            timer.Dispose();
        }

        public static void CreateNewMeasure(object state)
        {
            Console.WriteLine($"Merenje je izv u: {DateTime.Now}");

            var arguments = (Tuple<List<Device>>)state;
            List<Device> devs = arguments.Item1;
            Random random = new Random();

            foreach (Device dev in devs)
            {
                //dev.Measurements.Clear();
                for (int i = 0; i < 5; i++)
                {
                    dev.RecordMeasurement(random);
                }
            }
        }
    }
}
