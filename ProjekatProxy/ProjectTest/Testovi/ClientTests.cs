using ProjekatProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTest.Testovi
{
    public class ClientTests
    {
        ClientHandler handler = new ClientHandler();
        Proxy p=null;
        Server server = null;

        public ClientTests(Proxy p,Server s)
        {
            p = p; server=s;
            
        }

        public void TestZaKreiranjeKlijenta()
        {
            handler.Handler(p, server);
        }












    }
}
