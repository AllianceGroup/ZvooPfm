using System;
using System.Drawing;
using System.IO;
using System.Web;
using mPower.Documents.DocumentServices.Membership;

namespace Default.Helpers
{
    public class ImageHelper
    {
        private readonly UserDocumentService _userService;

        private string AffiliateName(string userId)
        {
            string affiliateName = null;
            var user = _userService.GetById(userId);
            if (user != null)
            {
                affiliateName = user.AffiliateName;
            }
            return string.IsNullOrEmpty(affiliateName) ? "unknown_affiliate" : affiliateName;
        }

        public ImageHelper(UserDocumentService userService)
        {
            _userService = userService;
        }

        public Image GetResizedImage(Stream rawFileStream, int maxImageWidth = Constants.Images.MaxImageWidth, int maxImageHeight = Constants.Images.MaxImageHeight)
        {
            var fullsizeImage = Image.FromStream(rawFileStream);

            // Prevent using images internal thumbnail
            fullsizeImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
            fullsizeImage.RotateFlip(RotateFlipType.Rotate180FlipNone);

            var newWidth = fullsizeImage.Width > maxImageWidth ? maxImageWidth : fullsizeImage.Width;
            var newHeight = fullsizeImage.Height * newWidth / fullsizeImage.Width;
            if (newHeight > maxImageHeight)
            {
                // Resize with height instead
                newHeight = maxImageHeight;
                newWidth = fullsizeImage.Width * maxImageHeight / fullsizeImage.Height;
            }

            var newImage = fullsizeImage.GetThumbnailImage(newWidth, newHeight, null, IntPtr.Zero);

            // Clear handle to original file so that we can overwrite it if necessary
            fullsizeImage.Dispose();

            return newImage;
        }

        public string GenerateThumbnail(string userId, Stream rawFileStream, string fileName, int width, int height)
        {
            string thumbnailName;
            if (width > 0 && width < Constants.Images.MaxImageWidth && height > 0 && height < Constants.Images.MaxImageHeight)
            {
                thumbnailName = GenerateThumbnailName(fileName, width, height);
                var thumbnail = GetResizedImage(rawFileStream, width, height);
                thumbnail.Save(GetDiskLocation(userId, thumbnailName));
            }
            else
            {
                var path = GetDiskLocation(userId, fileName);
                if (!(new FileInfo(path)).Exists)
                {
                    var thumbnail = GetResizedImage(rawFileStream);
                    thumbnail.Save(path);
                }
                thumbnailName = fileName;
            }
            return GenerateImageUrl(userId, thumbnailName);
        }

        public string GetThumbnailUrl(string userId, string fileName, int width, int height)
        {
            var thumbnailName = GenerateThumbnailName(fileName, width, height);
            var thumbnailPath = GetDiskLocation(userId, thumbnailName);
            if (!(new FileInfo(thumbnailPath)).Exists)
            {
                var originImagePath = GetDiskLocation(userId, fileName);
                if (!(new FileInfo(originImagePath)).Exists)
                {
                    return string.Empty;
                }
                var stream = new FileStream(originImagePath, FileMode.Open);
                GenerateThumbnail(userId, stream, fileName, width, height);
                stream.Close();
            }
            return GenerateImageUrl(userId, thumbnailName);
        }

        public string GetDiskLocation(string userId, string fileName)
        {
            var userUploadsFolder = HttpContext.Current.Server.MapPath(GenerateImageBaseUrl(userId));
            Directory.CreateDirectory(userUploadsFolder);
            return Path.Combine(userUploadsFolder, fileName);
        }

        private string GenerateImageBaseUrl(string userId)
        {
            const string userFilesFolder = "UserFiles";
            return String.Format("~/{0}/{1}/{2}/", userFilesFolder, AffiliateName(userId), userId);
        }

        private string GenerateImageUrl(string userId, string imageName)
        {
            return (GenerateImageBaseUrl(userId) + imageName).TrimStart('~');
        }

        private static string GenerateThumbnailName(string fileName, int width, int height)
        {
            var fileExtension = (new FileInfo(fileName)).Extension;
            var fileNameWithoutExtension = fileName.Substring(0, fileName.Length - fileExtension.Length);
            var thumbnailName = String.Format("{0}_{1}x{2}{3}", fileNameWithoutExtension, width, height, fileExtension);
            return thumbnailName;
        }
    }
}