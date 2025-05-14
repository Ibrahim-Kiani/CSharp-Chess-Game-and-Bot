using ChessWPF.Data;
using System;
using System.Windows;
using System.Windows.Controls;

using ChessWPF;               // for MainWindow

namespace ChessWPF.Pages
{
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void OnLogin_Click(object sender, RoutedEventArgs e)
        {
            string em = EmailBox.Text.Trim();
            string pw = PasswordBox.Password;

            if (!DataAccess.AuthenticateUser(em, pw))
            {
                ErrorText.Text = "Invalid credentials.";
                return;
            }

            // get the running MainWindow
            var main = Application.Current.MainWindow as MainWindow;
            if (main == null) return;

            // Navigate, supplying the new 6‐parameter delegate
            NavigationService.Navigate(
                new MainMenuPage(main.OnStartGame)
            );
        }
        private void OnGoToRegister_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegisterPage());
        }

    }
}
