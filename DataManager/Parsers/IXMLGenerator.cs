using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransferProtocolLibrary.DataManagement.Parsers
{
    interface IXMLGenerator
    {
        string GetXMLTableString(string tableName, List<List<KeyValuePair<string, object>>> rows);
    }
}
