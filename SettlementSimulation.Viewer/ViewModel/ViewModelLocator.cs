using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;

namespace SettlementSimulation.Viewer.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<StepperOneViewModel>();
            SimpleIoc.Default.Register<StepperTwoViewModel>();
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public StepperOneViewModel StepperOne
        {
            get
            {
                return ServiceLocator.Current.GetInstance<StepperOneViewModel>();
            }
        }

        public StepperTwoViewModel StepperTwo
        {
            get
            {
                return ServiceLocator.Current.GetInstance<StepperTwoViewModel>();
            }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}