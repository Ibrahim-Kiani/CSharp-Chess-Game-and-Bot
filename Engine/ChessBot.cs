using System;
using System.Collections.Generic;
using System.Linq;
using ChessWPF.Models;

namespace ChessWPF.Engine
{
    // Difficulty levels map to search depths: Easy=1, Medium=3, Hard=5
    public enum BotDifficulty { Easy, Medium, Hard }

    // Represents a potential move on the board
    public struct Move
    {
        public int FromRow, FromCol, ToRow, ToCol;
    }

    public static class ChessBot
    {
        /// <summary>
        /// Returns the best move using negamax + alpha-beta with a simple evaluation function.
        /// Easy: depth=1; Medium: depth=3; Hard: depth=5
        /// Always returns a move if any are available, even when in check.
        /// </summary>
        public static Move GetBestMove(GameEngine engine, BotDifficulty difficulty)
        {
            int depth = difficulty switch
            {
                BotDifficulty.Easy => 2,
                BotDifficulty.Medium => 3,
                BotDifficulty.Hard => 4,
                _ => 3
            };
            bool isWhite = engine.WhiteTurn;

            var moves = GenerateMoves(engine.Board, isWhite);
            if (moves == null || moves.Count == 0)
                return default;  // No legal moves: checkmate or stalemate

            // Evaluate each candidate move
            var scored = new List<(Move move, int score)>();
            foreach (var m in moves)
            {
                var boardCopy = CloneBoard(engine.Board);
                ApplyMove(boardCopy, m);
                if (!IsInCheck(boardCopy, isWhite))
                {
                    int score = -Negamax(boardCopy, depth - 1, int.MinValue + 1, int.MaxValue - 1, !isWhite);
                    scored.Add((m, score));
                }
                    
            }

            // Sort descending by score
            scored.Sort((a, b) => b.score.CompareTo(a.score));

            // Easy: pick randomly among top 3 for weaker play
            if (difficulty == BotDifficulty.Easy && scored.Count > 0)
            {
                var top = scored.Take(Math.Min(3, scored.Count)).ToList();
                return top[new Random().Next(top.Count)].move;
            }

            // Medium: sometimes randomize among top 2
            if (difficulty == BotDifficulty.Medium && scored.Count > 1)
            {
                var rnd = new Random();
                return (rnd.NextDouble() < 0.2)
                    ? scored[1].move
                    : scored[0].move;
            }

            // Hard or fallback: always pick the best
            return scored[0].move;
        }

        // Core negamax search with alpha-beta pruning
        private static bool IsInCheck(ChessPiece[,] board, bool isWhite)
        {
            // 1) Find the king:
            int kr = -1, kc = -1;
            for (int r = 0; r < 8; r++)
                for (int c = 0; c < 8; c++)
                    if (board[r, c] is King k && k.IsWhite == isWhite)
                    {
                        kr = r; kc = c;
                        break;
                    }
            if (kr < 0) return false;

            // 2) See if any opponent pseudo‐legal move attacks that square:
            foreach (var m in GenerateMoves(board, !isWhite))
                if (m.ToRow == kr && m.ToCol == kc)
                    return true;

            return false;
        }
        private static int Negamax(ChessPiece[,] board, int depth, int alpha, int beta, bool isWhite)
        {
            if (depth == 0)
                return Evaluate(board);

            var moves = GenerateMoves(board, isWhite);
            if (moves.Count == 0)
                return Evaluate(board); // In-check detection happens at higher level

            foreach (var m in moves)
            {
                var copy = CloneBoard(board);
                ApplyMove(copy, m);
                int val = -Negamax(copy, depth - 1, -beta, -alpha, !isWhite);
                alpha = Math.Max(alpha, val);
                if (alpha >= beta)
                    break;  // beta cutoff
            }
            return alpha;
        }

        // Generate all pseudo-legal moves (doesn't remove moves that leave king in check)
        private static List<Move> GenerateMoves(ChessPiece[,] board, bool isWhite)
        {
            var list = new List<Move>();
            for (int r = 0; r < 8; r++)
                for (int c = 0; c < 8; c++)
                {
                    var p = board[r, c];
                    if (p != null && p.IsWhite == isWhite)
                    {
                        foreach (var t in p.GetPossibleMoves(board, r, c))
                            list.Add(new Move { FromRow = r, FromCol = c, ToRow = t.row, ToCol = t.col });
                    }
                }
            return list;
        }

        // Apply a move to a board copy (includes pawn auto-promotion to queen)
        private static void ApplyMove(ChessPiece[,] board, Move m)
        {
            var p = board[m.FromRow, m.FromCol];
            board[m.ToRow, m.ToCol] = p;
            board[m.FromRow, m.FromCol] = null;
            p.HasMoved = true;

            // Auto-promotion to queen for simplicity
            if (p is Pawn && (m.ToRow == 0 || m.ToRow == 7))
                board[m.ToRow, m.ToCol] = new Queen(p.IsWhite) { HasMoved = true };
        }

        // Deep clone the board array
        private static ChessPiece[,] CloneBoard(ChessPiece[,] board)
        {
            var nb = new ChessPiece[8, 8];
            for (int r = 0; r < 8; r++)
                for (int c = 0; c < 8; c++)
                    nb[r, c] = board[r, c];
            return nb;
        }

        // Basic evaluation: material + piece-square tables + mobility
        private static int Evaluate(ChessPiece[,] board)
        {
            int score = 0;
            int mobilityWeight = 5;
            for (int r = 0; r < 8; r++)
                for (int c = 0; c < 8; c++)
                {
                    var p = board[r, c];
                    if (p == null) continue;
                    int v = p switch
                    {
                        Pawn _ => 100 + PawnTable[r, c],
                        Knight _ => 320 + KnightTable[r, c],
                        Bishop _ => 330 + BishopTable[r, c],
                        Rook _ => 500 + RookTable[r, c],
                        Queen _ => 900 + QueenTable[r, c],
                        King _ => 20000 + KingTable[r, c],
                        _ => 0
                    };
                    score += p.IsWhite ? v : -v;
                }
            // Mobility term
            score += mobilityWeight * GenerateMoves(board, true).Count;
            score -= mobilityWeight * GenerateMoves(board, false).Count;
            return score;
        }

        // Piece-square tables give positional value
        private static readonly int[,] PawnTable = new int[8, 8]
        {
            {  0,  0,  0,  0,  0,  0,  0,  0 },
            { 50, 50, 50, 50, 50, 50, 50, 50 },
            { 10, 10, 20, 30, 30, 20, 10, 10 },
            {  5,  5, 10, 25, 25, 10,  5,  5 },
            {  0,  0,  0, 20, 20,  0,  0,  0 },
            {  5, -5,-10,  0,  0,-10, -5,  5 },
            {  5, 10, 10,-20,-20, 10, 10,  5 },
            {  0,  0,  0,  0,  0,  0,  0,  0 }
        };
        private static readonly int[,] KnightTable = new int[8, 8]
        {
            { -50,-40,-30,-30,-30,-30,-40,-50 },
            { -40,-20,  0,  0,  0,  0,-20,-40 },
            { -30,  0, 10, 15, 15, 10,  0,-30 },
            { -30,  5, 15, 20, 20, 15,  5,-30 },
            { -30,  0, 15, 20, 20, 15,  0,-30 },
            { -30,  5, 10, 15, 15, 10,  5,-30 },
            { -40,-20,  0,  5,  5,  0,-20,-40 },
            { -50,-40,-30,-30,-30,-30,-40,-50 }
        };
        private static readonly int[,] BishopTable = new int[8, 8]
        {
            { -20,-10,-10,-10,-10,-10,-10,-20 },
            { -10,  0,  0,  0,  0,  0,  0,-10 },
            { -10,  0,  5, 10, 10,  5,  0,-10 },
            { -10,  5,  5, 10, 10,  5,  5,-10 },
            { -10,  0, 10, 10, 10, 10,  0,-10 },
            { -10, 10, 10, 10, 10, 10, 10,-10 },
            { -10,  5,  0,  0,  0,  0,  5,-10 },
            { -20,-10,-10,-10,-10,-10,-10,-20 }
        };
        private static readonly int[,] RookTable = new int[8, 8]
        {
            {  0,  0,  0,  0,  0,  0,  0,  0 },
            {  5, 10, 10, 10, 10, 10, 10,  5 },
            { -5,  0,  0,  0,  0,  0,  0, -5 },
            { -5,  0,  0,  0,  0,  0,  0, -5 },
            { -5,  0,  0,  0,  0,  0,  0, -5 },
            { -5,  0,  0,  0,  0,  0,  0, -5 },
            { -5,  0,  0,  0,  0,  0,  0, -5 },
            {  0,  0,  0,  5,  5,  0,  0,  0 }
        };
        private static readonly int[,] QueenTable = new int[8, 8]
        {
            { -20,-10,-10, -5, -5,-10,-10,-20 },
            { -10,  0,  0,  0,  0,  0,  0,-10 },
            { -10,  0,  5,  5,  5,  5,  0,-10 },
            {  -5,  0,  5,  5,  5,  5,  0, -5 },
            {   0,  0,  5,  5,  5,  5,  0, -5 },
            { -10,  5,  5,  5,  5,  5,  0,-10 },
            { -10,  0,  5,  0,  0,  0,  0,-10 },
            { -20,-10,-10, -5, -5,-10,-10,-20 }
        };
        private static readonly int[,] KingTable = new int[8, 8]
        {
            { -30,-40,-40,-50,-50,-40,-40,-30 },
            { -30,-40,-40,-50,-50,-40,-40,-30 },
            { -30,-40,-40,-50,-50,-40,-40,-30 },
            { -30,-40,-40,-50,-50,-40,-40,-30 },
            { -20,-30,-30,-40,-40,-30,-30,-20 },
            { -10,-20,-20,-20,-20,-20,-20,-10 },
            {  20, 20,  0,  0,  0,  0, 20, 20 },
            {  20, 30, 10,  0,  0, 10, 30, 20 }
        };
    }
}
