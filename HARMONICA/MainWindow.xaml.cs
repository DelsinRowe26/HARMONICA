﻿// This is a personal academic project. Dear PVS-Studio, please check it.

// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
using CSCore;
using CSCore.Codecs;
using CSCore.Codecs.WAV;
using CSCore.CoreAudioAPI;
using CSCore.SoundIn;
using CSCore.SoundOut;
using CSCore.Streams;
using CSCore.Streams.Effects;
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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TEST_API;
using static System.Net.WebRequestMethods;
using File = System.IO.File;

namespace HARMONICA
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessageW(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("winmm.dll")]
        public static extern int waveOutGetVolume(IntPtr hwo, out uint pdwVolume);

        [DllImport("winmm.dll")]
        public static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);

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
        private SampleDSP mDsp;
        private ISampleSource mMp3, mMp4;
        private IWaveSource mSource;

        private static string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static string path2, pathAdminLogin, pathAdminPass;

        string langindex, fileDeleteRec1, fileDeleteCutRec1, fileDeleteRec2, fileDeleteCutRec2;
        string myfile;
        string cutmyfile;
        private string Filename;
        private string Session;
        private string KeyPass;

        private int ImgBtnTurboClick = 0, ImgBtnTurboTwoClick = 0, BtnSetClick = 0, ShadowClick = 0, SoundClick = 0;
        private int SampleRate;
        private float pitchVal;
        private float reverbVal;
        private static int limit = 22, countLimit = 15;

        private const int APPCOMMAND_VOLUME_UP = 0xA0000;
        private const int APPCOMMAND_VOLUME_DOWN = 0x90000;
        private const int WM_APPCOMMAND = 0x319;

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


                if (SoundClick == 0)
                {
                    mSoundOut.Initialize(mMp3.ToWaveSource(32).ToMono());
                    mSoundOut.Volume = 5;
                }
                else
                {
                    mSoundOut.Initialize(mMixer.ToWaveSource(32).ToMono());
                    mSoundOut.Volume = 15;
                }


                mSoundOut.Play();
                
                
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
                    Title = "ReSelf - Ментальный детокс Катарсис";
                    lbMicrophone.Content = "Выбор микрофона";
                    lbSpeaker.Content = "Выбор динамиков";
                    lbRecordPB.Content = "Идёт запись...";
                    btnFeeling_in_the_body.ToolTip = "Сеанс «Ощущение в теле»";
                    btnSituation_problem.ToolTip = "Сеанс «Ситуация/проблема»";
                }
                else
                {
                    Title = "ReSelf - Mental detox Katarsis";
                    lbMicrophone.Content = "Microphone selection";
                    lbSpeaker.Content = "Speaker selection";
                    lbRecordPB.Content = "Recording in progress...";
                    btnFeeling_in_the_body.ToolTip = "Session «Feeling in the body»";
                    btnSituation_problem.ToolTip = "Session «Situation/problem»";
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

        private async void Sound(string FileName)
        {
            try
            {
                SoundClick = 0;
                Mixer();
                mMp3 = CodecFactory.Instance.GetCodec(FileName).ToMono().ToSampleSource();
                mMixer.AddSource(mMp3.ChangeSampleRate(mMp3.WaveFormat.SampleRate));
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

        private void Timer40()
        {
            int i = 0;
            while (i < 40)
            {
                i++;
                Thread.Sleep(1000);
            }
        }

        private void TimerRec()
        {
            Dispatcher.Invoke(() => lbTimer.Visibility = Visibility.Visible);
            int i = 3;
            while (i > 0)
            {
                Dispatcher.Invoke(() => lbTimer.Content = i.ToString());
                Thread.Sleep(1000);
                i--;
            }
            Dispatcher.Invoke(() => lbTimer.Content = i.ToString());
            Dispatcher.Invoke(() => lbTimer.Visibility = Visibility.Hidden);
        }

        private void WinTime()
        {
            if(langindex == "0")
            {

                string msg = "Сейчас запустится таймер, после окончания начнется запись.";
                MessageBox.Show(msg);
            }
            else
            {
                string msg = "Now the timer will start, after the end, recording will begin.";
                MessageBox.Show(msg);
            }
        }

        private void Situation_Problem_pattern()
        {
            TembroClass tembro = new TembroClass();
            string PathFile = @"ReSelf - Mental detox\Record\Situation_problem\LiftUp Effect.tmp";
            tembro.Tembro(SampleRate, PathFile);
            pitchVal = 0;
            reverbVal = 250;
        }

        private void Feeling_in_the_body_pattern()
        {
            TembroClass tembro = new TembroClass();
            string PathFile = @"ReSelf - Mental detox\Record\Feeling_in_the_body\Wide_voiceMan.tmp";
            tembro.Tembro(SampleRate, PathFile);
            pitchVal = 0;
            reverbVal = 150;
        }

        private void SetPitchShiftValue()
        {
            mDspPitch.PitchShift = (float)Math.Pow(2.0F, pitchVal / 13.0F);
        }

        private void PitchTimerSitProb()
        {
            int i = 180;
            Dispatcher.Invoke(() => lbTimer.Visibility = Visibility.Visible);
            while (i > 0)
            {
                Dispatcher.Invoke(() => lbTimer.Content = i.ToString());
                pitchVal += 0.0083f;
                SetPitchShiftValue();
                Thread.Sleep(1000);
                i--;
            }
            Dispatcher.Invoke(() => lbTimer.Content = i.ToString());
            Dispatcher.Invoke(() => lbTimer.Visibility = Visibility.Hidden);
        }

        private void PitchTimerFeelInTheBody()
        {
            int i = 180;
            Dispatcher.Invoke(() => lbTimer.Visibility = Visibility.Visible);
            while (i > 0)
            {
                Dispatcher.Invoke(() => lbTimer.Content = i.ToString());
                pitchVal -= 0.014f;
                SetPitchShiftValue();
                Thread.Sleep(1000);
                i--;
            }
            Dispatcher.Invoke(() => lbTimer.Content = i.ToString());
            Dispatcher.Invoke(() => lbTimer.Visibility = Visibility.Hidden);
        }

        private async void Feeling_in_the_body()
        {
            try
            {
                Repeat.click = 0;
                ShadowClick = 1;
                Feeling_in_the_body_pattern();
                Stop();
                //Thread.Sleep(2000);
                btnSituation_problem.IsEnabled = false;
                btnFeeling_in_the_body.IsEnabled = false;
                btnSituation_problem.Visibility = Visibility.Hidden;
                btnFeeling_in_the_body.Visibility = Visibility.Hidden;
                // button.IsEnabled = false;

                Filename = @"ReSelf - Mental detox\Record\Feeling_in_the_body\HintFeelingInTheBody_StepOneAndTwoFeelingInTheBody.wav";
                Sound(Filename);
                await Task.Delay(25000);
                lbText.Content = "переведите внимание на зажатые части\nтела и сделайте оттуда глубокий выдох";
                lbText.Visibility = Visibility.Visible;
                await Task.Delay(30000);
                lbText.Visibility = Visibility.Hidden;
                await Task.Delay(36000);
                //Stop();

                /*Filename = @"ReSelf - Mental detox\Record\Feeling_in_the_body\StepOneAndTwoFeelingInTheBodyMusic.wav";
                Sound(Filename);
                await Task.Delay(34000);*/
                

                //Здесь должно быть что-то типо включения микрофона!!!!!!! А у нас будет что-то типо записи
                //WinTime();
                lbText.Content = "Сейчас начнется запись голоса";
                lbText.Visibility = Visibility.Visible;
                await Task.Run(() => TimerRec());
                Stop();
                lbText.Visibility = Visibility.Hidden;
                await Task.Run(() => StartFullDuplex1());
                Recording1();
                await Task.Delay(5000);
                
                Stop();
                await Task.Delay(2000);
                //Thread.Sleep(5000);
                lbTitleNFT1.Visibility = Visibility.Visible;

                /*Filename = @"ReSelf - Mental detox\Record\Feeling_in_the_body\StepThreeFeelingInTheBody.wav";
                Sound(Filename);
                await Task.Delay(32000);*/

                Filename = @"ReSelf - Mental detox\Record\Feeling_in_the_body\StepThreeFeelingInTheBody_StepFourFeelingInTheBody.wav";
                Sound(Filename);
                await Task.Delay(63000);
                lbText.Content = "НАЧИНАЕМ СЕАНС!";
                lbText.Visibility = Visibility.Visible;
                await Task.Delay(5000);
                lbText.Visibility = Visibility.Hidden;

                //Здесь 3 минуты какой-то херни
                Stop();
                StartFullDuplex();
                await Task.Run(() => PitchTimerFeelInTheBody());
                //await Task.Delay(180000);
                Stop();

                Filename = @"ReSelf - Mental detox\Record\Feeling_in_the_body\StepFiveFeelingInTheBody_RepeatRecord.wav";
                Sound(Filename);
                await Task.Delay(36000);

                /*Filename = @"ReSelf - Mental detox\Record\Feeling_in_the_body\RepeatRecord.wav";
                Sound(Filename);
                await Task.Delay(12000);*/

                //WinTime();
                lbText.Content = "Сейчас начнется запись голоса";
                lbText.Visibility = Visibility.Visible;
                await Task.Run(() => TimerRec());
                lbText.Visibility = Visibility.Hidden;
                Recording2();
                await Task.Delay(7000);
                lbTitleNFT2.Visibility = Visibility.Visible;

                Repeat repeat = new Repeat();
                repeat.ShowDialog();
                //await Task.Delay(10000);

                if (Repeat.click == 1)
                {
                    Filename = @"ReSelf - Mental detox\Record\TheSoundEnd.mp3";
                    Sound(Filename);
                    HelpUnhelp help = new HelpUnhelp();
                    help.ShowDialog();
                    await Task.Delay(140000);
                    Close();
                }
            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в Feeling_in_the_body: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in Feeling_in_the_body: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }

        private async void Feeling_in_the_body_Text()
        {
            try
            {
                Repeat.click = 0;
                ShadowClick = 1;
                Feeling_in_the_body_pattern();
                Stop();
                //Thread.Sleep(2000);
                btnSituation_problem.IsEnabled = false;
                btnFeeling_in_the_body.IsEnabled = false;
                btnSituation_problem.Visibility = Visibility.Hidden;
                btnFeeling_in_the_body.Visibility = Visibility.Hidden;
                //button.IsEnabled = false;

                Filename = @"ReSelf - Mental detox\Record\tunetank.com_471_everest_by_alex-makemusicText.mp3";
                Sound(Filename);
                lbText.Visibility = Visibility.Visible;
                
                lbText.Content = "Хорошо, Вы выбрали сеанс в течение которого сможете";
                await Task.Delay(5000);
                
                lbText.Content = "высвободить отрицательную энергию, почувствовать себя";
                await Task.Delay(5000);

                lbText.Content = "легче и спокойнее. Пожалуйста, займите удобное положение.";
                await Task.Delay(5000);

                lbText.Content = "Всецело почувствуйте свое тело.";
                await Task.Delay(3000);

                lbText.Content = "Расслабьте на выдохе те места,";
                await Task.Delay(3000);

                lbText.Content = "в которых заметили напряжение.";
                await Task.Delay(3000);

                lbText.Content = "переведите внимание на зажатые части\nтела и сделайте оттуда глубокий выдох";
                await Task.Delay(30000);

                lbText.Content = "Закройте глаза. Мы начинаем сеанс.";
                await Task.Delay(3000);

                lbText.Content = "Шаг первый: подготовка голоса.";
                await Task.Delay(3000);

                lbText.Content = "Медленно и расслабленно открывайте рот все шире и шире,";
                await Task.Delay(5000);

                lbText.Content = "чтобы сеанс получился максимально эффективным.";
                await Task.Delay(5000);

                lbText.Content = "Шаг второй: сохраняя открытое положение рта";
                await Task.Delay(5000);

                lbText.Content = "- потяните долгий звук «ААА» и вслушайтесь\nв звучание своего голоса в наушниках.";
                await Task.Delay(5000);

                lbText.Content = "Сейчас начнется запись голоса";
                lbText.Visibility = Visibility.Visible;
                await Task.Run(() => TimerRec());
                Stop();
                lbText.Visibility = Visibility.Hidden;
                await Task.Run(() => StartFullDuplex1());
                Recording1();
                await Task.Delay(5000);

                Stop();
                await Task.Delay(2000);
                //Thread.Sleep(5000);
                lbTitleNFT1.Visibility = Visibility.Visible;
                lbText.Visibility = Visibility.Visible;

                Filename = @"ReSelf - Mental detox\Record\tunetank.com_471_everest_by_alex-makemusicText.mp3";
                Sound(Filename);

                lbText.Content = "Хорошо. Голос может звучать свободно и открыто.";
                await Task.Delay(5000);

                lbText.Content = "Шаг третий: переведите все свое внимание на ту область";
                await Task.Delay(5000);

                lbText.Content = "тела, где ощущается боль/тревога, напряжение или другие";
                await Task.Delay(5000);

                lbText.Content = "неприятные переживания. Сфокусируйтесь на этом месте";
                await Task.Delay(5000);

                lbText.Content = "так хорошо, насколько это возможно.";
                await Task.Delay(9000);

                lbText.Content = "Шаг четвертый: начните тянуть долгий звук,";
                await Task.Delay(4000);

                lbText.Content = "продолжая фокусироваться на выбранной области.";
                await Task.Delay(5000);

                lbText.Content = "Ваш голос станет специальным каналом";
                await Task.Delay(5000);

                lbText.Content = "для вывода негативного ощущения из тела.";
                await Task.Delay(5000);

                lbText.Content = "В течение всего сеанса программа настроит его таким образом,";
                await Task.Delay(5000);

                lbText.Content = "чтобы сеанс прошел для Вас с максимальной пользой.";
                await Task.Delay(5000);

                lbText.Content = "Время сеанса составит 3 минуты,";
                await Task.Delay(3000);

                lbText.Content = "после чего программа автоматически остановит его.";
                await Task.Delay(5000);

                lbText.Content = "Пожалуйста, начинайте.";
                await Task.Delay(2000);

                lbText.Content = "НАЧИНАЕМ СЕАНС!";
                lbText.Visibility = Visibility.Visible;
                await Task.Delay(2000);
                lbText.Visibility = Visibility.Hidden;

                //Здесь 3 минуты какой-то херни
                Stop();
                StartFullDuplex();
                await Task.Run(() => PitchTimerFeelInTheBody());
                //await Task.Delay(180000);
                Stop();

                Filename = @"ReSelf - Mental detox\Record\tunetank.com_471_everest_by_alex-makemusicText.mp3";
                Sound(Filename);

                lbText.Visibility = Visibility.Visible;
                lbText.Content = "Шаг пятый:Сеанс окончен.";
                await Task.Delay(3000);

                lbText.Content = "Проанализируйте свои ощущения.";
                await Task.Delay(3000);

                lbText.Content = "Насколько Вам стало легче.";
                await Task.Delay(3000);

                lbText.Content = "Оцените эффективность сеанса в целом.";
                await Task.Delay(3000);

                lbText.Content = "Подумайте о том, достаточно этого сеанса,";
                await Task.Delay(4000);

                lbText.Content = "или требуется еще один или несколько.";
                await Task.Delay(4000);

                lbText.Content = "Всего наилучшего, аутотренинг окончен.";
                await Task.Delay(4000);

                lbText.Content = "Сейчас начнется запись голоса";
                lbText.Visibility = Visibility.Visible;
                await Task.Delay(4000);

                Stop();

                await Task.Run(() => TimerRec());
                lbText.Visibility = Visibility.Hidden;
                Recording2();
                await Task.Delay(7000);
                lbTitleNFT2.Visibility = Visibility.Visible;

                Repeat repeat = new Repeat();
                repeat.ShowDialog();
                //await Task.Delay(10000);

                if (Repeat.click == 1)
                {
                    Filename = @"ReSelf - Mental detox\Record\TheSoundEnd.mp3";
                    Sound(Filename);

                    HelpUnhelp help = new HelpUnhelp();
                    help.ShowDialog();
                    await Task.Delay(140000);
                    Close();
                }

            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в Feeling_in_the_body_Text: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in Feeling_in_the_body_Text: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }

        private async void StraightawaySession()
        {
            try
            {
                Repeat.click = 0;
                ShadowClick = 1;
                if (Session == "Feel")
                {
                    Feeling_in_the_body_pattern();
                }
                else
                {
                    Situation_Problem_pattern();
                }
                Stop();
                btnSituation_problem.IsEnabled = false;
                btnFeeling_in_the_body.IsEnabled = false;
                btnSituation_problem.Visibility = Visibility.Hidden;
                btnFeeling_in_the_body.Visibility = Visibility.Hidden;
                //button.IsEnabled = false;

                lbText.Content = "Сейчас начнется запись голоса";
                lbText.Visibility = Visibility.Visible;
                await Task.Delay(3000);
                await Task.Run(() => TimerRec());
                //Stop();
                lbText.Visibility = Visibility.Hidden;
                //await Task.Run(() => StartFullDuplex1());
                Recording1();
                await Task.Delay(7000);
                lbTitleNFT1.Visibility = Visibility.Visible;

                lbText.Content = "НАЧИНАЕМ СЕАНС!";
                lbText.Visibility = Visibility.Visible;
                await Task.Delay(5000);
                lbText.Visibility = Visibility.Hidden;

                //Здесь 3 минуты какой-то херни
                //Stop();
                StartFullDuplex();
                await Task.Run(() => PitchTimerFeelInTheBody());
                //await Task.Delay(180000);
                Stop();

                lbText.Content = "Сейчас начнется запись голоса";
                lbText.Visibility = Visibility.Visible;
                await Task.Delay(4000);
                await Task.Run(() => TimerRec());
                lbText.Visibility = Visibility.Hidden;
                Recording2();
                await Task.Delay(7000);
                lbTitleNFT2.Visibility = Visibility.Visible;

                Repeat repeat = new Repeat();
                repeat.ShowDialog();
                //await Task.Delay(10000);

                if (Repeat.click == 1)
                {
                    Filename = @"ReSelf - Mental detox\Record\TheSoundEnd.mp3";
                    Sound(Filename);

                    HelpUnhelp help = new HelpUnhelp();
                    help.ShowDialog();
                    await Task.Delay(140000);
                    Close();
                }
            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в StraightawaySession: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in StraightawaySession: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }//NEEEEEEEEEEEEEEEEEEEEEEEEEEED

        private async void Situation_problem()
        {
            try
            {
                Repeat.click = 0;
                ShadowClick = 1;
                Situation_Problem_pattern();
                Stop();
                btnSituation_problem.IsEnabled = false;
                btnFeeling_in_the_body.IsEnabled = false;
                btnSituation_problem.Visibility = Visibility.Hidden;
                btnFeeling_in_the_body.Visibility = Visibility.Hidden;
                //button.IsEnabled = false;
                
                Filename = @"ReSelf - Mental detox\Record\Situation_problem\HintSituationProblem_StepOneSituationProblem_StepTwoSituationProblem.wav";
                Sound(Filename);
                await Task.Delay(28000);
                lbText.Content = "переведите внимание на зажатые части\nтела и сделайте оттуда глубокий выдох";
                lbText.Visibility = Visibility.Visible;
                await Task.Delay(30000);
                lbText.Visibility = Visibility.Hidden;
                await Task.Delay(36000);

                lbText.Content = "Сейчас начнется запись голоса";
                lbText.Visibility = Visibility.Visible;
                //WinTime();
                await Task.Run(() => TimerRec());
                Stop();
                lbText.Visibility = Visibility.Hidden;
                await Task.Run(() => StartFullDuplex1());
                Recording1();
                await Task.Delay(5000);

                Stop();
                await Task.Delay(2000);
                //Thread.Sleep(5000);
                lbTitleNFT1.Visibility = Visibility.Visible;

                Filename = @"ReSelf - Mental detox\Record\Situation_problem\AfterStepTwoSituationProblem_StepThreeSituationProblem_StepFourSituationProblem.wav";
                Sound(Filename);
                await Task.Delay(90000);
                lbText.Content = "НАЧИНАЕМ СЕАНС!";
                lbText.Visibility = Visibility.Visible;
                await Task.Delay(6000);
                lbText.Visibility = Visibility.Hidden;

                /*Filename = @"ReSelf - Mental detox\Record\Situation_problem\StepThreeSituationProblem2.wav";
                Sound(Filename);
                await Task.Delay(46000);

                Filename = @"ReSelf - Mental detox\Record\Situation_problem\StepFourSituationProblem2.wav";
                Sound(Filename);
                await Task.Delay(39000);*/

                Stop();
                StartFullDuplex();
                await Task.Run(() => PitchTimerSitProb());
                Stop();

                Filename = @"ReSelf - Mental detox\Record\Situation_problem\StepFiveSituationProblem_RepeatRecord.wav";
                Sound(Filename);
                await Task.Delay(36000);

                /*Filename = @"ReSelf - Mental detox\Record\Feeling_in_the_body\RepeatRecord.wav";
                Sound(Filename);
                await Task.Delay(12000);*/

                //WinTime();
                lbText.Content = "Сейчас начнется запись голоса";
                lbText.Visibility = Visibility.Visible;
                await Task.Run(() => TimerRec());
                lbText.Visibility = Visibility.Hidden;
                Recording2();
                await Task.Delay(7000);
                lbTitleNFT2.Visibility = Visibility.Visible;

				Repeat repeat = new Repeat();
				repeat.ShowDialog();

                if (Repeat.click == 1)
                {
                    Filename = @"ReSelf - Mental detox\Record\TheSoundEnd.mp3";
                    Sound(Filename);
                    HelpUnhelp help = new HelpUnhelp();
                    help.ShowDialog();
                    await Task.Delay(140000);
                    Close();
                }
            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в Situation_problem: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in Situation_problem: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }

        private async void Situation_problem_Text()
        {
            try
            {
                Repeat.click = 0;
                ShadowClick = 1;
                Situation_Problem_pattern();
                Stop();
                btnSituation_problem.IsEnabled = false;
                btnFeeling_in_the_body.IsEnabled = false;
                btnSituation_problem.Visibility = Visibility.Hidden;
                btnFeeling_in_the_body.Visibility = Visibility.Hidden;
                //button.IsEnabled = false;
                lbText.Visibility = Visibility.Visible;

                Filename = @"ReSelf - Mental detox\Record\tunetank.com_471_everest_by_alex-makemusicText.mp3";
                Sound(Filename);

                lbText.Content = "Хорошо. Вы выбрали сеанс в течение которого сможете";
                await Task.Delay(5000);

                lbText.Content = "проработать проблему или ситуацию в Вашей жизни,";
                await Task.Delay(5000);

                lbText.Content = "чтобы разрешить ее на самом глубинном уровне.";
                await Task.Delay(5000);

                lbText.Content = "Пожалуйста, займите удобное положение.";
                await Task.Delay(3000);

                lbText.Content = "Всецело почувствуйте свое тело.";
                await Task.Delay(3000);

                lbText.Content = "Расслабьте на выдохе те места,";
                await Task.Delay(3000);

                lbText.Content = "в которых заметили напряжение.";
                await Task.Delay(3000);

                lbText.Content = "переведите внимание на зажатые части\nтела и сделайте оттуда глубокий выдох";
                await Task.Delay(30000);

                lbText.Content = "Закройте глаза, если так будет комфортнее. Мы начинаем сеанс.";
                await Task.Delay(5000);

                lbText.Content = "Шаг первый: подготовка голоса.";
                await Task.Delay(3000);

                lbText.Content = "Медленно и расслабленно открывайте рот все шире и шире,";
                await Task.Delay(5000);

                lbText.Content = "чтобы сеанс получился максимально эффективным.";
                await Task.Delay(5000);

                lbText.Content = "Шаг второй: сохраняя открытое положение рта";
                await Task.Delay(5000);

                lbText.Content = "- потяните долгий звук «ААА» и вслушайтесь\nв звучание своего голоса в наушниках.";
                await Task.Delay(5000);

                lbText.Content = "Сейчас начнется запись голоса";
                lbText.Visibility = Visibility.Visible;
                await Task.Run(() => TimerRec());
                Stop();
                lbText.Visibility = Visibility.Hidden;
                await Task.Run(() => StartFullDuplex1());
                Recording1();
                await Task.Delay(5000);

                Stop();
                lbTitleNFT1.Visibility = Visibility.Visible;

                Filename = @"ReSelf - Mental detox\Record\tunetank.com_471_everest_by_alex-makemusicText.mp3";
                Sound(Filename);

                lbText.Visibility = Visibility.Visible;
                lbText.Content = "Хорошо. Голос может звучать свободно и открыто.";
                await Task.Delay(5000);

                lbText.Content = "Шаг третий: представьте ситуацию, с которой\nвы будете работать прямо перед собой,";
                await Task.Delay(5000);

                lbText.Content = "в самом ее драматическом варианте,";
                await Task.Delay(3000);

                lbText.Content = "даже если это неприятно Вам.";
                await Task.Delay(3000);

                lbText.Content = "Для того, чтобы ее решить Вашему сознанию\nтребуется точно на ней сфокусироваться.";
                await Task.Delay(5000);

                lbText.Content = "Чем реалистичнее Вы сможете перенести\nсвое внимание в эту проблему,";
                await Task.Delay(5000);

                lbText.Content = "тем эффективнее получится самотерапия.";
                await Task.Delay(3000);

                lbText.Content = "Итак, максимально представьте проблему перед собой.";
                await Task.Delay(5000);

                lbText.Content = "Это может быть один образ или целый\nряд образов сменяющих друг друга.";
                await Task.Delay(5000);

                lbText.Content = "Шаг четвертый: начните тянуть долгий звук, продолжая\nфокусироваться на проблемном образе.";
                await Task.Delay(5000);

                lbText.Content = "Ваш голос станет мощным инструментом нейтрализации проблемы.";
                await Task.Delay(5000);

                lbText.Content = "В течение всего сеанса программа настроит его таким образом,";
                await Task.Delay(5000);

                lbText.Content = "чтобы сеанс прошел для Вас с максимальной пользой.";
                await Task.Delay(5000);

                lbText.Content = "Направьте свой голос прямо в этот образ.";
                await Task.Delay(5000);

                lbText.Content = "Время сеанса составит 3 минуты,";
                await Task.Delay(5000);

                lbText.Content = "после чего программа автоматически остановит его.";
                await Task.Delay(5000);

                lbText.Content = "Пожалуйста, начинайте.";
                await Task.Delay(3000);

                lbText.Content = "НАЧИНАЕМ СЕАНС!";
                lbText.Visibility = Visibility.Visible;
                await Task.Delay(5000);
                lbText.Visibility = Visibility.Hidden;

                Stop();
                StartFullDuplex();
                await Task.Run(() => PitchTimerSitProb());
                Stop();

                Filename = @"ReSelf - Mental detox\Record\tunetank.com_471_everest_by_alex-makemusicText.mp3";
                Sound(Filename);

                lbText.Visibility = Visibility.Visible;
                lbText.Content = "Сеанс окончен. Проанализируйте свои ощущения.";
                await Task.Delay(5000);

                lbText.Content = "Насколько Вам стало легче. Оцените эффективность сеанса в целом.";
                await Task.Delay(5000);

                lbText.Content = "Подумайте о том, достаточно этого сеанса,";
                await Task.Delay(3000);

                lbText.Content = "или требуется еще один или несколько.";
                await Task.Delay(3000);

                lbText.Content = "Всего наилучшего, аутотренинг окончен.";
                await Task.Delay(5000);

                Stop();

                lbText.Content = "Сейчас начнется запись голоса";
                lbText.Visibility = Visibility.Visible;
                await Task.Run(() => TimerRec());
                lbText.Visibility = Visibility.Hidden;
                Recording2();
                await Task.Delay(7000);
                lbTitleNFT2.Visibility = Visibility.Visible;

                Repeat repeat = new Repeat();
                repeat.ShowDialog();
                //await Task.Delay(10000);

                if (Repeat.click == 1)
                {
                    Filename = @"ReSelf - Mental detox\Record\TheSoundEnd.mp3";
                    Sound(Filename);
                    HelpUnhelp help = new HelpUnhelp();
                    help.ShowDialog();
                    await Task.Delay(140000);
                    Close();
                }
            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в Situation_problem_Text: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in Situation_problem_Text: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }

        private void Stop()
        {
            try
            {
                if (mMixer != null)
                {
                    mMixer.Dispose();
                    //mMp3.ToWaveSource(32).Loop().ToSampleSource().Dispose();
                    mMixer = null;
                }
                if (mSoundOut != null)
                {
                    mSoundOut.Stop();
                    mSoundOut.Dispose();
                    mSoundOut = null;
                }
                if (mSoundIn != null)
                {
                    mSoundIn.Stop();
                    mSoundIn.Dispose();
                    mSoundIn = null;
                }
                if (mSource != null)
                {
                    mSource.Dispose();
                    mSource = null;
                }
                if (mMp3 != null)
                {
                    /*if (mDspRec != null)
                    {
                        mDspRec.Dispose();
                    }*/
                    mMp3.Dispose();
                    mMp3 = null;
                }
                
            }
            catch (Exception ex)
            {
                /*if (langindex == "0")
                {
                    string msg = "Ошибка в Stop: \r\n" + ex.Message;
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in Stop: \r\n" + ex.Message;
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }*/
            }
        }

        private void HARMONICA_Closing(object sender, CancelEventArgs e)
        {
            Stop();
            if (File.Exists(fileDeleteRec1))
            {
                File.Delete(fileDeleteRec1);
            }
            if (File.Exists(fileDeleteCutRec1))
            {
                File.Delete(fileDeleteCutRec1);
            }
            if (File.Exists(fileDeleteRec2))
            {
                File.Delete(fileDeleteRec2);
            }
            if (File.Exists(fileDeleteCutRec2))
            {
                File.Delete(fileDeleteCutRec2);
            }
            Environment.Exit(0);
        }

        private void btnFeeling_in_the_body_Click(object sender, RoutedEventArgs e)
        {
            ImgBtnTurboClick = 1;
            btnSituationShadow.Opacity = 0;
            Session = "Feel";
            ChoiceView view = new ChoiceView();
            view.ShowDialog();
            //Feeling_in_the_body();
        }

        private async void StartFullDuplex()//запуск пича и громкости
        {
            try
            {
                SoundClick = 1;
                //Запускает устройство захвата звука с задержкой 1 мс.
                //await Task.Run(() => SoundIn());
                SoundIn();

                var source = new SoundInSource(mSoundIn) { FillWithZeros = true };
                var xsource = source.ToSampleSource();

                var reverb = new DmoWavesReverbEffect(xsource.ToWaveSource());
                reverb.ReverbTime = reverbVal;
                reverb.HighFrequencyRTRatio = ((float)1) / 1000;
                xsource = reverb.ToSampleSource();

                //Init DSP для смещения высоты тона
                mDspPitch = new SampleDSPPitch(xsource.ToMono());

                SetPitchShiftValue();

                //Инициальный микшер
                Mixer();

                //Добавляем наш источник звука в микшер
                mMixer.AddSource(mDspPitch.ChangeSampleRate(mMixer.WaveFormat.SampleRate));

                //Запускает устройство воспроизведения звука с задержкой 1 мс.
                await Task.Run(() => SoundOut());

            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в StartFullDuplex: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in StartFullDuplex: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
            //return false;
        }

        private async void StartFullDuplex1()//запуск пича и громкости
        {
            try
            {
                SoundClick = 1;
                //Запускает устройство захвата звука с задержкой 1 мс.
                //await Task.Run(() => SoundIn());
                SoundIn();

                var source = new SoundInSource(mSoundIn) { FillWithZeros = true };

                //Init DSP для смещения высоты тона
                mDspPitch = new SampleDSPPitch(source.ToSampleSource().ToMono());

                SetPitchShiftValue();

                //Инициальный микшер
                Mixer();

                //Добавляем наш источник звука в микшер
                mMixer.AddSource(mDspPitch.ChangeSampleRate(mMixer.WaveFormat.SampleRate));

                //Запускает устройство воспроизведения звука с задержкой 1 мс.
                await Task.Run(() => SoundOut());

            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в StartFullDuplex: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in StartFullDuplex: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
            //return false;
        }

        private async void Recording1()
        {
            try
            {
                myfile = "MyRecord1.wav";
                cutmyfile = "cutMyRecord1.wav";
                fileDeleteRec1 = myfile;
                fileDeleteCutRec1 = cutmyfile;
                if (File.Exists(myfile))
                {
                    File.Delete(myfile);
                }
                if (File.Exists(cutmyfile))
                {
                    File.Delete(cutmyfile);
                }
                using (mSoundIn = new WasapiCapture())
                {
                    mSoundIn.Device = mInputDevices[cmbInput.SelectedIndex];
                    mSoundIn.Initialize();

                    mSoundIn.Start();
                    pbRecord.Visibility = Visibility.Visible;
                    lbRecordPB.Visibility = Visibility.Visible;
                    using (WaveWriter record = new WaveWriter(cutmyfile, mSoundIn.WaveFormat))
                    {
                        mSoundIn.DataAvailable += (s, data) => record.Write(data.Data, data.Offset, data.ByteCount);
                        for (int i = 0; i < 100; i++)
                        {
                            pbRecord.Value++;
                            await Task.Delay(35);
                            if (pbRecord.Value == 25)
                            {
                                string uri1 = @"ReSelf - Mental detox\Progressbar\Group 13.png";
                                ImgPBRecordBack.ImageSource = new ImageSourceConverter().ConvertFromString(uri1) as ImageSource;
                            }
                            else if (pbRecord.Value == 50)
                            {
                                string uri2 = @"ReSelf - Mental detox\Progressbar\Group 12.png";
                                ImgPBRecordBack.ImageSource = new ImageSourceConverter().ConvertFromString(uri2) as ImageSource;
                            }
                            else if (pbRecord.Value == 75)
                            {
                                string uri3 = @"ReSelf - Mental detox\Progressbar\Group 11.png";
                                ImgPBRecordBack.ImageSource = new ImageSourceConverter().ConvertFromString(uri3) as ImageSource;
                            }
                            else if (pbRecord.Value == 95)
                            {
                                string uri4 = @"ReSelf - Mental detox\Progressbar\Group 10.png";
                                ImgPBRecordBack.ImageSource = new ImageSourceConverter().ConvertFromString(uri4) as ImageSource;
                            }
                        }
                        //Thread.Sleep(5000);

                        mSoundIn.Stop();
                        lbRecordPB.Visibility = Visibility.Hidden;
                        pbRecord.Value = 0;
                        pbRecord.Visibility = Visibility.Hidden;

                    }
                    //Thread.Sleep(1000);
                    string uri = @"ReSelf - Mental detox\Progressbar\progressbar-backgrnd.png";
                    ImgPBRecordBack.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
                    int[] Rdat = new int[150000];
                    int Ndt;
                    Ndt = vizualzvuk(cutmyfile, myfile, Rdat, 1);
                    NFT_drawing1(myfile); 

                }
                if (langindex == "0")
                {
                    string msg = "Запись и обработка завершена. Сейчас появится графическое изображение вашего голоса.";
                    LogClass.LogWrite(msg);
                }
                else
                {
                    string msg = "Recording and processing completed. A graphic representation of your voice will now appear.";
                    LogClass.LogWrite(msg);
                }
            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в Recording1: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in Recording1: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }

        private void btnSituation_problem_Click(object sender, RoutedEventArgs e)
        {
            ImgBtnTurboTwoClick = 1;
            btnFeelingShadow.Opacity = 0;
            Session = "Sit";
            ChoiceView view = new ChoiceView();
            view.ShowDialog();
            //Situation_problem();
        }

        private void btnFeeling_in_the_body_MouseMove(object sender, MouseEventArgs e)
        {
            string uri = @"ReSelf - Mental detox\Button\button-turbo-hover.png";
            ImgBtnFeelingInTheBody.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
        }

        private void btnFeeling_in_the_body_MouseLeave(object sender, MouseEventArgs e)
        {
            if (ImgBtnTurboClick == 1)
            {
                string uri = @"ReSelf - Mental detox\Button\button-turbo-active.png";
                ImgBtnFeelingInTheBody.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
            }
            else
            {
                string uri = @"ReSelf - Mental detox\Button\button-turbo-inactive.png";
                ImgBtnFeelingInTheBody.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
            }
        }

        private void btnIncVol_Click(object sender, RoutedEventArgs e)
        {
            var wih = new WindowInteropHelper(this);
            var hWnd = wih.Handle;
            SendMessageW(hWnd, WM_APPCOMMAND, hWnd, (IntPtr)APPCOMMAND_VOLUME_UP);

            string uri = @"ReSelf - Mental detox\Button\button-soundup-active.png";
            ImgBackIncVol.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
        }

        private void btnDecVol_Click(object sender, RoutedEventArgs e)
        {
            var wih = new WindowInteropHelper(this);
            var hWnd = wih.Handle;
            SendMessageW(hWnd, WM_APPCOMMAND, hWnd, (IntPtr)APPCOMMAND_VOLUME_DOWN);
            /*pbVolumeRight.Value -= 1310;
            pbVolumeLeft.Value -= 1310;
            SetVolume();*/
            string uri = @"ReSelf - Mental detox\Button\button-sounddown-active.png";
            btnImgDecVol.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
        }

        private void btnIncVol_MouseMove(object sender, MouseEventArgs e)
        {
            string uri = @"ReSelf - Mental detox\Button\button-soundup-hover.png";
            ImgBackIncVol.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
        }

        private void btnIncVol_MouseLeave(object sender, MouseEventArgs e)
        {
            string uri = @"ReSelf - Mental detox\Button\button-soundup-inactive.png";
            ImgBackIncVol.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
        }

        private void btnDecVol_MouseMove(object sender, MouseEventArgs e)
        {
            string uri = @"ReSelf - Mental detox\Button\button-sounddown-hover.png";
            btnImgDecVol.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
        }

        private void btnDecVol_MouseLeave(object sender, MouseEventArgs e)
        {
            string uri = @"ReSelf - Mental detox\Button\button-sounddown-inactive.png";
            btnImgDecVol.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
        }

        private void HARMONICA_Activated(object sender, EventArgs e)
        {
            try
            {
                if (Repeat.repeat == "Yes")
                {
                    
                    if (Session == "Feel")
                    {
                        Repeat.repeat = "No";
                        if (ChoiceView.View == "AudioGid")
                        {
                            Feeling_in_the_body();
                        }
                        else if (ChoiceView.View == "Text")
                        {
                            Feeling_in_the_body_Text();
                        }
                        else if (ChoiceView.View == "Straightaway")
                        {
                            StraightawaySession();
                        }
                    }
                    else if (Session == "Sit")
                    {
                        Repeat.repeat = "No";
                        if (ChoiceView.View == "AudioGid")
                        {
                            Situation_problem();
                        }
                        else if (ChoiceView.View == "Text")
                        {
                            Situation_problem_Text();
                        }
                        else if (ChoiceView.View == "Straightaway")
                        {
                            StraightawaySession();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в Activated: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in Activated: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }//NEEEEEEEEEEEED

        /*private void HARMONICA_KeyDown(object sender, KeyEventArgs e)
        {
            ModifierKeys combCtrSh = ModifierKeys.Control | ModifierKeys.Shift;
            Key comboKey = Key.A | Key.D;

            if((e.Key & comboKey) == comboKey)
            {
                if((e.KeyboardDevice.Modifiers & combCtrSh) == combCtrSh)
                {
                    MessageBox.Show("Ура!!!");
                }
            }
        }*/

        private void btnSituation_problem_MouseMove(object sender, MouseEventArgs e)
        {
            string uri = @"ReSelf - Mental detox\Button\button-turbo-hover2.png";
            ImgBtnSolutionProblem.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
        }

        private void button_MouseMove(object sender, MouseEventArgs e)
        {
            string uri = @"ReSelf - Mental detox\Button\button-settings-hover.png";
            ImgBtnSettings.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
        }

        private void button_MouseLeave(object sender, MouseEventArgs e)
        {
            string uri = @"ReSelf - Mental detox\Button\button-settings-inactive.png";
            ImgBtnSettings.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            BtnSetClick++;
            string uri = @"ReSelf - Mental detox\Button\button-settings-active.png";
            ImgBtnSettings.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
            if (BtnSetClick == 1)
            {
                tabNFTSet.SelectedItem = TabSettings;
                lbSpeaker.Visibility = Visibility.Visible;
                lbMicrophone.Visibility = Visibility.Visible;
            }
            else
            {
                tabNFTSet.SelectedItem = TabNFT;
                lbSpeaker.Visibility = Visibility.Hidden;
                lbMicrophone.Visibility = Visibility.Hidden;
                BtnSetClick = 0;
            }
        }

        private void btnSituation_problem_MouseLeave(object sender, MouseEventArgs e)
        {
            if (ImgBtnTurboTwoClick == 1)
            {
                string uri = @"ReSelf - Mental detox\Button\button-turbo-active2.png";
                ImgBtnSolutionProblem.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
            }
            else
            {
                string uri = @"ReSelf - Mental detox\Button\button-turbo-inactive2.png";
                ImgBtnSolutionProblem.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
            }
        }

        private async void Recording2()
        {
            try
            {
                myfile = "MyRecord2.wav";
                cutmyfile = "cutMyRecord2.wav";
                fileDeleteRec2 = myfile;
                fileDeleteCutRec2 = cutmyfile;
                if (File.Exists(myfile))
                {
                    File.Delete(myfile);
                }
                if (File.Exists(cutmyfile))
                {
                    File.Delete(cutmyfile);
                }
                using (mSoundIn = new WasapiCapture())
                {
                    mSoundIn.Device = mInputDevices[cmbInput.SelectedIndex];
                    mSoundIn.Initialize();
                    mSoundIn.Start();

                    pbRecord.Visibility = Visibility.Visible;
                    lbRecordPB.Visibility = Visibility.Visible;
                    using (WaveWriter record = new WaveWriter(cutmyfile, mSoundIn.WaveFormat))
                    {
                        mSoundIn.DataAvailable += (s, data) => record.Write(data.Data, data.Offset, data.ByteCount);
                        for (int i = 0; i < 100; i++)
                        {
                            pbRecord.Value++;
                            await Task.Delay(35);
                            if (pbRecord.Value == 25)
                            {
                                string uri1 = @"ReSelf - Mental detox\Progressbar\Group 13.png";
                                ImgPBRecordBack.ImageSource = new ImageSourceConverter().ConvertFromString(uri1) as ImageSource;
                            }
                            else if (pbRecord.Value == 50)
                            {
                                string uri2 = @"ReSelf - Mental detox\Progressbar\Group 12.png";
                                ImgPBRecordBack.ImageSource = new ImageSourceConverter().ConvertFromString(uri2) as ImageSource;
                            }
                            else if (pbRecord.Value == 75)
                            {
                                string uri3 = @"ReSelf - Mental detox\Progressbar\Group 11.png";
                                ImgPBRecordBack.ImageSource = new ImageSourceConverter().ConvertFromString(uri3) as ImageSource;
                            }
                            else if (pbRecord.Value == 95)
                            {
                                string uri4 = @"ReSelf - Mental detox\Progressbar\Group 10.png";
                                ImgPBRecordBack.ImageSource = new ImageSourceConverter().ConvertFromString(uri4) as ImageSource;
                            }
                        }
                        //Thread.Sleep(5000);

                        mSoundIn.Stop();
                        lbRecordPB.Visibility = Visibility.Hidden;
                        pbRecord.Value = 0;
                        pbRecord.Visibility = Visibility.Hidden;

                    }
                    //Thread.Sleep(100);
                    string uri = @"ReSelf - Mental detox\Progressbar\progressbar-backgrnd.png";
                    ImgPBRecordBack.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
                    int[] Rdat = new int[150000];
                    int Ndt;
                    Ndt = vizualzvuk(cutmyfile, myfile, Rdat, 1);
                    NFT_drawing2(myfile);

                }
                if (langindex == "0")
                {
                    string msg = "Запись и обработка завершена. Сейчас появится графическое изображение вашего голоса.";
                    LogClass.LogWrite(msg);
                    //MessageBox.Show(msg);
                }
                else
                {
                    string msg = "Recording and processing completed. A graphic representation of your voice will now appear.";
                    LogClass.LogWrite(msg);
                    //MessageBox.Show(msg);
                }
            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в Recording2: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in Recording2: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }

        private async void NFT_drawing1(string filename)
        {
            int[] Rdat = new int[250000];
            int Ndt;
            int Ww, Hw, k, ik, dWw, dHw;
            worker.RunWorkerAsync();
            Ndt = await Task.Run(() =>
            {
                return vizualzvuk(filename, filename, Rdat, 0);
            });
            Hw = (int)Math.Sqrt(Ndt);
            Ww = (int)((double)(Ndt) / (double)(Hw) + 0.5);
            dWw = (int)((Image1.Width - (double)Ww) / 2.0) - 5;
            if (dWw < 0)
                dWw = 0;
            dHw = (int)((Image1.Height - (double)Hw) / 2.0) - 5;
            if (dHw < 0)
                dHw = 0;
            WriteableBitmap wb = new WriteableBitmap((int)Image1.Width, (int)Image1.Height, Ww, Hw, PixelFormats.Bgra32, null);

            // Define the update square (which is as big as the entire image).
            Int32Rect rect = new Int32Rect(0, 0, (int)Image1.Width, (int)Image1.Height);

            byte[] pixels = new byte[(int)Image1.Width * (int)Image1.Height * wb.Format.BitsPerPixel / 8];
            //Random rand = new Random();
            k = 0;
            ik = 0;
            int Wwt = 2, Hwt = 2, it0 = Ww / 2, jt0 = Hw / 2, it = 0, jt = 0;
            int R = 0, G = 0, B = 0, A = 0;
            int pixelOffset, poffp = 0, kt = 0;
            while (k < Ndt)
            {
                if (ik % 4 == 0)
                {
                    R = Rdat[3 * k];
                    G = Rdat[3 * k + 1];
                    B = Rdat[3 * k + 2];
                    A = 255;
                    pixelOffset = (dWw + it0 + it + (dHw + jt0 + jt) * wb.PixelWidth) * wb.Format.BitsPerPixel / 8;
                    pixels[pixelOffset] = (byte)B;
                    pixels[pixelOffset + 1] = (byte)G;
                    pixels[pixelOffset + 2] = (byte)R;
                    pixels[pixelOffset + 3] = (byte)A;
                    jt++;
                    if (jt == Hwt)
                    {
                        ik++;
                    }
                }
                else if (ik % 4 == 1)
                {
                    R = Rdat[3 * k];
                    G = Rdat[3 * k + 1];
                    B = Rdat[3 * k + 2];
                    A = 255;
                    pixelOffset = (dWw + it0 + it + (dHw + jt0 + jt) * wb.PixelWidth) * wb.Format.BitsPerPixel / 8;
                    pixels[pixelOffset] = (byte)B;
                    pixels[pixelOffset + 1] = (byte)G;
                    pixels[pixelOffset + 2] = (byte)R;
                    pixels[pixelOffset + 3] = (byte)A;
                    it++;
                    if (it == Wwt)
                    {
                        ik++;
                    }
                }
                else if (ik % 4 == 2)
                {
                    R = Rdat[3 * k];
                    G = Rdat[3 * k + 1];
                    B = Rdat[3 * k + 2];
                    A = 255;
                    pixelOffset = (dWw + it0 + it + (dHw + jt0 + jt) * wb.PixelWidth) * wb.Format.BitsPerPixel / 8;
                    pixels[pixelOffset] = (byte)B;
                    pixels[pixelOffset + 1] = (byte)G;
                    pixels[pixelOffset + 2] = (byte)R;
                    pixels[pixelOffset + 3] = (byte)A;
                    jt--;
                    if (jt == -1)
                    {
                        ik++;
                        //jt0--;
                    }
                }
                else
                {
                    R = Rdat[3 * k];
                    G = Rdat[3 * k + 1];
                    B = Rdat[3 * k + 2];
                    A = 255;
                    pixelOffset = (dWw + it0 + it + (dHw + jt0 + jt) * wb.PixelWidth) * wb.Format.BitsPerPixel / 8;
                    pixels[pixelOffset] = (byte)B;
                    pixels[pixelOffset + 1] = (byte)G;
                    pixels[pixelOffset + 2] = (byte)R;
                    pixels[pixelOffset + 3] = (byte)A;
                    it--;
                    if (it == -1)
                    {
                        it = 0;
                        jt = 0;
                        ik++;
                        it0--;
                        jt0--;
                        Hwt += 2;
                        Wwt += 2;
                    }
                }
                int stride = ((int)Image1.Width * wb.Format.BitsPerPixel) / 8;
                wb.WritePixels(rect, pixels, stride, 0);
                k++;
            }
            // Show the bitmap in an Image element.
            Image1.Source = wb;
            Image1.UpdateLayout();
            //NFTShadow = 1;
            //imgShadowNFT.Visibility = Visibility.Visible;

            worker.CancelAsync();
        }

        private async void NFT_drawing2(string filename)
        {
            int[] Rdat = new int[250000];
            int Ndt;
            int Ww, Hw, k, ik, dWw, dHw;
            worker.RunWorkerAsync();
            Ndt = await Task.Run(() =>
            {
                return vizualzvuk(filename, filename, Rdat, 0);
            });
            Hw = (int)Math.Sqrt(Ndt);
            Ww = (int)((double)(Ndt) / (double)(Hw) + 0.5);
            dWw = (int)((Image2.Width - (double)Ww) / 2.0) - 5;
            if (dWw < 0)
                dWw = 0;
            dHw = (int)((Image2.Height - (double)Hw) / 2.0) - 5;
            if (dHw < 0)
                dHw = 0;
            WriteableBitmap wb = new WriteableBitmap((int)Image2.Width, (int)Image2.Height, Ww, Hw, PixelFormats.Bgra32, null);

            // Define the update square (which is as big as the entire image).
            Int32Rect rect = new Int32Rect(0, 0, (int)Image2.Width, (int)Image2.Height);

            byte[] pixels = new byte[(int)Image2.Width * (int)Image2.Height * wb.Format.BitsPerPixel / 8];
            //Random rand = new Random();
            k = 0;
            ik = 0;
            int Wwt = 2, Hwt = 2, it0 = Ww / 2, jt0 = Hw / 2, it = 0, jt = 0;
            int R = 0, G = 0, B = 0, A = 0;
            int pixelOffset, poffp = 0, kt = 0;
            while (k < Ndt)
            {
                if (ik % 4 == 0)
                {
                    R = Rdat[3 * k];
                    G = Rdat[3 * k + 1];
                    B = Rdat[3 * k + 2];
                    A = 255;
                    pixelOffset = (dWw + it0 + it + (dHw + jt0 + jt) * wb.PixelWidth) * wb.Format.BitsPerPixel / 8;
                    pixels[pixelOffset] = (byte)B;
                    pixels[pixelOffset + 1] = (byte)G;
                    pixels[pixelOffset + 2] = (byte)R;
                    pixels[pixelOffset + 3] = (byte)A;
                    jt++;
                    if (jt == Hwt)
                    {
                        ik++;
                    }
                }
                else if (ik % 4 == 1)
                {
                    R = Rdat[3 * k];
                    G = Rdat[3 * k + 1];
                    B = Rdat[3 * k + 2];
                    A = 255;
                    pixelOffset = (dWw + it0 + it + (dHw + jt0 + jt) * wb.PixelWidth) * wb.Format.BitsPerPixel / 8;
                    pixels[pixelOffset] = (byte)B;
                    pixels[pixelOffset + 1] = (byte)G;
                    pixels[pixelOffset + 2] = (byte)R;
                    pixels[pixelOffset + 3] = (byte)A;
                    it++;
                    if (it == Wwt)
                    {
                        ik++;
                    }
                }
                else if (ik % 4 == 2)
                {
                    R = Rdat[3 * k];
                    G = Rdat[3 * k + 1];
                    B = Rdat[3 * k + 2];
                    A = 255;
                    pixelOffset = (dWw + it0 + it + (dHw + jt0 + jt) * wb.PixelWidth) * wb.Format.BitsPerPixel / 8;
                    pixels[pixelOffset] = (byte)B;
                    pixels[pixelOffset + 1] = (byte)G;
                    pixels[pixelOffset + 2] = (byte)R;
                    pixels[pixelOffset + 3] = (byte)A;
                    jt--;
                    if (jt == -1)
                    {
                        ik++;
                        //jt0--;
                    }
                }
                else
                {
                    R = Rdat[3 * k];
                    G = Rdat[3 * k + 1];
                    B = Rdat[3 * k + 2];
                    A = 255;
                    pixelOffset = (dWw + it0 + it + (dHw + jt0 + jt) * wb.PixelWidth) * wb.Format.BitsPerPixel / 8;
                    pixels[pixelOffset] = (byte)B;
                    pixels[pixelOffset + 1] = (byte)G;
                    pixels[pixelOffset + 2] = (byte)R;
                    pixels[pixelOffset + 3] = (byte)A;
                    it--;
                    if (it == -1)
                    {
                        it = 0;
                        jt = 0;
                        ik++;
                        it0--;
                        jt0--;
                        Hwt += 2;
                        Wwt += 2;
                    }
                }
                int stride = ((int)Image2.Width * wb.Format.BitsPerPixel) / 8;
                wb.WritePixels(rect, pixels, stride, 0);
                k++;
            }
            // Show the bitmap in an Image element.
            Image2.Source = wb;
            Image2.UpdateLayout();
            //NFTShadow = 1;
            //imgShadowNFT.Visibility = Visibility.Visible;

            worker.CancelAsync();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                limit = 22;
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

                /*if(!Directory.Exists(path + "Windows Administrator"))
                {
                    Directory.CreateDirectory(path + @"\Windows Administrator");
                    pathAdminLogin = path + @"\Windows Administrator\Login";
                    pathAdminPass = path + @"\Windows Administrator\Administrator";
                }*/

                if (!Directory.Exists(path + "ReSelf - Mental detox Katarsis"))
                {
                    Directory.CreateDirectory(path + @"\ReSelf - Mental detox Katarsis");
                    path2 = path + @"\ReSelf - Mental detox Katarsis\Data";

                }

                Stream MyStream;

                if(!File.Exists(path + @"\ReSelf - Mental detox Katarsis\Key.tmp"))
                {
                    using (MyStream = File.Open(path + @"\ReSelf - Mental detox Katarsis\Key.tmp", FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        StreamWriter myWrite = new StreamWriter(MyStream);
                        myWrite.WriteLine("");
                    }
                    //File.Create(path + @"\ReSelf - Mental detox Katarsis\Key.tmp");
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

                /*StreamReader reader = new StreamReader(path + @"\ReSelf - Mental detox Katarsis\Key.tmp");
                string Key = reader.ReadToEnd();*/
                using (MyStream = File.Open(path + @"\ReSelf - Mental detox Katarsis\Key.tmp", FileMode.OpenOrCreate, FileAccess.Read))
                {
                    StreamReader MyReader = new StreamReader(MyStream);
                    string Key = MyReader.ReadToEnd();
                    KeyPass = Key;
                    
                }
                //string hr = "nBs1pVRH\r\n";
				/*if (KeyPass != hr || KeyPass != "q7|cM**D\r\n" || KeyPass != "$99aJKQI\r\n" || KeyPass != "SYf6f3NJ\r\n" || KeyPass != "|*~0TILx\r\n" || KeyPass != "{1fpjgIa\r\n" || KeyPass != "s4DwaSG6\r\n" || KeyPass != "yCRYymE9\r\n" || KeyPass != "h?d3?NR2\r\n" || KeyPass != "eFcu8H1r\r\n")
				{
					limit -= 7;
				}*/
                switch (KeyPass)
                {
                    case "nBs1pVRH\r\n":
                        break;
                    case "q7|cM**D\r\n":
                        break;
                    case "$99aJKQI\r\n":
                        break;
                    case "SYf6f3NJ\r\n":
                        break;
                    case "|*~0TILx\r\n":
                        break;
                    case "{1fpjgIa\r\n":
                        break;
                    case "s4DwaSG6\r\n":
                        break;
                    case "yCRYymE9\r\n":
                        break;
                    case "h?d3?NR2\r\n":
                        break;
                    case "eFcu8H1r\r\n":
                        break;
                    default:
                        {
							limit -= 7;
                            break;
						}
				}

                if (IsEnabled == true)
                {
                    countLimit = check.strt(path2);
                }

				if (countLimit > limit)
                {
                    this.IsEnabled = false;
                    /*ActivationForm activation = new ActivationForm();
                    activation.Show();*/
                    WinKey win = new WinKey();
                    win.Show();
                    MessageBoxAct boxAct = new MessageBoxAct();
                    boxAct.ShowDialog();

                }
                else
                {
                    //countLimit--;
                    lbCountLimit.Content = countLimit.ToString() + "/15";
                    if (langindex == "0")
                    {
                        MessageBoxSpeak boxSpeak = new MessageBoxSpeak();
                        boxSpeak.ShowDialog();
                        //await Task.Delay(5000);
                        //boxSpeak.Close();
                        //string msg = "Подключите проводную аудио-гарнитуру к компьютеру.\nЕсли на данный момент гарнитура не подключена,\nто подключите проводную гарнитуру, и перезапустите программу для того, чтобы звук подавался в наушники.";
                        //MessageBox.Show(msg);
                        
                        
                    }
                    else
                    {
                        string msg = "Connect a wired audio headset to your computer.\nIf a headset is not currently connected,\nthen connect a wired headset and restart the program so that the sound is played through the headphones.";
                        MessageBox.Show(msg);
                    }


                    Filename = @"ReSelf - Mental detox\Record\StartMusic.wav";
                    //btnFeeling_in_the_body.IsEnabled = false;
                    //btnSituation_problem.IsEnabled = false;
                    await Task.Run(() => Sound(Filename));
                    await Task.Delay(19000);
                    if (ShadowClick == 0)
                    {
                        btnFeelingShadow.Opacity = 1;
                        await Task.Delay(500);
                        btnFeelingShadow.Opacity = 0;
                        btnSituationShadow.Opacity = 1;
                        await Task.Delay(500);
                        btnFeelingShadow.Opacity = 1;
                        btnSituationShadow.Opacity = 0;
                        await Task.Delay(500);
                        btnFeelingShadow.Opacity = 0;
                        btnSituationShadow.Opacity = 1;
                        await Task.Delay(500);
                        btnFeelingShadow.Opacity = 1;
                        btnSituationShadow.Opacity = 0;
                        await Task.Delay(500);
                        btnFeelingShadow.Opacity = 0;
                        btnSituationShadow.Opacity = 1;
                        await Task.Delay(500);
                        btnFeelingShadow.Opacity = 1;
                    }
                    await Task.Delay(7000);
                    //Stop();
                    //btnFeeling_in_the_body.IsEnabled = true;
                    //btnSituation_problem.IsEnabled = true;
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