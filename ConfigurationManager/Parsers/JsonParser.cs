using System.IO;
using System.Collections.Generic;
using System.Text;
using System;

namespace TransferProtocolLibrary.Config.Parsers
{
    public class JsonParser : IParser
    {
        public readonly string text;

        private int lastIndex;

        public JsonParser(string text)
        {
            this.text = text;

            GetString();
            if (this.text[lastIndex - 1] != '{')
            {
                lastIndex = 0;
                this.text = "{" + this.text + "}";
            }
        }

        public List<KeyValuePair<string, object>> CreateTree()
        {
            lastIndex = 0;
            GetString();
            return GetObject();
        }

        private string GetString()
        {
            char[] chars = { '\"', '\'', ':', ',', '{', '}', '[', ']' };
            int index = lastIndex;
            if ((index = text.IndexOfAny(chars, index)) >= 0)
            {
                string result;
                char ch = text[index];
                if (ch == '\"' || ch == '\'')
                {
                    int end;
                    do
                    {
                        end = text.IndexOf(text[index], index + 1);
                    }
                    while (JsonUtility.IsEscape(text, end));
                    result = text.Substring(index + 1, end - index - 1);
                    result = JsonUtility.ExpandEscape(result);
                    lastIndex = text.IndexOfAny(chars, end + 1) + 1;
                }
                else
                {
                    result = text.Substring(lastIndex, index - lastIndex).Trim();
                    lastIndex = index + 1;
                }
                return result;
            }
            else
            {
                throw new InvalidDataException();
            }
        }

        private object GetValue()
        {
            string value = GetString();
            switch (text[lastIndex - 1])
            {
                case ',':
                case '}':
                case ']':
                    return value;
                case '{':
                    var obj = GetObject();
                    GetString();
                    return obj;
                case '[':
                    var array = GetArray();
                    GetString();
                    return array;
                default:
                    return null;
            }
        }

        private List<KeyValuePair<string, object>> GetObject()
        {
            List<KeyValuePair<string, object>> list = new List<KeyValuePair<string, object>>();
            while (true)
            {
                string key = GetString();
                if (text[lastIndex - 1] == ':')
                {
                    object value = GetValue();
                    list.Add(new KeyValuePair<string, object>(key, value));
                    switch (text[lastIndex - 1])
                    {
                        case ',':
                            break;
                        case '}':
                            return list;
                        default:
                            throw new InvalidDataException();
                    }
                }
                else if (text[lastIndex - 1] == '}')
                {
                    return list;
                }
                else
                {
                    throw new InvalidDataException();
                }
            }
        }

        private List<KeyValuePair<string, object>> GetArray()
        {
            List<KeyValuePair<string, object>> list = new List<KeyValuePair<string, object>>();
            string key = "element";
            while (true)
            {
                object value = GetValue();
                switch (text[lastIndex - 1])
                {
                    case ',':
                        list.Add(new KeyValuePair<string, object>(key, value));
                        break;
                    case ']':
                        if (!(value is string s) || s != "")
                        {
                            list.Add(new KeyValuePair<string, object>(key, value));
                        }
                        return list;
                    default:
                        throw new InvalidDataException();
                }
            }
        }
    }
}
