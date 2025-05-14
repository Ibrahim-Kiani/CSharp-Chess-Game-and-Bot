using System.Windows;

namespace ChessWPF.Windows
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void ClassicWood_Click(object sender, RoutedEventArgs e)
        {
            ((App)Application.Current).CurrentTheme = BoardTheme.ClassicWood;
            MessageBox.Show("Theme changed to Classic Wood!");
        }

        private void Gray_Click(object sender, RoutedEventArgs e)
        {
            ((App)Application.Current).CurrentTheme = BoardTheme.Gray;
            MessageBox.Show("Theme changed to Gray!");
        }

        private void Green_Click(object sender, RoutedEventArgs e)
        {
            ((App)Application.Current).CurrentTheme = BoardTheme.Green;
            MessageBox.Show("Theme changed to Green!");
        }

        private void Blue_Click(object sender, RoutedEventArgs e)
        {
            ((App)Application.Current).CurrentTheme = BoardTheme.Blue;
            MessageBox.Show("Theme changed to Blue!");
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
