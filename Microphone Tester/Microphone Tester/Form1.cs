using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.ObjectModel;   
using NAudio.Wave;

namespace Microphone_Tester
{
    public partial class Form1 : Form
    {
        private WaveIn waveIn;
        private WaveOut waveOut;
        private BufferedWaveProvider waveProvider;
        private int selectedSampleRate;
        public Form1()
        {
            InitializeComponent();
            InitializeMicrophones();
            InitializeSampleRates();
        }

        private void InitializeMicrophones()
        {
            var devices = new Collection<WaveInCapabilities>();

            for (int i = 0; i < WaveIn.DeviceCount; i++)
            {
                devices.Add(WaveIn.GetCapabilities(i));
            }
            comboBox1.DataSource = devices;
            comboBox1.DisplayMember = "ProductName";
        }

        private void InitializeSampleRates()
        {
            comboBox2.Items.AddRange(new object[] { 44100, 48000, 96000 });
            comboBox2.SelectedIndex = 0;

            selectedSampleRate = (int)comboBox2.SelectedItem;
            comboBox2.SelectedIndexChanged += (sender, e) =>
            {
                selectedSampleRate = (int)comboBox2.SelectedItem;
            };
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (waveIn == null)
            {
                StartListening();
                button1.Text = "Stop";
            }
            else
            {
                StopListening();
                button1.Text = "Start";
            }
        }
        private void StartListening()
        {
            int selectedDeviceIndex = comboBox1.SelectedIndex;
            if (selectedDeviceIndex == -1)
            {
                MessageBox.Show("Please select a microphone.");
                return;
            }
            waveIn = new WaveIn();
            waveIn.DeviceNumber = selectedDeviceIndex;
            waveIn.DataAvailable += WaveInDataAvailable;
            waveIn.WaveFormat = new WaveFormat(selectedSampleRate, 16, 1);
            waveOut = new WaveOut();
            waveProvider = new BufferedWaveProvider(new WaveFormat(selectedSampleRate, 16, 1));
            waveOut.Init(waveProvider);
            waveOut.Play();
            waveIn.StartRecording();
        }

        private void WaveInDataAvailable(object sender, WaveInEventArgs e)
        {
            waveProvider.AddSamples(e.Buffer, 0, e.BytesRecorded);
        }

        private void StopListening()
        {
            if (waveIn != null)
            {
                waveIn.StopRecording();
                waveIn.Dispose();
                waveIn = null;
            }

            if (waveOut != null)
            {
                waveOut.Stop();
                waveOut.Dispose();
                waveOut = null;
            }

            if (waveProvider != null)
            {
                waveProvider.ClearBuffer();
                waveProvider = null;
            }
        }
    }
}
