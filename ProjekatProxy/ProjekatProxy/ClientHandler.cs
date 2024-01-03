using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjekatProxy
{
    public class ClientHandler
    {
        private List<Client> clients; //Lista klijenata
        private Client currentClient=null; // Trenutni klijent sa kojim mozete vrsiti operacije
        private Proxy proxy=null; // Referenca na proxy
        private Server server=null; // Referenca na server

        public ClientHandler()
        {
            clients = new List<Client>();
        }

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
                if (current == null)
                {
                    Console.WriteLine("Molimo vas kreirajte klijenta pre nego zatrazite podatke");
                    return;
                }


                Console.WriteLine("\n1 - Svi podaci odabranog ID-ja");
                Console.WriteLine("2 - Poslednje azuriranje vrednosti odabranog ID-ja");
                Console.WriteLine("3 - Poslednje azuriranje vrednosti svakog ID-ja");
                Console.WriteLine("4 - Sva analogna merenja");
                Console.WriteLine("5 - Sva digitalna merenja");
                Console.WriteLine("X - Izlaz\n");

                tmp = Console.ReadLine();


                switch (tmp)
                {
                    case "1":                     
                        SlanjeZahteva(current, proxy, server,1);                                      
                        break;
                    case "2":
                        SlanjeZahteva(current, proxy, server,2);
                        break;
                    case "3":
                        SlanjeZahtevaBezUnosa(current, proxy, server, 3);
                        break;
                    case "4":
                        SlanjeZahtevaBezUnosa(current,proxy, server,4);
                        break;
                    case "5":
                        SlanjeZahtevaBezUnosa(current, proxy, server, 5);
                        break;
                }

            } while(!tmp.ToUpper().Equals("X"));
        }

        private void SlanjeZahtevaBezUnosa(Client current, Proxy proxy, Server server, int v)
        {
            try
            {
               
                current.SandMessage("0"); // Pomocno slanje poruka kako ne bi morali da unosimo poruku
                if (proxy.AcceptClientMessage(current.Name, v) != null) //Ako postoji lokalna kopija preskace se ovo
                {
                    server.AcceptMessageFromProxy(v);
                    proxy.AcceptDataFromServer();
                    proxy.SendDataToClient();
                }
                current.AcceptDataFromProxy();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void SlanjeZahteva(Client current,Proxy proxy,Server server,int br)
        {
            try
            {  
               
                current.SendMessage(); //Slanje poruke sa trenutnog klijenta

                if (proxy.AcceptClientMessage(current.Name, br) != null) //Prihvatanje poruke od strane proxy + provera + slanje zahteva serveru
                { 
                    if (server.AcceptMessageFromProxy(br) == null)//Prihvatanje poruke od strane server
                    {
                        Console.WriteLine("Trenutno ne postoji nijedan uredjaj\n");
                        return;

                    }
                    proxy.AcceptDataFromServer(); // Proxy prihvata listu merenja sa servera
                    proxy.SendDataToClient();
                }
                current.AcceptDataFromProxy();

            }catch(FormatException)
            {
                Console.WriteLine("ID koji ste uneli nije validan, molimo vas unesite broj");
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message+"Slanje zahteva");
            }

        }

        //Metoda za biranje klijenta sa kojim zelimo da rukujemo
        private void ChooseClient()
        {
            Console.WriteLine("\n-----------Clients--------------\n");
            foreach (Client client in clients)
            {
                Console.WriteLine(client.Name);
            }
            Console.WriteLine("\n--------------------------------\n");
            Console.Write("Unesi ime: ");
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

        //Kreiranje novog klijenta
        private void CreateNewClient()
        {
            try
            {
                Console.WriteLine("Unesi ime klijenta: ");
                string ime= Console.ReadLine();
                Client client = new Client(ime);

                if (ime[0]>48 && ime[0] < 57)
                {
                    Console.WriteLine("Ime korisnika ne sme krenuti brojem!");
                    return;
                }
                foreach(Client c in clients)
                {
                    if (c.Name.Equals(ime))
                    {
                        Console.WriteLine("Vec postoji klijent sa tim imenom!");
                        return;
                    }
                }

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
