using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ionic.Zip;
using NLog;
using mPower.Framework;
using mPower.Framework.Environment;
using mPower.Framework.Utils;
using NUnit.Framework;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace mPower.OfferingsSystem
{
    public class PackageDownloader : IPackageDownloader
    {
        private readonly Logger _logger = MPowerLogManager.CurrentLogger;

        private const string EncryptedFileExtension = ".pgp";
        private readonly MPowerSettings _settings;
        private readonly UploadUtil _uploadUtil;
        private const string Passphrase = "Frontline2013";
        private const string FromAccessDirectoryPath = "from_access";
        private const string PgpPrivateKey =
@"-----BEGIN PGP PRIVATE KEY BLOCK-----
Version: BCPG C# v1.6.1.0

lQOsBFHYWFoBCACbRJmBLB0gjeYoCp7cF74H2TrvuX3yy18/a0UA6+Syxr2v1ruy
T8AgubloL50DCtuI59ORtdQFi2sGq34PCcu4MGGfPHj3m3/3yF0s62M5ieptcTYL
lOKJIFm5OA8IK45jKgI24TdMaMDOBDMpvaTsYhYGlke3LCDNZXsysoDWuxAEAG2o
zioglA/ylxKHf4EDz/nRA2kpGzxYZQapw6lXjH7h3758RDu4LDtNtqOxTBPtiAgW
89L0c4uNbsGsbm09DjrXHuAHBg0cV+jCEfr0OJfyKBMEQSm02lzDiwi6a7UTOtIG
zqo9ZZY3Xrs2zzKtMPoDGAjMdSem0XyS67t5ABEBAAH/AwMC3sfxLfoRMo1gsSSq
Altt8MZdKuQf3W3K67dLLYNO5xmr2lJqbuOCoK/96ZBeBtKylLIeLtl65hIk12gY
ynA35fucu12dHUJaWHKOavHe3B4HjL7Qp6BOQ9RnohE6TN79kBfBaIamwnsQZBOS
c+PdbZIemgcqvZPUgZNjGwnnNSva2LHMNvR2xrQSot72TowYLStIJ7CtW6rJpPnu
0aEDhn7yj14AOhAI4zz3OXc2gDkUabfhkDQCYh8IJMxsPuy+oCA2VAE6MD5OjnJf
Uo4mcCAwXAw/Okq4hlRPDC3nfz5om5koLNL1RPVMYgFXjI5af/p0Fy+I5EbRtCKr
oz1bM8U5HBwO/R+NsAC8peBBeJbEJlLn7v2858Hn6yk8mhbgkBo83f9OdEWQFLAj
f0Wcjw+Kyz2ccNvJqsdY+n8oVrFHVL1bYIieHe6xC9U3eIRGClyH9itOWGe1fSzX
ZuZ0HwMs156IeoWhypKEsN+C4yBvpqyNSIEysBdqkjrKnCzwfWgkLFSZ39rHEczF
h5Nq7NGCj+n52eqW6ZMSRyHvLqhhe8z+C9PdSefeKckt7Ovk+VO3qc1SFym36qDV
YkMd/pWT077olX3orcpOK/Dv6621mnCEBuQNEax11fQpQrM55WCobOGnVVOMyRZ6
SrtDe2OGNpTG9U9bs++lZQXLfY1wbsOUYjyRhSRffHjh/cNtw8DrSDwZKW4v74iQ
zoY6ggsxWC5RTQWtv66/8WMcEfQ4ZLZTs93UzLzu7WDvNuCwK3aJnEQ6CyTSrCVq
KsW+tcvqo31ntcShnhXcf6z6Kz6KSdaW8N9nLgojoJm34D7mYdFohcMU+IOhiO2c
f6R5Vjcl28NaqFzvEaLr6ybha0TTaGrz/pJDvvW6D7QXYWxleC5zaGtvckBwYXJh
bGVjdC5jb22JARwEEAECAAYFAlHYWFoACgkQy55Xy8HKqkHXIgf7BQi6tJWalTyy
yEPzAt3rd93fkRLpfrsd1Nq2uTGtbDKDmYv5cF0sQlbPYv874t+tyGP5+wOgqHJG
qEmmQV0D5CoD3/PLzoURPi6grdbARWJ1sAngn/pNqF40HxroAnvVA0UnpSUJUQ0q
pp8CApaTF8WWLQ5stSeRnc4fQlshAjxEtLiCQKb0dyD4Dfe4Lm016regcRj+Rj06
a4ZoqNZSFhNPE2IO9tZNJ0su0ebP3g099SKnjvIP96/q9Bs5fbXMt50Cl+oqhEun
na04f6XfeK8TX7ypvqPc/RH6X9j6FcRSozk4RaEjHvKpC61YwceYYs5CiIdMGsxD
yjY1FrQpxw==
=bQIi
-----END PGP PRIVATE KEY BLOCK-----";

        public PackageDownloader(MPowerSettings settings, UploadUtil uploadUtil)
        {
            _settings = settings;
            _uploadUtil = uploadUtil;
        }

        public PackageInfo Download()
        {
            var existFiles = GetExistFiles(_settings.AccessDataLocalPath);
            var url = _settings.AccessDataFtpUrl;
            var client = new SftpClient(url, _settings.AccessDataUsername, _settings.AccessDataPassword);
            client.Connect();
            var files = client.ListDirectory(FromAccessDirectoryPath).Where(x => !x.IsDirectory && !existFiles.Contains(x.Name.Replace(EncryptedFileExtension, ""))).ToList();
            client.ChangeDirectory(FromAccessDirectoryPath);
            var downloadedFiles = new ConcurrentBag<string>();
            if (_settings.IsProd || _settings.IsStage)
            {
                Parallel.ForEach(files,
                                 (sftpFile) =>
                                 {
                                     using (var memoryStream = new MemoryStream())
                                     {
                                         client.DownloadFile(sftpFile.Name, memoryStream);
                                         using (var decryptedStream = DecrypteStream(memoryStream))
                                         {
                                             var filename = SaveFile(sftpFile, decryptedStream);
                                             downloadedFiles.Add(filename);
                                         }
                                     }
                                 });
            }
            ExtractContentAsync(downloadedFiles.ToList());
            return new PackageInfo
                       {
                           LocalPath = _settings.AccessDataLocalPath
                       };
        }

        private IEnumerable<string> GetExistFiles(string path)
        {
            return Directory.EnumerateFiles(path).Select(Path.GetFileName);
        }

        private string SaveFile(SftpFile sftpFile, Stream decryptedStream)
        {
            var filename = sftpFile.Name.Replace(EncryptedFileExtension, "");
            var fs = File.Create(Path.Combine(_settings.AccessDataLocalPath, filename));
            decryptedStream.CopyTo(fs);
            fs.Flush();
            fs.Close();
            return filename;
        }

        //Decrypt PGP stream
        private Stream DecrypteStream(Stream memoryStream)
        {
            var decodedInputStream = PgpUtilities.GetDecoderStream(memoryStream);
            decodedInputStream.Seek(0, SeekOrigin.Begin);
            var pgpEncryptedObjectStream = new PgpObjectFactory(decodedInputStream);
            var key = CryptographicKey.CreateFromAsc(PgpPrivateKey);
            var secretKeyRingBundle = new PgpSecretKeyRingBundle(key.Value);
            PgpEncryptedDataList encryptedDataList;

            {
                var tmp = pgpEncryptedObjectStream.NextPgpObject();
                if (!(tmp is PgpEncryptedDataList))
                {
                    tmp = pgpEncryptedObjectStream.NextPgpObject();
                }
                encryptedDataList = (PgpEncryptedDataList) tmp;
            }
            PgpPublicKeyEncryptedData encryptedData = null;
            PgpPrivateKey privateKey = null;
            {
                foreach (PgpPublicKeyEncryptedData encData in encryptedDataList.GetEncryptedDataObjects())
                {
                    if (secretKeyRingBundle.Contains(encData.KeyId))
                    {
                        PgpSecretKey secretKey = secretKeyRingBundle.GetSecretKey(encData.KeyId);
                        privateKey = secretKey.ExtractPrivateKey(Passphrase.ToCharArray());
                        encryptedData = encData;
                        break;
                    }
                }
                if (privateKey == null)
                {
                    throw new Exception("privateKey isn't correct");
                }
            }
            Stream decryptedStream = encryptedData.GetDataStream(privateKey);


            var pgpDecryptedObjectStream = new PgpObjectFactory(decryptedStream);

            PgpLiteralData literalData;
            {

                var nextObject = pgpDecryptedObjectStream.NextPgpObject();
                if (nextObject is PgpCompressedData)
                {
                    var compressedData = (PgpCompressedData)nextObject;
                    var compressedDataObjectStream = new PgpObjectFactory(compressedData.GetDataStream());
                    literalData = (PgpLiteralData)compressedDataObjectStream.NextPgpObject();
                }
                else if (nextObject is PgpLiteralData)
                {
                    literalData = (PgpLiteralData)nextObject;
                }
                else
                {
                    throw new InvalidDataException();
                }
            }

            var output = BcReadLiteralData(literalData);
            return new MemoryStream(output);
        }

        private byte[] BcReadLiteralData(PgpLiteralData ld)
        {
            Stream inLd = ld.GetDataStream();
            byte[] output = Org.BouncyCastle.Utilities.IO.Streams.ReadAll(inLd);

            return output;
        }

        private void ExtractContentAsync(List<string> downloadedFiles)
        {
            if (downloadedFiles.Any())
            {
                Task.Factory.StartNew(() => ExtractContent(downloadedFiles));
            }
        }

        private void ExtractContent(List<string> downloadedFiles)
        {
            var files = Directory.GetFiles(_settings.AccessDataLocalPath).Where(file => file.EndsWith(".zip") && downloadedFiles.Contains(file));
            Parallel.ForEach(files, new ParallelOptions{MaxDegreeOfParallelism = 10}, file =>
            {
                try
                {

                    var zip = new ZipFile(Path.Combine(_settings.AccessDataLocalPath, file));
                    zip.ExtractAll(_uploadUtil.AccessDataImagesPath, ExtractExistingFileAction.OverwriteSilently);
                }
                catch (Exception ex)
                {
                    _logger.ErrorException("Error ocured while extraxting content from " + file,ex);
                }
            });
        }
    }

    [TestFixture]
    public class PackageDownloaderTest
    {
        [Test]
        public void test_sftp_download()
        {
            var settings = new MPowerSettings
            {
                AccessDataFtpUrl = "ftp.adchosted.com",
                AccessDataLocalPath = "d:\\access_local_folder",
                AccessDataUsername = "mpower01",
                AccessDataPassword = "Cp3#gcnDu3=a"
            };
            var downloader = new PackageDownloader(settings, new UploadUtil(settings));
            downloader.Download();
        }

        [Test]
        public void ConcurrentBag_test()
        {
            var bag = new ConcurrentBag<int>();
            var count = 1000000;
            var items = Enumerable.Range(0, count);
            Parallel.ForEach(items, bag.Add);
            Assert.DoesNotThrow(() => bag.ToDictionary(x => x, y => y));
        }

        [Test]
        public void enumerable_eception_yield_test()
        {
            var result = iterate().ToList();
            Assert.AreEqual(1, result[0]);
            Assert.AreEqual(2, result[1]);
            Assert.AreEqual(2, result.Count);
        }

        private IEnumerable<int> iterate()
        {
            IEnumerable<int> values = Get();
            var enumerator = values.GetEnumerator();
            var hasValue = true;
            while (hasValue)
            {
                int value;
                try
                {
                    hasValue = enumerator.MoveNext();
                    value = enumerator.Current;
                }
                catch (Exception)
                {
                    break;
                }
                if (hasValue)
                {
                    yield return value;
                }
            }
        }

        private IEnumerable<int> Get()
        {
            yield return 1;
            yield return 2;
        }
        //[Test]
        //public void test_image_extracter()
        //{
        //    var settings = new MPowerSettings()
        //    {
        //        AccessDataLocalPath = "d:\\access_local_folder",
        //    };
        //    var downloader = new PackageDownloader(settings, new GuidIdGenerator());
        //    downloader.Download();
        //}

        private class GuidIdGenerator : IIdGenerator
        {
            public string Generate()
            {
                return Guid.NewGuid().ToString();
            }
        }
    }
}