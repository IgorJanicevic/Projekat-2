using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjekatProxy
{
    public interface IServer
    {
        void WriteData(Measurement measurement);
        string AcceptMessageFromProxy(int br);
        void AcceptProxy();
        void LogEvent(string message);
    }
}
