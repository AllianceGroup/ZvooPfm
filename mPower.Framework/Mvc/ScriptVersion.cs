using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace mPower.Framework.Mvc
{
    public class ScriptVersion
    {

        private static volatile ScriptVersion _instance;
        private static readonly object SyncRoot = new Object();

        private readonly string _scriptVersionFile;

        private ScriptVersion()
        {
            _scriptVersionFile = Path.Combine(AppDomain.CurrentDomain.RelativeSearchPath, "ScriptVersion.txt");

            Version = GetScriptVersion();
        }

        public string Version { get; private set; }

        public static ScriptVersion Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new ScriptVersion();
                        }
                    }
                }

                return _instance;
            }
        }

        public void SetScriptVersion(string version)
        {
            if (!File.Exists(_scriptVersionFile))
            {
                using (File.Create(_scriptVersionFile)) { }
            }

            using (var sw = new StreamWriter(_scriptVersionFile))
            {
                sw.WriteLine(version);
            }

            Version = version;
        }

        private string GetScriptVersion()
        {
            var version = "1";
            if (File.Exists(_scriptVersionFile))
            {
                using (var sw = new StreamReader(_scriptVersionFile))
                {
                    version = sw.ReadLine();
                }
            }

            return version;
        }

        public string AppendScriptVersion(string path)
        {
            path = path.ToLower();
            var result = path;
            if (path.Contains("custom/") || path.Contains("paralect/"))
            {
                result = String.Format("{0}?{1}", path, Version);
            }

            return result;
        }

        public string AppendCssVersion(string path)
        {
            return String.Format("{0}?{1}", path, Version);
        }
    }
}
