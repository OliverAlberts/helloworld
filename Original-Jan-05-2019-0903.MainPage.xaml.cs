using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
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
using Windows.System.Threading;
using Windows.Devices.Gpio;

using TMP75Sensor;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x407 dokumentiert.

namespace App1_Forground_Tmp75_181118
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        I2CTemperatureSensor_TMP75 TempSensor1, TempSensor2;
        ThreadPoolTimer timer;
        private const int UWP_PIN = 23, NH_PIN = 24;
        public GpioPin uwpPin, nhPin;
        

        public MainPage()
        {
            TempSensor1 = new I2CTemperatureSensor_TMP75();
            TempSensor2 = new I2CTemperatureSensor_TMP75();

            timer = ThreadPoolTimer.CreatePeriodicTimer(Timer_tick, TimeSpan.FromMilliseconds(2000));

            

            this.InitializeComponent();
            SensorInitialization();
            GpioInitializsation();
            //this.DataContext = TempSensor1;
            //this.DataContext = TempSensor2;

            
        }

        public void GpioInitializsation()
        {
            var gpio = GpioController.GetDefault();

            if (gpio == null)
            {
                throw new Exception("There is no GPIO Controller on this device!");
            }

            uwpPin = gpio.OpenPin(UWP_PIN); // set Pin für Umwälzpumpe auf GPIO 23
            nhPin = gpio.OpenPin(NH_PIN);   // set Pin für Nachheizung auf GPIO 24
        }

        private void Timer_tick(ThreadPoolTimer timer)
        {
            TempSensor1.SetPropertyTemp();
            TempSensor2.SetPropertyTemp();
            Debug.WriteLine("Timer ausgelöst");
            Debug.WriteLine(TempSensor1.Temperature);
            Debug.WriteLine(TempSensor2.Temperature);
            var task = this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                TextBlockS1.Text = TempSensor1.Temperature;
                TextBlockS2.Text = TempSensor2.Temperature;
            });
        }

        public async void SensorInitialization()
        {
            try
            {
                await TempSensor1.InitAsync(0x48);
                await TempSensor2.InitAsync(0x49);
            }
            catch(Exception e)
            {
                TextBlockS1.Text = "I2C initialization failed: " + e.Message;
            }

            TempSensor1.ConverterResolution = I2CTemperatureSensor_TMP75.ConverterResoutionBits.Resolution12Bits;
            TempSensor2.ConverterResolution = I2CTemperatureSensor_TMP75.ConverterResoutionBits.Resolution12Bits;

            
        }

        

        
    }
}
