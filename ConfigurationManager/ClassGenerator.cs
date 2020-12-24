using System;
using System.Collections.Generic;
using System.Reflection;

namespace TransferProtocolLibrary.Config
{

    public static class ClassGenerator
    {
        public static object ParseFromTree(Type type, object tree)
        {
            if (ParseToBasicType(type, tree as string, out object obj))
            {
                return obj;
            }
            else if (type.IsArray)
            {
                return ParseToArray(type.GetElementType(), tree as List<KeyValuePair<string, object>>);
            }
            else if (type.IsEnum)
            {
                return ParseToEnum(type, tree as string);
            }
            else
            {
                return ParseToCustomType(type, tree as List<KeyValuePair<string, object>>);
            }
        }

        private static object ParseToArray(Type elementType, List<KeyValuePair<string, object>> list)
        {
            if (list == null)
            {
                throw new ArgumentException();
            }

            Array array = Array.CreateInstance(elementType, list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                array.SetValue(ParseFromTree(elementType, list[i].Value), i);
            }
            return array;
        }

        private static object ParseToEnum(Type enumType, string line)
        {
            if (line == null)
            {
                throw new ArgumentException();
            }

            object e = Enum.Parse(enumType, line);
            if (Enum.IsDefined(enumType, e))
            {
                return e;
            }
            else
            {
                throw new ArgumentException();
            }
        }

        private static object ParseToCustomType(Type type, List<KeyValuePair<string, object>> list)
        {
            if (list == null)
            {
                throw new ArgumentException();
            }

            Dictionary<string, object> pairs = new Dictionary<string, object>(list.Count);

            foreach (var pair in list)
            {
                pairs[pair.Key] = pair.Value;
            }

            ConstructorInfo constructor = type.GetConstructors()[0];
            ParameterInfo[] parameters = constructor.GetParameters();
            object[] objs = new object[parameters.Length];

            for (int i = 0; i < objs.Length; i++)
            {
                objs[i] = ParseFromTree(parameters[i].ParameterType, pairs[parameters[i].Name]);
            }

            return constructor.Invoke(objs);
        }

        private static bool ParseToBasicType(Type type, string value, out object obj)
        {
            bool result = true;

            if (value == null)
            {
                obj = null;
                return false;
            }

            if (type == typeof(string))
            {
                obj = value;
            }
            else if (type == typeof(bool))
            {
                obj = bool.Parse(value);
            }
            else if (type == typeof(byte))
            {
                obj = byte.Parse(value);
            }
            else if (type == typeof(sbyte))
            {
                obj = sbyte.Parse(value);
            }
            else if (type == typeof(short))
            {
                obj = short.Parse(value);
            }
            else if (type == typeof(int))
            {
                obj = int.Parse(value);
            }
            else if (type == typeof(long))
            {
                obj = long.Parse(value);
            }
            else if (type == typeof(char))
            {
                obj = char.Parse(value);
            }
            else if (type == typeof(float))
            {
                obj = float.Parse(value);
            }
            else if (type == typeof(double))
            {
                obj = double.Parse(value);
            }
            else if (type == typeof(decimal))
            {
                obj = decimal.Parse(value);
            }
            else if (type == typeof(ushort))
            {
                obj = ushort.Parse(value);
            }
            else if (type == typeof(uint))
            {
                obj = uint.Parse(value);
            }
            else if (type == typeof(ulong))
            {
                obj = ulong.Parse(value);
            }
            else
            {
                obj = null;
                result = false;
            }
            return result;
        }
    }
}
