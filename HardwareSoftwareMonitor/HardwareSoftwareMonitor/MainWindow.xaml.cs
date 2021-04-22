using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Windows;

namespace HardwareSoftwareMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
    }
}
