// This is a personal academic project. Dear PVS-Studio, please check it.

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
        private ISampleSource mMp3;
        private IWaveSource mSource;

        private static string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static string path2;

        string langindex, fileDeleteRec1, fileDeleteCutRec1, fileDeleteRec2, fileDeleteCutRec2;
        string myfile;
        string cutmyfile;
        private string Filename;

        private int SampleRate;
        private float pitchVal;
        private float reverbVal;
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



                mSoundOut.Initialize(mMixer.ToWaveSource(32).ToMono());


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
                    lbRecordPB.Content = "Идёт запись...";
                    btnFeeling_in_the_body.ToolTip = "Сеанс «Ощущение в теле»\nХорошо, Вы выбрали сеанс в течение которого сможете\nвысвободить отрицательную энергию, почувствовать себя легче и спокойнее.\nПожалуйста, займите удобное положение.\nВсецело почувствуйте свое тело.\nРасслабьте на выдохе те места, в которых заметили напряжение (играет спокойная музыка – 30 секунд).\nЗакройте глаза. Мы начинаем сеанс.";
                    btnSituation_problem.ToolTip = "Сеанс «Ситуация/проблема»\nХорошо, Вы выбрали сеанс в течение которого сможете\nпроработать проблему или ситуацию в Вашей жизни,\nчтобы разрешить ее на самом глубинном уровне.\nПожалуйста, займите удобное положение.\nВсецело почувствуйте свое тело.\nРасслабьте на выдохе те места, в которых заметили напряжение (играет спокойная музыка – 30 секунд).\nЗакройте глаза, если так будет комфортнее. Мы начинаем сеанс.";
                }
                else
                {
                    Title = "HARMONICA";
                    lbRecordPB.Content = "Recording in progress...";
                    btnFeeling_in_the_body.ToolTip = "Session «Feeling in the body»\nWell, you have chosen a session during\nwhich you can release negative energy,\nfeel lighter and calmer.\nPlease take a comfortable position.\nFeel your whole body.Relax as you exhale those places\nwhere you noticed tension (calm music plays - 30 seconds).\nClose your eyes. We start the session.";
                    btnSituation_problem.ToolTip = "Session «Situation/problem»\nGood. You have chosen a session during which you will be able to\nwork through a problem or situation in your life in order to resolve it at the deepest level.\nPlease take a comfortable position. Feel your whole body.\nRelax as you exhale those places where you noticed tension (calm music plays - 30 seconds).\nClose your eyes if that makes you feel more comfortable. We start the session.";
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
                Mixer();
                mMp3 = CodecFactory.Instance.GetCodec(FileName).ToMono().ToSampleSource();
                //mDspPitch = new SampleDSPPitch(mMp3.ToWaveSource(32).ToSampleSource());
                //SampleRate = mDspRec.WaveFormat.SampleRate;
                mMixer.AddSource(mMp3.ChangeSampleRate(mMp3.WaveFormat.SampleRate).ToWaveSource(32).ToSampleSource());
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
            string PathFile = @"HARMONICA\Record\Situation_problem\LiftUp Effect.tmp";
            tembro.Tembro(SampleRate, PathFile);
            pitchVal = 0;
            reverbVal = 250;
        }

        private void Feeling_in_the_body_pattern()
        {
            TembroClass tembro = new TembroClass();
            string PathFile = @"HARMONICA\Record\Feeling_in_the_body\Wide_voiceMan.tmp";
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
            int i = 0;
            while(i < 180)
            {
                pitchVal += 0.0083f;
                SetPitchShiftValue();
                i++;
                Thread.Sleep(1000);
            }
        }

        private void PitchTimerFeelInTheBody()
        {
            int i = 0;
            while (i < 180)
            {
                pitchVal -= 0.014f;
                SetPitchShiftValue();
                i++;
                Thread.Sleep(1000);
            }
        }

        private async void Feeling_in_the_body()
        {
            try
            {
                Feeling_in_the_body_pattern();
                Stop();
                //Thread.Sleep(2000);
                btnSituation_problem.IsEnabled = false;
                btnFeeling_in_the_body.IsEnabled = false;
                Filename = @"HARMONICA\Record\Feeling_in_the_body\HintFeelingInTheBody.wav";
                Sound(Filename);
                await Task.Run(() => Timer30());

                Filename = @"HARMONICA\Record\Feeling_in_the_body\StepOneAndTwoFeelingInTheBody.wav";
                Sound(Filename);
                await Task.Run(() => Timer40());

                //Здесь должно быть что-то типо включения микрофона!!!!!!! А у нас будет что-то типо записи
                WinTime();
                await Task.Run(() => TimerRec());
                Recording1();

                await Task.Delay(7000);
                //Thread.Sleep(5000);

                Filename = @"HARMONICA\Record\Feeling_in_the_body\StepThreeFeelingInTheBody.wav";
                Sound(Filename);
                await Task.Run(() => Timer30());

                Filename = @"HARMONICA\Record\Feeling_in_the_body\StepFourFeelingInTheBody.wav";
                Sound(Filename);
                await Task.Run(() => Timer40());

                //Здесь 3 минуты какой-то херни
                Stop();
                StartFullDuplex();
                await Task.Run(() => PitchTimerFeelInTheBody());
                //await Task.Delay(180000);
                Stop();

                Filename = @"HARMONICA\Record\Feeling_in_the_body\StepFiveFeelingInTheBody.wav";
                Sound(Filename);
                await Task.Run(() => Timer30());

                Filename = @"HARMONICA\Record\Feeling_in_the_body\RepeatRecord.wav";
                Sound(Filename);
                await Task.Delay(12000);

                WinTime();
                await Task.Run(() => TimerRec());
                Recording2();

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

        private async void Situation_problem()
        {
            try
            {
                Situation_Problem_pattern();
                Stop();
                btnSituation_problem.IsEnabled = false;
                btnFeeling_in_the_body.IsEnabled = false;
                Filename = @"HARMONICA\Record\Situation_problem\HintSituationProblem.wav";
                Sound(Filename);
                await Task.Run(() => Timer30());

                Filename = @"HARMONICA\Record\Situation_problem\StepOneSituationProblem.wav";
                Sound(Filename);
                await Task.Delay(24000);

                Filename = @"HARMONICA\Record\Situation_problem\StepTwoSituationProblem.wav";
                Sound(Filename);
                await Task.Delay(17000);

                WinTime();
                await Task.Run(() => TimerRec());
                Recording1();

                Filename = @"HARMONICA\Record\Situation_problem\AfterStepTwoSituationProblem.wav";
                Sound(Filename);
                await Task.Delay(7000);

                Filename = @"HARMONICA\Record\Situation_problem\StepThreeSituationProblem.wav";
                Sound(Filename);
                await Task.Delay(46000);

                Filename = @"HARMONICA\Record\Situation_problem\StepFourSituationProblem.wav";
                Sound(Filename);
                await Task.Delay(39000);

                Stop();
                StartFullDuplex();
                await Task.Run(() => PitchTimerSitProb());
                Stop();

                Filename = @"HARMONICA\Record\Situation_problem\StepFiveSituationProblem.wav";
                Sound(Filename);
                await Task.Delay(24000);

                Filename = @"HARMONICA\Record\Feeling_in_the_body\RepeatRecord.wav";
                Sound(Filename);
                await Task.Delay(12000);

                WinTime();
                await Task.Run(() => TimerRec());
                Recording2();
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

        private void Stop()
        {
            try
            {
                if (mMixer != null)
                {
                    mMixer.Dispose();
                    mMp3.ToWaveSource(32).Loop().ToSampleSource().Dispose();
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
            Environment.Exit(0);
        }

        private void btnFeeling_in_the_body_Click(object sender, RoutedEventArgs e)
        {
            Feeling_in_the_body();
        }

        private async void StartFullDuplex()//запуск пича и громкости
        {
            try
            {
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

        private async void Recording1()
        {
            try
            {
                //StreamReader FileRecord = new StreamReader("Data_Create.tmp");
                //StreamReader FileCutRecord = new StreamReader("Data_cutCreate.tmp");
                //myfile = FileRecord.ReadToEnd();
                //cutmyfile = FileCutRecord.ReadToEnd();
                //NFTRecordClick = 1;
                myfile = "MyRecord1.wav";
                cutmyfile = "cutMyRecord1.wav";
                fileDeleteRec1 = myfile;
                fileDeleteCutRec1 = cutmyfile;
                //FileRecord.Close();
                //FileCutRecord.Close();
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
                                string uri1 = @"HARMONICA\Progressbar\Group 13.png";
                                ImgPBRecordBack.ImageSource = new ImageSourceConverter().ConvertFromString(uri1) as ImageSource;
                            }
                            else if (pbRecord.Value == 50)
                            {
                                string uri2 = @"HARMONICA\Progressbar\Group 12.png";
                                ImgPBRecordBack.ImageSource = new ImageSourceConverter().ConvertFromString(uri2) as ImageSource;
                            }
                            else if (pbRecord.Value == 75)
                            {
                                string uri3 = @"HARMONICA\Progressbar\Group 11.png";
                                ImgPBRecordBack.ImageSource = new ImageSourceConverter().ConvertFromString(uri3) as ImageSource;
                            }
                            else if (pbRecord.Value == 95)
                            {
                                string uri4 = @"HARMONICA\Progressbar\Group 10.png";
                                ImgPBRecordBack.ImageSource = new ImageSourceConverter().ConvertFromString(uri4) as ImageSource;
                            }
                        }
                        //Thread.Sleep(5000);

                        mSoundIn.Stop();
                        lbRecordPB.Visibility = Visibility.Hidden;
                        pbRecord.Value = 0;
                        pbRecord.Visibility = Visibility.Hidden;

                    }
                    Thread.Sleep(100);
                    string uri = @"HARMONICA\Progressbar\progressbar-backgrnd.png";
                    ImgPBRecordBack.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
                    int[] Rdat = new int[150000];
                    int Ndt;
                    Ndt = vizualzvuk(cutmyfile, myfile, Rdat, 1);
                    NFT_drawing1(myfile);
                    //File.Move(myfile, @"Record\" + myfile);
                    //CutRecord cutRecord = new CutRecord();
                    //cutRecord.CutFromWave(cutmyfile, myfile, start, end);

                }
                if (langindex == "0")
                {
                    //ImgBtnRecordClick = 0;
                    //string uri = @"Neurotuners\button\button-record-inactive.png";
                    //ImgRecordingBtn.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
                    string msg = "Запись и обработка завершена. Сейчас появится графическое изображение вашего голоса.";
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    //btnPlayerEffect.Opacity = 1;
                    //WinSkip skip = new WinSkip();
                    //skip.ShowDialog();
                }
                else
                {
                    //string uri = @"Neurotuners\button\button-record-inactive.png";
                    //ImgRecordingBtn.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
                    //btnPlayer.IsEnabled = true;
                    string msg = "Recording and processing completed. A graphic representation of your voice will now appear.";
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    //btnPlayerEffect.Opacity = 1;
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
            Situation_problem();
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
                                string uri1 = @"HARMONICA\Progressbar\Group 13.png";
                                ImgPBRecordBack.ImageSource = new ImageSourceConverter().ConvertFromString(uri1) as ImageSource;
                            }
                            else if (pbRecord.Value == 50)
                            {
                                string uri2 = @"HARMONICA\Progressbar\Group 12.png";
                                ImgPBRecordBack.ImageSource = new ImageSourceConverter().ConvertFromString(uri2) as ImageSource;
                            }
                            else if (pbRecord.Value == 75)
                            {
                                string uri3 = @"HARMONICA\Progressbar\Group 11.png";
                                ImgPBRecordBack.ImageSource = new ImageSourceConverter().ConvertFromString(uri3) as ImageSource;
                            }
                            else if (pbRecord.Value == 95)
                            {
                                string uri4 = @"HARMONICA\Progressbar\Group 10.png";
                                ImgPBRecordBack.ImageSource = new ImageSourceConverter().ConvertFromString(uri4) as ImageSource;
                            }
                        }
                        //Thread.Sleep(5000);

                        mSoundIn.Stop();
                        lbRecordPB.Visibility = Visibility.Hidden;
                        pbRecord.Value = 0;
                        pbRecord.Visibility = Visibility.Hidden;

                    }
                    Thread.Sleep(100);
                    string uri = @"Neurotuners\element\progressbar-backgrnd1.png";
                    ImgPBRecordBack.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
                    int[] Rdat = new int[150000];
                    int Ndt;
                    Ndt = vizualzvuk(cutmyfile, myfile, Rdat, 1);
                    NFT_drawing2(myfile);

                }
                if (langindex == "0")
                {
                    string msg = "Запись и обработка завершена. Сейчас появится графическое изображение вашего голоса. Сейчас вы можете нажав на картинку прослушать свою запись. Либо начать новую сессию нажав на кнопку записи.";
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                }
                else
                {
                    string msg = "Recording and processing completed. A graphic representation of your voice will now appear. Now you can click on the picture to listen to your recording. Or start a new session by clicking on the record button.";
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
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
                else
                {

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


                    Filename = @"HARMONICA\Record\Start.wav";
                    //btnFeeling_in_the_body.IsEnabled = false;
                    //btnSituation_problem.IsEnabled = false;
                    Sound(Filename);
                    await Task.Run(() => Timer30());
                    Stop();
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
