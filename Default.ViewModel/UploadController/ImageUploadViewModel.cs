using System;

namespace Default.ViewModel.UploadController
{
    [Serializable]
    public class ImageUploadViewModel
    {
        public string PropertyName { get; set; }

        public string ImageName { get; set; }

        public int ImagePreviewWidth { get; set; }

        public int ImagePreviewHeight { get; set; }
    }
}