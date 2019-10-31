using GalaSoft.MvvmLight;

namespace SettlementSimulation.Viewer.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public StepperTwoViewModel StepperTwoViewModel { get; }
        public StepperOneViewModel StepperOneViewModel { get; }
        public MainViewModel()
        {
            var serviceLocator = new ViewModelLocator();
            StepperTwoViewModel = serviceLocator.StepperTwo;
            StepperOneViewModel = serviceLocator.StepperOne;
        }
    }
}