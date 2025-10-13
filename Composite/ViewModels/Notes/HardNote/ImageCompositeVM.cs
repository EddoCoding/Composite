using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Media.Imaging;

namespace Composite.ViewModels.Notes.HardNote
{
    public partial class ImageCompositeVM : CompositeBaseVM
    {

        BitmapImage _imageSource;
        [ObservableProperty] double _aspectRatio;
        [ObservableProperty] double _originalWidth;
        [ObservableProperty] double _originalHeight;
        [ObservableProperty] string _horizontalImage;

        public double DisplayWidth
        {
            get
            {
                if (OriginalWidth == 0 || OriginalHeight == 0) return 500;

                const double maxSize = 500;

                if (OriginalWidth <= maxSize && OriginalHeight <= maxSize) return OriginalWidth;

                double scaleFactor = Math.Min(maxSize / OriginalWidth, maxSize / OriginalHeight);
                return OriginalWidth * scaleFactor;
            }
        }
        public double DisplayHeight
        {
            get
            {
                if (OriginalWidth == 0 || OriginalHeight == 0) return 500;

                const double maxSize = 500;

                if (OriginalWidth <= maxSize && OriginalHeight <= maxSize) return OriginalHeight;

                double scaleFactor = Math.Min(maxSize / OriginalWidth, maxSize / OriginalHeight);
                return OriginalHeight * scaleFactor;
            }
        }
        public BitmapImage ImageSource
        {
            get => _imageSource;
            set
            {
                SetProperty(ref _imageSource, value); 
                CalculateAspectRatio();
            }
        }

        public ImageCompositeVM() => Id = Guid.NewGuid();

        void CalculateAspectRatio()
        {
            OriginalWidth = ImageSource.PixelWidth;
            OriginalHeight = ImageSource.PixelHeight;
            AspectRatio = OriginalWidth / OriginalHeight;
        }
    }
}