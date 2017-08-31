using Iva_Calculator_Prism.Notifications;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iva_Calculator_Prism.ViewModels
{
    public class AddCompanyPopupViewModel : BindableBase, IInteractionRequestAware
    {
        private CompanyNameAddedNotification notification;

        private string _companyName;
        public string CompanyName
        {
            get { return _companyName; }
            set
            {
                SetProperty(ref _companyName, value);
            }
        }

        public DelegateCommand OkCommand { get; private set; }
        public DelegateCommand CancelCommand { get; private set; }
        public Action FinishInteraction { get; set; }

        public AddCompanyPopupViewModel()
        {
            OkCommand = new DelegateCommand(AcceptNewName);
            CancelCommand = new DelegateCommand(CancelInteraction);
        }

        public INotification Notification
        {
            get
            {
                return notification;
            }
            set
            {
                if (value is CompanyNameAddedNotification)
                {
                    notification = value as CompanyNameAddedNotification;
                    CompanyName = string.Empty;
                    OnPropertyChanged(() => Notification);
                }
            }
        }
        
        public void AcceptNewName()
        {
            if (notification != null)
            {
                notification.NewCompanyName = CompanyName;
                notification.Confirmed = true;
            }

            FinishInteraction();
        }

        public void CancelInteraction()
        {
            if (notification != null)
            {
                notification.NewCompanyName = string.Empty;
                notification.Confirmed = false;
            }

            FinishInteraction();
        }
    }
}
