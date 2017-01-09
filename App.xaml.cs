using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
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
using Microsoft.WindowsAzure.MobileServices;

namespace smartbus
{
    public class UsersTable
    {
        public string id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }

    public class DriversTable
    {
        public string id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public bool islogin { get; set; }

        public string dest { get; set; }
        public bool check1 { get; set; }
        public bool check2 { get; set; }
        public bool check3 { get; set; }
        public bool check4 { get; set; }

    }

    static class DriverDetail
    {
        static string id = string.Empty;
        static string username = string.Empty;
        static string password = string.Empty;
        public static string Driver_ID
        {
            get { return id; }
            set { id = value; }
        }
        public static string Driver_Username
        {
            get { return username; }
            set { username = value; }
        }
        public static string Driver_Password
        {
            get { return password; }
            set { password = value; }
        }

    }

    //string CurrentDriver = string.Empty;

    public class BusLocation
    {
        public string id { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Bus_Id { get; set; }
        public bool islogin { get; set; }
    }

    sealed partial class App : Application
    {
        public App()    
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }
        public static MobileServiceClient MobileService = new MobileServiceClient("https://smartbustroid.azurewebsites.net");
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
