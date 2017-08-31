using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iva_Calculator_Prism.Notifications
{
    public class CompanyNameAddedNotification : Confirmation
    {
        public string NewCompanyName { get; set; }

        public CompanyNameAddedNotification()
        {
            NewCompanyName = null;
        }
    }
}
