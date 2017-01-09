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
using Microsoft.WindowsAzure.MobileServices;

namespace smartbus
{
    public sealed partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            this.InitializeComponent();
        }
        //public static MobileServiceClient MobileService = new MobileServiceClient("https://smartbustroid.azurewebsites.net");
        private string Username = string.Empty;
        private string Password = string.Empty;
        private string PasswordC = string.Empty;
        private bool UserRbt = false;
        private bool DriRbt = false;
        private async void register_bt_Click(object sender, RoutedEventArgs e)
        {
            Username = username_regpage.Text;
            Password = password_regpage.Password;
            PasswordC = passwordcon_regpage.Password;
            
            if (!String.IsNullOrWhiteSpace(Username) && !String.IsNullOrWhiteSpace(Password) && !String.IsNullOrWhiteSpace(PasswordC))
            {
                if (PasswordC == Password)
                {
                    if (UserRbt)
                    {
                        List<UsersTable> userList = await App.MobileService.GetTable<UsersTable>().ToListAsync();
                        foreach (UsersTable registereduser in userList)
                        {
                            if (registereduser.username == Username)
                            {
                                var SameUserError = new MessageDialog("This username already exists!. Try Different usernaem" , "Username Not Available");
                                await SameUserError.ShowAsync();
                                this.Frame.Navigate(typeof(RegisterPage));
                                return;
                            }
                        }
                        UsersTable newuser = new UsersTable
                        {
                            username = Username,
                            password = Password
                        };
                        App.MobileService.GetTable<UsersTable>().InsertAsync(newuser);
                        var SucDia = new MessageDialog("User Registeration Successful");
                        await SucDia.ShowAsync();
                        NavigateBack();
                        // TOAST FOR SER REGISTRATION SUCCESSFULL
                    }
                    else if (DriRbt)
                    {
                        List<DriversTable> driverList = await App.MobileService.GetTable<DriversTable>().ToListAsync();
                        foreach (DriversTable registereddriver in driverList)
                        {
                            if (registereddriver.username == Username)
                            {
                                var SameUserError = new MessageDialog("This username already exists!. Try Different usernaem", "Username Not Available");
                                await SameUserError.ShowAsync();
                                this.Frame.Navigate(typeof(RegisterPage));
                                return;
                            }
                        }
                        DriversTable newdriver = new DriversTable
                        {
                            username = Username,
                            password = Password,
                            islogin = false
                        };
                        await App.MobileService.GetTable<DriversTable>().InsertAsync(newdriver);

                        List<DriversTable> DriverList= await App.MobileService.GetTable<DriversTable>().ToListAsync();
                        foreach (DriversTable Driver in DriverList)
                        {
                            if (Driver.username == Username && Driver.password == Password)
                            {
                                BusLocation NewBus = new BusLocation
                                {
                                    Bus_Id = Driver.id,
                                    Latitude = "0.00000000",
                                    Longitude = "0.00000000",
                                    islogin = false
                                
                                };
                                await App.MobileService.GetTable<BusLocation>().InsertAsync(NewBus);
                                var SucDia = new MessageDialog("Driver Registration Successful");
                                await SucDia.ShowAsync();
                                break;
                            }
                        };
                        NavigateBack();
                    }
                    else
                    {
                        var ErrDia = new MessageDialog("Please Select one User type");
                        await ErrDia.ShowAsync();
                    }
                }
                else
                {
                    var ErrDia = new MessageDialog("Password doesn't MATCH");
                    await ErrDia.ShowAsync();
                }
            }
            else
            {
                var ErrDia = new MessageDialog("All The Fields are Compulsary");
                await ErrDia.ShowAsync();
            }
        }
        private void driver_rbt_Checked(object sender, RoutedEventArgs e)
        {
            UserRbt = false;
            DriRbt = true;
        }
        private void user_rbt_Checked(object sender, RoutedEventArgs e)
        {
            UserRbt = true;
            DriRbt = false;
        }
        private void NavigateBack()
        {
            username_regpage.Text = "";
            password_regpage.Password = "";
            passwordcon_regpage.Password = "";
            UserRbt = false;
            DriRbt = false;
            this.Frame.Navigate(typeof(MainPage));
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            NavigateBack();
        }
    }
}
