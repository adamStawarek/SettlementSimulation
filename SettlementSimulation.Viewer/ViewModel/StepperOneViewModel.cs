using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.WindowsAPICodePack.Dialogs;
using SettlementSimulation.Viewer.Commands;
using SettlementSimulation.Viewer.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using FastBitmapLib;
using SettlementSimulation.AreaGenerator;
using SettlementSimulation.AreaGenerator.Helpers;
using SettlementSimulation.AreaGenerator.Models;

namespace SettlementSimulation.Viewer.ViewModel
{
    public class StepperOneViewModel : ViewModelBase
    {

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
                Messenger.Default.Send(new SetColorMapCommand() { ColorMap = _selectedColorMap });
            }
        }

        private string _mapDirectory;
        public string MapDirectory
        {
            get => _mapDirectory;
            set
            {
                _mapDirectory = value;
                if (!string.IsNullOrEmpty(_mapDirectory))
                {
                    ConfigurationManagerHelper.SetSettings("MapDirectory", _mapDirectory);
                }
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<Bitmap> _maps;
        public ObservableCollection<Bitmap> Maps
        {
            get => _maps;
            set
            {
                _maps = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region commands
        public RelayCommand OpenFolderCommand { get; }

        public RelayCommand GenerateColorMapCommand { get; }

        public RelayCommand<object> SetHeightMapCommand { get; set; }

        public RelayCommand<object> SetColorMapCommand { get; set; } 
        #endregion

        public StepperOneViewModel()
        {
            OpenFolderCommand = new RelayCommand(OpenFolder);
            GenerateColorMapCommand = new RelayCommand(GenerateColorMap);
            SetHeightMapCommand = new RelayCommand<object>(SetHeightMap);
            SetColorMapCommand = new RelayCommand<object>(SetColorMap);
            Maps = new ObservableCollection<Bitmap>();

            var mapDirectory = ConfigurationManagerHelper.GetSettings("MapDirectory");
            if (!string.IsNullOrEmpty(mapDirectory))
                SetMaps(mapDirectory);
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

        private void GenerateColorMap()
        {
            if (_selectedHeightMap == null) return;

            var terrainHelper = new TerrainHelper();
            TerrainHelper.SetTerrains(CreatePixelMatrix(_selectedHeightMap));

            var colorMapDictionary = terrainHelper.GetAllTerrains()
                .OrderBy(t=>t.UpperBound)
                .ToDictionary(t => t.UpperBound, t => Color.FromArgb(t.Color.R, t.Color.G, t.Color.B));

            var bitmap = new Bitmap(this._selectedHeightMap);
            using (var fastBitmap = bitmap.FastLock())
            {
                for (int i = 0; i < fastBitmap.Width; i++)
                {
                    for (int j = 0; j < fastBitmap.Height; j++)
                    {
                        var pixel = fastBitmap.GetPixel(i, j);
                        var intensity = (pixel.R + pixel.G + pixel.B) / 3;
                        var color = colorMapDictionary.First(f => f.Key >= intensity).Value;
                        fastBitmap.SetPixel(i, j, color);
                    }
                }
            }

            var mapDirectory = MapDirectory + $"/{_selectedHeightMap.Tag.ToString().Replace(".jpg", "_color.png").Replace(".png", "_color.png")}";
            bitmap.Save(mapDirectory);

            bitmap.Tag = $"{_selectedHeightMap.Tag.ToString().Replace(".png","_color.png")}";
            Maps.Add(bitmap);
            SelectedColorMap = bitmap;
        }

        private void OpenFolder()
        {
            var dialog = new CommonOpenFileDialog { IsFolderPicker = true, Multiselect = true };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                SetMaps(dialog.FileName);
            }
        }

        private void SetMaps(string directory)
        {
            MapDirectory = directory;
            string[] allowedExtensions = new[] { ".png", ".jpg" };
            var files = Directory.GetFiles(directory)
                .Where(file => allowedExtensions.Any(file.ToLower().EndsWith))
                .Select(file => new Bitmap(file) { Tag = file.Split('\\').Last() })
                .OrderBy(b=>b.Tag);
            Maps = new ObservableCollection<Bitmap>(files);
        }

        private Pixel[,] CreatePixelMatrix(Bitmap bitmap)
        {
            var pixels = new Pixel[bitmap.Width, bitmap.Height];
            using (var fastBitmap = bitmap.FastLock())
            {
                for (int i = 0; i < fastBitmap.Width; i++)
                {
                    for (int j = 0; j < fastBitmap.Height; j++)
                    {
                        var pixel = fastBitmap.GetPixel(i, j);
                        pixels[i, j] = new Pixel(pixel.R, pixel.G, pixel.B);
                    }
                }
            }
            return pixels;
        }

    }
}