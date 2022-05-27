using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace BluetoothSample.ViewModels
{
    public class DevicesViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // gestor de BLE
        private IAdapter bleAdapter;
        private IBluetoothLE bleHandler;

        public DevicesViewModel()
        {
            DeviceList = new ObservableCollection<IDevice>();
            ScanBLECommand = new Command(ScanBLE);

            // obteniendo las instancias del hardware ble
            bleHandler = CrossBluetoothLE.Current;
            bleAdapter = CrossBluetoothLE.Current.Adapter;

            // configurando evento inicial del proceso de escaneo de dispositivos 
            // y cambios de estado
            bleHandler.StateChanged += BleHandler_StateChanged;

            bleAdapter.ScanMode = ScanMode.LowPower; // se establece que el escaneo de advertising se optimiza para bajo consumo 
            bleAdapter.ScanTimeout = 120000; // tiempo de busqueda de dispositivos en advertising
            bleAdapter.ScanTimeoutElapsed += BleAdapter_ScanTimeoutElapsed;

            // cuando se 'descubre' un dispositivo
            // se ejecuta cuando BLE encuentra un dispositivo que esta en advertising
            bleAdapter.DeviceDiscovered += BleAdapter_DeviceDiscovered;

            Status = "Ready...";
        }

        private void BleHandler_StateChanged(object sender, BluetoothStateChangedArgs e)
        {
            Status = $"Bluetooth status: {e.NewState}";
        }

        private void BleAdapter_ScanTimeoutElapsed(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Bluetooth scan finished");
            Status = $"Bluetooth status: {bleHandler.State}";
        }

        private void BleAdapter_DeviceDiscovered(object sender, DeviceEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Bluetooth device discovered");
            var device = e.Device;

            // buscando en la lista de dispositivos en memoria si ya existe
            //List<IDevice> lstDispositivoRepetido = (from disps in modelo.ListaDispositivos
            //                                        where disps.Name == dispositivoDescubierto.Name
            //                                        select disps).ToList();

            //// no hay repetidos
            //if (!lstDispositivoRepetido.Any())
            //{
            DeviceList.Add(device);
            //}

        }

        private string _status;
        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                OnPropertyChanged("Status");
            }
        }

        private ObservableCollection<IDevice> _deviceList;
        public ObservableCollection<IDevice> DeviceList
        {
            get
            {
                return _deviceList;
            }
            set
            {
                _deviceList = value;
                OnPropertyChanged("DeviceList");
            }
        }

        public Command ScanBLECommand { get; set; }
        
        public async void ScanBLE()
        {
            try
            {
                if (!bleAdapter.IsScanning)
                {
                    System.Diagnostics.Debug.WriteLine("Comienza el escaneo");
                    Status = "Escanenado por dispositivos BLE";
                    DeviceList.Clear();
                    //await bleAdapter.StartScanningForDevicesAsync();
                    var systemDevices = bleAdapter.GetSystemConnectedOrPairedDevices();
                    systemDevices.ForEach(e => DeviceList.Add(e));
                }
            }
            catch (System.Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("BluetoothSample", ex.Message, "Ok");
            }
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
