using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ProjekatProxy
{
  
    public class SendMeasureToServerDTO
    {
       

        public void SlanjeMerenja(Server server,Device device)
        {
            try
            {

                string devID = String.Format("{0}", device.UniqueID);
                server.LogEvent("Device ID: "+devID);
                

                foreach (Measurement m in device.Measurements)
                {
                   
                    string measureString = String.Format("{0},{1},{2},{3}", m.DeviceID, m.Value, m.IsAnalog ? 1 : 0, m.Timestamp);                 
                    server.WriteData(m);                             
                }
                
               

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "GRESKA PRI SLANJU!!!!!");
            }

            
        }
       
    }
}
