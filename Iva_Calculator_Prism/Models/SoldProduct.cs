using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Iva_Calculator_Prism.Models
{
    public class SoldProduct : Product, INotifyPropertyChanged
    {
        private decimal _soldAmount;
        [XmlIgnore]
        public decimal SoldAmount
        {
            get { return _soldAmount; }
            set
            {
                _soldAmount = value;
                UpdateFinalAmount();
                OnPropertyChanged();
            }
        }

        private decimal _returnAmount;
        [XmlIgnore]
        public decimal ReturnAmount
        {
            get { return _returnAmount; }
            set
            {
                _returnAmount = value;
                UpdateFinalAmount();
                OnPropertyChanged();
            }
        }

        private decimal _discountAmount;
        [XmlIgnore]
        public decimal DiscountAmount
        {
            get { return _discountAmount; }
            set
            {
                _discountAmount = value;
                UpdateFinalAmount();
                OnPropertyChanged();
            }
        }

        private decimal _finalAmount;
        [XmlIgnore]
        public decimal FinalAmount
        {
            get { return _finalAmount; }
            set
            {
                _finalAmount = value;
                OnPropertyChanged();
            }
        }

        public SoldProduct()
        {
            _soldAmount = 0;
            _returnAmount = 0;
            _discountAmount = 0;
            _finalAmount = 0;
        }
        
        private void UpdateFinalAmount()
        {
            FinalAmount = SoldAmount - (ReturnAmount + DiscountAmount);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
