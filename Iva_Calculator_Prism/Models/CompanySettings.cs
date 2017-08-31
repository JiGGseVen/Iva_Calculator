using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Iva_Calculator_Prism.Models
{
    [Serializable]
    public class CompanySettings
    {
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public ObservableCollection<BoughtProduct> BoughtProductsList { get; set; }
        public ObservableCollection<SoldProduct> SoldProductsList { get; set; }

    }
}
