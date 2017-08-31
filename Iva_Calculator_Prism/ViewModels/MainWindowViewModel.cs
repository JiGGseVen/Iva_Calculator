using System;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Events;
using Iva_Calculator_Prism.Models;
using Iva_Calculator_Prism.Services;
using Iva_Calculator_Prism.Events;

namespace Iva_Calculator_Prism.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;

        private AppSettings _appSettings;

        private string _title = "Calculador Iva";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public DelegateCommand<string> NavigateCommand { get; set; }

        public MainWindowViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<UpdateAppSettingsEvent>().Subscribe(UpdateAppSettings);
            NavigateCommand = new DelegateCommand<string>(Navigate);
            LoadAppSettings();
        }
        private void LoadAppSettings()
        {
            _appSettings = FileIOService.GetAppSettings();
        }

        private void UpdateAppSettings(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        private void Navigate(string uri)
        {
            NavigationParameters parameters = new NavigationParameters();
            parameters.Add("AppSettings", _appSettings);
            _regionManager.RequestNavigate("ContentRegion", uri, parameters);
        }
        
    }
}
