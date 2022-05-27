using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using BluetoothSample.Resources;

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
            SortDevicesCommand = new Command(SortDevicesListByRssi);

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

            Status = AppResources.BluetoothIsReady;
            HasRows = false;
        }

        private void BleHandler_StateChanged(object sender, BluetoothStateChangedArgs e)
        {
            Status = $"{AppResources.BluetoothStatus}: {e.NewState}";
            IsConnected = e.NewState == BluetoothState.On;
        }

        private void BleAdapter_ScanTimeoutElapsed(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Bluetooth scan finished");
            Status = $"{AppResources.BluetoothStatus}: {bleHandler.State}";
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
            HasRows = DeviceList.Count > 0;
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

        private bool _isConnected;
        public bool IsConnected
        {
            get
            {
                return _isConnected;
            }
            set
            {
                _isConnected = value;
                OnPropertyChanged("IsConnected");
            }
        }

        private bool _hasRows;
        public bool HasRows
        {
            get
            {
                return _hasRows;
            }
            set
            {
                _hasRows = value;
                OnPropertyChanged("HasRows");
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
                    Status = AppResources.ScanningDevices;
                    DeviceList.Clear();
                    //await bleAdapter.StartScanningForDevicesAsync();
                    var systemDevices = bleAdapter.GetSystemConnectedOrPairedDevices();
                    systemDevices.ForEach(e => DeviceList.Add(e));
                    HasRows = DeviceList.Count > 0;
                }
            }
            catch (System.Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("BluetoothSample", ex.Message, "Ok");
            }
        }

        public Command SortDevicesCommand { get; set; }

        public void SortDevicesListByRssi()
        {
            if (DeviceList.Count == 0) return;
            var list = new List<IDevice>();
            list.AddRange(DeviceList.OrderBy(d => d.Rssi));
            DeviceList.Clear();
            list.ForEach(e => DeviceList.Add(e));
            HasRows = DeviceList.Count > 0;
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
