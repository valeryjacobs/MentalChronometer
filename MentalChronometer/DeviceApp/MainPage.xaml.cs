﻿using Microsoft.AspNet.SignalR.Client;
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
using GIS = GHIElectronics.UWP.Shields;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DeviceApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private GIS.FEZHAT hat;
        private DispatcherTimer timer;
        private bool next;
        private int i;
        private const string signalRHub = "http://localhost:4361/";
        private const string signalRHubProxy = "MentalChronometerHub";
        private string _connectionid = "";
        private const string _piID = "Pi01";

        public MainPage()
        {
            this.InitializeComponent();

           this.Setup();
        }

        private async void Setup()
        {
             
             var hubConnection = new HubConnection(signalRHub);
            var hubProxy = hubConnection.CreateHubProxy(signalRHubProxy);
            hubProxy.On<string>("toDevice", HandleEvent);

            hubConnection.Start().Wait();
            _connectionid = hubConnection.ConnectionId;
            //chat.Invoke<string>("Register", _piID);


            //this.hat = await GIS.FEZHAT.CreateAsync();

            //this.hat.S1.SetLimits(500, 2400, 0, 180);
            //this.hat.S2.SetLimits(500, 2400, 0, 180);

            //this.timer = new DispatcherTimer();
            //this.timer.Interval = TimeSpan.FromMilliseconds(100);
            //this.timer.Tick += Timer_Tick;
            //this.timer.Start();
        }

        private async void HandleEvent(string payload)
        {

        }

        private void Timer_Tick(object sender, object e)
        {
            double x, y, z;

            this.hat.GetAcceleration(out x, out y, out z);

            this.LightTextBox.Text = this.hat.GetLightLevel().ToString("P2");
            this.TempTextBox.Text = this.hat.GetTemperature().ToString("N2");
            this.AccelTextBox.Text = $"({x:N2}, {y:N2}, {z:N2})";
            this.Button18TextBox.Text = this.hat.IsDIO18Pressed().ToString();
            this.Button22TextBox.Text = this.hat.IsDIO22Pressed().ToString();
            this.AnalogTextBox.Text = this.hat.ReadAnalog(GIS.FEZHAT.AnalogPin.Ain1).ToString("N2");

            if ((this.i++ % 5) == 0)
            {
                this.LedsTextBox.Text = this.next.ToString();

                this.hat.DIO24On = this.next;
                this.hat.D2.Color = this.next ? GIS.FEZHAT.Color.White : GIS.FEZHAT.Color.Black;
                this.hat.D3.Color = this.next ? GIS.FEZHAT.Color.White : GIS.FEZHAT.Color.Black;

                this.hat.WriteDigital(GIS.FEZHAT.DigitalPin.DIO16, this.next);
                this.hat.WriteDigital(GIS.FEZHAT.DigitalPin.DIO26, this.next);

                this.hat.SetPwmDutyCycle(GIS.FEZHAT.PwmPin.Pwm5, this.next ? 1.0 : 0.0);
                this.hat.SetPwmDutyCycle(GIS.FEZHAT.PwmPin.Pwm6, this.next ? 1.0 : 0.0);
                this.hat.SetPwmDutyCycle(GIS.FEZHAT.PwmPin.Pwm7, this.next ? 1.0 : 0.0);
                this.hat.SetPwmDutyCycle(GIS.FEZHAT.PwmPin.Pwm11, this.next ? 1.0 : 0.0);
                this.hat.SetPwmDutyCycle(GIS.FEZHAT.PwmPin.Pwm12, this.next ? 1.0 : 0.0);

                this.next = !this.next;
            }

            if (this.hat.IsDIO18Pressed())
            {
                this.hat.S1.Position += 5.0;
                this.hat.S2.Position += 5.0;

                if (this.hat.S1.Position >= 180.0)
                {
                    this.hat.S1.Position = 0.0;
                    this.hat.S2.Position = 0.0;
                }
            }

            if (this.hat.IsDIO22Pressed())
            {
                if (this.hat.MotorA.Speed == 0.0)
                {
                    this.hat.MotorA.Speed = 0.5;
                    this.hat.MotorB.Speed = -0.7;
                }
            }
            else
            {
                if (this.hat.MotorA.Speed != 0.0)
                {
                    this.hat.MotorA.Speed = 0.0;
                    this.hat.MotorB.Speed = 0.0;
                }
            }
        }
    }
}
