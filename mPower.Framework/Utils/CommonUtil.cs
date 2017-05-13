using System;
using System.Diagnostics;
using System.IO;

namespace mPower.Framework.Utils
{
    public class CommonUtil
    {
        public string GenerateUniqueDateName()
        {
            return DateTime.Now.ToString("MM-dd-yyyy-hh-mm-tt");
        }

        public void CopyDirectory(string from, string to)
        {
            if (!Directory.Exists(to))
                Directory.CreateDirectory(to);

            CopyDirectory(new DirectoryInfo(from), new DirectoryInfo(to));
        }

        private void CopyDirectory(DirectoryInfo source, DirectoryInfo destination)
        {
            foreach (DirectoryInfo dir in source.GetDirectories())
                CopyDirectory(dir, destination.CreateSubdirectory(dir.Name));
            foreach (FileInfo file in source.GetFiles())
                file.CopyTo(Path.Combine(destination.FullName, file.Name), true);
        }

        public void RunProcess(string pathToProcess, string arguments)
        {
            var startIfo = new ProcessStartInfo(pathToProcess);
            startIfo.Arguments = arguments;
            startIfo.UseShellExecute = true;

            var process = Process.Start(startIfo);
            process.WaitForExit();
        }
    }
}
