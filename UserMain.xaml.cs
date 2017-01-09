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
using Windows.Services.Maps;
using Windows.Devices.Geolocation;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls.Maps;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.ApplicationModel;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace smartbus
{
    
    public sealed partial class UserMain : Page
    {
        public UserMain()
        {
            this.InitializeComponent();
            this.Loaded += UserMain_Loaded;
        }

        // Icons for map
        MapIcon UserLocIcon = new MapIcon();
        MapIcon[] DriverIcon = new MapIcon[25];
        private async void UserMain_Loaded(object sender, RoutedEventArgs e)
        {
            UserMap.MapServiceToken = "AKs4qGlBeOVCM1QL6YsY~dFLxTRCFZAK9cyU6nYjDow~AlfE7e6w-DJpHo-8d-_3-LKJW_sQlhBNMQh37nzvhlcenKKqnZiXCR48G87zL4Cn";
            UserMap.ZoomLevel = 15;
            var accessStatus = await Geolocator.RequestAccessAsync();

            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:
                    Geolocator geolocator = new Geolocator { DesiredAccuracyInMeters = 1, ReportInterval = 10000 };
                    try
                    {
                        Geoposition map_pos = await geolocator.GetGeopositionAsync();
                        UserLocIcon.NormalizedAnchorPoint = new Point(0.5,1);
                        UserLocIcon.Title = "Your Location";
                        Package packageUsr = Package.Current;
                        PackageId packageIdUsr = packageUsr.Id;
                        String outputUsr = packageIdUsr.Name;
                        string strUsr = string.Format("ms-appx://{0}/Assets/user.png", outputUsr);
                        UserLocIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri(strUsr));

                        Package packageDrv = Package.Current;
                        PackageId packageIdDrv = packageDrv.Id;
                        String outputDrv = packageIdDrv.Name;
                        string strDrv = string.Format("ms-appx://{0}/Assets/bus.png", outputDrv);
                        for (int i = 0; i < 25; i++)
                        {
                            DriverIcon[i] = new MapIcon();
                            DriverIcon[i].NormalizedAnchorPoint = new Point(0.5, 1);
                            DriverIcon[i].Image = RandomAccessStreamReference.CreateFromUri(new Uri(strDrv));
                            DriverIcon[i].ZIndex = i;
                        }
                        GetDriverPos();
                        BasicGeoposition PointLoc = new BasicGeoposition()
                        {
                            Latitude = map_pos.Coordinate.Latitude,
                            Longitude = map_pos.Coordinate.Longitude
                        };
                        UserLocIcon.Location = new Geopoint(PointLoc);
                        UserMap.MapElements.Add(UserLocIcon);
                        UserMap.Center = new Geopoint(PointLoc);
                        UpdateLocationData(map_pos);
                    }
                    catch (Exception E)
                    {
                        var Err3Msg = new MessageDialog(E.Message.ToString(), "Error");
                        await Err3Msg.ShowAsync();
                    }

                    // if the user position stored in the variable "geolocator" changes, then change in the map also
                    geolocator.PositionChanged += geolocator_PostionChanged;
                    break;
                    
                case GeolocationAccessStatus.Denied:
                    var ErrMsg = new MessageDialog("Please Provide Access to your locaion.", "Error");
                    await ErrMsg.ShowAsync();
                    break;
                case GeolocationAccessStatus.Unspecified:
                    var Err2Msg = new MessageDialog("Please Provide Access to your locaion.", "Error");
                    await Err2Msg.ShowAsync();
                    break;      
            }
        }

        private async void UserMap_MapElementClick(MapControl sender, MapElementClickEventArgs args)
        {
            var msf = new MessageDialog("roopansh Sir");
            await msf.ShowAsync();
            foreach (var item in UserMap.MapElements.ToList())
            {
                UserMap.MapElements.Remove(item);
                UserMap.MapElements.Clear();
            }
        }

        private async void GetDriverPos()
        {
            await Task.Delay(10000);
            double Long , Lat;
            List<BusLocation> AllDriversPos = await App.MobileService.GetTable<BusLocation>().ToListAsync();
            int i = 0;
            foreach (BusLocation BusLoc in AllDriversPos)
            {
                if (BusLoc.islogin == true)
                {
                    Long = Double.Parse(BusLoc.Longitude);
                    Lat = Double.Parse(BusLoc.Latitude);
                    BasicGeoposition PointLoc = new BasicGeoposition()
                    {
                        Latitude = Lat,
                        Longitude = Long
                    };
                    UserMap.MapElements.Remove(DriverIcon[i]);
                    DriverIcon[i].Location = new Geopoint(PointLoc);
                    DriverIcon[i].Title = "Bus-" + (i+1);
                    UserMap.MapElements.Add(DriverIcon[i]);
                    i++;
                }
                else
                {
                    UserMap.MapElements.Remove(DriverIcon[i]);
                }
            }
            GetDriverPos();
            
        }

        private async void geolocator_PostionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                UpdateLocationData(args.Position);
            });
        }

        private void UpdateLocationData(Geoposition map_pos)
        {
            UserMap.MapElements.Remove(UserLocIcon);
            BasicGeoposition PointLoc = new BasicGeoposition()
            {
                Latitude = map_pos.Coordinate.Latitude,
                Longitude = map_pos.Coordinate.Longitude
            };
            UserLocIcon.Location = new Geopoint(PointLoc);
            UserMap.MapElements.Add(UserLocIcon);
            //UserMap.Center = new Geopoint(PointLoc);
        }

        private void logout_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void showdetails_Click(object sender, RoutedEventArgs e)
        {
            showDetails();
        }
        
        private async void showDetails()
        {
            int i = 0;
            details.Text = "";
            List<DriversTable> dd = await App.MobileService.GetTable<DriversTable>().ToListAsync();
            foreach (DriversTable driver in dd)
            {
                
                if (driver.islogin)
                {
                    ++i;
                    details.Text += "Bus Destination - " + i + " " + driver.dest + ".\n";
                }
            }
            await Task.Delay(5000);
            showDetails();
        }
    }
}