using ChessWPF.Engine;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ChessWPF.Pages
{
    public partial class MainMenuPage : Page
    {
        // This callback will be called when the game is ready to start.
        private Action<string, string, TimeSpan, bool, BotDifficulty, bool> startGameCallback;

        public MainMenuPage(Action<string, string, TimeSpan, bool, BotDifficulty, bool> callback)
        {
            InitializeComponent();
            startGameCallback = callback;
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            string p1 = Player1Box.Text.Trim();
            bool vsBot = BotRadio.IsChecked == true;
            bool humanIsWhite = vsBot? (WhiteRadio.IsChecked == true): true;
            string p2 = vsBot ? "Computer" : Player2Box.Text.Trim();

            if (p1.Length < 3 || p1.Length > 15)
            {
                MessageBox.Show("Player1 name must be 3–15 chars"); return;
            }
            if (!vsBot && (p2.Length < 3 || p2.Length > 15))
            {
                MessageBox.Show("Player2 name must be 3–15 chars"); return;
            }

            // time
            if (TimeBox.SelectedIndex < 0)
            {
                MessageBox.Show("Choose time"); return;
            }
            TimeSpan time = TimeSpan
              .FromMinutes(new[] { 3, 5, 10 }[TimeBox.SelectedIndex]);

            // bot difficulty
            BotDifficulty level = BotDifficulty.Easy;
            if (vsBot)
            {
                if (DifficultyBox.SelectedIndex < 0)
                {
                    MessageBox.Show("Choose difficulty"); return;
                }
                level = DifficultyBox.SelectedIndex switch
                {
                    0 => BotDifficulty.Easy,
                    1 => BotDifficulty.Medium,
                    _ => BotDifficulty.Hard
                };
            }
            

            // invoke the new callback signature:
            startGameCallback(
              p1, p2, time,
              vsBot, level,
              humanIsWhite
            );
        }

    }
}
