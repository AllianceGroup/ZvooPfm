using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CsvHelper.Configuration;

namespace mPower.OfferingsSystem
{
    public class MPowerCsvReader: IDisposable
    {
        private readonly TextReader _textReader;

        public CsvConfiguration Configuration = new CsvConfiguration();

        public MPowerCsvReader(TextReader textReader)
        {
            _textReader = textReader;
        }

        public IEnumerable<T> GetRecords<T>() where T : new()
        {
            var methods = GetSettersFor<T>();
            var methodsIndexes = new Dictionary<int, MethodInfo>();
           
                var first = true;
                while (true)
                {
                    var redord = new T();
                    try
                    {
                        if (first)
                        {
                            var arr = Read();
                            for (int i = 0; i < arr.Length; i++)
                            {
                                if (methods.ContainsKey(arr[i]))
                                {
                                    methodsIndexes.Add(i, methods[arr[i]]);
                                }
                            }
                            first = false;
                        }

                        var fields = Read();
                        for (int i = 0; i < fields.Length; i++)
                        {
                            if (methodsIndexes.ContainsKey(i))
                            {
                                methodsIndexes[i].Invoke(redord, new object[] { fields[i] });
                            }
                        }
                    }
                    catch {
                        break;
                    }
                    yield return redord;
                }
        }

        //private char[] buffer;
        //private int index = 0;

        //public string ReadLine()
        //{
        //    var sr = _textReader.ReadBlock(buffer, 0, 1000);
        //    if (buffer)
        //}

        public string[] Read()
        {
            var line = _textReader.ReadLine();
            var arr = line.Split(',');
            var fields = new List<string>();
            Action<string> trimAndAdd = s => fields.Add(s.Trim('"')); 
            var quotString = false;
            var current = string.Empty;
            for (int i = 0; i < arr.Length; i++)
            {
                if (quotString)
                {
                    if (current.EndsWith("\""))
                    {
                        trimAndAdd(current);
                        current = arr[i];
                        quotString = false;
                    }
                    else
                    {
                        current += "," + arr[i];
                    }
                }
                if (!quotString)
                {
                    current = arr[i];
                    if (current.StartsWith("\""))
                    {
                        quotString = true;
                    }
                    else
                    {
                        trimAndAdd(current);
                    }
                }
            }
            if (current.EndsWith("\""))
            {
                trimAndAdd(current);
            }
            return fields.ToArray();
        }

        private Dictionary<string, MethodInfo> GetSettersFor<T>()
        {
            var methods = new Dictionary<string, MethodInfo>();
            var type = typeof(T);
            foreach (var property in type.GetProperties())
            {
                var setter = property.GetSetMethod();
                var attr =(CsvFieldAttribute)
                    property.GetCustomAttributes(true).FirstOrDefault(x => x.GetType() == typeof (CsvFieldAttribute));
                var key = property.Name;
                if (attr != null && !attr.Ignore)
                {
                    key = attr.Name;

                }
                if (setter != null)
                {
                    methods.Add(key, setter);
                }
                //if (property.PropertyType != typeof(string))
                //{
                //    converters.Add(key, new ReferenceConverter());
                //}
            }
            return methods;
        }

        public void Dispose()
        {
            _textReader.Dispose();
        }
    }
}