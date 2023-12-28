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
     

        public void Create(Device dev)
        {
            try
            {
                timer = new Timer(CreateNewMeasure, new Tuple<Device>(dev), 0, 1 * 60 * 100);
            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }

        public void Dispose()
        {
            if(timer != null) 
            timer.Dispose();
        }

        public static void CreateNewMeasure(object state)
        {
            Console.WriteLine($"Merenje je izv u: {DateTime.Now}");

            var arguments = (Tuple<Device>)state;
            Device dev = arguments.Item1;
            Random random = new Random();
           
                //dev.Measurements.Clear();
                for (int i = 0; i < 1; i++)
                {
                    dev.RecordMeasurement(random);
                }
            
        }
    }
}
