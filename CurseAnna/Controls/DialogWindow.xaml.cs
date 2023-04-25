using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace CurseAnna.Controls
{
    /// <summary>
    /// Логика взаимодействия для DialogWindow.xaml
    /// </summary>
    public partial class DialogWindow : Window
    {
        private string Message { get; set; }
        private string Header { get; set; }
        private int Mode { get; set; }

        private DispatcherTimer LabelTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 1), Tag = new TimeSpan(0, 0, 15) };
        private DispatcherTimer CloseTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 15) };

        /// <summary>
        /// Свой MessageBox. Принимает выводимый текст, текст заголовка, режим диалога, время отображения для спам окна.
        /// Режимы диалога: 0)ОК 1)Да-Нет 
        /// 2)Cпам окно - завершение по истечении времени 3)Спам окно - завершение кнопкой 
        /// 4-5)Спам окно соответсвенно 2 и 3 с отображаемым таймером
        /// </summary>
        public DialogWindow(string message, int mode)
        {
            InitializeComponent();
            Message = message;
            Mode = mode;
        }
        /// <summary>
        /// Свой MessageBox. Принимает выводимый текст, текст заголовка, режим диалога, время отображения для спам окна.
        /// Режимы диалога: 0)ОК 1)Да-Нет 
        /// 2)Cпам окно - завершение по истечении времени 3)Спам окно - завершение кнопкой 
        /// 4-5)Спам окно соответсвенно 2 и 3 с отображаемым таймером
        /// </summary>
        public DialogWindow(string message, string header, int mode)
        {
            InitializeComponent();
            Header = header;
            Message = message;
            Mode = mode;
        }
        /// <summary>
        /// Свой MessageBox. Принимает выводимый текст, текст заголовка, режим диалога, время отображения для спам окна.
        /// Режимы диалога: 0)ОК 1)Да-Нет 
        /// 2)Cпам окно - завершение по истечении времени 3)Спам окно - завершение кнопкой 
        /// 4-5)Спам окно соответсвенно 2 и 3 с отображаемым таймером
        /// </summary>
        public DialogWindow(string message, string header, int mode, TimeSpan time)
        {
            InitializeComponent();
            Header = header;
            Message = message;
            Mode = mode;
            CloseTimer.Interval = time;
            LabelTimer.Tag = time;
        }



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GBForm.Header = Header;
            TBMessage.Text = Message;

            BtnCollapce();
            switch (Mode)
            {
                case 0:
                    BtnOK.Visibility = Visibility.Visible;
                    break;
                case 1:
                    BtnYes.Visibility = Visibility.Visible;
                    BtnNo.Visibility = Visibility.Visible;
                    break;
                case 2:
                case 3:
                    CloseTimer.Tick += Timer_Tick;
                    CloseTimer.Start();
                    this.Topmost = true;
                    break;
                case 4:
                case 5:
                    CloseTimer.Tick += Timer_Tick;
                    LabelTimer.Tick += LabelTimer_Tick;
                    CloseTimer.Start();
                    LabelTimer.Start();
                    this.Topmost = true;
                    LTimer.Visibility = Visibility.Visible;
                    LTimer.Content = CloseTimer.Interval;
                    break;
            }
        }


        private void BtnCollapce()
        {
            BtnYes.Visibility = Visibility.Collapsed;
            BtnNo.Visibility = Visibility.Collapsed;
            BtnOK.Visibility = Visibility.Collapsed;
        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            LabelTimer.Stop();
            CloseTimer.Stop();
            List<int> UseBtnModes = new List<int>(2) { 3, 5 };

            if (UseBtnModes.Contains(Mode))
            {
                LTimer.Visibility = Visibility.Collapsed;
                BtnOK.Visibility = Visibility.Visible;
            }
            else this.Close();
        }

        private void LabelTimer_Tick(object sender, EventArgs e)
        {
            LabelTimer.Tag = ((TimeSpan)LabelTimer.Tag) - new TimeSpan(0, 0, 1);
            LTimer.Content = LabelTimer.Tag;
        }



        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void BtnYes_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        private void BtnNo_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
