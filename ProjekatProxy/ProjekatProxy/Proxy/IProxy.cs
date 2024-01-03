using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjekatProxy
{
    public interface IProxy
    {
        void ProxyAcceptClient(string name);
        string AcceptClientMessage(string name, int br);
        void AcceptDataFromServer();
        void SendDataToClient();
    }
}
