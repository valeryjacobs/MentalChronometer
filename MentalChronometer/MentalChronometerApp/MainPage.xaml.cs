using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Windows.System.Profile;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using GIS = GHIElectronics.UWP.Shields;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MentalChronometerApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const string signalRHub = "http://mentalchronometer.azurewebsites.net/";
        //private const string signalRHub = "http://localhost:4361/";
        private const string signalRHubProxy = "MentalChronometerHub";
        private GIS.FEZHAT hat;
        private DispatcherTimer timer;
        private DispatcherTimer sensorTimer;
        private bool next;
        private int i;
        private bool stateLeft = false;
        private GIS.FEZHAT.Color currentColor;
        private IHubProxy hubProxy;
        private HubConnection hubConnection;
        private string DeviceID = "";
        private string DeviceName = "";
        private double temp = 0.0;
        private double lumen = 0.0;

        public MainPage()
        {
            this.InitializeComponent();

            HardwareToken token = HardwareIdentification.GetPackageSpecificToken(null);
            IBuffer hardwareId = token.Id;
            HashAlgorithmProvider hasher = HashAlgorithmProvider.OpenAlgorithm("MD5");
            IBuffer hashed = hasher.HashData(hardwareId);
            DeviceID = CryptographicBuffer.EncodeToHexString(hashed);
            DeviceName = GetHostName();


            Setup();


            SetupHardware();
        }

        private async void SetupHardware()
        {
            this.hat = await GIS.FEZHAT.CreateAsync();
            this.hat.DIO24On = true;
        }

        private async void Setup()
        {

            var querystringData = new Dictionary<string, string>();
            // querystringData.Add("DeviceId", DeviceID);

            hubConnection = new HubConnection(signalRHub);
            hubProxy = hubConnection.CreateHubProxy(signalRHubProxy);
            hubProxy.On<string>("toDevice", HandleEvent);

            hubProxy.On("init", Init);
            hubProxy.On("start", Start);
            hubProxy.On("stop", Stop);
            hubProxy.On("reset", Reset);
            hubProxy.On("stopSensing", StopSensing);


            hubConnection.Start().Wait();

            await hubProxy.Invoke("Register", DeviceID, DeviceName);

            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromMilliseconds(new Random().Next(300, 2000));
            this.timer.Tick += Timer_Tick;

            this.sensorTimer = new DispatcherTimer();
            this.sensorTimer.Interval = TimeSpan.FromMilliseconds(250);
            this.sensorTimer.Tick += SensorTimer_Tick;
        }

        private void SensorTimer_Tick(object sender, object e)
        {
            
            if (this.hat != null)
            {
                lumen = this.hat.GetLightLevel();
                temp = this.hat.GetTemperature();
            }

            if (this.hubConnection.State == ConnectionState.Connected)
            {
                this.hubProxy.Invoke("sensorMessage", DeviceID,DateTime.Now.ToUniversalTime().ToString(), temp, lumen);
            }

        }

        private string GetHostName()
        {
            foreach (Windows.Networking.HostName name in Windows.Networking.Connectivity.NetworkInformation.GetHostNames())
            {
                if (Windows.Networking.HostNameType.DomainName == name.Type)
                {
                    return name.DisplayName;
                }
            }
            return null;
        }

        private async void StopSensing()
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
                {
                    this.sensorTimer.Start();
                }
             );
        }

        private void Timer_Tick(object sender, object e)
        {
            stateLeft = !stateLeft;

            if (stateLeft)
            {
                this.hat.D2.Color = GIS.FEZHAT.Color.Black;
                this.hat.D3.Color = currentColor;
            }
            else
            {
                this.hat.D2.Color = currentColor;
                this.hat.D3.Color = GIS.FEZHAT.Color.Black;
            }
        }

        private async void HandleEvent(string payload)
        {

        }

        private async void Start()
        {
            if (this.hat != null)
            {
                this.hat.D2.Color = GIS.FEZHAT.Color.Green;
                this.hat.D3.Color = GIS.FEZHAT.Color.Green;              
            }

            currentColor = GIS.FEZHAT.Color.Magneta;

            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                timer.Start();
            }
            );
        }

        private async void Stop()
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                timer.Stop();
            }
             );

            if (this.hat != null)
            {
                this.hat.D2.Color = GIS.FEZHAT.Color.Red;
                this.hat.D3.Color = GIS.FEZHAT.Color.Red;
            }
        }

        private async void Reset()
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
           () =>
           {
               timer.Stop();
           }
            );

            if (this.hat != null)
            {
                this.hat.D2.Color = GIS.FEZHAT.Color.Yellow;
                this.hat.D3.Color = GIS.FEZHAT.Color.Yellow;
            }
        }


        private async void Init()
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
          () =>
          {
              this.sensorTimer.Start();
          }
           );


            if (this.hat != null)
            {
                this.hat.D2.Color = GIS.FEZHAT.Color.White;
                this.hat.D3.Color = GIS.FEZHAT.Color.White;
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
