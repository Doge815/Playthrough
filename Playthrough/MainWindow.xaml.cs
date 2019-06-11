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

            MMDeviceEnumerator names = new MMDeviceEnumerator();
            var devices = names.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);
            foreach (var device in devices)
                dev.Add(device.FriendlyName);
            CbDevices.ItemsSource = dev;
            CbDevices.SelectedItem = 1;
        }

        void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            float[] volume = new float[8];
            for (int i = 0; i < 8; i++)
            {
                volume[i] = 0f;
                for (int index = e.BytesRecorded*i/8; index < e.BytesRecorded*(i+1)/8; index += 2)
                {
                    short sample = (short)((e.Buffer[index + 1] << 8) | e.Buffer[index + 0]);
                    float val = Math.Abs(sample / 32768f);
                    if (val > volume[i]) volume[i] = val;
                }
            }
            NAudio.CoreAudioApi.MMDeviceEnumerator devEnum = new NAudio.CoreAudioApi.MMDeviceEnumerator();
            NAudio.CoreAudioApi.MMDevice defaultDevice = devEnum.GetDefaultAudioEndpoint(NAudio.CoreAudioApi.DataFlow.Render, NAudio.CoreAudioApi.Role.Multimedia);
            VolumeBar.Value = (100 - (defaultDevice.AudioMeterInformation.MasterPeakValue * 100f));
            VolumeBar.Value = (100 - (volume[3]*100));
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

        private void CbDevices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InputSound.DeviceNumber = ((ComboBox)sender).SelectedIndex;
        }

        private void CbDevices_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            MMDeviceEnumerator names = new MMDeviceEnumerator();
            var devices = names.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);
            foreach (var device in devices)
                dev.Add(device.FriendlyName);
            CbDevices.ItemsSource = dev;
        }
    }
}
