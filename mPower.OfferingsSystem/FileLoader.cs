using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NLog;
using mPower.Framework;

namespace mPower.OfferingsSystem
{
    public class FileLoader: IFileLoader
    {
        private readonly MPowerSettings _settings;

        private readonly Logger _logger = MPowerLogManager.CurrentLogger;

        public FileLoader(MPowerSettings settings)
        {
            _settings = settings;
        }

        public IEnumerable<TextReader> LoadFor<T>()
        {
            var files = Directory.EnumerateFiles(_settings.AccessDataLocalPath).Where(x => x.ToLower().EndsWith(typeof (T).Name.ToLower() + ".csv"));
            //files = OrderFileNamesBy(files, x=> x.CreationTime,true);
            foreach (var filename in files)
            {
                var stopwatch = Stopwatch.StartNew();
                _logger.Info("Start reading CSV file ({0}).", filename);
                var fs = new FileStream(Path.Combine(_settings.AccessDataLocalPath, filename), FileMode.Open, FileAccess.Read, FileShare.Read);
                var bufferedStream = new BufferedStream(fs, 2 ^ 20);
                yield return new StreamReader(bufferedStream);
                stopwatch.Stop();
                _logger.Info("CSV file ({0}) was read. Elapsed time: {1}", filename, stopwatch.Elapsed);
            }
        }

        //private IEnumerable<string> OrderFilesByAccessCreationDate(IEnumerable<string> files)
        //{
        //    return
        //        files.OrderByDescending(
        //            x => DateTime.ParseExact(x.Split('-')[3], "yyyyMMdd", CultureInfo.InvariantCulture));
        //}


        //private IEnumerable<string> OrderFileNamesBy<TKey>(IEnumerable<string> files, Func<FileInfo,TKey> order, bool desc = false)
        //{
        //    var fileInfos = files.Select(x => new FileInfo(x));
        //    fileInfos = !desc ? fileInfos.OrderBy(order) : fileInfos.OrderByDescending(order);
        //    return fileInfos.Select(x => x.FullName);
        //}
    }
}