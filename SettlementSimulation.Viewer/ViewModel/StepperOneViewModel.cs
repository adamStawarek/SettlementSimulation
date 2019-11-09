using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.WindowsAPICodePack.Dialogs;
using SettlementSimulation.Viewer.Commands;
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
        public bool CanContinue => SelectedColorMap != null && SelectedHeightMap != null;

        private Bitmap _selectedHeightMap;
        public Bitmap SelectedHeightMap
        {
            get => _selectedHeightMap;
            set
            {
                _selectedHeightMap = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(CanContinue));
                Messenger.Default.Send(new SetHeightMapCommand() { HeightMap = _selectedHeightMap });
            }
        }

        private Bitmap _selectedColorMap;
        public Bitmap SelectedColorMap
        {
            get => _selectedColorMap;
            set
            {
                _selectedColorMap = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(CanContinue));
                //Messenger.Default.Send(new SetHeightMapCommand() { HeightMap = _selectedHeightMap });
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

        public RelayCommand<object> SetHeightMapCommand { get; set; }

        public RelayCommand<object> SetColorMapCommand { get; set; }

        public StepperOneViewModel()
        {
            Instance = this;
            OpenFolderCommand = new RelayCommand(OpenFolder);
            SetHeightMapCommand = new RelayCommand<object>(SetHeightMap);
            SetColorMapCommand = new RelayCommand<object>(SetColorMap);
            HeightMaps = new List<Bitmap>();
            SetHeightMaps(@"C:\Users\adams\Desktop\memes");
        }

        private void SetHeightMap(object o)
        {
            var bitmap = o as Bitmap;
            SelectedHeightMap = bitmap;
        }

        private void SetColorMap(object o)
        {
            var bitmap = o as Bitmap;
            SelectedColorMap = bitmap;
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