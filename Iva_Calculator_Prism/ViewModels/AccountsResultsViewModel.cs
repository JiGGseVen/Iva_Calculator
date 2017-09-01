using Iva_Calculator_Prism.Models;
using Iva_Calculator_Prism.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Controls;

namespace Iva_Calculator_Prism.ViewModels
{
    public class AccountsResultsViewModel : BindableBase, INavigationAware
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

        private List<CompanySettings> _companiesList;
        public List<CompanySettings> CompaniesList
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

        private string _currentMonth;
        public string CurrentMonth
        {
            get { return _currentMonth; }
            set
            {
                SetProperty(ref _currentMonth, value);
            }
        }

        private string _currentFileName;
        public string CurrentFileName
        {
            get { return _currentFileName; }
            set
            {
                SetProperty(ref _currentFileName, value);
            }
        }
       
        public DelegateCommand<CompanySettings> CompanyChangedCommand { get; set; }
        public DelegateCommand OpenFileCommand { get; set; }
        public DelegateCommand<ItemCollection> ExportToExcelCommand { get; set; }
        
        public AccountsResultsViewModel()
        {
            CompanyChangedCommand = new DelegateCommand<CompanySettings>(SelectedCompanyChanged);
            OpenFileCommand = new DelegateCommand(OpenSaftFile);
            ExportToExcelCommand = new DelegateCommand<ItemCollection>(ExportDataToExcel);
        }

        private void ExportDataToExcel(ItemCollection items)
        {
            List<Product> allProducts = new List<Product>();
            allProducts.AddRange(CompanyBoughtList.ToList());
            allProducts.AddRange(CompanySoldList.ToList());

            ExcelDataExportService.ExportDataToExcel(allProducts);
        }

        private void OpenSaftFile()
        {
            Stream stream = FileIOService.LoadSaftFileData();
            if (stream != null)
                LoadCompanieSaftValues(stream);
        }

        private void SelectedCompanyChanged(CompanySettings companySettings)
        {
            LoadCurrentCompanyValues(companySettings);
        }

        private void LoadCurrentCompanyValues(CompanySettings company)
        {
            if (company != null)
            {
                CompanyBoughtList = new ObservableCollection<BoughtProduct>(company.BoughtProductsList
                    .Cast<BoughtProduct>().ToList());
                CompanySoldList = new ObservableCollection<SoldProduct>(company.SoldProductsList
                    .Cast<SoldProduct>().ToList());
            }
        }

        private void LoadCompanieSaftValues(Stream fileStream)
        {
            using (fileStream)
            {
                XmlDataService XMLFileService = new XmlDataService(fileStream);

                GetMonthToProcess(XMLFileService);
                ProcessBoughtProducts(XMLFileService);
                ProcessSoldProducts(XMLFileService);
                GetFileName(fileStream);
            }
        }

        private void GetFileName(Stream fileStream)
        {
            var name = (string)fileStream.GetType().GetProperty("Name").GetValue(fileStream, null);
            if(name != null)
            {
                CurrentFileName = Path.GetFileNameWithoutExtension(name);
            }
        }

        private void GetMonthToProcess(XmlDataService xmlDataService)
        {
            CurrentMonth = "Mes processado: " + xmlDataService.GetMonthOfProcess();
        }

        private void ProcessBoughtProducts(XmlDataService xmlDataService)
        {
            if(CompanyBoughtList.Count > 0)
            {
                foreach (BoughtProduct product in _companyBoughtList)
                {
                    xmlDataService.FillProductAmounts(product);
                }
            }
        }

        private void ProcessSoldProducts(XmlDataService xmlDataService)
        {
            if (CompanySoldList.Count > 0)
            {
                foreach (SoldProduct product in _companySoldList)
                {
                    xmlDataService.FillProductAmounts(product);
                }
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            var parameters = navigationContext.Parameters;
            AppSettings = (AppSettings)parameters["AppSettings"];
            LoadCompaniesList();
        }
        private void LoadCompaniesList()
        {
            CompaniesList = AppSettings.CompanySettingsList;
        }
        
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }
    }
}
