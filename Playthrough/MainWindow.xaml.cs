using NAudio.Wave;
using NAudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NAudio.CoreAudioApi;

namespace Playthrough
{
    public partial class MainWindow : Window
    {
        private int RATE = 44100;
        private int CHANNELS = 2;

        private BufferedWaveProvider Provider { get; set; }
        private DirectSoundOut OutputSound { get; set; }
        private WaveIn InputSound { get; set; }

        List<string> dev = new List<string>();

        public MainWindow()
        {
            InitializeComponent();

            InputSound = new WaveIn();
            InputSound.DeviceNumber = 0;
            InputSound.DataAvailable += waveIn_DataAvailable;
            InputSound.WaveFormat = new WaveFormat(RATE, CHANNELS);

            OutputSound = new DirectSoundOut();
            Provider = new BufferedWaveProvider(new WaveFormat(RATE, CHANNELS));
            OutputSound.Init(Provider);

            CbDevices.ItemsSource = typeof(Colors).GetProperties();
            for (int i = 0; i < WaveIn.DeviceCount; i++)
            {
                WaveInCapabilities deviceInfo = WaveIn.GetCapabilities(i);
                dev.Add(deviceInfo.ProductName);
            }
            CbDevices.ItemsSource = dev;
        }

        void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
#if false
            float val = 0;
            for (int index = 0; index < e.BytesRecorded; index += 2)
            {
                short sample = (short)((e.Buffer[index + 1] << 8) |
                                        e.Buffer[index + 0]);
                val += sample / 32768f;
            }
            pp.Value = Math.Abs(100-(val*zz.Value));
            System.Diagnostics.Debug.Print(val.ToString());
#else
            NAudio.CoreAudioApi.MMDeviceEnumerator devEnum = new NAudio.CoreAudioApi.MMDeviceEnumerator();
            NAudio.CoreAudioApi.MMDevice defaultDevice = devEnum.GetDefaultAudioEndpoint(NAudio.CoreAudioApi.DataFlow.Render, NAudio.CoreAudioApi.Role.Multimedia);
            VolumeBar.Value = (100 - (defaultDevice.AudioMeterInformation.MasterPeakValue * 100f));
#endif
            Provider.AddSamples(e.Buffer, 0, e.BytesRecorded);
        }
        private void BtStart_Click(object sender, RoutedEventArgs e)
        {
            OutputSound.Play();
            InputSound.StartRecording();
        }

        private void BtStop_Click(object sender, RoutedEventArgs e)
        {
            InputSound.StopRecording();
            OutputSound.Stop();
            VolumeBar.Value = 100;
            dev.Add(DateTime.Now.ToString());

        }
    }
}
