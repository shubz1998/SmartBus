using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Geolocation;
using Windows.UI.Popups;
using Windows.Services.Maps;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Core;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Data;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;
using Windows.ApplicationModel;

namespace smartbus
{
    public sealed partial class DriverMain  : Page
    {
        public DriverMain()
        {
            this.InitializeComponent();
            this.Loaded += DriverMain_Loaded;
        }

        MapIcon DriverLocIcon = new MapIcon();

        
        private async void DriverMain_Loaded(object sender, RoutedEventArgs e)
        {
            var AccessStatus = await Geolocator.RequestAccessAsync();
            DriverMap.ZoomLevel = 15;
            DriverLocIcon.Title = "Your Location";
            DriverLocIcon.NormalizedAnchorPoint=new Point(0.5,1);
            Package package = Package.Current;
            PackageId packageId = package.Id;
            String output = packageId.Name;
            string str = string.Format("ms-appx://{0}/Assets/bus.png", output);
            DriverLocIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri(str));

            switch (AccessStatus)
            {
                case GeolocationAccessStatus.Allowed:
                    Geolocator geolocator = new Geolocator { DesiredAccuracyInMeters = 1 , ReportInterval = 10000 };
                    try
                    {
                        Geoposition map_pos = await geolocator.GetGeopositionAsync();
                        UpdateLocationData(map_pos);
                    }
                    catch(Exception E)
                    {
                        var ErrMsg = new MessageDialog(E.Message.ToString(), "Error");
                        await ErrMsg.ShowAsync();
                    }
                    geolocator.PositionChanged += geolocator_PostionChanged;
                    break;

                case GeolocationAccessStatus.Denied:
                    var Err1Msg = new MessageDialog("Please Provide Access to your locaion.", "Error");
                    await Err1Msg.ShowAsync();
                    break;
                case GeolocationAccessStatus.Unspecified:
                    var Err2Msg = new MessageDialog("Please Provide Access to your locaion.", "Error");
                    await Err2Msg.ShowAsync();
                    break;
            }

//            FetchArduinoData(); // TO BE IMPLEMENTED
            
        }

        private async void FetchArduinoData()
        {
            // ID"S HAVE TO BE CHANGED ACCORDING TO YOUR DEVICES
            UInt16 vid = 0x045E;
            UInt16 pid = 0x078F;

            string aqs = SerialDevice.GetDeviceSelectorFromUsbVidPid(vid, pid);

            var myDevices = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(aqs, null);

            if (myDevices.Count == 0)
            {
                var Err2Msg = new MessageDialog("Device not found!", "Error");
                await Err2Msg.ShowAsync();
                return;
            }

            SerialDevice device = await SerialDevice.FromIdAsync(myDevices[0].Id);

            //FindAllAsync(aqs);
        }

        private async void geolocator_PostionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                UpdateLocationData(args.Position);
            });
        }

        private async void logout_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
            //onlogout make Logged_In false
           
            DriversTable newdriver = new DriversTable
            {
                islogin = false,
                id = DriverDetail.Driver_ID,
                username = DriverDetail.Driver_Username,
                password = DriverDetail.Driver_Password
            };

            App.MobileService.GetTable<DriversTable>().UpdateAsync(newdriver);

            List<BusLocation> AllBusLocation = await App.MobileService.GetTable<BusLocation>().ToListAsync();
            foreach (BusLocation Bus in AllBusLocation)
            {
                if (Bus.Bus_Id == DriverDetail.Driver_ID)
                {
                    BusLocation NewBus = new BusLocation
                    {
                        id = Bus.id,
                        islogin = false,
                        Bus_Id = DriverDetail.Driver_ID,
                    };
                    App.MobileService.GetTable<BusLocation>().UpdateAsync(NewBus);
                    break;
                }
            }
            DriverDetail.Driver_Username = string.Empty;
            DriverDetail.Driver_Password = string.Empty;
            DriverDetail.Driver_ID = string.Empty;
        }

        private async void UpdateLocationData(Geoposition pos)
        {
            DriverMap.MapElements.Remove(DriverLocIcon);
            BasicGeoposition PointLoc = new BasicGeoposition()
            {
                Latitude = pos.Coordinate.Latitude,
                Longitude = pos.Coordinate.Longitude
            };
            DriverLocIcon.Location = new Geopoint(PointLoc);
            DriverMap.MapElements.Add(DriverLocIcon);
            DriverMap.Center = new Geopoint(PointLoc);

            try
            {
                List<BusLocation> AllBusLocation = await App.MobileService.GetTable<BusLocation>().ToListAsync();
                foreach (BusLocation Bus in AllBusLocation)
                {
                    if (Bus.Bus_Id == DriverDetail.Driver_ID)
                    {
                        try
                        {
                            Bus.Longitude = pos.Coordinate.Longitude.ToString("0.00000000");
                            Bus.Latitude = pos.Coordinate.Latitude.ToString("0.00000000");
                            Bus.islogin = true;
                            App.MobileService.GetTable<BusLocation>().UpdateAsync(Bus);
                            break;
                        }
                        catch (Exception ex)
                        {
                            var ErrMsg = new MessageDialog("Failed to connect to the Server.", "Error");
                            await ErrMsg.ShowAsync();
                            break;
                        }
                    }
                }
            }
            catch
            {
                var ErrMsg = new MessageDialog("No internet connection", "Error");
                await ErrMsg.ShowAsync();
            }
        }
        private string Dest = string.Empty;
        private bool check1 = false;
        private bool check2 = false;
        private bool check3 = false;
        private bool check4 = false;

        private void submit_Click(object sender, RoutedEventArgs e)
        {
            Dest = destination.Text;
            if (check_1.IsChecked == true)
            {
                check1 = true;
            }
            else
            {
                check1 = false;
            }

            if (check_2.IsChecked == true)
            {
                check2 = true;
            }
            else
            {
                check2 = false;
            }

            if (check_3.IsChecked == true)
            {
                check3 = true;
            }
            else
            {
                check3 = false;
            }

            if (check_4.IsChecked == true)
            {
                check4 = true;
            }
            else
            {
                check4 = false;
            }

            DriversTable newuser = new DriversTable
            {
                islogin = true,
                id = DriverDetail.Driver_ID,
                username = DriverDetail.Driver_Username,
                password = DriverDetail.Driver_Password,
                dest = Dest,
                check1 = check1,
                check2 = check2,
                check3 = check3,
                check4 = check4
            };
            App.MobileService.GetTable<DriversTable>().UpdateAsync(newuser);
        }
    }
}