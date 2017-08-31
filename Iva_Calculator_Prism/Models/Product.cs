using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Iva_Calculator_Prism.Models
{
    [XmlInclude(typeof(BoughtProduct)) , XmlInclude(typeof(SoldProduct))]
    public class Product
    {
        private string _productName;
        public string ProductName
        {
            get { return _productName; }
            set
            {
                if(value != null)
                {
                    _productName = value;
                }
            }
        }

        private int _productAccountCode;
        public int ProductAccountCode
        {
            get { return _productAccountCode; }
            set
            {
                _productAccountCode = value;
            }
        }

        private int _productReturnCode;
        public int ProductReturnCode
        {
            get { return _productReturnCode; }
            set
            {
                _productReturnCode = value;
            }
        }

        private int _productDiscountCode;
        public int ProductDiscountCode
        {
            get { return _productDiscountCode; }
            set
            {
                _productDiscountCode = value;
            }
        }
        
        public void ChangeAccountName(string NewName)
        {
            ProductName = NewName;
        }

        public void ChangeAccountCode(int NewAccountCode)
        {
            ProductAccountCode = NewAccountCode;
        }
    }
}
