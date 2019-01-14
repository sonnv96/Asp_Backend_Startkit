using Nois.WpfApp.Framework.View;
using System.Windows.Input;

namespace Nois.App.Operation.Models
{
    public class SelectColumn : ViewModel
    {
        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                RaisePropertyChanged("Title");
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }

        public ICommand CheckboxCheck { get; set; }
    }
}
