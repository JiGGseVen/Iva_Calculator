using Iva_Calculator_Prism.Events;
using Iva_Calculator_Prism.Models;
using Iva_Calculator_Prism.Notifications;
using Iva_Calculator_Prism.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Iva_Calculator_Prism.ViewModels
{
    public class SettingsViewModel : BindableBase, INavigationAware
    {
        private AppSettings _appSettings;
        public AppSettings AppSettings
        {
            get { return _appSettings; }
            set
            {
                SetProperty(ref _appSettings, value);
            }
        }

        private ObservableCollection<CompanySettings> _companiesList;
        public ObservableCollection<CompanySettings> CompaniesList
        {
            get { return _companiesList; }
            set
            {
                SetProperty(ref _companiesList, value);
            }
        }

        private ObservableCollection<BoughtProduct> _companyBoughtList;
        public ObservableCollection<BoughtProduct> CompanyBoughtList
        {
            get { return _companyBoughtList; }
            set
            {
                SetProperty(ref _companyBoughtList, value);
            }
        }

        private ObservableCollection<SoldProduct> _companySoldList;
        public ObservableCollection<SoldProduct> CompanySoldList
        {
            get { return _companySoldList; }
            set
            {
                SetProperty(ref _companySoldList, value);
            }
        }

        private string _savedStatus;
        public string SavedStatus
        {
            get { return _savedStatus; }
            set
            {
                SetProperty(ref _savedStatus, value);
            }
        }

        private readonly IEventAggregator _eventAggregator;
        public DelegateCommand<CompanySettings> SelectionChangedCommand { get; set; }
        public DelegateCommand RaiseAddCompanyPopupViewCommand { get; private set; }
        public DelegateCommand<CompanySettings> RemoveCompanyCommand { get; set; }
        public DelegateCommand<object> CellEditingEndingCommand { get; set; }
        public InteractionRequest<CompanyNameAddedNotification> AddCompanyPopupViewRequest { get; private set; }
        public InteractionRequest<IConfirmation> ConfirmationRequest { get; private set; }

        private CompanySettings CurrentCompany;

        public SettingsViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            SelectionChangedCommand = new DelegateCommand<CompanySettings>(CompaniesListSelectionChanged);
            RemoveCompanyCommand = new DelegateCommand<CompanySettings>(RemoveSelectedCompany);
            AddCompanyPopupViewRequest = new InteractionRequest<CompanyNameAddedNotification>();
            RaiseAddCompanyPopupViewCommand = new DelegateCommand(RaiseAddCompanyPopupView);
            CellEditingEndingCommand = new DelegateCommand<object>(CellEditingFinished);
            ConfirmationRequest = new InteractionRequest<IConfirmation>();

            CompaniesList = new ObservableCollection<CompanySettings>();
            CompanyBoughtList = new ObservableCollection<BoughtProduct>();
            CompanySoldList = new ObservableCollection<SoldProduct>();
        }

        private void CellEditingFinished(object obj)
        {
            
        }

        private void RaiseAddCompanyPopupView()
        {
            // In this case we are passing a simple notification as a parameter.
            // The custom popup view we are using for this interaction request does not have a DataContext of its own
            // so it will inherit the DataContext of the window, which will be this same notification.

            CompanyNameAddedNotification notification = new CompanyNameAddedNotification();
            notification.Title = "Adicionar nova Empresa";

            AddCompanyPopupViewRequest.Raise(notification,
                            returned =>
                            {
                                if (returned != null && returned.Confirmed && returned.NewCompanyName != null)
                                {
                                    CompaniesList.Add(new CompanySettings()
                                    {
                                        CompanyName = returned.NewCompanyName,
                                        BoughtProductsList = new ObservableCollection<BoughtProduct>(),
                                        SoldProductsList = new ObservableCollection<SoldProduct>()
                                    });
                                }
                            });

            if(CompaniesList.Count == 1)
                CurrentCompany = CompaniesList.First();
        }

        private void RemoveSelectedCompany(CompanySettings selectedCompany)
        {
            CompaniesList.Remove(selectedCompany);
        }
        
        private void CompaniesListSelectionChanged(CompanySettings selectedCompany)
        {
            CurrentCompany = selectedCompany;
            LoadCurrentCompanySettings(CurrentCompany);
        }

        private void UpdateAppSettings()
        {
            _eventAggregator.GetEvent<UpdateAppSettingsEvent>().Publish(AppSettings);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            var parameters = navigationContext.Parameters;
            AppSettings = (AppSettings)parameters["AppSettings"];
            LoadCompaniesList();

            if(CompaniesList.Count > 0)
            {
                CurrentCompany = CompaniesList.First();
                LoadCurrentCompanySettings(CurrentCompany);
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            if(VerifyChangeOnCollection())
                SaveCurrentSettings();
        }

        private void LoadCompaniesList()
        {
            if(AppSettings.CompanySettingsList != null)
                CompaniesList = new ObservableCollection<CompanySettings>(AppSettings.CompanySettingsList);
        }

        private void LoadCurrentCompanySettings(CompanySettings company)
        {
            if (company != null)
            {
                CompanyBoughtList = new ObservableCollection<BoughtProduct>(company.BoughtProductsList.Cast<BoughtProduct>().ToList());
                CompanySoldList = new ObservableCollection<SoldProduct>(company.SoldProductsList.Cast<SoldProduct>().ToList());
            }
        }

        public void SaveCurrentSettings()
        {
            FileIOService.SaveAppSettings(AppSettings);
        }

        private bool VerifyChangeOnCollection()
        {
            if (!CurrentCompany.BoughtProductsList.SequenceEqual(CompanyBoughtList) || 
                    !CurrentCompany.SoldProductsList.SequenceEqual(CompanySoldList))
            {
                if (ShowSaveChangesDialog())
                {
                    CurrentCompany.BoughtProductsList = CompanyBoughtList;
                    CurrentCompany.SoldProductsList = CompanySoldList;

                    if (AppSettings != null)
                    {
                        CompanySettings item = CompaniesList.Where(x => x == CurrentCompany).First();
                        AppSettings.CompanySettingsList = CompaniesList.ToList();
                    }

                    return true;

                }else
                {
                    return false;
                }
                
            }
            else
            {
                return false;
            }
        }

        private bool ShowSaveChangesDialog()
        {
            bool interactionResult = false;

            ConfirmationRequest.Raise(
                new Confirmation { Content = "Alteracoes foram feitas, deseja guardar?", Title = "Guardar alteracoes" },
                c => { interactionResult = c.Confirmed ? true : false; });

            return interactionResult;
        }
        
    }
}
