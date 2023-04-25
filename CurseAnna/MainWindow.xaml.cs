using CurseAnna.Classes;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace CurseAnna
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Settings Settings1;

        private long Balance;
        static string[] Horses = { "Красная","Синяя","Зеленая","Фиолетовая","Желтая" };
        Border[] BHorses;
        Controls.PBRoad[] PBHorses;
        Label[] LHorses;
        Game[] Games;

        private MediaPlayer Player = new MediaPlayer();// плеер

        private uint TimerNum = 0;
        private DispatcherTimer Timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 1) };
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (BHorses[TimerNum].Opacity == 0.5)
            {
                BHorses[TimerNum].Opacity = 1;
                PBHorses[TimerNum].Opacity = 1;
            }
            else
            {
                BHorses[TimerNum].Opacity = 0.5;
                PBHorses[TimerNum].Opacity = 0.5;
            }
        }


        public MainWindow()
        {
            InitializeComponent();
            Timer.Tick += Timer_Tick;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Settings1 = new Settings();

            Balance = Settings1.Values.Balance;
            TBBalance.Text = Balance.ToString();//вкладка настройки
            TBSetStartBalance.Text = Balance.ToString();
            SVolume.Value = Settings1.Values.Volume;

            BHorses = new Border[5] { BHorseRed, BHorseBlue, BHorseGreen, BHorsePurple, BHorseYellow };
            PBHorses = new Controls.PBRoad[5] { PBHorseRed, PBHorseBlue, PBHorseGreen, PBHorsePurple, PBHorseYellow };
            LHorses = new Label[5] { LHorseRed, LHorseBlue, LHorseGreen, LHorsePurple, LHorseYellow };
            for (int i = 0; i < PBHorses.Length; i++)
                PBHorses[i].SetColor(BHorses[i].Background);

            ChangeCountHorse(Settings1.Values.HorseCount);

            foreach (ComboBoxItem i in CBSetHorseCount.Items)// (настройки
            {
                if(Convert.ToUInt16(i.Content) == Settings1.Values.HorseCount)
                {
                    CBSetHorseCount.SelectedItem = i;
                    break;
                }
            }

            foreach (ComboBoxItem i in CBSetMinSpeed.Items)
            {
                if (Convert.ToUInt16(i.Content) == Settings1.Values.MinSpeed)
                {
                    CBSetMinSpeed.SelectedItem = i;
                    break;
                }
            }

            foreach (ComboBoxItem i in CBSetMaxSpeed.Items)
            {
                if (Convert.ToUInt16(i.Content) == Settings1.Values.MaxSpeed)
                {
                    CBSetMaxSpeed.SelectedItem = i;
                    break;
                }
            }// ) настройки                                                  


            CBBet.Items.Clear();
            LBSetBets.Items.Clear();//настройки
            foreach (int n in Settings1.Values.Bets)
            {
                CBBet.Items.Add(n);
                
                TextBox tb = new TextBox()//настройки ( 
                {
                    MinWidth = 200,
                    Text = n.ToString()
                };
                tb.PreviewTextInput += TBSetStartBalance_PreviewTextInput;
                LBSetBets.Items.Add(tb);// ) заполнение листбокса ставок
            }
            CBBet.SelectedIndex = 0;
        }


        public void ChangeCountHorse(int count)
        {
            CBHorse.Items.Clear();

            for (int i = 0;  i < PBHorses.Length; i++)
            {
                if (i < count)
                {
                    CBHorse.Items.Add(Horses[i]);
                    BHorses[i].Visibility = Visibility.Visible;
                    PBHorses[i].Visibility = Visibility.Visible;
                    LHorses[i].Visibility = Visibility.Visible;
                }
                else
                {
                    BHorses[i].Visibility = Visibility.Collapsed;
                    PBHorses[i].Visibility = Visibility.Collapsed;
                    LHorses[i].Visibility = Visibility.Collapsed;
                }
            }
            CBHorse.SelectedIndex = 0;
        }

        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt16(BtnPlay.Tag) == 0)
            {
                Games = new Game[Settings1.Values.HorseCount];
                for (int i = 0; i < Games.Length; i++)
                {
                    if (i == CBHorse.SelectedIndex)
                        TimerNum = (UInt16)i;
                    Games[i] = new Game(PBHorses[i], LHorses[i], Settings1.Values.MinSpeed, Settings1.Values.MaxSpeed, i);
                    Games[i].EndGame += EndGame;//подписываемся на события завершения 
                }

                if (File.Exists(Environment.CurrentDirectory + @"\Sound.mp3"))//если есть звуковой файл - он будет воспроизведен
                {
                    Player.Open(new Uri(Environment.CurrentDirectory + @"\Sound.mp3"));
                    Player.Play();
                }
                BtnPlay.Tag = 1;
                Timer.Start();
                LHorse.Content = " -Победитель- ";
                BtnPlay.Content = "Покинуть трибуны";
                BtnPlay.ToolTip = "Автоматическое поражение!";
                CBBet.IsEnabled = false;
                CBHorse.IsEnabled = false;
                TISettings.IsEnabled = false;
                BtnSave.IsEnabled = false;
            }
            else if (Convert.ToInt16(BtnPlay.Tag) == 1)
            {
                foreach (var it in Games)
                {
                    it.CancelAsync();
                    it.EndGame -= EndGame;
                }
                Games = null;

                Player.Close();
                BtnPlay.Tag = 0;
                Timer.Stop();
                BHorses[TimerNum].Opacity = 1;
                PBHorses[TimerNum].Opacity = 1;
                BtnPlay.Content = "Играть";
                BtnPlay.ToolTip = null;
                CBBet.IsEnabled = true;
                CBHorse.IsEnabled = true; 
                TISettings.IsEnabled = true;
                BtnSave.IsEnabled = true;

                new Controls.DialogWindow("Вы проиграли!\n-" + Convert.ToInt32(CBBet.SelectedItem), 0).ShowDialog();
                Balance -= Convert.ToInt32(CBBet.SelectedItem);
                TBBalance.Text = Balance.ToString();
            }
        }

        private void EndGame(int num)
        {

            int busyCount = 0;
            bool AllEnd = true;
            foreach (var it in Games) 
            {
                if (it.IsBusy())//занят ли поток
                {
                    busyCount++;
                    AllEnd = false;
                }
                else it.EndGame -= EndGame;//отписываемся от события чтобы класс спокойно можно было высвободить из памяти
            }

            if(Games.Length -1 == busyCount)
            {
                LHorse.Content = Horses[num] + " " + LHorse.Content;
                if (num == CBHorse.SelectedIndex)
                {
                    new Controls.DialogWindow("Вы выиграли!\n+" + Convert.ToInt32(CBBet.SelectedItem), 0).ShowDialog();
                    Balance += Convert.ToInt32(CBBet.SelectedItem);
                }
                else
                {
                    new Controls.DialogWindow("Вы проиграли!\n-" + Convert.ToInt32(CBBet.SelectedItem), 0).ShowDialog();
                    Balance -= Convert.ToInt32(CBBet.SelectedItem);
                }
                TBBalance.Text = Balance.ToString();
            }

            if(AllEnd)
            {
                BtnPlay.Tag = 0;
                BtnPlay.Content = "Играть";
                Timer.Stop();
                BHorses[TimerNum].Opacity = 1;
                PBHorses[TimerNum].Opacity = 1;
                BtnPlay.ToolTip = null;
                CBBet.IsEnabled = true;
                CBHorse.IsEnabled = true;
                TISettings.IsEnabled = true;
                BtnSave.IsEnabled = true;
                Player.Close();
                //Games = null;
            }

        }


        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (Games == null)
            {
                new Controls.DialogWindow("Нечего сохранять...", 0).ShowDialog();
                return;
            }
            //wpf класс для выбора директории сохранения
            SaveFileDialog sfd = new SaveFileDialog()
            {
                AddExtension = true,
                DefaultExt = ".txt",
                CheckPathExists = true,
                CreatePrompt = false,
                OverwritePrompt = false,
                Title = "Выберите расположение файла",
                //RootFolder = Environment.SpecialFolder.MyDocuments,
            };
            if(sfd.ShowDialog() == true)
            {
                //если выбрать существующий файл - то допишет, а так создаст новый
                using (StreamWriter w = new StreamWriter(sfd.FileName, true))        
                {
                    w.WriteLine(DateTime.Now);
                    w.WriteLine(LHorse.Content);
                    foreach (Game game in Games)
                    {
                        Report r = game.GetReport();
                        w.WriteLine("".PadLeft(10 + 8 * r.Speed.Length, '-'));

                        string s = Horses[r.HorseNum].PadLeft(10);
                        for (int i = 0; i < r.Speed.Length; i++)
                            s+= ("Этап" + (i+1)).PadLeft(8);
                        w.WriteLine(s);

                        s = "дистанция".PadLeft(10);
                        foreach (int n in r.Traveled)
                            s += n.ToString().PadLeft(8);
                        w.WriteLine(s);

                        s = "скорость".PadLeft(10);
                        foreach (int n in r.Speed)
                            s += n.ToString().PadLeft(8);
                        w.WriteLine(s);
                    }
                    w.WriteLine("\n");
                }
            }
        }



        //событие возникает перед присвоением значения для textbox
        private void TBBalance_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            try
            { Convert.ToInt32(e.Text); }
            catch { e.Handled = true; }//если пытаются ввести не числа - удерживаем цепочку событий
        }
        //событие изменения(присвоения) текста
        private void TBBalance_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TBBalance.Text == "")
                Balance = 0;
            else
            {
                //защита от перполнения
                try { Balance = Convert.ToInt32(TBBalance.Text); }
                catch { }
            }
            TBBalance.Text = Balance.ToString();
        }


        //закрытие окна - пасхалка
        private void Window_Closed(object sender, EventArgs e)
        {
            if (Convert.ToInt32(TBBalance.Text) < 0)
                new Controls.DialogWindow("Эй, ты нам денег должен!", 0).ShowDialog();
        }




        //НАСТРОЙКИ
        private void SVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SVolume.SelectionEnd = SVolume.Value;
            Player.Volume = SVolume.Value/100;
        }

        private void LBDelItem_Click(object sender, RoutedEventArgs e)
        {
            List<TextBox> list = new List<TextBox>(LBSetBets.SelectedItems.Count);
            foreach (TextBox item in LBSetBets.SelectedItems)
                list.Add(item);

            foreach (var item in list)
            {
                item.PreviewTextInput -= TBSetStartBalance_PreviewTextInput;
                LBSetBets.Items.Remove(item);
            }
        }

        private void LBClear_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in LBSetBets.Items)
                (item as TextBox).PreviewTextInput -= TBSetStartBalance_PreviewTextInput;
            LBSetBets.Items.Clear();
        }

        private void LBAdd_Click(object sender, RoutedEventArgs e)
        {
            TextBox tb = new TextBox()
            {
                MinWidth = 200,
            };
            tb.PreviewTextInput += TBSetStartBalance_PreviewTextInput;
            LBSetBets.Items.Add(tb);
        }

        private void TBSetStartBalance_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            try
            { Convert.ToInt32(e.Text); }
            catch { e.Handled = true; }
        }

        private void TBSetStartBalance_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TBSetStartBalance.Text == "")
                Settings1.Values.Balance = 0;
            else
            {
                try { Settings1.Values.Balance = Convert.ToInt32(TBSetStartBalance.Text); }
                catch { }
            }
            TBSetStartBalance.Text = Settings1.Values.Balance.ToString();
        }

        private void BtnSetDefault_Click(object sender, RoutedEventArgs e)
        {
            Settings1.SetDefault();
            if (!Settings1.Save())
                new Controls.DialogWindow("Не удалось сохранить O_o" +
                    "\nСкорее всего файл настроек используется другим процессом или был изменен." +
                    "\nРекомендуемые действия - удаление файла настроек", 0).ShowDialog();
            Window_Loaded(null, null);
        }

        private void BtnSetConfirm_Click(object sender, RoutedEventArgs e)
        {
            ushort horseCount = Convert.ToUInt16((CBSetHorseCount.SelectedItem as ComboBoxItem).Content);
            ushort maxSpeed = Convert.ToUInt16((CBSetMaxSpeed.SelectedItem as ComboBoxItem).Content);
            ushort minSpeed = Convert.ToUInt16((CBSetMinSpeed.SelectedItem as ComboBoxItem).Content);
            long balance = Convert.ToInt64(TBSetStartBalance.Text);
            List<uint> bets = new List<uint>(LBSetBets.Items.Count);

            if (minSpeed >= maxSpeed)
            {
                new Controls.DialogWindow("Скорости лошадей должны отличаться!" +
                    "\nПричем, минимальная скорость должна быть меньше максимальной", 0).ShowDialog();

                return;
            }

            try
            {
                foreach (TextBox item in LBSetBets.Items)
                {
                    uint val;
                    if (item.Text == "")
                        val = 0;
                    else val = Convert.ToUInt32(item.Text);

                    if (!bets.Contains(val))
                        bets.Add(val);
                }
                bets.Sort();
            }
            catch
            {
                new Controls.DialogWindow("Воу, Воу, Воу -" +
                    "\nСтавки слишком высоки!", 0).ShowDialog();
                return;
            }

            Settings1.Values.Volume = Convert.ToUInt16(SVolume.Value);
            Settings1.Values.HorseCount = horseCount;
            Settings1.Values.MaxSpeed = maxSpeed;
            Settings1.Values.MinSpeed = minSpeed;
            Settings1.Values.Balance = balance;
            Settings1.Values.Bets = bets;
            if (!Settings1.Save())
                new Controls.DialogWindow("Не удалось сохранить O_o" +
                    "\nСкорее всего файл настроек используется другим процессом или был изменен." +
                    "\nРекомендуемые действия - удаление файла настроек", 0).ShowDialog();
            Window_Loaded(null, null);
        }


    }
}
