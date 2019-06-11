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

namespace Playthrough
{
    public partial class MainWindow : Window
    {
        private const int RATE = 44100;
        private const int CHANNELS = 2;

        public BufferedWaveProvider bwp;
        DirectSoundOut output;
        WaveIn waveIn;

        public MainWindow()
        {
            InitializeComponent();

            waveIn = new WaveIn();
            waveIn.DeviceNumber = 0;
            waveIn.DataAvailable += waveIn_DataAvailable;
            waveIn.WaveFormat = new WaveFormat(RATE, CHANNELS);
            output = new DirectSoundOut();
            bwp = new BufferedWaveProvider(new WaveFormat(RATE, CHANNELS));
            output.Init(bwp);
        }

        void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            NAudio.CoreAudioApi.MMDeviceEnumerator devEnum = new NAudio.CoreAudioApi.MMDeviceEnumerator();
            NAudio.CoreAudioApi.MMDevice defaultDevice = devEnum.GetDefaultAudioEndpoint(NAudio.CoreAudioApi.DataFlow.Render, NAudio.CoreAudioApi.Role.Multimedia);
            float Volume = defaultDevice.AudioMeterInformation.MasterPeakValue * 100f;
            VolumeBar.Value = (100 - (Volume * VolumeMultiplier.Value));
            System.Diagnostics.Debug.Print(Volume.ToString());

            bwp.AddSamples(e.Buffer, 0, e.BytesRecorded);
        }
        private void BtStart_Click(object sender, RoutedEventArgs e)
        {
            output.Play();
            waveIn.StartRecording();
        }

        private void BtStop_Click(object sender, RoutedEventArgs e)
        {
            waveIn.StopRecording();
            output.Stop();
            VolumeBar.Value = 100;
        }
    }
}
