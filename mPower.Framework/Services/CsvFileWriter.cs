using System.IO;
using System.Text;

namespace mPower.Framework.Services
{
    public class CsvFileWriter : StreamWriter
    {
        public CsvFileWriter(Stream stream)
            : base(stream)
        {
        }

        public CsvFileWriter(string filename)
            : base(filename)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        public void WriteRow(CsvRow row)
        {
            StringBuilder builder = new StringBuilder();
            foreach (string value in row)
            {
                // Add separator if this isn't the first value
                if (builder.Length > 0)
                    builder.Append(',');

                if (value.IndexOfAny(new char[] { '"', ',' }) != -1)
                {
                    // Special handling for values that contain comma or quote
                    // Enclose in quotes and double up any double quotes
                    builder.AppendFormat("\"{0}\"", value.Replace("\"", "\"\""));
                }
                else builder.Append(value);
            }
            row.LineText = builder.ToString();
            WriteLine(row.LineText);
        }
    }
}