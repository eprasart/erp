using System;
using System.IO;
using System.Collections.Generic;

namespace ERP
{
    public class Setting
    {
        private  string mPath;
        private  SortedDictionary<string, string> dictionary = new SortedDictionary<string, string>();

        public  string Path
        {
            set
            {
                mPath = value;
                Load();
            }
            get { return mPath; }
        }

        private  void Load()
        {
            dictionary.Clear();
            if (!File.Exists(Path)) return;
            StreamReader sr = new StreamReader(Path);
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                if (line.Length == 0) continue;
                int equalPos = line.IndexOf('=');   // First '='
                if (equalPos == -1) continue;   // Prevent from bad line
                string key = line.Substring(0, equalPos).Trim();
                string value = line.Substring(equalPos + 1).TrimStart();
                Add(key, value);
            }
            sr.Close();
        }

        public  void Save()
        {
            StreamWriter sw = new StreamWriter(Path);
            
            foreach (KeyValuePair<string, string> p in dictionary)
            {
                sw.WriteLine(p.Key + " = " + p.Value);
            }
            sw.Close();
        }

        public  string Get(string key, string defaultvalue = "")
        {
            string s;
            if (!dictionary.TryGetValue(key, out s))
            {
                Add(key, defaultvalue);
                return defaultvalue;
            }
            return s;
        }

        public  void Set(string key, string value)
        {
            if (dictionary.ContainsKey(key))
                dictionary[key] = value;
            else
                Add(key, value);
        }

        private  void Add(string key, string value)
        {
            dictionary.Add(key, value);
        }

        public  void Delete(string key)
        {
            if (dictionary.ContainsKey(key))
                dictionary.Remove(key);
        }
    }
}