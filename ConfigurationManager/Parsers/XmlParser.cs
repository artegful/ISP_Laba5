using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace TransferProtocolLibrary.Config.Parsers
{
    public class XmlParser : IParser
    {
        public readonly string text;
        private int lastIndex;
        private Dictionary<string, string> entities;

        public XmlParser(string text)
        {
            this.text = text;
            entities = new Dictionary<string, string>()
            {
                ["lt"] = "<",
                ["gt"] = ">",
                ["amp"] = "&",
                ["apos"] = "\'",
                ["quot"] = "\""
            };
        }

        public List<KeyValuePair<string, object>> CreateTree()
        {
            lastIndex = 0;
            SkipDeclarations();
            var element = GetElement();
            return (List<KeyValuePair<string, object>>)(element.Value);
        }

        private void SkipDeclarations()
        {
            int index = lastIndex;
            while ((index = text.IndexOf('<', index)) >= 0)
            {
                char ch = text[index + 1];
                if ((ch != '!') && (ch != '?'))
                {
                    lastIndex = index;
                    return;
                }
                else
                {
                    index += 2;
                }
            }

            throw new InvalidDataException();
        }

        private KeyValuePair<string, object> GetElement()
        {
            lastIndex++;
            int endTag = GetEndTagIndex(out bool isSelfClosing);

            string name = GetElementName(endTag);
            TryGetAttributes(endTag, out List<KeyValuePair<string, object>> list);
            lastIndex = endTag;
            if (isSelfClosing)
            {
                if (list == null)
                {
                    list = new List<KeyValuePair<string, object>>();
                }
                return new KeyValuePair<string, object>(name, list);
            }

            int index;
            lastIndex++;
            StringBuilder content = new StringBuilder();
            while ((index = text.IndexOf('<', lastIndex)) >= 0)
            {
                if (list == null)
                {
                    content.Append(text.Substring(lastIndex, index - lastIndex));
                }
                lastIndex = index;
                switch (text[lastIndex + 1])
                {
                    case '/':
                        lastIndex = GetEndTagIndex(out _);
                        if (list != null)
                        {
                            return new KeyValuePair<string, object>(name, list);
                        }
                        else
                        {
                            return new KeyValuePair<string, object>(name, content.ToString());
                        }
                    case '!':
                        TrySkipComment();
                        break;
                    default:
                        if (list == null)
                        {
                            list = new List<KeyValuePair<string, object>>();
                        }
                        list.Add(GetElement());
                        lastIndex++;
                        break;
                }
            }
            throw new InvalidDataException();
        }

        private int GetEndTagIndex(out bool isSelfClosing)
        {
            int endTag = text.IndexOf('>', lastIndex);
            if (endTag < 0)
            {
                throw new InvalidDataException();
            }
            isSelfClosing = text[endTag - 1] == '/';
            return endTag;
        }

        private string GetElementName(int endTag)
        {
            char[] chars = { ' ', '\t', '\v', '\r', '\n', '\f', '/' };
            int index = text.IndexOfAny(chars, lastIndex, endTag - lastIndex);
            if (index < 0)
            {
                index = endTag;
            }
            string name = text.Substring(lastIndex, index - lastIndex);
            lastIndex = index;
            return name;
        }

        private bool TryGetAttributes(int endTag, out List<KeyValuePair<string, object>> attributes)
        {
            attributes = null;
            char[] chars = { '\"', '\'' };
            int index = lastIndex;
            while ((index = text.IndexOf('=', index, endTag - index)) >= 0)
            {
                try
                {
                    string key = text.Substring(lastIndex, index - lastIndex).Trim();
                    int start = text.IndexOfAny(chars, index) + 1;
                    int end = text.IndexOf(text[start - 1], start);
                    string value = text.Substring(start, end - start);
                    if (attributes == null)
                    {
                        attributes = new List<KeyValuePair<string, object>>();
                    }
                    attributes.Add(new KeyValuePair<string, object>(key, value));
                    lastIndex = end + 1;
                    index = lastIndex;
                }
                catch (ArgumentOutOfRangeException)
                {
                    throw new InvalidDataException();
                }
            }
            return attributes != null;
        }

        private bool TrySkipComment()
        {
            try
            {
                if (text.Substring(lastIndex, 4) == "<!--")
                {
                    int start = lastIndex + 4;
                    int end = text.IndexOf("-->", start);
                    if (end >= 0)
                    {
                        lastIndex = end + 3;
                        return true;
                    }
                    else
                    {
                        throw new InvalidDataException();
                    }
                }
            }
            catch (ArgumentOutOfRangeException) { }
            return false;
        }
    }
}
