using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using BluetoothSample.Views;

namespace BluetoothSample.ViewModels
{
    public class MainViewModel
    {
        public Command ShowDevicesListCommand { get; set; }

        public MainViewModel()
        {
            ShowDevicesListCommand = new Command(ShowDevicesList);
        }

        public void ShowDevicesList()
        {
            App.Current.MainPage.Navigation.PushAsync(new DevicesView());
        }
    }
}
