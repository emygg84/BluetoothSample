using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BluetoothSample.ViewModels;
using BluetoothSample.Tests.FakeModels;
using Plugin.BLE.Abstractions.Contracts;

namespace BluetoothSample.Tests.ViewModels
{
    [TestClass]
    public class DevicesViewModelTests
    {
        [TestMethod]
        public void SortDevicesByRssiTest()
        {
            //Initialize
            var vm = new DevicesViewModel();

            //Arrange
            vm.DeviceList.Add(new DeviceBLE
            {
                Name = "Bluetooth 2",
                Rssi = 2
            });

            vm.DeviceList.Add(new DeviceBLE
            {
                Name = "Bluetooth 1",
                Rssi = 1
            });

            vm.DeviceList.Add(new DeviceBLE
            {
                Name = "Bluetooth 3",
                Rssi = 1
            });

            // Act
            vm.SortDevicesListByRssi();
            var firstDevice = vm.DeviceList.FirstOrDefault();
            var lastDevice = vm.DeviceList.LastOrDefault();

            //Assert
            Assert.AreEqual(firstDevice.Rssi, 1);
            Assert.AreEqual(lastDevice.Rssi, 3);
        }



        protected List<IDevice> GetDevices()
        {
            var devices = new List<IDevice>()
            {
                new DeviceBLE
                {
                    Name = "Bluetooth 2",
                    Rssi = 2
                },
                new DeviceBLE
                {
                    Name = "Bluetooth 1",
                    Rssi = 1
                },
                new DeviceBLE
                {
                    Name = "Bluetooth 1",
                    Rssi = 3
                }
            };  
            
            return devices;
        }
    }
}
