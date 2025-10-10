using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Scandalous.WPF.ViewModels
{
    public partial class PreviewViewModel : BaseViewModel
    {
        [ObservableProperty]
        private ImageSource? _previewImage;

        [ObservableProperty]
        private double _scale = 1.0;

        [ObservableProperty]
        private double _offsetX = 0.0;

        [ObservableProperty]
        private double _offsetY = 0.0;

        public bool HasPreviewImage => PreviewImage != null;

        partial void OnPreviewImageChanged(ImageSource? value)
        {
            OnPropertyChanged(nameof(HasPreviewImage));
            // Reset zoom and pan when a new image is loaded
            Scale = 1.0;
            OffsetX = 0.0;
            OffsetY = 0.0;
        }
    }
}
