using System;
using System.Collections.Generic;

namespace EivaaChess.Game
{
    /// <summary>
    /// Chess Engine — sealed class.
    /// Original obfuscated name: SechDMG (Sech=search, DMG=difficulty/game).
    /// TypeDefIndex: 6232
    /// Namespace: EivaaChess.Game
    ///
    /// Original used mailbox 10x12 board representation (per dump.cs fields
    /// `mailboxBoard`, `mailbox64`). This reconstruction uses the 0x88
    /// representation (simpler, functionally equivalent) ported from
    /// the working engine-test implementation.
    ///
    /// Constants (matching original IL2CPP):
    ///   MAX_DEPTH = 10
    ///   OPENING_MOVES_COUNT = 5
    ///   BOARD_SQUARES = 64
    ///   PIECE_TYPES = 12
    ///   MAX_PLY = 32
    ///   MAX_MOVES = 1280
    /// </summary>
    public sealed class ChessEngine
    {
        // ================================================================
        // CONSTANTS (from dump.cs)
        // ================================================================

        private const int MAX_DEPTH = 10;
        private const int OPENING_MOVES_COUNT = 5;
        internal const int EMPTY_PIECE = 0;
        internal const int OFFBOARD = 1;
        internal const int COLOR_NONE = 0;
        internal const int PIECE_ROOK = 1;      // was HAST (elephant)
        internal const int PIECE_KNIGHT = 2;    // was BAJIR
        internal const int PIECE_BISHOP = 3;    // was NOKA (boat)
        internal const int CHAMYA = 4;          // queen (kept original name)
        internal const int PIECE_KING = 5;      // was SENSA
        internal const int PIECE_EMPTY = 6;     // was SUNYA (zero)
        private const int BOARD_SQUARES = 64;
        private const int PIECE_TYPES = 12;
        private const int MAX_PLY = 32;
        private const int MAX_MOVES = 1280;

        // Castling square constants (0x88 representation)
        private const int A1 = 56, B1 = 57, C1 = 58, D1 = 59, E1 = 60, F1 = 61, G1 = 62, H1 = 63;
        private const int A8 = 0,  B8 = 1,  C8 = 2,  D8 = 3,  E8 = 4,  F8 = 5,  G8 = 6,  H8 = 7;

        // Additional constants from original (kept for fidelity)
        private const int D_P_P = 10;     // doubled pawn penalty
        private const int I_P_P = 20;     // isolated pawn penalty
        private const int B_P_P = 8;      // bishop pair bonus
        private const int P_P_B = 20;     // passed pawn bonus
        private const int R_S_O_F_B = 10; // rook on open file bonus
        private const int R_O_F_B = 15;   // rook on open file
        private const int R_O_S_B = 20;   // rook on semi-open file

        // ================================================================
        // STATIC FIELDS (from dump.cs)
        // ================================================================

        private static readonly int[] mailboxBoard;     // 0x0 — 10x12 mailbox (legacy)
        private static readonly int[] mailbox64;        // 0x8 — 64→mailbox conversion
        private static readonly bool[] slidingPieces;   // 0x10 — was khasriSakto
        private static readonly int[] moveOffsets;      // 0x18 — was offsets
        private static readonly int[,] pieceOffsets;    // 0x20
        private static readonly int[] castleMask;       // 0x28
        private static long[] zobristKeys;              // 0x30 — was zChabis (key)

        private static readonly int[] pieceValues;      // 0x38 — was gutiDaam
        private static readonly int[] pawnPST;          // 0x40 — was pawnPcSq
        private static readonly int[] knightPST;        // 0x48 — was knightPcSq
        private static readonly int[] bishopPST;        // 0x50 — was bishopPcSq
        private static readonly int[] kingPST;          // 0x58 — was kingPcSq
        private static readonly int[] kingEndgamePST;   // 0x60 — was kingEndgamePcSq
        private static readonly int[] flipBoard;        // 0x68 — was flip

        // ================================================================
        // INSTANCE FIELDS (from dump.cs)
        // ================================================================

        private int[] pieceColor;             // 0x10 — was pColor
        private int[] pieces;                 // 0x18 — was piece
        private int currentSide;              // 0x20 — was cSide
        private int opponentSide;             // 0x24 — was xSide
        private int castlingRights;           // 0x28 — was castle
        private int enPassantSquare;          // 0x2C — was yonP
        private int fiftyMoveRule;            // 0x30 — was fifty
        private int searchPly;                // 0x34 — was ply
        private int fullMoveNumber;           // 0x38 — was moves

        private int[,] historyHeuristic;      // 0x40 — was heuristicHistory
        private HistoryMove[] moveHistory;    // 0x48 — was history
        private Move repetitionMove;          // 0x50 — was repMove
        private OpeningBook openingBook;      // 0x58 — was khaataKhol
        private ScoredMove[] scoredMoves;     // 0x60 — was moveList
        private int[] moveIndexList;          // 0x68
        private Move[,] principalVariation;   // 0x70 — was pv
        private int[] pvLength;               // 0x78
        private bool followPV;                // 0x80
        private bool isThinking;              // 0x81 — was bSochatAahe (Marathi)

        private int[,] pawnRank;              // 0x88
        private int[] pieceMaterial;          // 0x90 — was pieceMat
        private int[] pawnMaterial;           // 0x98 — was pawnMat

        // Search state (added for our implementation)
        private Move bestMove;
        private bool searching;
        private List<Move>[] moveList;
        private Stack<HistoryMove> history;

        // 0x88 board representation (replaces mailbox 10x12)
        private int[] board;
        private int halfMoveClock;

        // ================================================================
        // MOVE OFFSETS (0x88)
        // ================================================================

        private static readonly int[] knightOffsets = { -33, -31, -18, -14, 14, 18, 31, 33 };
        private static readonly int[] kingOffsets   = { -17, -16, -15, -1, 1, 15, 16, 17 };
        private static readonly int[] bishopDirs    = { -17, -15, 15, 17 };
        private static readonly int[] rookDirs      = { -16, -1, 1, 16 };
        private static readonly int[] queenDirs     = { -17, -16, -15, -1, 1, 15, 16, 17 };

        // ================================================================
        // STATIC CONSTRUCTOR (.cctor) — RVA 0xEE7FEC
        // Original: ChessEngine_cctor
        // ================================================================

        static ChessEngine()
        {
            // Initialize static arrays
            slidingPieces = new bool[14];
            for (int i = 0; i < 14; i++) slidingPieces[i] = true;

            moveOffsets = new int[8];
            pieceOffsets = new int[14, 8];
            castleMask = new int[64];
            zobristKeys = new long[64 * 12 + 1];

            pieceValues = new int[] { 0, 100, 320, 330, 500, 1000, 20000, 0, 0, 0, 0, 0, 0, 0 };

            pawnPST = new int[128];
            knightPST = new int[128];
            bishopPST = new int[128];
            kingPST = new int[128];
            kingEndgamePST = new int[128];
            flipBoard = new int[128];
            mailboxBoard = new int[120];
            mailbox64 = new int[64];

            // Initialize PST tables (standard values from chess programming literature)
            int[] pawnTable = {
                 0,  0,  0,  0,  0,  0,  0,  0,
                 5, 10, 10,-20,-20, 10, 10,  5,
                 5, -5,-10,  0,  0,-10, -5,  5,
                 0,  0,  0, 20, 20,  0,  0,  0,
                 5,  5, 10, 25, 25, 10,  5,  5,
                10, 10, 20, 30, 30, 20, 10, 10,
                50, 50, 50, 50, 50, 50, 50, 50,
                 0,  0,  0,  0,  0,  0,  0,  0
            };
            int[] knightTable = {
                -50,-40,-30,-30,-30,-30,-40,-50,
                -40,-20,  0,  5,  5,  0,-20,-40,
                -30,  5, 10, 15, 15, 10,  5,-30,
                -30,  0, 15, 20, 20, 15,  0,-30,
                -30,  5, 15, 20, 20, 15,  5,-30,
                -30,  0, 10, 15, 15, 10,  0,-30,
                -40,-20,  0,  0,  0,  0,-20,-40,
                -50,-40,-30,-30,-30,-30,-40,-50
            };
            int[] bishopTable = {
                -20,-10,-10,-10,-10,-10,-10,-20,
                -10,  5,  0,  0,  0,  0,  5,-10,
                -10, 10, 10, 10, 10, 10, 10,-10,
                -10,  0, 10, 10, 10, 10,  0,-10,
                -10,  5,  5, 10, 10,  5,  5,-10,
                -10,  0,  5, 10, 10,  5,  0,-10,
                -10,  0,  0,  0,  0,  0,  0,-10,
                -20,-10,-10,-10,-10,-10,-10,-20
            };
            int[] kingTable = {
                 20, 30, 10,  0,  0, 10, 30, 20,
                 20, 20,  0,  0,  0,  0, 20, 20,
                -10,-20,-20,-20,-20,-20,-20,-10,
                -20,-30,-30,-40,-40,-30,-30,-20,
                -30,-40,-40,-50,-50,-40,-40,-30,
                -30,-40,-40,-50,-50,-40,-40,-30,
                -30,-40,-40,-50,-50,-40,-40,-30,
                -30,-40,-40,-50,-50,-40,-40,-30
            };
            int[] kingEndgameTable = {
                -50,-30,-30,-30,-30,-30,-30,-50,
                -30,-30,  0,  0,  0,  0,-30,-30,
                -30,-10, 20, 30, 30, 20,-10,-30,
                -30,-10, 30, 40, 40, 30,-10,-30,
                -30,-10, 30, 40, 40, 30,-10,-30,
                -30,-10, 20, 30, 30, 20,-10,-30,
                -30,-20,-10,  0,  0,-10,-20,-30,
                -50,-40,-30,-20,-20,-30,-40,-50
            };

            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    int sq88 = rank * 16 + file;
                    int sq64 = rank * 8 + file;
                    pawnPST[sq88] = pawnTable[sq64];
                    knightPST[sq88] = knightTable[sq64];
                    bishopPST[sq88] = bishopTable[sq64];
                    kingPST[sq88] = kingTable[sq64];
                    kingEndgamePST[sq88] = kingEndgameTable[sq64];
                    flipBoard[sq88] = (7 - rank) * 16 + file;
                    mailbox64[sq64] = (rank + 2) * 10 + file + 1;
                }
            }

            // Initialize Zobrist keys
            var rng = new System.Random(0x12345678);
            for (int i = 0; i < zobristKeys.Length; i++)
                zobristKeys[i] = ((long)rng.Next() << 32) | (uint)rng.Next();
        }

        // ================================================================
        // CONSTRUCTOR (.ctor) — RVA 0xEE3E34
        // ================================================================

        public ChessEngine()
        {
            board = new int[128];
            pieceColor = new int[128];
            pieces = new int[128];
            historyHeuristic = new int[2, 64];
            moveHistory = new HistoryMove[MAX_PLY * 4];
            principalVariation = new Move[MAX_PLY, MAX_PLY];
            pvLength = new int[MAX_PLY];
            pawnRank = new int[2, 10];
            pieceMaterial = new int[2];
            pawnMaterial = new int[2];
            scoredMoves = new ScoredMove[MAX_MOVES];
            moveIndexList = new int[MAX_MOVES];
            moveList = new List<Move>[MAX_PLY];
            for (int i = 0; i < MAX_PLY; i++)
                moveList[i] = new List<Move>(128);
            history = new Stack<HistoryMove>(MAX_PLY * 4);
            bestMove = new Move();
            searching = false;
            openingBook = new OpeningBook();
        }

        // ================================================================
        // PUBLIC API METHODS
        // ================================================================

        /// <summary>
        /// Get best move.
        /// Original: EkChalBatao (RVA 0xEE4948, Slot: not specified).
        /// Signature: GetBestMove(string a, string r, int d, bool eM).
        /// </summary>
        public string GetBestMove(string a, string r, int d, bool eM)
        {
            searching = false;
            bestMove = new Move();
            searchPly = 0;
            history.Clear();

            ParseFEN(a);
            searching = true;
            isThinking = true;

            for (int depth = 1; depth <= Math.Min(d, MAX_DEPTH); depth++)
            {
                int score = AlphaBeta(int.MinValue + 1, int.MaxValue, depth, false);
            }

            searching = false;
            isThinking = false;

            if (bestMove.fromSquare == 0 && bestMove.toSquare == 0)
            {
                var moves = GenerateLegalMoves();
                if (moves.Count > 0)
                    return MoveToCAN(moves[0]);
                return null;
            }

            return MoveToCAN(bestMove);
        }

        /// <summary>
        /// Start search.
        /// Original: SochnaSuruKro (RVA 0xEE4DFC).
        /// </summary>
        private void StartSearch(int d, bool e)
        {
            searchPly = 0;
            history.Clear();
            isThinking = true;
            searching = true;

            for (int depth = 1; depth <= Math.Min(d, MAX_DEPTH); depth++)
            {
                AlphaBeta(int.MinValue + 1, int.MaxValue, depth, e);
            }

            isThinking = false;
            searching = false;
        }

        /// <summary>
        /// Parse FEN string.
        /// Original: SamjoPHAN (RVA 0xEE4130).
        /// </summary>
        public void ParseFEN(string f)
        {
            for (int i = 0; i < 128; i++)
            {
                board[i] = EMPTY_PIECE;
                pieces[i] = EMPTY_PIECE;
                pieceColor[i] = COLOR_NONE;
            }
            castlingRights = 0;
            enPassantSquare = -1;
            fiftyMoveRule = 0;
            fullMoveNumber = 1;

            if (string.IsNullOrEmpty(f)) return;

            string[] parts = f.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return;

            int rank = 0, file = 0;
            foreach (char c in parts[0])
            {
                if (c == '/')
                {
                    rank++;
                    file = 0;
                }
                else if (char.IsDigit(c))
                {
                    file += c - '0';
                }
                else
                {
                    int sq88 = rank * 16 + file;
                    int color = char.IsUpper(c) ? 0 : 1;
                    int piece = char.ToLower(c) switch
                    {
                        'p' => 6,  // PIECE_PAWN (using original encoding where 6=pawn)
                        'n' => 2,  // PIECE_KNIGHT
                        'b' => 3,  // PIECE_BISHOP
                        'r' => 1,  // PIECE_ROOK
                        'q' => 4,  // CHAMYA (queen)
                        'k' => 5,  // PIECE_KING
                        _ => EMPTY_PIECE
                    };
                    board[sq88] = piece;
                    pieces[sq88] = piece;
                    pieceColor[sq88] = color;
                    file++;
                }
            }

            currentSide = (parts.Length > 1 && parts[1] == "w") ? 0 : 1;
            opponentSide = 1 - currentSide;

            if (parts.Length > 2)
            {
                if (parts[2].Contains('K')) castlingRights |= 1;
                if (parts[2].Contains('Q')) castlingRights |= 2;
                if (parts[2].Contains('k')) castlingRights |= 4;
                if (parts[2].Contains('q')) castlingRights |= 8;
            }

            if (parts.Length > 3 && parts[3] != "-")
            {
                int epFile = parts[3][0] - 'a';
                int epRank = 8 - (parts[3][1] - '0');
                enPassantSquare = epRank * 16 + epFile;
            }

            if (parts.Length > 4 && int.TryParse(parts[4], out int hm))
                fiftyMoveRule = hm;

            if (parts.Length > 5 && int.TryParse(parts[5], out int fm))
                fullMoveNumber = fm;
        }

        // ================================================================
        // ALPHA-BETA SEARCH — Original: DhoondoNormal (RVA 0xEE4E78)
        // ================================================================

        public int AlphaBeta(int a, int b, int d, bool eM)
        {
            if (d == 0)
                return QuiescenceSearch(a, b, eM);

            var moves = GenerateLegalMoves();
            if (moves.Count == 0)
            {
                if (IsInCheck(currentSide))
                    return -100000 + searchPly;
                return 0;
            }

            moves.Sort((m1, m2) => ScoreMove(m2).CompareTo(ScoreMove(m1)));

            int bestScore = int.MinValue + 1;
            bool isRoot = (searchPly == 0);

            for (int i = 0; i < moves.Count; i++)
            {
                Move move = moves[i];
                MakeMove(move);
                int score;
                if (isRoot)
                    score = -AlphaBeta(int.MinValue + 1, int.MaxValue, d - 1, false);
                else
                    score = -AlphaBeta(-b, -a, d - 1, false);
                UnmakeMove();

                if (score > bestScore)
                {
                    bestScore = score;
                    if (searchPly == 0)
                        bestMove = move;
                }

                if (score > a) a = score;
                if (a >= b && searchPly > 0) break;
            }

            return bestScore;
        }

        /// <summary>
        /// Quiescence search. Original: DhoondoDhainya (RVA 0xEE5290).
        /// </summary>
        public int QuiescenceSearch(int a, int b, bool eM)
        {
            int standPat = Evaluate();
            if (standPat >= b) return b;
            if (standPat > a) a = standPat;

            var captures = new List<Move>();
            GeneratePseudoMoves(captures);
            captures.RemoveAll(m => (m.moveFlags & Move.FLAG_CAPTURE) == 0 &&
                                    (m.moveFlags & Move.FLAG_PROMOTION) == 0);
            captures.Sort((m1, m2) => ScoreMove(m2).CompareTo(ScoreMove(m1)));

            foreach (var move in captures)
            {
                MakeMove(move);
                if (IsInCheck(currentSide == 0 ? 1 : 0))
                {
                    UnmakeMove();
                    continue;
                }
                int score = -QuiescenceSearch(-b, -a, eM);
                UnmakeMove();

                if (score >= b) return b;
                if (score > a) a = score;
            }

            return a;
        }

        /// <summary>
        /// Sort principal variation. Original: SortPV (RVA 0xEE6438).
        /// </summary>
        private void SortPrincipalVariation() { /* TODO: PV sorting */ }

        /// <summary>
        /// QuickSort moves. Original: QuickSortMoveList (RVA 0xEE6510).
        /// </summary>
        private void QuickSortMoves(int f, int t)
        {
            if (f < t)
            {
                int p = PartitionMoves(f, t);
                QuickSortMoves(f, p - 1);
                QuickSortMoves(p + 1, t);
            }
        }

        /// <summary>
        /// Partition for QuickSort. Original: PartitionMoveList (RVA 0xEE7260).
        /// </summary>
        private int PartitionMoves(int mFrom, int mTo)
        {
            int pivot = scoredMoves[mTo].s;
            int i = mFrom - 1;
            for (int j = mFrom; j < mTo; j++)
            {
                if (scoredMoves[j].s >= pivot)
                {
                    i++;
                    var temp = scoredMoves[i];
                    scoredMoves[i] = scoredMoves[j];
                    scoredMoves[j] = temp;
                }
            }
            var t2 = scoredMoves[i + 1];
            scoredMoves[i + 1] = scoredMoves[mTo];
            scoredMoves[mTo] = t2;
            return i + 1;
        }

        // ================================================================
        // MOVE GENERATION
        // ================================================================

        /// <summary>
        /// Generate all moves. Original: GeneratePlyMoves → GenerateMoves (RVA 0xEE5DB8).
        /// </summary>
        private void GenerateMoves()
        {
            // In original, this populated the scoredMoves array.
            // Here, we expose a public wrapper.
        }

        public List<Move> GenerateLegalMoves()
        {
            var pseudoMoves = new List<Move>();
            GeneratePseudoMoves(pseudoMoves);

            var legalMoves = new List<Move>();
            foreach (var move in pseudoMoves)
            {
                MakeMove(move);
                if (!IsInCheck(currentSide == 0 ? 1 : 0))
                    legalMoves.Add(move);
                UnmakeMove();
            }
            return legalMoves;
        }

        private void GeneratePseudoMoves(List<Move> moves)
        {
            for (int sq = 0; sq < 128; sq++)
            {
                if ((sq & 0x88) != 0) continue;
                if (board[sq] == EMPTY_PIECE) continue;
                if (pieceColor[sq] != currentSide) continue;

                switch (board[sq])
                {
                    case 6:  // PAWN
                        GeneratePawnMoves(sq, moves);
                        break;
                    case 2:  // KNIGHT
                        GenerateKnightMoves(sq, moves);
                        break;
                    case 3:  // BISHOP
                        GenerateSlidingMoves(sq, bishopDirs, moves);
                        break;
                    case 1:  // ROOK
                        GenerateSlidingMoves(sq, rookDirs, moves);
                        break;
                    case 4:  // QUEEN
                        GenerateSlidingMoves(sq, queenDirs, moves);
                        break;
                    case 5:  // KING
                        GenerateKingMoves(sq, moves);
                        break;
                }
            }
        }

        /// <summary>
        /// Generate capture moves. Original: GeneratePlyCaptureMoves → GenerateCaptures (RVA 0xEE6CB8).
        /// </summary>
        private void GenerateCaptures()
        {
            // Used by QuiescenceSearch — already handled in our implementation.
        }

        private void GeneratePawnMoves(int from, List<Move> moves)
        {
            int forward = currentSide == 0 ? -16 : 16;
            int startRank = currentSide == 0 ? 6 : 1;
            int promoRank = currentSide == 0 ? 0 : 7;

            int to = from + forward;
            if (OnBoard(to) && board[to] == EMPTY_PIECE)
            {
                if (to / 16 == promoRank)
                    AddPromotionMove(from, to, 0);
                else
                    moves.Add(Move.Create(from, to, 0));

                if (from / 16 == startRank)
                {
                    int to2 = from + 2 * forward;
                    if (OnBoard(to2) && board[to2] == EMPTY_PIECE)
                        moves.Add(Move.Create(from, to2, Move.FLAG_DOUBLE_PAWN));
                }
            }

            int[] capDirs = currentSide == 0 ? new[] { -15, -17 } : new[] { 15, 17 };
            foreach (int d in capDirs)
            {
                int capTo = from + d;
                if (!OnBoard(capTo)) continue;
                if (board[capTo] != EMPTY_PIECE && pieceColor[capTo] != currentSide)
                {
                    if (capTo / 16 == promoRank)
                        AddPromotionMove(from, capTo, Move.FLAG_CAPTURE);
                    else
                        moves.Add(Move.Create(from, capTo, Move.FLAG_CAPTURE));
                }
                else if (capTo == enPassantSquare && enPassantSquare >= 0)
                {
                    moves.Add(Move.Create(from, capTo, Move.FLAG_CAPTURE | Move.FLAG_EN_PASSANT));
                }
            }
        }

        /// <summary>
        /// Add promotion move. Original: AddPlyPromotionMove → AddPromotionMove (RVA 0xEE7504).
        /// </summary>
        private void AddPromotionMove(int mFrom, int mTo, int moveFlags)
        {
            // Adds to the moveList field in original; here we add to moveList[searchPly]
            moveList[searchPly].Add(Move.Create(mFrom, mTo, (byte)moveFlags, 5));  // Queen
            moveList[searchPly].Add(Move.Create(mFrom, mTo, (byte)moveFlags, 4));  // Rook
            moveList[searchPly].Add(Move.Create(mFrom, mTo, (byte)moveFlags, 3));  // Bishop
            moveList[searchPly].Add(Move.Create(mFrom, mTo, (byte)moveFlags, 2));  // Knight
        }

        /// <summary>
        /// Add move. Original: AddPlyMove → AddMove (RVA 0xEE73D0).
        /// </summary>
        private void AddMove(int mFrom, int mTo, int moveFlags)
        {
            moveList[searchPly].Add(Move.Create(mFrom, mTo, (byte)moveFlags));
        }

        private void GenerateKnightMoves(int from, List<Move> moves)
        {
            foreach (int offset in knightOffsets)
            {
                int to = from + offset;
                if (!OnBoard(to)) continue;
                if (pieceColor[to] == currentSide) continue;

                byte flags = (board[to] != EMPTY_PIECE) ? Move.FLAG_CAPTURE : (byte)0;
                moves.Add(Move.Create(from, to, flags));
            }
        }

        private void GenerateSlidingMoves(int from, int[] dirs, List<Move> moves)
        {
            foreach (int dir in dirs)
            {
                int to = from + dir;
                while (OnBoard(to))
                {
                    if (pieceColor[to] == currentSide) break;
                    byte flags = (board[to] != EMPTY_PIECE) ? Move.FLAG_CAPTURE : (byte)0;
                    moves.Add(Move.Create(from, to, flags));
                    if (board[to] != EMPTY_PIECE) break;
                    to += dir;
                }
            }
        }

        private void GenerateKingMoves(int from, List<Move> moves)
        {
            foreach (int offset in kingOffsets)
            {
                int to = from + offset;
                if (!OnBoard(to)) continue;
                if (pieceColor[to] == currentSide) continue;
                byte flags = (board[to] != EMPTY_PIECE) ? Move.FLAG_CAPTURE : (byte)0;
                moves.Add(Move.Create(from, to, flags));
            }

            // Castling
            if (currentSide == 0 && from == E1)
            {
                if ((castlingRights & 1) != 0 && board[E1 + 1] == EMPTY_PIECE && board[E1 + 2] == EMPTY_PIECE
                    && board[H1] == PIECE_ROOK && pieceColor[H1] == 0
                    && !IsSquareAttacked(E1, 1) && !IsSquareAttacked(E1 + 1, 1))
                    moves.Add(Move.Create(E1, E1 + 2, Move.FLAG_CASTLE));
                if ((castlingRights & 2) != 0 && board[E1 - 1] == EMPTY_PIECE && board[E1 - 2] == EMPTY_PIECE && board[E1 - 3] == EMPTY_PIECE
                    && board[A1] == PIECE_ROOK && pieceColor[A1] == 0
                    && !IsSquareAttacked(E1, 1) && !IsSquareAttacked(E1 - 1, 1))
                    moves.Add(Move.Create(E1, E1 - 2, Move.FLAG_CASTLE));
            }
            else if (currentSide == 1 && from == E8)
            {
                if ((castlingRights & 4) != 0 && board[E8 + 1] == EMPTY_PIECE && board[E8 + 2] == EMPTY_PIECE
                    && board[H8] == PIECE_ROOK && pieceColor[H8] == 1
                    && !IsSquareAttacked(E8, 0) && !IsSquareAttacked(E8 + 1, 0))
                    moves.Add(Move.Create(E8, E8 + 2, Move.FLAG_CASTLE));
                if ((castlingRights & 8) != 0 && board[E8 - 1] == EMPTY_PIECE && board[E8 - 2] == EMPTY_PIECE && board[E8 - 3] == EMPTY_PIECE
                    && board[A8] == PIECE_ROOK && pieceColor[A8] == 1
                    && !IsSquareAttacked(E8, 0) && !IsSquareAttacked(E8 - 1, 0))
                    moves.Add(Move.Create(E8, E8 - 2, Move.FLAG_CASTLE));
            }
        }

        // ================================================================
        // MAKE / UNMAKE MOVE
        // ================================================================

        /// <summary>
        /// Make move. Original: Make (RVA 0xEE6570).
        /// </summary>
        public bool MakeMove(Move chal)
        {
            int from = chal.fromSquare;
            int to = chal.toSquare;
            int piece = board[from];
            int captured = board[to];

            var hist = new HistoryMove
            {
                move = chal,
                capture = captured,
                castlingRights = castlingRights,
                ep = enPassantSquare,
                fiftyMoveRule = fiftyMoveRule,
                capturedColor = captured != EMPTY_PIECE ? pieceColor[to] : COLOR_NONE,
                epCapturedSquare = -1
            };
            history.Push(hist);

            board[to] = piece;
            pieces[to] = piece;
            pieceColor[to] = currentSide;
            board[from] = EMPTY_PIECE;
            pieces[from] = EMPTY_PIECE;
            pieceColor[from] = COLOR_NONE;

            if ((chal.moveFlags & Move.FLAG_EN_PASSANT) != 0)
            {
                int epCaptured = currentSide == 0 ? to + 16 : to - 16;
                hist.epCapturedSquare = epCaptured;
                hist.epCapturedPiece = board[epCaptured];
                hist.epCapturedColor = pieceColor[epCaptured];
                board[epCaptured] = EMPTY_PIECE;
                pieces[epCaptured] = EMPTY_PIECE;
                pieceColor[epCaptured] = COLOR_NONE;
            }

            enPassantSquare = (chal.moveFlags & Move.FLAG_DOUBLE_PAWN) != 0
                ? (currentSide == 0 ? from - 16 : from + 16)
                : -1;

            if (chal.promotionPiece != 0)
            {
                board[to] = chal.promotionPiece;
                pieces[to] = chal.promotionPiece;
            }

            if ((chal.moveFlags & Move.FLAG_CASTLE) != 0)
            {
                if (to == E1 + 2) { board[E1 + 1] = board[H1]; pieceColor[E1 + 1] = currentSide; board[H1] = EMPTY_PIECE; pieceColor[H1] = COLOR_NONE; }
                else if (to == E1 - 2) { board[E1 - 1] = board[A1]; pieceColor[E1 - 1] = currentSide; board[A1] = EMPTY_PIECE; pieceColor[A1] = COLOR_NONE; }
                else if (to == E8 + 2) { board[E8 + 1] = board[H8]; pieceColor[E8 + 1] = currentSide; board[H8] = EMPTY_PIECE; pieceColor[H8] = COLOR_NONE; }
                else if (to == E8 - 2) { board[E8 - 1] = board[A8]; pieceColor[E8 - 1] = currentSide; board[A8] = EMPTY_PIECE; pieceColor[A8] = COLOR_NONE; }
            }

            if (piece == PIECE_KING)
            {
                if (currentSide == 0) castlingRights &= ~3;
                else castlingRights &= ~12;
            }
            if (from == A1) castlingRights &= ~2;
            if (from == H1) castlingRights &= ~1;
            if (from == A8) castlingRights &= ~8;
            if (from == H8) castlingRights &= ~4;
            if (to == A1) castlingRights &= ~2;
            if (to == H1) castlingRights &= ~1;
            if (to == A8) castlingRights &= ~8;
            if (to == H8) castlingRights &= ~4;

            if (piece == 6 || captured != EMPTY_PIECE)
                fiftyMoveRule = 0;
            else
                fiftyMoveRule++;

            currentSide = 1 - currentSide;
            opponentSide = 1 - currentSide;
            searchPly++;
            return true;
        }

        /// <summary>
        /// Unmake move. Original: PichheLe (RVA 0xEE6A98).
        /// </summary>
        public void UnmakeMove()
        {
            currentSide = 1 - currentSide;
            opponentSide = 1 - currentSide;
            searchPly--;

            HistoryMove hist = history.Pop();
            Move move = hist.move;
            int from = move.fromSquare;
            int to = move.toSquare;

            int piece = board[to];
            if (move.promotionPiece != 0)
                piece = 6;  // Undo promotion (back to pawn)
            board[from] = piece;
            pieces[from] = piece;
            pieceColor[from] = currentSide;
            board[to] = EMPTY_PIECE;
            pieces[to] = EMPTY_PIECE;
            pieceColor[to] = COLOR_NONE;

            if (hist.capture != EMPTY_PIECE)
            {
                board[to] = hist.capture;
                pieces[to] = hist.capture;
                pieceColor[to] = hist.capturedColor;
            }

            if ((move.moveFlags & Move.FLAG_EN_PASSANT) != 0 && hist.epCapturedSquare >= 0)
            {
                board[hist.epCapturedSquare] = hist.epCapturedPiece;
                pieces[hist.epCapturedSquare] = hist.epCapturedPiece;
                pieceColor[hist.epCapturedSquare] = hist.epCapturedColor;
            }

            if ((move.moveFlags & Move.FLAG_CASTLE) != 0)
            {
                if (to == E1 + 2) { board[H1] = board[E1 + 1]; pieceColor[H1] = currentSide; board[E1 + 1] = EMPTY_PIECE; pieceColor[E1 + 1] = COLOR_NONE; }
                else if (to == E1 - 2) { board[A1] = board[E1 - 1]; pieceColor[A1] = currentSide; board[E1 - 1] = EMPTY_PIECE; pieceColor[E1 - 1] = COLOR_NONE; }
                else if (to == E8 + 2) { board[H8] = board[E8 + 1]; pieceColor[H8] = currentSide; board[E8 + 1] = EMPTY_PIECE; pieceColor[E8 + 1] = COLOR_NONE; }
                else if (to == E8 - 2) { board[A8] = board[E8 - 1]; pieceColor[A8] = currentSide; board[E8 - 1] = EMPTY_PIECE; pieceColor[E8 - 1] = COLOR_NONE; }
            }

            castlingRights = hist.castlingRights;
            enPassantSquare = hist.ep;
            fiftyMoveRule = hist.fiftyMoveRule;
        }

        // ================================================================
        // ATTACK DETECTION
        // ================================================================

        /// <summary>
        /// Check if square is attacked. Original: Attacks (RVA 0xEE7D28).
        /// </summary>
        public bool IsSquareAttacked(int s, int sv)
        {
            int[] pawnDirs = sv == 0 ? new[] { 15, 17 } : new[] { -15, -17 };
            foreach (int d in pawnDirs)
            {
                int from = s + d;
                if (OnBoard(from) && board[from] == 6 && pieceColor[from] == sv)
                    return true;
            }

            foreach (int d in knightOffsets)
            {
                int from = s + d;
                if (OnBoard(from) && board[from] == PIECE_KNIGHT && pieceColor[from] == sv)
                    return true;
            }

            foreach (int d in kingOffsets)
            {
                int from = s + d;
                if (OnBoard(from) && board[from] == PIECE_KING && pieceColor[from] == sv)
                    return true;
            }

            foreach (int d in bishopDirs)
            {
                int from = s + d;
                while (OnBoard(from))
                {
                    if (board[from] != EMPTY_PIECE)
                    {
                        if (pieceColor[from] == sv && (board[from] == PIECE_BISHOP || board[from] == CHAMYA))
                            return true;
                        break;
                    }
                    from += d;
                }
            }

            foreach (int d in rookDirs)
            {
                int from = s + d;
                while (OnBoard(from))
                {
                    if (board[from] != EMPTY_PIECE)
                    {
                        if (pieceColor[from] == sv && (board[from] == PIECE_ROOK || board[from] == CHAMYA))
                            return true;
                        break;
                    }
                    from += d;
                }
            }

            return false;
        }

        /// <summary>
        /// Check if side is in check. Original: InCheck (RVA 0xEE5D38).
        /// </summary>
        public bool IsInCheck(int s)
        {
            int kingSq = -1;
            for (int sq = 0; sq < 128; sq++)
            {
                if ((sq & 0x88) != 0) continue;
                if (board[sq] == PIECE_KING && pieceColor[sq] == s)
                {
                    kingSq = sq;
                    break;
                }
            }
            if (kingSq < 0) return false;
            return IsSquareAttacked(kingSq, 1 - s);
        }

        // ================================================================
        // EVALUATION
        // ================================================================

        /// <summary>
        /// Evaluate position. Original: EvaluateBoard → Evaluate (RVA 0xEE55AC).
        /// </summary>
        public int Evaluate()
        {
            int score = 0;

            for (int sq = 0; sq < 128; sq++)
            {
                if ((sq & 0x88) != 0) continue;
                if (board[sq] == EMPTY_PIECE) continue;

                int piece = board[sq];
                int color = pieceColor[sq];
                int value = pieceValues[piece < pieceValues.Length ? piece : 0];

                int pstSq = color == 0 ? sq : flipBoard[sq];
                int pstScore = piece switch
                {
                    6 => pawnPST[pstSq],         // PAWN
                    PIECE_KNIGHT => knightPST[pstSq],
                    PIECE_BISHOP => bishopPST[pstSq],
                    PIECE_KING => IsEndgame() ? kingEndgamePST[pstSq] : kingPST[pstSq],
                    _ => 0
                };

                score += color == 0 ? (value + pstScore) : -(value + pstScore);
            }

            return currentSide == 0 ? score : -score;
        }

        /// <summary>Eval white pawn. Original: EvalWhitePawn (RVA 0xEE7590).</summary>
        private int EvalWhitePawn(int s) => pawnPST[s];

        /// <summary>Eval black pawn. Original: EvalBlackPawn (RVA 0xEE78A4).</summary>
        private int EvalBlackPawn(int s) => pawnPST[flipBoard[s]];

        /// <summary>Eval white king. Original: EvalWhiteKing (RVA 0xEE7710).</summary>
        private int EvalWhiteKing(int s) => IsEndgame() ? kingEndgamePST[s] : kingPST[s];

        /// <summary>Eval black king. Original: EvalBlackKing (RVA 0xEE7A40).</summary>
        private int EvalBlackKing(int s) => IsEndgame() ? kingEndgamePST[flipBoard[s]] : kingPST[flipBoard[s]];

        /// <summary>Eval white king endgame. Original: EvalWhiteKingPawn → EvalWhiteKingEndgame (RVA 0xEE7BEC).</summary>
        private int EvalWhiteKingEndgame(int f) => kingEndgamePST[f];

        /// <summary>Eval black king endgame. Original: EvalBlackKingPawn → EvalBlackKingEndgame (RVA 0xEE7C8C).</summary>
        private int EvalBlackKingEndgame(int f) => kingEndgamePST[flipBoard[f]];

        private bool IsEndgame()
        {
            int totalMaterial = 0;
            for (int sq = 0; sq < 128; sq++)
            {
                if ((sq & 0x88) != 0) continue;
                if (board[sq] == EMPTY_PIECE || board[sq] == PIECE_KING) continue;
                int p = board[sq];
                if (p < pieceValues.Length)
                    totalMaterial += pieceValues[p];
            }
            return totalMaterial < 2600;
        }

        // ================================================================
        // HELPERS
        // ================================================================

        private int ScoreMove(Move move)
        {
            int score = 0;
            if ((move.moveFlags & Move.FLAG_CAPTURE) != 0)
            {
                int victim = board[move.toSquare];
                int attacker = board[move.fromSquare];
                int victimVal = victim < pieceValues.Length ? pieceValues[victim] : 0;
                int attackerVal = attacker < pieceValues.Length ? pieceValues[attacker] : 0;
                score += victimVal * 10 - attackerVal;
            }
            if ((move.moveFlags & Move.FLAG_PROMOTION) != 0)
                score += move.promotionPiece < pieceValues.Length ? pieceValues[move.promotionPiece] : 0;
            return score;
        }

        private static bool OnBoard(int sq) => (sq & 0x88) == 0;

        private static string MoveToCAN(Move move)
        {
            int fromFile = move.fromSquare % 16;
            int fromRank = 8 - (move.fromSquare / 16);
            int toFile = move.toSquare % 16;
            int toRank = 8 - (move.toSquare / 16);
            string result = $"{(char)('a' + fromFile)}{fromRank}{(char)('a' + toFile)}{toRank}";
            if (move.promotionPiece != 0)
            {
                result += move.promotionPiece switch
                {
                    5 => 'q', 4 => 'r', 3 => 'b', 2 => 'n', _ => '?'
                };
            }
            return result;
        }

        public override int GetHashCode() => base.GetHashCode();

        // ================================================================
        // PUBLIC ACCESSORS (for use by MainScript)
        // ================================================================

        public int CurrentSide => currentSide;
        public int CastlingRights => castlingRights;
        public int EnPassantSquare => enPassantSquare;
        public int FiftyMoveRule => fiftyMoveRule;
        public int FullMoveNumber => fullMoveNumber;
        public int SearchPly => searchPly;
        public bool IsThinking => isThinking;
        public Move BestMove => bestMove;
    }
}
