using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransferProtocolLibrary.Config
{
    public interface IParser
    {
        List<KeyValuePair<string, object>> CreateTree();
    }
}
