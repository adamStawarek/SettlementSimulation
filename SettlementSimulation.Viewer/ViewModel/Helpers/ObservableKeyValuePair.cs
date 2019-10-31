using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SettlementSimulation.Viewer.ViewModel.Helpers
{
    public class ObservableKeyValuePair<T, K> : INotifyPropertyChanged
    {
        private T _key;
        private K _value;

        public ObservableKeyValuePair(T key, K value)
        {
            Key = key;
            Value = value;
        }

        public T Key
        {
            get => _key;
            set
            {
                if (Equals(value, _key)) return;
                _key = value;
                OnPropertyChanged();
            }
        }

        public K Value
        {
            get => _value;
            set
            {
                if (Equals(value, _value)) return;
                _value = value;
                OnPropertyChanged();
            }
        }


        #region property changed
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}