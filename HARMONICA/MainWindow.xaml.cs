using CSCore;
using CSCore.Codecs;
using CSCore.CoreAudioAPI;
using CSCore.SoundIn;
using CSCore.SoundOut;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TEST_API;

namespace HARMONICA
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        [DllImport("BiblZvuk.dll", CallingConvention = CallingConvention.Cdecl)]
        //unsafe
        public static extern int vizualzvuk(string filename, string secfile, int[] Rdat, int ParV);

        private FileInfo fileInfo = new FileInfo("window.tmp");
        private FileInfo fileInfo1 = new FileInfo("Data_Load.tmp");
        private FileInfo FileLanguage = new FileInfo("Data_Language.tmp");
        private FileInfo fileinfo = new FileInfo("DataTemp.tmp");

        private MMDeviceCollection mOutputDevices;
        private MMDeviceCollection mInputDevices;
        private WasapiOut mSoundOut;
        private WasapiCapture mSoundIn;
        private SimpleMixer mMixer;
        private SampleDSPPitch mDspPitch;
        private ISampleSource mMp3;

        private static string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static string path2;

        string langindex;

        private int SampleRate;
        private static int limit = 20;

        BackgroundWorker worker;

        public MainWindow()
        {
            InitializeComponent();
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                for (int i = 0; i < 100; i++)
                {
                    if (worker.CancellationPending == true)
                    {
                        //e.Cancel = true;
                        (sender as BackgroundWorker).ReportProgress(100);
                        break;
                        //return;
                    }
                    (sender as BackgroundWorker).ReportProgress(i);
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в worker_DoWork: \r\n" + ex.Message;
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in worker_DoWork: \r\n" + ex.Message;
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }//не нужно

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                PBNFT.Value = e.ProgressPercentage;
                /*if (PBNFT.Value == 25)
                {
                    string uri = @"Neurotuners\progressbar\Group 13.png";
                    ImgPBNFTback.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
                }
                else if (PBNFT.Value == 50)
                {
                    string uri = @"Neurotuners\progressbar\Group 12.png";
                    ImgPBNFTback.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
                }
                else if (PBNFT.Value == 75)
                {
                    string uri = @"Neurotuners\progressbar\Group 11.png";
                    ImgPBNFTback.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
                }
                else if (PBNFT.Value == 100)
                {
                    PBNFT.Visibility = Visibility.Hidden;
                    lbPBNFT.Visibility = Visibility.Hidden;
                    string uri = @"Neurotuners\element\progressbar-backgrnd.png";
                    ImgPBNFTback.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
                    /*if (!File.Exists(@"Image\" + RecordName + ".bmp"))
                    {
                        SaveToBmp(Image1, @"Image\" + RecordName + ".bmp");
                        if (langindex == "0")
                        {
                            string msg = "NFT картинка сохранена.";
                            LogClass.LogWrite(msg);
                            MessageBox.Show(msg);
                        }
                        else
                        {
                            string msg = "NFT picture saved.";
                            LogClass.LogWrite(msg);
                            MessageBox.Show(msg);
                        }
                    }
                    //imgPBNFTBack.Visibility = Visibility.Hidden;
                }*/
            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в worker_ProgressChanged: \r\n" + ex.Message;
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in worker_ProgressChanged: \r\n" + ex.Message;
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }

        private void SoundIn()
        {
            try
            {
                mSoundIn = new WasapiCapture(/*false, AudioClientShareMode.Exclusive, 1*/);
                Dispatcher.Invoke(() => mSoundIn.Device = mInputDevices[cmbInput.SelectedIndex]);
                mSoundIn.Initialize();
                mSoundIn.Start();
            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в SoundIn: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in SoundIn: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }

        private void SoundOut()
        {
            try
            {

                mSoundOut = new WasapiOut(/*false, AudioClientShareMode.Exclusive, 1*/);
                Dispatcher.Invoke(() => mSoundOut.Device = mOutputDevices[cmbOutput.SelectedIndex]);
                //mSoundOut.Device = mOutputDevices[cmbOutput.SelectedIndex];



                //mSoundOut.Initialize(mMixer.ToWaveSource(32).ToMono());


                mSoundOut.Play();
                mSoundOut.Volume = 10;
                
            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в SoundOut: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                    
                }
                else
                {
                    string msg = "Error in SoundOut: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                    
                }
            }
        }

        private void Mixer()
        {
            try
            {

                mMixer = new SimpleMixer(1, SampleRate) //стерео, 44,1 КГц
                {
                    FillWithZeros = true,
                    DivideResult = true, //Для этого установлено значение true, чтобы избежать звуков тиков из-за превышения -1 и 1.
                };
            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в Mixer: \r\n" + ex.Message;
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in Mixer: \r\n" + ex.Message;
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }

        private void Languages()
        {
            try
            {
                StreamReader FileLanguage = new StreamReader("Data_Language.tmp");
                File.WriteAllText("Data_Load.tmp", "1");
                File.WriteAllText("DataTemp.tmp", "0");
                langindex = FileLanguage.ReadToEnd();

                if (langindex == "0")
                {
                    Title = "ГАРМОНИКА";
                    btnFeeling_in_the_body.ToolTip = "Ощущение в теле";
                    btnSituation_problem.ToolTip = "Ситуация/проблема";
                }
                else
                {
                    Title = "HARMONICA";
                    btnFeeling_in_the_body.ToolTip = "Feeling in the body";
                    btnSituation_problem.ToolTip = "Situation/problem";
                }

            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в Languages: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in Languages: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }

        private async void Sound()
        {
            try
            {
                Mixer();
                mMp3 = CodecFactory.Instance.GetCodec("").ToMono().ToSampleSource();
                mDspPitch = new SampleDSPPitch(mMp3.ToWaveSource(32).ToSampleSource());
                //SampleRate = mDspRec.WaveFormat.SampleRate;
                mMixer.AddSource(mDspPitch.ChangeSampleRate(mDspPitch.WaveFormat.SampleRate).ToWaveSource(32).Loop().ToSampleSource());
                await Task.Run(() => SoundOut());
            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в Sound: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in Sound: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }

        private void Timer30()
        {
            int i = 0;
            while(i < 30)
            {
                i++;
                Thread.Sleep(1000);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SoftCl.IsSoftwareInstalled("Microsoft Visual C++ 2015-2022 Redistributable (x86) - 14.32.31332") == false)
                {
                    Process.Start("VC_redist.x86.exe");
                }

                MMDeviceEnumerator deviceEnum = new MMDeviceEnumerator();
                mInputDevices = deviceEnum.EnumAudioEndpoints(DataFlow.Capture, DeviceState.Active);
                MMDevice activeDevice = deviceEnum.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Multimedia);

                SampleRate = activeDevice.DeviceFormat.SampleRate;

                foreach (MMDevice device in mInputDevices)
                {
                    cmbInput.Items.Add(device.FriendlyName);
                    if (device.DeviceID == activeDevice.DeviceID) cmbInput.SelectedIndex = cmbInput.Items.Count - 1;
                }


                //Находит устройства для вывода звука и заполняет комбобокс
                activeDevice = deviceEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                mOutputDevices = deviceEnum.EnumAudioEndpoints(DataFlow.Render, DeviceState.Active);

                foreach (MMDevice device in mOutputDevices)
                {
                    cmbOutput.Items.Add(device.FriendlyName);
                    if (device.DeviceID == activeDevice.DeviceID) cmbOutput.SelectedIndex = cmbOutput.Items.Count - 1;
                }

                if (!Directory.Exists(path + "HARMONICA"))
                {
                    Directory.CreateDirectory(path + @"\HARMONICA");
                    path2 = path + @"\HARMONICA\Data";

                }

                string[] filename = File.ReadAllLines(fileInfo1.FullName);
                if (filename.Length == 1)
                {
                    Languages();
                }
                if (!File.Exists("log.tmp"))
                {
                    File.Create("log.tmp").Close();
                }
                else
                {
                    if (File.ReadAllLines("log.tmp").Length > 1000)
                    {
                        File.WriteAllText("log.tmp", " ");
                    }
                }

                if (check.strt(path2) > limit)
                {
                    this.IsEnabled = false;

                    ActivationForm activation = new ActivationForm();
                    activation.Show();
                    MessageBoxAct boxAct = new MessageBoxAct();
                    boxAct.ShowDialog();

                }

                if (langindex == "0")
                {
                    string msg = "Подключите проводную аудио-гарнитуру к компьютеру.\nЕсли на данный момент гарнитура не подключена,\nто подключите проводную гарнитуру, и перезапустите программу для того, чтобы звук подавался в наушники.";
                    MessageBox.Show(msg);
                }
                else
                {
                    string msg = "Connect a wired audio headset to your computer.\nIf a headset is not currently connected,\nthen connect a wired headset and restart the program so that the sound is played through the headphones.";
                    MessageBox.Show(msg);
                }
            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в Loaded: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in Loaded: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }
    }
}
