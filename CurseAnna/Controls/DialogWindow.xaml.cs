﻿using System.Windows;
using System.Windows.Input;


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

        /// <summary>
        /// Свой MessageBox. Принимает выводимый текст, текст заголовка и режим диалога.
        /// Режимы диалога: 0)ОК 1) Да-Нет
        /// </summary>
        public DialogWindow(string message, int mode)
        {
            InitializeComponent();
            Message = message;
            Mode = mode;
        }
        /// <summary>
        /// Свой MessageBox. Принимает выводимый текст, текст заголовка и режим диалога.
        /// Режимы диалога: 0)ОК 1) Да-Нет
        /// </summary>
        public DialogWindow(string message, string header, int mode)
        {
            InitializeComponent();
            Header = header;
            Message = message;
            Mode = mode;
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
            }
        }


        private void BtnCollapce()
        {
            BtnYes.Visibility = Visibility.Collapsed; 
            BtnNo.Visibility = Visibility.Collapsed;
            BtnOK.Visibility = Visibility.Collapsed;
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
