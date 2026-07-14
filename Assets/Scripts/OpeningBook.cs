using System;
using System.Collections.Generic;

namespace EivaaChess.Game
{
    /// <summary>
    /// Opening Book.
    /// Original obfuscated name: KhaataKhol (Khaata=record, Khol=open).
    /// TypeDefIndex: 6231
    ///
    /// Original fields:
    ///   private Random random;            // 0x10 — was kk_rand
    ///   private int[] openingIndex;       // 0x18 — was kk_idx
    ///   private short[] openingMoves;     // 0x20 — was kk_chals
    ///
    /// Original method: SuruBatao(int h) → GetOpeningMove(int h) (RVA 0xEE3C0C)
    /// </summary>
    public class OpeningBook
    {
        private Random random;            // 0x10
        private int[] openingIndex;       // 0x18
        private short[] openingMoves;     // 0x20

        private readonly Dictionary<string, List<string>> book;

        public OpeningBook()
        {
            random = new Random();
            openingIndex = new int[0];
            openingMoves = new short[0];
            book = new Dictionary<string, List<string>>();
            BuildOpeningRepertoire();
        }

        /// <summary>
        /// Get opening move for given move number.
        /// Original: SuruBatao(int h) returned short (-1 = no move).
        /// </summary>
        public short GetOpeningMove(int h)
        {
            // Returns -1 (no move) — for FEN-based lookup, use GetOpeningMove(string fen, int moveNum)
            return -1;
        }

        /// <summary>
        /// Get opening move for current FEN position.
        /// Returns CAN string or null.
        /// </summary>
        public string GetOpeningMove(string fen, int moveNumber)
        {
            if (moveNumber > 10) return null;

            string key = NormalizeFEN(fen);
            if (!book.TryGetValue(key, out var moves) || moves.Count == 0)
                return null;

            return moves[random.Next(moves.Count)];
        }

        private static string NormalizeFEN(string fen)
        {
            if (string.IsNullOrEmpty(fen)) return "";
            var parts = fen.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 4) return fen;
            return $"{parts[0]} {parts[1]} {parts[2]} {parts[3]}";
        }

        private void BuildOpeningRepertoire()
        {
            Add("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq -", new[]
            {
                "e2e4", "d2d4", "g1f3", "c2c4", "e2e3", "b1c3"
            });

            Add("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3", new[]
            {
                "c7c5", "e7e5", "e7e6", "c7c6", "d7d5", "g8f6"
            });

            Add("rnbqkbnr/pppppppp/8/8/3P4/8/PPP1PPPP/RNBQKBNR b KQkq d3", new[]
            {
                "d7d5", "g8f6", "e7e6", "c7c5", "f7f5"
            });

            Add("rnbqkbnr/pppppppp/8/8/8/5N2/PPPPPPPP/RNBQKB1R b KQkq -", new[]
            {
                "d7d5", "g8f6", "c7c5", "e7e6"
            });

            Add("rnbqkbnr/pppppppp/8/8/2P5/8/PP1PPPPP/RNBQKBNR b KQkq c3", new[]
            {
                "e7e5", "c7c5", "e7e6", "g8f6"
            });

            Add("rnbqkbnr/pppp1ppp/8/4p3/4P3/8/PPPP1PPP/RNBQKBNR w KQkq e6", new[]
            {
                "g1f3", "f1c4", "b1c3", "d2d4", "f1b5"
            });

            Add("rnbqkbnr/pp1ppppp/8/2p5/4P3/8/PPPP1PPP/RNBQKBNR w KQkq c6", new[]
            {
                "g1f3", "b1c3", "f1c4", "d2d4"
            });

            Add("rnbqkbnr/pppp1ppp/8/4p3/4P3/8/PPPP1PPP/RNBQKBNR w KQkq -", new[]
            {
                "d2d4", "g1f3", "c2c3"
            });

            Add("rnbqkbnr/pp1ppppp/8/2p5/4P3/8/PPPP1PPP/RNBQKBNR w KQkq -", new[]
            {
                "d2d4", "c2c4", "g1f3"
            });

            Add("rnbqkbnr/ppp1pppp/8/3p4/3P4/8/PPP1PPPP/RNBQKBNR w KQkq d6", new[]
            {
                "c2c4", "g1f3", "b1c3", "c1f4", "c1g5"
            });

            Add("rnbqkb1r/pppppppp/5n2/8/3P4/8/PPP1PPPP/RNBQKBNR w KQkq -", new[]
            {
                "c2c4", "g1f3", "g2g3", "c1f4"
            });

            Add("rnbqkb1r/pppppppp/5n2/8/8/5N2/PPPPPPPP/RNBQKB1R w KQkq -", new[]
            {
                "g2g3", "c2c4", "d2d4", "b1c3"
            });

            Add("r1bqkbnr/pppp1ppp/2n5/4p3/4P3/5N2/PPPP1PPP/RNBQKB1R w KQkq -", new[]
            {
                "f1b5", "b1c3", "d2d4", "f1c4"
            });

            Add("r1bqkbnr/pppp1ppp/2n5/4p3/2B1P3/5N2/PPPP1PPP/RNBQK2R b KQkq -", new[]
            {
                "f8c5", "g8f6", "d7d6", "b7b6"
            });

            Add("r1bqk1nr/pppp1ppp/2n5/1b2p3/4P3/5N2/PPPP1PPP/RNBQKB1R w KQkq -", new[]
            {
                "b1c3", "a2a3", "d2d4", "c2c3"
            });
        }

        private void Add(string fenKey, string[] moves)
        {
            book[fenKey] = new List<string>(moves);
        }
    }
}
