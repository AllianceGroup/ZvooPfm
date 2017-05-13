using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using SoundInTheory.DynamicImage.Fluent;
using SoundInTheory.DynamicImage.Util;

namespace mPower.Framework.Utils
{
    public enum OfferImageSizeEnum
    {
        InlineOfferListing,
        NetworkOffers,
    }

    public class Uploads
    {
        public const string UploadsRootDir = "Uploads";
        public const string GeneratedLogosDir = "generated_logos";
        public const string OffersContentDir = "OffersContent";

        public static string GetGeneratedLogoRelativeUrl(string filename)
        {
            return "~/" + Path.Combine(UploadsRootDir, OffersContentDir, GeneratedLogosDir, filename);
        }
    }

    public class UploadUtil
    {
        private readonly Dictionary<OfferImageSizeEnum, Tuple<int, int>> _imageSizesInfo = new Dictionary<OfferImageSizeEnum, Tuple<int, int>>
        {
            {OfferImageSizeEnum.InlineOfferListing, Tuple.Create(120, 80)},
            {OfferImageSizeEnum.NetworkOffers, Tuple.Create(69, 69)}
        };

        private readonly MPowerSettings _settings;

        private readonly List<string> _imagesExtensions = new List<string> {".bmp", ".jpg", ".jpeg", ".png", ".gif", ".tif"};

        public string GeneratedLogosPath
        {
            get { return CheckDirectoryExists(Uploads.OffersContentDir, Uploads.GeneratedLogosDir); }
        }

        public string AccessDataImagesPath
        {
            get { return CheckDirectoryExists(Uploads.OffersContentDir); }
        }

        public UploadUtil(MPowerSettings settings)
        {
            _settings = settings;
        }

        public bool IsImage(HttpPostedFileBase file)
        {
            return _imagesExtensions.Contains(Path.GetExtension(file.FileName.ToLowerInvariant()));
        }

        #region Merchant

        public void SaveMerchantLogo(HttpPostedFileBase file, string appName, string userId)
        {
            var filename = "merchant_logo" + Path.GetExtension(file.FileName);
            file.SaveAs(GetMerchantLogoPath(appName, userId, filename));
        }

        private string GetMerchantLogoPath(string appName, string userId, string fileName)
        {
            return Path.Combine(CheckDirectoryExists(appName, userId), fileName);
        }

        #endregion

        #region Campaign

        public void SaveCampaignLogo(HttpPostedFileBase file, string appName, string campaignId)
        {
            foreach (var size in _imageSizesInfo)
            {
                var filename = GenerateLogoFileName(campaignId, Path.GetExtension(file.FileName), size.Value.Item1, size.Value.Item2);
                var bmp = Resize(file, size.Value.Item1, size.Value.Item2);
                bmp.Save(GetCampaigLogoPath(appName, filename));
            }
        }

        private string GenerateLogoFileName(string campaingId, string extension, OfferImageSizeEnum size)
        {
            var sizeInfo = _imageSizesInfo[size];
            return GenerateLogoFileName(campaingId, extension, sizeInfo.Item1, sizeInfo.Item2);
        }

        private static string GenerateLogoFileName(string campaingId, string extension, int width, int height)
        {
            return string.Format("{0}_{1}x{2}.{3}", campaingId, width, height, extension.Trim('.'));
        }

        private string GetCampaigLogoPath(string appName, string fileName)
        {
            return Path.Combine(CheckDirectoryExists(appName, "CampaignLogos"), fileName);
        }

        public string GetCampaigLogoUrl(string appName, string campaignId, string fileTitle, OfferImageSizeEnum size = OfferImageSizeEnum.InlineOfferListing)
        {
            // fileTitle is name of file when user uploads it (also is showed during campaign editing)
            // fileName is name under which file is saved at our files storage
            var fileExtension = string.IsNullOrEmpty(fileTitle) ? "" : fileTitle.Split('.').LastOrDefault();
            var fileName = GenerateLogoFileName(campaignId, fileExtension, size);

            return File.Exists(GetCampaigLogoPath(appName, fileName)) 
                ? string.Format("/Uploads/{0}/CampaignLogos/{1}", appName, fileName) 
                : null;
        }

        #endregion

        private static FastBitmap Resize(HttpPostedFileBase file, int width, int height)
        {
            var memoryStream = new MemoryStream();
            file.InputStream.Position = 0;
            file.InputStream.CopyTo(memoryStream);
            var layer = LayerBuilder.Image.SourceBytes(memoryStream.ToArray())
                .WithFilter(FilterBuilder.Resize.To(width, height)).ToLayer();
            layer.Process();
            return layer.Bitmap;
        }

        private string CheckDirectoryExists(params string[] paths)
        {
            var dir = Path.Combine(_settings.UploadRootPath, Path.Combine(paths));
            
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            return dir;
        }
    }
}