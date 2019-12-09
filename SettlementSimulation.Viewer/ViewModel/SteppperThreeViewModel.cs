using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using SettlementSimulation.AreaGenerator.Models;
using SettlementSimulation.Engine;
using SettlementSimulation.Engine.Helpers;
using SettlementSimulation.Engine.Models.Buildings;
using SettlementSimulation.Viewer.Commands;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SettlementSimulation.Viewer.ViewModel
{
    public class StepperThreeViewModel : ViewModelBase
    {
        #region fields
        private SettlementInfo _settlementInfo;
        private StructureGenerator _generator;
        private readonly ViewModelLocator _viewModelLocator;
        #endregion

        #region properties
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
        public Dictionary<string, Color> StructuresLegend { get; set; }
        #endregion

        #region commands
        public RelayCommand StartSimulationCommand { get; }
        #endregion

        public StepperThreeViewModel()
        {
            _viewModelLocator = new ViewModelLocator();

            Messenger.Default.Register<SetSettlementInfoCommand>(this, this.SetSettlementInfo);
            StructuresLegend = new Dictionary<string, Color>();
            CurrentGeneration = 0;

            StartSimulationCommand = new RelayCommand(StartSimulation);
            SetUpStructureLegend();
        }

        private void SetSettlementInfo(SetSettlementInfoCommand obj)
        {
            _settlementInfo = obj.SettlementInfo;
        }

        private void StartSimulation()
        {
            var maxIterations = _viewModelLocator.Designer.EndY;
            var breakpoints = _viewModelLocator.Designer.Breakpoints;

            _generator = new StructureGeneratorBuilder()
                .WithMaxIterations(maxIterations)
                .WithBreakpoints(breakpoints)
                .WithFields(_settlementInfo.Fields)
                .WithMainRoad(_settlementInfo.MainRoad)
                .Build();

            _generator.Breakpoint += OnBreakpoint;
            _generator.NextEpoch += OnNextEpoch;
            _generator.Finished += OnFinished;
            
            _generator.Start();
        }

       
        private void OnBreakpoint(object sender, EventArgs e)
        {
            var settlementState = _generator.SettlementState;
            //DO something with settlement state
        }

        private void OnNextEpoch(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }


        private void OnFinished(object sender, EventArgs e)
        {
            //update map and current generation, hide spinner, display message box
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
