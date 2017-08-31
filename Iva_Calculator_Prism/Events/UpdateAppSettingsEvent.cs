using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Iva_Calculator_Prism.Models;

namespace Iva_Calculator_Prism.Events
{
    public class UpdateAppSettingsEvent : PubSubEvent<AppSettings>
    {
    }
}
