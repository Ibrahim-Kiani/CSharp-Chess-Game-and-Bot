using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using ChessWPF.Engine;
using ChessWPF.Pages;
using ChessWPF.Windows; // For SettingsWindow

namespace ChessWPF
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string player1Name;
        private string player2Name;
        private string selectedTimeFormat;
        private TimeSpan player1Time;
        private TimeSpan player2Time;
        private DispatcherTimer gameTimer;
        private bool isWhiteTurn;

        public string Player1Name { get => player1Name; set { player1Name = value; OnPropertyChanged(); } }
        public string Player2Name { get => player2Name; set { player2Name = value; OnPropertyChanged(); } }
        public string SelectedTimeFormat { get => selectedTimeFormat; set { selectedTimeFormat = value; OnPropertyChanged(); } }
        public string Player1Time => player1Time.ToString(@"mm\:ss");
        public string Player2Time => player2Time.ToString(@"mm\:ss");

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            // Initially, show the MainMenuPage (which collects player info)
            MainFrame.Navigated += OnFrameNavigated;
            MainFrame.Navigate(new Pages.LoginPage());
        }
        private void OnFrameNavigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            // Hide sidebar when on Login, Register, or MainMenu
            bool hide = e.Content is Pages.LoginPage
                     || e.Content is Pages.RegisterPage
                     || e.Content is Pages.MainMenuPage;

            SidebarPanel.Visibility = hide
                ? Visibility.Collapsed
                : Visibility.Visible;
        }
        // Callback from MainMenuPage when the game starts.
        public void OnStartGame(string p1, string p2, TimeSpan selectedTime, bool vsBot, BotDifficulty level, bool humanIsWhite)
        {
            Player1Name = p1;
            Player2Name = p2;
            player1Time = selectedTime;
            player2Time = selectedTime;
            SelectedTimeFormat = selectedTime.TotalMinutes + " min";
            isWhiteTurn = true;
            StartTimer();

            // Navigate to the chessboard page with the provided info.
            // After collecting p1, p2, vsBot, level, humanIsWhite...
            // e.g. TimeSpan selectedTime = TimeSpan.FromMinutes(5);
            MainFrame.Navigate(new ChessBoardPage(
                p1,
                p2,
                selectedTime,      // ← your 3/5/10-min choice
                vsBot,
                level,
                humanIsWhite,
                OnTurnSwitch,
                OnGameEnd
            ));


        }

        private void StartTimer()
        {
            gameTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            gameTimer.Tick += (s, e) =>
            {
                if (isWhiteTurn)
                    player1Time = player1Time.Subtract(TimeSpan.FromSeconds(1));
                else
                    player2Time = player2Time.Subtract(TimeSpan.FromSeconds(1));

                OnPropertyChanged(nameof(Player1Time));
                OnPropertyChanged(nameof(Player2Time));

                if (player1Time.TotalSeconds <= 0)
                {
                    gameTimer.Stop();
                    MessageBox.Show($"{Player2Name} wins on time!");
                }
                else if (player2Time.TotalSeconds <= 0)
                {
                    gameTimer.Stop();
                    MessageBox.Show($"{Player1Name} wins on time!");
                }
            };
            gameTimer.Start();
        }

        private void OnTurnSwitch(bool whiteTurn)
        {
            isWhiteTurn = whiteTurn;
        }

        private void OnGameEnd(string message)
        {
            gameTimer?.Stop();
            MessageBox.Show(message);
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new MainMenuPage(OnStartGame));
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            // Open the modal SettingsWindow.
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.Owner = this;
            settingsWindow.ShowDialog();

            // After closing, refresh the current ChessBoardPage if active.
            if (MainFrame.Content is Pages.ChessBoardPage boardPage)
            {
                boardPage.RefreshBoard();
            }
        }
        private void GoLogin_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new LoginPage());
        }

        private void GoRegister_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new RegisterPage());
        }

        private void GoGameSelect_Click(object sender, RoutedEventArgs e)
        {
            // Assuming you still want to pass your OnStartGame callback in here:
            MainFrame.Navigate(new MainMenuPage(OnStartGame));
        }

        
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
