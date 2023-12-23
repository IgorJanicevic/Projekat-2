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
        List<double> GetDataFromProxy();
        void LogEvent(string message);
    }
}
