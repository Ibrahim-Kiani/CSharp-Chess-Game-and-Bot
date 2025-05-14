using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ChessWPF.Engine;
using ChessWPF.Models;
using System.Threading.Tasks;
using ChessWPF.Models;    // for Queen if you auto-promote
using ChessWPF.Windows;
using System.Windows.Threading;

namespace ChessWPF.Pages
{
    public partial class ChessBoardPage : Page
    {
        private readonly string player1Name;
        private readonly string player2Name;
        private readonly Action<bool> onTurnSwitch;
        private readonly Action<string> onGameEnd;
        private GameEngine game;
        private (int row, int col)? selectedPos = null;
        private List<(int row, int col)> highlightedMoves = new List<(int, int)>();
        private readonly bool vsBot;
        private readonly BotDifficulty botLevel;
        private readonly DispatcherTimer _timer;
        private TimeSpan _whiteTime;
        private TimeSpan _blackTime;

        private readonly bool humanIsWhite;

        public ChessBoardPage(
        string p1, string p2,
        TimeSpan initialTime,
        bool vsBot, BotDifficulty level,
        bool humanIsWhite,
        
        Action<bool> turnSwitch,
        Action<string> gameEnd)
        {
            InitializeComponent();
            this.vsBot = vsBot;
            this.botLevel = level;
            this.humanIsWhite = humanIsWhite;
            this.onTurnSwitch = turnSwitch;
            this.onGameEnd = gameEnd;
            _whiteTime = initialTime;
            _blackTime = initialTime;
            UpdateWhiteTimer(_whiteTime);
            UpdateBlackTimer(_blackTime);
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += Timer_Tick;
            _timer.Start();
            game = new GameEngine();

            // draw initial board
            DrawBoard();

            // if human picked Black and vsBot, let bot (White) play first
            _ = Dispatcher.BeginInvoke(new Action(BotMoveIfNeeded));
        }
        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            game.UndoLastMove();   // implement this in your GameEngine
            if (vsBot)
            {
                game.UndoLastMove();
                game.WhiteTurn = !game.WhiteTurn;
                onTurnSwitch?.Invoke(game.WhiteTurn);
            }
                
            game.WhiteTurn = !game.WhiteTurn;
            onTurnSwitch?.Invoke(game.WhiteTurn);
            DrawBoard();
            DrawCaptured();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            // Who’s turn? subtract 1 second from that player’s clock.
            if (game.WhiteTurn)
            {
                _whiteTime = _whiteTime.Subtract(TimeSpan.FromSeconds(1));
                UpdateWhiteTimer(_whiteTime);

                if (_whiteTime <= TimeSpan.Zero)
                {
                    _timer.Stop();
                    onGameEnd?.Invoke($"{player2Name} wins on time!");
                }
            }
            else
            {
                _blackTime = _blackTime.Subtract(TimeSpan.FromSeconds(1));
                UpdateBlackTimer(_blackTime);

                if (_blackTime <= TimeSpan.Zero)
                {
                    _timer.Stop();
                    onGameEnd?.Invoke($"{player1Name} wins on time!");
                }
            }
        }
        private void DrawCaptured()
        {
            CapturedWhitePanel.Children.Clear();
            CapturedBlackPanel.Children.Clear();
            foreach (var p in game.CapturedPieces)
            {
                var img = new Image
                {
                    Source = new BitmapImage(new Uri(p.GetImagePath(), UriKind.Relative)),
                    Width = 64,
                    Height = 64,
                    Margin = new Thickness(2)
                };
                if (p.IsWhite)
                    CapturedWhitePanel.Children.Add(img);
                else
                    CapturedBlackPanel.Children.Add(img);
            }
        }
        private async void BotMoveIfNeeded()
        {
            // only run when playing vs bot AND it's bot's turn
            if (!vsBot || game.WhiteTurn == humanIsWhite)
                return;

            // get the bot's best move off the UI thread
            var move = await Task.Run(() =>
                ChessWPF.Engine.ChessBot.GetBestMove(game, botLevel)
            );

            // apply it
            bool ok = game.MovePiece(
                (move.FromRow, move.FromCol),
                (move.ToRow, move.ToCol)
            );
            if (ok)
            {
                // auto‐promote to queen or handle via UI if you prefer
                if (game.IsPawnPromotionPosition((move.ToRow, move.ToCol)))
                {
                    game.PromotePawn(
                        move.ToRow, move.ToCol,
                        new Queen(!humanIsWhite) { HasMoved = true }
                    );
                }

                DrawBoard();
                onTurnSwitch?.Invoke(game.WhiteTurn);

                // check for endgame
                if (game.IsCheckmate())
                    onGameEnd?.Invoke("Checkmate!");
                else if (game.IsStalemate())
                    onGameEnd?.Invoke("Stalemate!");
            }
        }

        public void RefreshBoard()
        {
            DrawBoard();
        }

        private void DrawBoard()
        {
            ChessBoardGrid.Children.Clear();

            // Get the current theme colors from App
            var currentTheme = ((App)Application.Current).CurrentTheme;
            Brush lightSquare, darkSquare;
            switch (currentTheme)
            {
                case BoardTheme.ClassicWood:
                    lightSquare = Brushes.BurlyWood;
                    darkSquare = Brushes.SaddleBrown;
                    break;
                case BoardTheme.Gray:
                    lightSquare = Brushes.WhiteSmoke;
                    darkSquare = Brushes.Gray;
                    break;
                case BoardTheme.Green:
                    lightSquare = Brushes.Beige;
                    darkSquare = Brushes.Green;
                    break;
                case BoardTheme.Blue:
                    lightSquare = Brushes.LightBlue;
                    darkSquare = Brushes.DarkBlue;
                    break;
                default:
                    lightSquare = Brushes.White;
                    darkSquare = Brushes.Gray;
                    break;
            }

            bool light = true;
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    Border cell = new Border
                    {
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(1),
                        Tag = (row: r, col: c)
                    };

                    Brush cellBg = light ? lightSquare : darkSquare;
                    if (highlightedMoves.Contains((r, c)))
                    {
                        var piece = game.Board[r, c];
                        cellBg = (piece != null) ? Brushes.Red : Brushes.LightGreen;
                    }
                    cell.Background = cellBg;
                    cell.MouseLeftButtonDown += Cell_MouseLeftButtonDown;

                    ChessPiece pieceOnCell = game.Board[r, c];
                    if (pieceOnCell != null)
                    {
                        Image pieceImage = new Image
                        {
                            Width = 64,
                            Height = 64,
                            Stretch = Stretch.Uniform,
                            Source = new BitmapImage(new Uri(pieceOnCell.GetImagePath(), UriKind.Relative))
                        };
                        cell.Child = pieceImage;
                    }
                    ChessBoardGrid.Children.Add(cell);
                    light = !light;
                }
                light = !light;
            }
            DrawCaptured();
        }
        public void UpdateWhiteTimer(TimeSpan time)
        {
            Player1Timer.Text = time.ToString(@"mm\:ss");
        }

        public void UpdateBlackTimer(TimeSpan time)
        {
            Player2Timer.Text = time.ToString(@"mm\:ss");
        }
        private void Cell_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 1) Ignore all clicks on the bot’s turn
            if (vsBot && game.WhiteTurn != humanIsWhite)
                return;

            var clicked = (Border)sender;
            var pos = ((int row, int col))clicked.Tag;

            // 2) Select or move
            if (selectedPos == null)
            {
                TrySelect(pos);
            }
            else
            {
                var clickedPiece = game.Board[pos.row, pos.col];
                // if you clicked your own piece, switch selection
                if (clickedPiece != null && clickedPiece.IsWhite == game.WhiteTurn)
                {
                    TrySelect(pos);
                }
                else
                {
                    // attempt move
                    if (game.MovePiece(selectedPos.Value, pos))
                    {
                        // promotion?
                        if (game.IsPawnPromotionPosition(pos))
                        {
                            // popup UI or auto‐queen:
                            var win = new ChessWPF.Windows.PromotionWindow(
                                          game.Board[pos.row, pos.col].IsWhite)
                            { Owner = Application.Current.MainWindow };
                            if (win.ShowDialog() == true && win.PromotedPiece != null)
                                game.PromotePawn(pos.row, pos.col, win.PromotedPiece);
                        }

                        // clear selection/highlights
                        selectedPos = null;
                        highlightedMoves.Clear();

                        DrawBoard();
                        onTurnSwitch?.Invoke(game.WhiteTurn);

                        // endgame?
                        if (game.IsCheckmate())
                            onGameEnd?.Invoke("Checkmate!");
                        else if (game.IsStalemate())
                            onGameEnd?.Invoke("Stalemate!");

                        // 3) now let the bot move
                        _ = Dispatcher.BeginInvoke(new Action(BotMoveIfNeeded));
                    }
                    else
                    {
                        // invalid: clear
                        selectedPos = null;
                        highlightedMoves.Clear();
                        DrawBoard();
                    }
                }
            }
        }

        private void TrySelect((int row, int col) pos)
        {
            var piece = game.Board[pos.row, pos.col];
            if (piece != null
                && piece.IsWhite == game.WhiteTurn
                && (!vsBot || piece.IsWhite == humanIsWhite))
            {
                selectedPos = pos;
                highlightedMoves = piece.GetPossibleMoves(
                    game.Board, pos.row, pos.col);
                DrawBoard();
            }
        }

    }
}
