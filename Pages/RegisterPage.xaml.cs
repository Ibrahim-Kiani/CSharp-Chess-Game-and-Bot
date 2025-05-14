using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ChessWPF.Data;
using ChessWPF.Pages;

namespace ChessWPF.Pages
{
    public partial class RegisterPage : Page
    {
        public RegisterPage() => InitializeComponent();

        private void OnRegister_Click(object sender, RoutedEventArgs e)
        {
            string u = UsernameBox.Text.Trim();
            string em = EmailBox.Text.Trim();
            string pw = PasswordBox.Password;

            // Validate Username 3–15 chars
            if (u.Length < 3 || u.Length > 15)
            {
                ErrorText.Text = "Username must be 3–15 characters.";
                return;
            }
            // Validate Email format
            if (!Regex.IsMatch(em,
                @"^[^@\s]{1,64}@[^@\s]+\.[^@\s]+$"))
            {
                ErrorText.Text = "Invalid email.";
                return;
            }
            // Validate Password 6–20 chars
            if (pw.Length < 6 || pw.Length > 20)
            {
                ErrorText.Text = "Password must be 6–20 characters.";
                return;
            }

            bool ok = DataAccess.RegisterUser(u, em, pw);
            if (!ok)
            {
                ErrorText.Text = "Username or email already taken.";
                return;
            }
            MessageBox.Show("Registration successful! Please log in.");
            // Navigate to LoginPage
            NavigationService.Navigate(new LoginPage());
        }
        private void OnGoToLogin_Click(object sender, MouseButtonEventArgs e)
        {
            // navigate back to the login page
            NavigationService?.Navigate(new LoginPage());
        }
    }
}
