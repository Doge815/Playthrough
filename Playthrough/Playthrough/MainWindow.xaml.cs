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
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int RATE = 44100; // sample rate of the sound card
        private int BUFFERSIZE = (int)Math.Pow(2, 16); // must be a multiple of 2

        public BufferedWaveProvider bwp;
        DirectSoundOut output;
        WaveIn waveIn;

        public MainWindow()
        {
            InitializeComponent();
            waveIn = new WaveIn();
            waveIn.DeviceNumber = 0;
            waveIn.DataAvailable += waveIn_DataAvailable;
            int sampleRate = RATE;
            int channels = 1;
            waveIn.WaveFormat = new WaveFormat(sampleRate, channels);
            waveIn.StartRecording();
            output = new DirectSoundOut();
            bwp = new BufferedWaveProvider(new WaveFormat(RATE, 1));
            output.Init(bwp);
            output.Play();
        }

        void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            float val = 0;
            for (int index = 0; index < e.BytesRecorded; index += 2)
            {
                short sample = (short)((e.Buffer[index + 1] << 8) |
                                        e.Buffer[index + 0]);
                val += sample / BUFFERSIZE;
            }
            pp.Value = Math.Abs(100-(val*zz.Value));
            System.Diagnostics.Debug.Print(val.ToString());
            bwp.AddSamples(e.Buffer, 0, e.BytesRecorded);
        }
        private void BtStart_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
