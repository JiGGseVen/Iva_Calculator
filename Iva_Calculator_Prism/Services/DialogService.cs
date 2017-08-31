using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Iva_Calculator_Prism.Services
{
    public static class DialogService
    {
        public static void ShowMessageBox(string message)
        {
            MessageBox.Show(message);
        }
    }
}
