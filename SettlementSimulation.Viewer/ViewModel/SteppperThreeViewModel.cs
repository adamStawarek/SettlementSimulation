using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using SettlementSimulation.Engine;
using SettlementSimulation.Engine.Models;

namespace SettlementSimulation.Viewer.ViewModel
{
    public class StepperThreeViewModel : ViewModelBase
    {
        #region properties
        private int _maxGeneration;
        public int MaxGeneration
        {
            get => _maxGeneration;
            set
            {
                _maxGeneration = value;
                SetPossibleBreakpoint();
                RaisePropertyChanged();
            }
        }
        private int _currentGeneration;
        public int CurrentGeneration
        {
            get => _currentGeneration;
            set
            {
                _currentGeneration = value;
                RaisePropertyChanged();
            }
        }
        public ObservableCollection<int> PossibleBreakpoints { get; set; }
        public ObservableCollection<int> SelectedBreakpoints { get; set; }
        public Dictionary<string, Color> StructuresLegend { get; set; }
        #endregion

        #region commands
        public RelayCommand<object> AddBreakpointCommand { get; }
        public RelayCommand<object> RemoveBreakpointCommand { get; }
        public RelayCommand StartSimulationCommand { get; }
        #endregion

        public StepperThreeViewModel()
        {
            PossibleBreakpoints = new ObservableCollection<int>();
            SelectedBreakpoints = new ObservableCollection<int>();
            StructuresLegend = new Dictionary<string, Color>();
            SelectedBreakpoints.CollectionChanged += delegate { SetPossibleBreakpoint(); };
            MaxGeneration = 1000;
            CurrentGeneration = 0;

            AddBreakpointCommand = new RelayCommand<object>(AddBreakpoint);
            RemoveBreakpointCommand = new RelayCommand<object>(RemoveBreakpoint);
            StartSimulationCommand = new RelayCommand(StartSimulation);

            SetUpStructureLegend();
        }

        private void StartSimulation()
        {
            throw new NotImplementedException();
        }

        private void SetUpStructureLegend()
        {
            var structures = ReflectionHelper.GetAllObjectsByType<Building>().ToList();
            for (int i = 0; i < structures.Count(); i++)
            {
                StructuresLegend.Add(structures[i].GetType().Name, GetRandColor(i));
            }
        }

        private void RemoveBreakpoint(object obj)
        {
            SelectedBreakpoints.Remove((int)obj);
        }

        private void AddBreakpoint(object obj)
        {
            SelectedBreakpoints.Add((int)obj);
        }

        private void SetPossibleBreakpoint()
        {
            PossibleBreakpoints.Clear();
            for (int i = 0; i < _maxGeneration; i += 100)
            {
                if (!SelectedBreakpoints.Contains(i))
                    PossibleBreakpoints.Add(i);
            }
        }

        private Color GetRandColor(int index)
        {
            byte red = 0;
            byte green = 0;
            byte blue = 0;

            for (int t = 0; t <= index / 8; t++)
            {
                int index_a = (index + t) % 8;
                int index_b = index_a / 2;

                //Color writers, take on values of 0 and 1
                int color_red = index_a % 2;
                int color_blue = index_b % 2;
                int color_green = ((index_b + 1) % 3) % 2;

                int add = 255 / (t + 1);

                red = (byte)(red + color_red * add);
                green = (byte)(green + color_green * add);
                blue = (byte)(blue + color_blue * add);
            }

            Color color = Color.FromArgb(red, green, blue);
            return color;
        }
    }
}
