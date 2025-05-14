using System;
using System.Collections.Generic;

namespace ChessWPF.Models
{
    public abstract class ChessPiece
    {
        public bool IsWhite { get; set; }
        public bool HasMoved { get; set; } = false; // for castling, pawn two-step, etc.
        public abstract string Symbol { get; }
        public abstract string ImageName { get; }  // for using images if desired
        public string GetImagePath()
        {
            // Ensure the folder name matches exactly as in your project.
            return $"Images/{ImageName}";
        }
        public ChessPiece(bool isWhite)
        {
            IsWhite = isWhite;
        }

        // Return all possible moves regardless of check state.
        // row and col are the piece’s current board coordinates.
        public abstract List<(int row, int col)> GetPossibleMoves(ChessPiece[,] board, int row, int col);

        // Helper: returns true if (r,c) is on board.
        protected bool IsOnBoard(int r, int c) => (r >= 0 && r < 8 && c >= 0 && c < 8);
    }

    public class Pawn : ChessPiece
    {
        public Pawn(bool isWhite) : base(isWhite) { }
        public override string Symbol => "P";
        public override string ImageName => IsWhite ? "WPawn.png" : "BPawn.png";

        public override List<(int row, int col)> GetPossibleMoves(ChessPiece[,] board, int row, int col)
        {
            var moves = new List<(int, int)>();
            int direction = IsWhite ? -1 : 1;
            int nextRow = row + direction;
            // Forward move if empty
            if (IsOnBoard(nextRow, col) && board[nextRow, col] == null)
            {
                moves.Add((nextRow, col));
                // Two-step if on starting row
                bool startingRow = (IsWhite && row == 6) || (!IsWhite && row == 1);
                if (startingRow && board[row + 2 * direction, col] == null)
                    moves.Add((row + 2 * direction, col));
            }
            // Captures
            foreach (int dcol in new int[] { -1, +1 })
            {
                int newCol = col + dcol;
                if (IsOnBoard(nextRow, newCol) && board[nextRow, newCol] != null &&
                    board[nextRow, newCol].IsWhite != IsWhite)
                    moves.Add((nextRow, newCol));
            }
            return moves;
        }
    }

    public class Rook : ChessPiece
    {
        public Rook(bool isWhite) : base(isWhite) { }
        public override string Symbol => "R";
        public override string ImageName => IsWhite ? "WRook.png" : "BRook.png";

        public override List<(int row, int col)> GetPossibleMoves(ChessPiece[,] board, int row, int col)
        {
            var moves = new List<(int, int)>();
            int[] dr = { -1, 1, 0, 0 };
            int[] dc = { 0, 0, -1, 1 };

            for (int i = 0; i < 4; i++)
            {
                int r = row + dr[i], c = col + dc[i];
                while (IsOnBoard(r, c))
                {
                    if (board[r, c] == null)
                    {
                        moves.Add((r, c));
                    }
                    else
                    {
                        if (board[r, c].IsWhite != IsWhite)
                            moves.Add((r, c));
                        break;
                    }
                    r += dr[i];
                    c += dc[i];
                }
            }
            return moves;
        }
    }

    public class Bishop : ChessPiece
    {
        public Bishop(bool isWhite) : base(isWhite) { }
        public override string Symbol => "B";
        public override string ImageName => IsWhite ? "WBishop.png" : "BBishop.png";

        public override List<(int row, int col)> GetPossibleMoves(ChessPiece[,] board, int row, int col)
        {
            var moves = new List<(int, int)>();
            int[] dr = { -1, -1, 1, 1 };
            int[] dc = { -1, 1, -1, 1 };
            for (int i = 0; i < 4; i++)
            {
                int r = row + dr[i], c = col + dc[i];
                while (IsOnBoard(r, c))
                {
                    if (board[r, c] == null)
                    {
                        moves.Add((r, c));
                    }
                    else
                    {
                        if (board[r, c].IsWhite != IsWhite)
                            moves.Add((r, c));
                        break;
                    }
                    r += dr[i];
                    c += dc[i];
                }
            }
            return moves;
        }
    }

    public class Knight : ChessPiece
    {
        public Knight(bool isWhite) : base(isWhite) { }
        public override string Symbol => "N";
        public override string ImageName => IsWhite ? "WKnight.png" : "BKnight.png";
        public override List<(int row, int col)> GetPossibleMoves(ChessPiece[,] board, int row, int col)
        {
            var moves = new List<(int, int)>();
            int[,] offsets = new int[,]
            {
                {-2,-1}, {-2,1}, {2,-1}, {2,1},
                {-1,-2}, {-1,2}, {1,-2}, {1,2}
            };

            for (int i = 0; i < 8; i++)
            {
                int r = row + offsets[i, 0], c = col + offsets[i, 1];
                if (IsOnBoard(r, c))
                {
                    if (board[r, c] == null || board[r, c].IsWhite != IsWhite)
                        moves.Add((r, c));
                }
            }
            return moves;
        }
    }

    public class Queen : ChessPiece
    {
        public Queen(bool isWhite) : base(isWhite) { }
        public override string Symbol => "Q";
        public override string ImageName => IsWhite ? "WQueen.png" : "BQueen.png";

        public override List<(int row, int col)> GetPossibleMoves(ChessPiece[,] board, int row, int col)
        {
            // Queen combines rook and bishop moves
            var moves = new List<(int, int)>();
            moves.AddRange(new Rook(IsWhite).GetPossibleMoves(board, row, col));
            moves.AddRange(new Bishop(IsWhite).GetPossibleMoves(board, row, col));
            return moves;
        }
    }

    public class King : ChessPiece
    {
        public King(bool isWhite) : base(isWhite) { }
        public override string Symbol => "K";
        public override string ImageName => IsWhite ? "WKing.png" : "BKing.png";

        public override List<(int row, int col)> GetPossibleMoves(ChessPiece[,] board, int row, int col)
        {
            var moves = new List<(int, int)>();
            for (int dr = -1; dr <= 1; dr++)
            {
                for (int dc = -1; dc <= 1; dc++)
                {
                    if (dr == 0 && dc == 0)
                        continue;
                    int r = row + dr, c = col + dc;
                    if (IsOnBoard(r, c))
                    {
                        if (board[r, c] == null || board[r, c].IsWhite != IsWhite)
                            moves.Add((r, c));
                    }
                }
            }
            // Castling (simplified):
            if (!HasMoved)
            {
                // King-side castling:
                if (cCastleClear(board, row, col, 1))
                    moves.Add((row, col + 2));
                // Queen-side castling:
                if (cCastleClear(board, row, col, -1))
                    moves.Add((row, col - 2));
            }
            return moves;
        }

        // Simplified castling check: Assumes the king is not in check and no square in between is attacked.
        private bool cCastleClear(ChessPiece[,] board, int row, int col, int direction)
        {
            int rookCol = (direction == 1) ? 7 : 0;
            ChessPiece rook = board[row, rookCol];
            if (rook is Rook && !rook.HasMoved)
            {
                // All squares between king and rook must be empty
                int step = direction;
                for (int c = col + step; c != rookCol; c += step)
                {
                    if (board[row, c] != null)
                        return false;
                }
                return true;
            }
            return false;
        }
    }
}
