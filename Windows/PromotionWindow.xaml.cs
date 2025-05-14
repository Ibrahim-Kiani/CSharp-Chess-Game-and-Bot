using System.Windows;
using ChessWPF.Models;

namespace ChessWPF.Windows
{
    public partial class PromotionWindow : Window
    {
        public ChessPiece PromotedPiece { get; private set; }
        private readonly bool isWhite;
        public PromotionWindow(bool pawnIsWhite)
        {
            InitializeComponent();
            isWhite = pawnIsWhite;
        }

        private void Queen_Click(object s, RoutedEventArgs e)
           => Promote(new Queen(isWhite));

        private void Rook_Click(object s, RoutedEventArgs e)
           => Promote(new Rook(isWhite));

        private void Bishop_Click(object s, RoutedEventArgs e)
           => Promote(new Bishop(isWhite));

        private void Knight_Click(object s, RoutedEventArgs e)
           => Promote(new Knight(isWhite));

        private void Promote(ChessPiece piece)
        {
            piece.HasMoved = true;
            PromotedPiece = piece;
            DialogResult = true;
        }
    }
}
