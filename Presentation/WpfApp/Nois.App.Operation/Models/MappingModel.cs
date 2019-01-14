using Nois.Core.Domain.SortedCode;
using Nois.WpfApp.Framework.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nois.App.Operation.Models
{
    public class MappingModel : ViewModelMediator
    {
        private string _chuteNo;
        public string ChuteNo
        {
            get { return _chuteNo; }
            set
            {
                _chuteNo = value;
                RaisePropertyChanged("ChuteNo");
            }
        }

        private string _chute;
        public string Chute
        {
            get { return _chute; }
            set
            {
                _chute = value;
                RaisePropertyChanged("Chute");
            }
        }

        private ICollection<Destination> _destinations;
        public ICollection<Destination> Destinations
        {
            get { return _destinations; }
            set
            {
                _destinations = value;
                RaisePropertyChanged("Destinations");
            }
        }

        private int _quantity;
        public int Quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value;
                RaisePropertyChanged("Quantity");
            }
        }

        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                RaisePropertyChanged("IsActive");
            }
        }

        public override int Priority
        {
            get
            {
                return 0;
            }
        }
    }
}
