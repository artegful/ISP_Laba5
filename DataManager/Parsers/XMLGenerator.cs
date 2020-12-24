using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransferProtocolLibrary.DataManagement.Parsers
{
    class XMLGenerator : IXMLGenerator
    {
        public string GetXMLTableString(string tableName, List<List<KeyValuePair<string, object>>> rows)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append($"<{tableName}>\n");

            foreach (var row in rows)
            {
                builder.Append($"<tablerow ");

                foreach (var pair in row)
                {
                    builder.Append($"{pair.Key}=\"{pair.Value.ToString()}\" ");
                }

                builder.Append($"/>\n");
            }

            builder.Append($"<{tableName}/>");

            return builder.ToString();
        }
    }
}
