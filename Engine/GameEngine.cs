using System.Collections.Generic;
using ChessWPF.Models;
using Org.BouncyCastle.Asn1.X509;

namespace ChessWPF.Engine
{
    public class GameEngine
    {
        public ChessPiece[,] Board { get; private set; } = new ChessPiece[8, 8];
        public bool WhiteTurn { get; set; } = true;
        private readonly Stack<(ChessPiece moved, (int r, int c) from, ChessPiece captured, (int r, int c) to)> _moveHistory= new();
        public List<ChessPiece> CapturedPieces { get; } = new();
        public GameEngine()
        {
            SetupBoard();
        }

        public void SetupBoard()
        {
            // Clear board
            for (int r = 0; r < 8; r++)
                for (int c = 0; c < 8; c++)
                    Board[r, c] = null;

            // Place pieces in starting positions:
            // Rooks
            Board[0, 0] = new Rook(false); Board[0, 7] = new Rook(false);
            Board[7, 0] = new Rook(true); Board[7, 7] = new Rook(true);
            // Knights
            Board[0, 1] = new Knight(false); Board[0, 6] = new Knight(false);
            Board[7, 1] = new Knight(true); Board[7, 6] = new Knight(true);
            // Bishops
            Board[0, 2] = new Bishop(false); Board[0, 5] = new Bishop(false);
            Board[7, 2] = new Bishop(true); Board[7, 5] = new Bishop(true);
            // Queens
            Board[0, 3] = new Queen(false);
            Board[7, 3] = new Queen(true);
            // Kings
            Board[0, 4] = new King(false);
            Board[7, 4] = new King(true);
            // Pawns
            for (int c = 0; c < 8; c++)
            {
                Board[1, c] = new Pawn(false);
                Board[6, c] = new Pawn(true);
            }
        }

        // Attempt to move a piece; returns true if move was successful.
        // Also switches turn if move is legal.
        public bool MovePiece((int row, int col) from, (int row, int col) to)
        {
            ChessPiece piece = Board[from.row, from.col];
            if (piece == null || piece.IsWhite != WhiteTurn)
                return false;

            var possible = piece.GetPossibleMoves(Board, from.row, from.col);
            if (!possible.Contains(to))
                return false;
            
            // Save state for rollback
            ChessPiece captured = Board[to.row, to.col];
            _moveHistory.Push((piece, from, captured, to));
            if (captured != null)
                CapturedPieces.Add(captured);
            bool wasCastle = piece is King && Math.Abs(to.col - from.col) == 2;

            // 1) Perform the king move (or pawn move)
            Board[to.row, to.col] = piece;
            Board[from.row, from.col] = null;
            piece.HasMoved = true;

            // 2) Handle castling: move the rook as well
            if (wasCastle)
            {
                // king‐side
                if (to.col > from.col)
                {
                    var rook = Board[to.row, 7];
                    Board[to.row, 5] = rook;
                    Board[to.row, 7] = null;
                    rook.HasMoved = true;
                }
                else // queen‐side
                {
                    var rook = Board[to.row, 0];
                    Board[to.row, 3] = rook;
                    Board[to.row, 0] = null;
                    rook.HasMoved = true;
                }
            }

            // 3) Pawn promotion: if a pawn reaches the last rank, replace it with a queen
            

            // 4) Check if move leaves own king in check → rollback
            if (IsInCheck(WhiteTurn))
            {
                // revert everything
                Board[from.row, from.col] = piece;
                Board[to.row, to.col] = captured;
                piece.HasMoved = false;

                // revert rook for castling
                if (wasCastle)
                {
                    if (to.col > from.col)
                    {
                        var rook = Board[to.row, 5];
                        Board[to.row, 7] = rook;
                        Board[to.row, 5] = null;
                        rook.HasMoved = false;
                    }
                    else
                    {
                        var rook = Board[to.row, 3];
                        Board[to.row, 0] = rook;
                        Board[to.row, 3] = null;
                        rook.HasMoved = false;
                    }
                }

                return false;
            }

            // 5) Finally, switch turns
            WhiteTurn = !WhiteTurn;
            return true;
        }

        public void UndoLastMove()
        {
            if (_moveHistory.Count == 0) return;
            var (moved, from, captured, to) = _moveHistory.Pop();

            // Undo piece placement
            Board[from.r, from.c] = moved;
            Board[to.r, to.c] = captured;
            moved.HasMoved = false; // or track original state if needed

            // If there was a capture, remove it from captured list
            if (captured != null)
                CapturedPieces.Remove(captured);
        }
        // Returns true if the current side's king is in check.
        public bool IsInCheck(bool white)
        {
            (int kingRow, int kingCol) = FindKing(white);
            foreach (var move in GetAllMoves(!white))
            {
                if (move == (kingRow, kingCol))
                    return true;
            }
            return false;
        }

        // Returns list of all moves for a side.
        private List<(int row, int col)> GetAllMoves(bool white)
        {
            var moves = new List<(int, int)>();
            for (int r = 0; r < 8; r++)
                for (int c = 0; c < 8; c++)
                {
                    ChessPiece piece = Board[r, c];
                    if (piece != null && piece.IsWhite == white)
                        moves.AddRange(piece.GetPossibleMoves(Board, r, c));
                }
            return moves;
        }

        // Finds the king for the given side.
        private (int row, int col) FindKing(bool white)
        {
            for (int r = 0; r < 8; r++)
                for (int c = 0; c < 8; c++)
                {
                    if (Board[r, c] is King && Board[r, c].IsWhite == white)
                        return (r, c);
                }
            return (-1, -1);
        }

        // Check for checkmate: if the current side is in check and no legal moves are available.
        public bool IsCheckmate()
        {
            if (!IsInCheck(WhiteTurn))
                return false;

            // Try every move for current side; if one does not leave king in check, not mate.
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    ChessPiece piece = Board[r, c];
                    if (piece != null && piece.IsWhite == WhiteTurn)
                    {
                        var moves = piece.GetPossibleMoves(Board, r, c);
                        foreach (var move in moves)
                        {
                            // Make a copy of the board temporarily.
                            var copy = (ChessPiece[,])Board.Clone();
                            ChessPiece p = copy[r, c];
                            copy[move.row, move.col] = p;
                            copy[r, c] = null;
                            // If the king is not in check, then not mate.
                            if (!IsInCheckAfter(copy, WhiteTurn))
                                return false;
                        }
                    }
                }
            }
            return true;
        }

        // Similar to IsInCheck but on a given board copy.
        private bool IsInCheckAfter(ChessPiece[,] boardCopy, bool white)
        {
            (int kingRow, int kingCol) = FindKingOnBoard(boardCopy, white);
            for (int r = 0; r < 8; r++)
                for (int c = 0; c < 8; c++)
                {
                    ChessPiece piece = boardCopy[r, c];
                    if (piece != null && piece.IsWhite != white)
                    {
                        var moves = piece.GetPossibleMoves(boardCopy, r, c);
                        if (moves.Contains((kingRow, kingCol)))
                            return true;
                    }
                }
            return false;
        }

        private (int row, int col) FindKingOnBoard(ChessPiece[,] boardCopy, bool white)
        {
            for (int r = 0; r < 8; r++)
                for (int c = 0; c < 8; c++)
                {
                    if (boardCopy[r, c] is King && boardCopy[r, c].IsWhite == white)
                        return (r, c);
                }
            return (-1, -1);
        }

        // Check for stalemate: not in check but no legal moves.
        public bool IsStalemate()
        {
            if (IsInCheck(WhiteTurn))
                return false;
            for (int r = 0; r < 8; r++)
                for (int c = 0; c < 8; c++)
                {
                    ChessPiece piece = Board[r, c];
                    if (piece != null && piece.IsWhite == WhiteTurn)
                    {
                        var moves = piece.GetPossibleMoves(Board, r, c);
                        foreach (var move in moves)
                        {
                            var copy = (ChessPiece[,])Board.Clone();
                            ChessPiece p = copy[r, c];
                            copy[move.row, move.col] = p;
                            copy[r, c] = null;
                            if (!IsInCheckAfter(copy, WhiteTurn))
                                return false;
                        }
                    }
                }
            return true;
        }
        // At bottom of GameEngine class:
        /// <summary>
        /// Returns true if the last move landed a pawn on the back rank.
        /// Call this immediately after MovePiece.
        /// </summary>
        public bool IsPawnPromotionPosition((int row, int col) pos)
        {
            var p = Board[pos.row, pos.col];
            return p is Pawn && (pos.row == 0 || pos.row == 7);
        }

        /// <summary>
        /// Replace the pawn at [r,c] with newPiece (e.g. a Queen, Rook, etc.).
        /// </summary>
        public void PromotePawn(int r, int c, ChessPiece newPiece)
        {
            Board[r, c] = newPiece;
        }

    }
}
