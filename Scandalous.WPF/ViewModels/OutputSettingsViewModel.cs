using CommunityToolkit.Mvvm.ComponentModel;
using Scandalous.Core.Validation;
using System.Collections.ObjectModel;

namespace Scandalous.WPF.ViewModels
{
    public partial class OutputSettingsViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string _outputPath = string.Empty;

        [ObservableProperty]
        private string _baseFileName = string.Empty;

        [ObservableProperty]
        private string _outputPathError = string.Empty;

        [ObservableProperty]
        private string _baseFileNameError = string.Empty;

        [ObservableProperty]
        private string _fileNamePreview = string.Empty;

        [ObservableProperty]
        private ObservableCollection<string> _recentFolders = new();

        [ObservableProperty]
        private ObservableCollection<string> _recentFiles = new();

        public bool IsOutputPathValid => string.IsNullOrEmpty(OutputPathError);
        public bool IsBaseFileNameValid => string.IsNullOrEmpty(BaseFileNameError);

        public OutputSettingsViewModel()
        {
            OnOutputPathChanged(OutputPath);
            OnBaseFileNameChanged(BaseFileName);
        }

        partial void OnOutputPathChanged(string value)
        {
            var (isValid, error) = FolderValidator.IsValid(value);
            OutputPathError = isValid ? string.Empty : error;
            OnPropertyChanged(nameof(IsOutputPathValid));
            UpdateFileNamePreview();

            if (isValid && !RecentFolders.Contains(value))
            {
                RecentFolders.Insert(0, value);
                if (RecentFolders.Count > 5)
                {
                    RecentFolders.RemoveAt(5);
                }
            }
        }

        partial void OnBaseFileNameChanged(string value)
        {
            var (isValid, error) = FileNameValidator.IsValid(value, isBaseNameOnly: true);
            BaseFileNameError = isValid ? string.Empty : error;
            OnPropertyChanged(nameof(IsBaseFileNameValid));
            UpdateFileNamePreview();
        }

        private void UpdateFileNamePreview()
        {
            if (IsBaseFileNameValid)
            {
                FileNamePreview = $"{BaseFileName}_yyyyMMdd_HHmmss.pdf";
            }
            else
            {
                FileNamePreview = string.Empty;
            }
        }
    }
}
