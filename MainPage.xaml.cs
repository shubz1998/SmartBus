using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Data.SqlClient;
using Windows.Services.Maps;
using Windows.UI.Notifications;

namespace smartbus
{
  
    public sealed partial class MainPage : Page
    {
        private string input_username = string.Empty;
        private string input_password = string.Empty;
        private bool LoginStatus = false;
        public MainPage()
        {
            this.InitializeComponent();
        }
        private async void loginbuttonuser_Click(object sender, RoutedEventArgs e)
        {
            input_username = username_textbox.Text;
            input_password = pass_textbox.Password;
            /*
            var notificationXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
            var toeastElement = notificationXml.GetElementsByTagName("text");
            toeastElement[0].AppendChild(notificationXml.CreateTextNode("Trying to logging in as a USER"));
            toeastElement[1].AppendChild(notificationXml.CreateTextNode("It might take a few seconds"));
            var toastNotification = new ToastNotification(notificationXml);
            ToastNotificationManager.CreateToastNotifier().Show(toastNotification);
            */  
            if (input_username == "" || input_password == "")
            {
                var dialog = new MessageDialog("Please Enter Username and Password both", "Error");
                await dialog.ShowAsync();
            }
            else
            {
                List<UsersTable> AllUsers = await App.MobileService.GetTable<UsersTable>().ToListAsync();
                foreach (UsersTable user in AllUsers)
                {
                    if (input_username == user.username && input_password == user.password)
                    { 
                        this.Frame.Navigate(typeof(UserMain));
                        LoginStatus = true;
                        break;
                    }
                }
                if (!LoginStatus)
                {
                    var dialog = new MessageDialog("Username and Password doesn't exist or doesn't match!", "Error");
                    await dialog.ShowAsync();
                    LoginStatus = false;
                }
            }    
        }

        private async void loginbuttondriver_Click(object sender, RoutedEventArgs e)
        {
            input_username = username_textbox.Text;
            input_password = pass_textbox.Password;

           /* var notificationXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
            var toeastElement = notificationXml.GetElementsByTagName("text");
            toeastElement[0].AppendChild(notificationXml.CreateTextNode("Trying to logging in as a DRIVER"));
            toeastElement[1].AppendChild(notificationXml.CreateTextNode("It might take a few seconds"));
            var toastNotification = new ToastNotification(notificationXml);
            ToastNotificationManager.CreateToastNotifier().Show(toastNotification);
            */
            if (input_username == "" || input_password == "")
            {
                var dialog = new MessageDialog("Please Enter Username and Password both", "Error");
                await dialog.ShowAsync();
            }
            else
            {
                try
                {
                    List<DriversTable> AllDrivers = await App.MobileService.GetTable<DriversTable>().ToListAsync();
                    List<BusLocation> AllBusLocation = await App.MobileService.GetTable<BusLocation>().ToListAsync();
                    foreach (DriversTable driver in AllDrivers)
                    {
                        if (input_username == driver.username && input_password == driver.password)
                        {

                            DriversTable newdriver = new DriversTable
                            {
                                username = input_username,
                                password = input_password,
                                islogin = true,
                                id = driver.id
                            };
                            DriverDetail.Driver_ID = driver.id;
                            DriverDetail.Driver_Username = input_username;
                            DriverDetail.Driver_Password = input_password;
                            foreach (BusLocation Bus in AllBusLocation)
                            {
                                if (Bus.Bus_Id == DriverDetail.Driver_ID)
                                {
                                    BusLocation NewBus = new BusLocation
                                    {
                                        id = Bus.id,
                                        islogin = true,
                                        Bus_Id = DriverDetail.Driver_ID,
                                    };
                                    App.MobileService.GetTable<BusLocation>().UpdateAsync(NewBus);
                                    break;
                                }
                            }
                            App.MobileService.GetTable<DriversTable>().UpdateAsync(newdriver);
                            this.Frame.Navigate(typeof(DriverMain));
                            LoginStatus = true;
                            break;
                        }
                    }
                }
                catch(Exception exc)
                {
                    //var dialog = new MessageDialog("Please Connect To Internet!", "Error");
                    //await dialog.ShowAsync();
                    var notification2Xml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
                    var toeastElement2 = notification2Xml.GetElementsByTagName("text");
                    toeastElement2[0].AppendChild(notification2Xml.CreateTextNode("NO INTERNET"));
                    toeastElement2[1].AppendChild(notification2Xml.CreateTextNode("Connect to INTERNET and TRY AGAIN"));
                    var toastNotification2 = new ToastNotification(notification2Xml);
                    ToastNotificationManager.CreateToastNotifier().Show(toastNotification2);
                }
                if (!LoginStatus)
                {
                    var dialog = new MessageDialog("Username and Password doesn't exist or doesn't match!", "Error");
                    await dialog.ShowAsync();
                    LoginStatus = false;
                }
            }
        }

        private void registerbutton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(RegisterPage));
        }

        private void username_pass_PasswordChanged(object sender, RoutedEventArgs e)
        {

        }

        private void username_textbox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
