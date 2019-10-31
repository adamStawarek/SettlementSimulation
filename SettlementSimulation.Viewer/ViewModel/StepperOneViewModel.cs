using SettlementSimulation.Viewer.Commands;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace SettlementSimulation.Viewer.ViewModel
{
    public class StepperOneViewModel : ViewModelBase
    {
        public static StepperOneViewModel Instance { get; set; }

        #region properties
        private Bitmap _selectedBitmap;
        public Bitmap SelectedBitmap
        {
            get => _selectedBitmap;
            set
            {
                _selectedBitmap = value;
                RaisePropertyChanged();
                Messenger.Default.Send(new SetHeightMapCommand() { HeightMap = _selectedBitmap });
            }
        }

        private string _heightMapsFolderPath;
        public string HeightMapsFolderPath
        {
            get => _heightMapsFolderPath;
            set
            {
                _heightMapsFolderPath = value;
                RaisePropertyChanged();
            }
        }

        private List<Bitmap> _heightMaps;
        public List<Bitmap> HeightMaps
        {
            get => _heightMaps;
            set
            {
                _heightMaps = value;
                RaisePropertyChanged();
            }
        } 
        #endregion

        public RelayCommand OpenFolderCommand { get; }

        public StepperOneViewModel()
        {
            Instance = this;
            OpenFolderCommand = new RelayCommand(OpenFolder);
            HeightMaps = new List<Bitmap>();
            SetHeightMaps(@"C:\Users\adams\Desktop\memes");
        }

        private void OpenFolder()
        {
            var dialog = new CommonOpenFileDialog { IsFolderPicker = true, Multiselect = true };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
               SetHeightMaps(dialog.FileName);
            }
        }

        private void SetHeightMaps(string directory)
        {
            HeightMapsFolderPath = directory;
            string[] allowedExtensions = new[] { ".png", ".jpg" };
            var files = Directory.GetFiles(directory)
                .Where(file => allowedExtensions.Any(file.ToLower().EndsWith))
                .Select(file => new Bitmap(file) { Tag = file.Split('\\').Last() });
            HeightMaps = new List<Bitmap>(files);
        }
    }
}