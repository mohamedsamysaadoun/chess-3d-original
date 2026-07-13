using System;

namespace EivaaChess.Game
{
    /// <summary>
    /// Move struct. Original obfuscated name: SechChal.
    /// TypeDefIndex: 6233
    ///
    /// Layout matches IL2CPP exactly (4 bytes total):
    ///   byte fromSquare       @ 0x0
    ///   byte toSquare         @ 0x1
    ///   byte promotionPiece   @ 0x2  (0=none, 2=knight, 3=bishop, 4=rook, 5=queen)
    ///   byte moveFlags        @ 0x3  (bit 0=capture, 1=castle, 2=ep, 3=promo, 4=double-pawn)
    /// </summary>
    [Serializable]
    internal struct Move
    {
        public byte fromSquare;       // 0x0 — was moveFrom
        public byte toSquare;         // 0x1 — was moveTo
        public byte promotionPiece;   // 0x2 — was promote
        public byte moveFlags;        // 0x3 — was bits

        // Flag bit constants
        public const byte FLAG_CAPTURE = 0x01;
        public const byte FLAG_CASTLE = 0x02;
        public const byte FLAG_EN_PASSANT = 0x04;
        public const byte FLAG_PROMOTION = 0x08;
        public const byte FLAG_DOUBLE_PAWN = 0x10;

        // Property: Empty move (default)
        public static Move Empty => new Move();

        // Operator overloads (matching original op_Equality / op_Inequality)
        public static bool operator ==(Move m1, Move m2) =>
            m1.fromSquare == m2.fromSquare &&
            m1.toSquare == m2.toSquare &&
            m1.promotionPiece == m2.promotionPiece;

        public static bool operator !=(Move m1, Move m2) => !(m1 == m2);

        public override bool Equals(object o) => o is Move m && this == m;

        public override int GetHashCode() =>
            fromSquare | (toSquare << 6) | (promotionPiece << 12);

        /// <summary>
        /// Parse CAN (Coordinate Algebraic Notation) string.
        /// Original: ParseRegularCAN (RVA 0xEE4CD4).
        /// Example: "e2e4", "e7e8q" (with promotion).
        /// </summary>
        public static Move ParseCAN(string can)
        {
            if (string.IsNullOrEmpty(can) || can.Length < 4)
                return Empty;

            var move = new Move();
            move.fromSquare = (byte)((8 - (can[1] - '0')) * 16 + (can[0] - 'a'));
            move.toSquare = (byte)((8 - (can[3] - '0')) * 16 + (can[2] - 'a'));

            if (can.Length >= 5)
            {
                move.promotionPiece = can[4] switch
                {
                    'q' => 5,
                    'r' => 4,
                    'b' => 3,
                    'n' => 2,
                    _ => (byte)0
                };
                move.moveFlags |= FLAG_PROMOTION;
            }
            return move;
        }

        /// <summary>
        /// Convert to CAN string. Original: ToString (RVA 0xEE4BB8).
        /// </summary>
        public override string ToString()
        {
            int fromFile = fromSquare % 16;
            int fromRank = 8 - (fromSquare / 16);
            int toFile = toSquare % 16;
            int toRank = 8 - (toSquare / 16);

            string result = $"{(char)('a' + fromFile)}{fromRank}{(char)('a' + toFile)}{toRank}";

            if (promotionPiece != 0)
            {
                result += promotionPiece switch
                {
                    5 => 'q',
                    4 => 'r',
                    3 => 'b',
                    2 => 'n',
                    _ => '?'
                };
            }
            return result;
        }

        // Helper for construction
        public static Move Create(int from, int to, byte flags, int promo = 0) => new Move
        {
            fromSquare = (byte)from,
            toSquare = (byte)to,
            moveFlags = flags,
            promotionPiece = (byte)promo
        };
    }

    /// <summary>
    /// ScoredMove struct (Move + score for sorting).
    /// Original obfuscated name: KeemtiChal (Keemti = valuable).
    /// TypeDefIndex: 6234
    /// </summary>
    [Serializable]
    internal struct ScoredMove
    {
        public Move move;  // 0x0
        public int s;      // 0x4 — score
    }

    /// <summary>
    /// HistoryMove struct (stored on history stack for UnmakeMove).
    /// TypeDefIndex: 6235
    /// Layout matches IL2CPP exactly (20 bytes).
    /// </summary>
    [Serializable]
    internal struct HistoryMove
    {
        public Move move;             // 0x0
        public int capture;           // 0x4
        public int castlingRights;    // 0x8
        public int ep;                // 0xC — en passant square before move
        public int fiftyMoveRule;     // 0x10

        // Extended fields for clean en passant unmake (not in original struct, but useful)
        public int capturedColor;
        public int epCapturedSquare;
        public int epCapturedPiece;
        public int epCapturedColor;
    }
}
