using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Management;
using System.Windows;

namespace HardwareSoftwareMonitor
{
    public class InstalledApplication
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string InstallLocation { get; set; }

        public InstalledApplication(RegistryKey _appKey)
        {
            object displayName = _appKey.GetValue("DisplayName");
            object displayVersion = _appKey.GetValue("DisplayVersion");
            object installLocation = _appKey.GetValue("InstallLocation");

            if (displayName != null)
                Name = displayName.ToString();
            else
                Name = "";

            if (displayVersion != null)
                Version = _appKey.GetValue("DisplayVersion").ToString();
            else
                Version = "";

            if (installLocation != null)
                InstallLocation = _appKey.GetValue("InstallLocation").ToString();
            else
                InstallLocation = "";
        }

    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BindingList<InstalledApplication> _softwareList = new BindingList<InstalledApplication>();

        public MainWindow()
        {
            InitializeComponent();

            /*Hardverelemek*/
            MotherboardManufacturer.Content = GetComponent("Win32_BaseBoard", "Manufacturer");
            MotherboardProduct.Content = GetComponent("Win32_BaseBoard", "Product");
            Processor.Content = GetComponent("Win32_Processor", "Name");
            Videocard.Content = GetComponent("Win32_VideoController", "Name");
            BIOSManufacturer.Content = GetComponent("Win32_BIOS", "Manufacturer");
            BIOS_Name.Content = GetComponent("Win32_BIOS", "Name");

            /*Szoftver*/
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"))
            {
                foreach (string item in key.GetSubKeyNames())
                {
                    RegistryKey appKey = key.OpenSubKey(item);
                    _softwareList.Add(new InstalledApplication(appKey));
                }
            }

            SoftwareGrid.ItemsSource = _softwareList;
        }
        static string GetComponent(string hardwareClass, string syntax)
        {
            string returnValue = "";

            ManagementObjectSearcher mos = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM " + hardwareClass);
            foreach (ManagementObject mj in mos.Get())
            {
                if (Convert.ToString(mj[syntax]) != "")
                    returnValue += Convert.ToString(mj[syntax]);
            }

            return returnValue;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<string> outputString = new List<string>();
            outputString.Add("dataname;value");
            outputString.Add(string.Join(";", "Motherboard Manufacturer", GetComponent("Win32_BaseBoard", "Manufacturer")));
            outputString.Add(string.Join(";", "Motherboard", GetComponent("Win32_BaseBoard", "Product")));
            outputString.Add(string.Join(";", "Processor", GetComponent("Win32_Processor", "Name")));
            outputString.Add(string.Join(";", "VideoCard", GetComponent("Win32_VideoController", "Name")));
            outputString.Add(string.Join(";", "BIOS Manufacturer", GetComponent("Win32_BIOS", "Manufacturer")));
            outputString.Add(string.Join(";", "BIOSName", GetComponent("Win32_BIOS", "Name")));

            outputString.Add("applicationame;applicationversion;installlocation");
            foreach(var i in _softwareList)
            {
                if(i.Name != "")
                outputString.Add(string.Join(";", i.Name, i.Version, i.InstallLocation));
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog() { 
                Filter = "Comma-separated values file (.csv) | *.csv"
            };

            if(saveFileDialog.ShowDialog() == true)
            {
                System.IO.File.WriteAllLines(saveFileDialog.FileName, outputString);
            }
        }
    }
}
