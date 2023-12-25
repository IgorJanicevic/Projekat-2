using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjekatProxy
{
    public class ClientHandler
    {
        private List<Client> clients = new List<Client>();
        private Client currentClient=null;
        private Proxy proxy=null;
        private Server server=null;

        public void Handler(Proxy pp,Server ss)
        {
            proxy = pp;
            string temp=null;
            server = ss;
            

            do
            {
                Console.WriteLine("\nIzaberite opciju: ");
                Console.WriteLine("1 - Kreiraj novog klijenta");
                Console.WriteLine("2 - Izaberi klijenta sa kojim zelis da trazis ostale podatke");
                Console.WriteLine("3 - Potrazi podatke");
                Console.WriteLine("X - Izlaz\n");

                temp= Console.ReadLine();

                switch(temp)
                {
                    case "1":
                        CreateNewClient();
                        break;
                    case "2":
                        ChooseClient();
                        break;
                     case "3":
                        SearchForDate(currentClient);
                        break;

                }

            } while (!temp.ToUpper().Equals("X"));
        }


        //Metoda za biranje opcija koje poseduje klijent
        private void SearchForDate(Client current)
        {
            string tmp = null;
            do
            {
                Console.WriteLine("\n1 - Svi podaci odabranog ID-ja");
                Console.WriteLine("2 - Poslednje azuriranje vrednosti odabranog ID-ja");
                Console.WriteLine("3 - Poslednje azuriranje vrednosti svakog ID-ja");
                Console.WriteLine("X - Izlaz\n");

                tmp = Console.ReadLine();

                switch (tmp)
                {
                    case "1":                     
                        SlanjeZahteva(current, proxy, server);                                      
                        break;
                    case "2":
                        SlanjeZahteva(current, proxy, server);
                        break;
                }

            } while(!tmp.ToUpper().Equals("X"));
        }

        private static void SlanjeZahteva(Client current,Proxy proxy,Server server)
        {
            try
            {             
                current.SendMessage();
                proxy.AcceptClientMessage(current.Name);
                server.AcceptMessageFromProxy();

            }catch(Exception e)
            {
                Console.WriteLine(e.Message+"Slanje zahteva");
            }

        }

        private void ChooseClient()
        {
            Console.WriteLine("\n-----------Clients--------------\n");
            foreach (Client client in clients)
            {
                Console.WriteLine(client.Name);
            }
            Console.WriteLine("\n--------------------------------\n");

            string name= Console.ReadLine();
            int br = 0;
            foreach(Client client in clients)
            {
                if (client.Name.ToUpper().Equals(name.ToUpper()))
                {
                    currentClient = client;
                    br = 1;

                }
            }
            if(br == 0) Console.WriteLine("Izabrali ste nepostojeceg klijenta");

        }

        private void CreateNewClient()
        {
            try
            {
                Console.WriteLine("Unesi ime klijenta: ");
                string ime= Console.ReadLine();
                Client client = new Client(ime);

                //Ako ne postoji nijedan klijent onda ce trenutni biti prvi koji se doda
                if(clients.Count() == 0)
                {
                    currentClient= client;
                    
                }
                clients.Add(client);

                proxy.ProxyAcceptClient(ime);
                Console.WriteLine("Uspesno ste kreirali novog klijenta");

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
