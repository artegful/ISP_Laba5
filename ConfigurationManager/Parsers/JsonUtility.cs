using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransferProtocolLibrary.Config.Parsers
{
    public static class JsonUtility
    {
        public static bool IsEscape(string text, int index)
        {
            bool isEscape = false;
            index--;
            while ((index >= 0) && (text[index] == '\\'))
            {
                isEscape = !isEscape;
                index--;
            }
            return isEscape;
        }

        public static string ExpandEscape(string str)
        {
            int index = 0;
            int shift = 0;
            StringBuilder sb = new StringBuilder(str);
            while ((index = str.IndexOf('\\', index)) >= 0)
            {
                char ch = str[index + 1];
                if ((ch == 'u') || (ch == 'U'))
                {
                    char[] chars = str.ToCharArray(index + 2, 4);
                    int result = 0;
                    for (int i = 0; i < 4; i++)
                    {
                        result <<= 4;
                        result += (chars[i] < 'A') ? chars[i] - '0' :
                            (chars[i] < 'a') ? chars[i] - 'A' + 10 : chars[i] - 'a' + 10;
                    }
                    sb[index + shift] = (char)result;
                    sb.Remove(index + 1 + shift, 5);
                    shift -= 5;
                    index += 6;
                }
                else
                {
                    switch (ch)
                    {
                        case 'b':
                            sb[index + shift] = '\b';
                            break;
                        case 't':
                            sb[index + shift] = '\t';
                            break;
                        case 'n':
                            sb[index + shift] = '\n';
                            break;
                        case 'f':
                            sb[index + shift] = '\f';
                            break;
                        case 'r':
                            sb[index + shift] = '\r';
                            break;
                        case '\"':
                            sb[index + shift] = '\"';
                            break;
                        case '\'':
                            sb[index + shift] = '\'';
                            break;
                        case '\\':
                            break;
                        case '\r':
                            if (str[index + 2] == '\n')
                            {
                                sb.Remove(index + shift, 2);
                                shift -= 2;
                                index++;
                                break;
                            }
                            else
                            {
                                goto case '\n';
                            }
                        case '\n':
                            sb.Remove(index + shift, 1);
                            shift--;
                            break;
                    }
                    sb.Remove(index + 1 + shift, 1);
                    shift--;
                    index += 2;
                }
            }
            return sb.ToString();
        }
    }
}
