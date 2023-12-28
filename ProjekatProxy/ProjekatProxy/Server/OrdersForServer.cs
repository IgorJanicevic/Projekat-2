using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjekatProxy
{
    public class OrdersForServer
    {
        string filePath = "C:\\Users\\HomePC\\Documents\\GitHub\\Projekat-2\\ProjekatProxy\\ProjekatProxy\\Server\\BazaPodataka.txt";
        //Metoda za dobavljanje svih merenja od odrjdenog ID-ja
        public List<Measurement> AllDataFromID(int devID)
        {
            List<Measurement> data = new List<Measurement>();
            int currentDeviceID = 0;

            // Otvaramo fajl
            if (File.Exists(filePath))
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        //U bazi podataka proveravamo da li su merenja koja trazimo iz datog uredjaja
                        if (line.StartsWith("Device ID:"))
                        {
                            int.TryParse(line.Substring("Device ID:".Length).Trim(), out currentDeviceID);
                        }
                        else if(currentDeviceID == devID)
                        {
                            //Dodajemo merenje u listu
                            data.Add(ParseMeasurement(line));
                        }
                        
                        
                    }
                }
            }
            else
            {
                Console.WriteLine("Fajl ne postoji.");
            }
            foreach(Measurement m in data)
                Console.WriteLine(m);

            return data;
        }


        public List<int> GetAllDeviceID()
        {
            List<int> deviceID = new List<int>();
            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        // Proveri da li linija sadrži "Device ID:".
                        if (line.Contains("Device ID:"))
                        {
                            // Izvuci Device ID vrednost iz linije.
                            int id = ExtractDeviceID(line);

                            foreach (int postojeci in deviceID)
                            {
                                if (id == postojeci)
                                {
                                    id = -1;
                                    break;
                                }
                            }

                            // Ako je izvučeno, dodaj u listu.
                            if (id != -1)
                            {
                                deviceID.Add(id);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom čitanja fajla: {ex.Message}");
            }
           
            // Pretvori listu u niz.
            return deviceID;
        }
       


        //Izvlacenje ID-ja iz DEVICE ID: .. iz .txt
        private static int ExtractDeviceID(string line)
        {
            
            try
            {
                // Parsiraj u celobrojnu vrednost.
                string ff = line.Substring(11);
                int br = int.Parse(ff);
                return br;
            }
            catch
            {
                // Ako dođe do greške, vrati -1.
                Console.WriteLine("Greska prilikom prikuljanja ID-ja za ispis poslednji azuriranih vrednosti");
                return -1;
            }
        }





        //Metoda za pretvaranje linije u merenje
        private Measurement ParseMeasurement(string line)
        {
            try
            {
                // Razdvoji vrednosti na osnovu zareza.
                string[] parts = line.Split(',');

                // Parsiraj vrednosti.
                int id = int.Parse(parts[0]);
                double value = double.Parse(parts[1]);
                bool isAnalog = bool.Parse(parts[2]);
                DateTime timestamp = DateTime.ParseExact(parts[3], "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                // Kreiraj Measurement objekat.
                Measurement measurement = new Measurement(id, isAnalog, value, timestamp);

                return measurement;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom parsiranja merenja: {ex.Message}");
                return null;
            }
        }
    }
}
