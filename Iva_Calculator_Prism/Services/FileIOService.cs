using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using Iva_Calculator_Prism.Models;
using Microsoft.Win32;

namespace Iva_Calculator_Prism.Services
{
    public static class FileIOService
    {
        private static readonly string settingsFilePath = AppDomain.CurrentDomain.BaseDirectory + "config2.xml";
        public static AppSettings GetAppSettings()
        {
            if (!File.Exists(settingsFilePath)) // create config file with default values
            {
                try
                {
                    using (FileStream fs = new FileStream(settingsFilePath, FileMode.Create))
                    {
                        XmlSerializer xs = new XmlSerializer(typeof(AppSettings));
                        AppSettings sxml = new AppSettings();
                        xs.Serialize(fs, sxml);
                        return sxml;
                    }
                }
                catch (Exception)
                {
                    throw new FileLoadException();
                }
                
            }
            else // read configuration from file
            {
                using (FileStream fs = new FileStream(settingsFilePath, FileMode.Open))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(AppSettings));
                    AppSettings sc = (AppSettings)xs.Deserialize(fs);
                    return sc;
                }
            }
        }

        public static bool SaveAppSettings(AppSettings appSettings)
        {
            if (!File.Exists(settingsFilePath))
            {
                return false; // don't do anything if file doesn't exist
            }

            using (FileStream fs = new FileStream(settingsFilePath, FileMode.Open))
            {
                XmlSerializer xs = new XmlSerializer(typeof(AppSettings));
                xs.Serialize(fs, appSettings);
                return true;
            }
        }

        public static Stream LoadSaftFileData()
        {
            Stream fileStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = "C:\\";
            openFileDialog1.Filter = "Ficheiros XML (*.xml)|*.xml";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            
            if ((bool)openFileDialog1.ShowDialog())
            {
                try
                {
                    if ((fileStream = openFileDialog1.OpenFile()) != null)
                    {
                        return fileStream;
                    }
                }
                catch (Exception ex)
                {
                    DialogService.ShowMessageBox(ex.Message);
                }
            }

            return null;
        }
    }
}
