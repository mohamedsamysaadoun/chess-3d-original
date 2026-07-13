using System;
using UnityEngine;

namespace EivaaChess.Game
{
    /// <summary>
    /// Move Coordinator.
    /// Original obfuscated name: EnSonchalok (Marathi: "current move handler").
    /// TypeDefIndex: 6150
    ///
    /// Original fields:
    ///   private string curPositionList;        // 0x10 (property)
    ///   private string curMovesList;           // 0x18 (property)
    ///   public bool chalatAahe;                // 0x20 — is animating
    ///   public string suruPHAN;                // 0x28 — starting FEN
    ///   public Player curMovingSide;           // 0x30
    ///   public string movesToDo;               // 0x38 — queued moves
    ///   public string inCheckFromPlace;        // 0x40
    ///   private string possibleCheckFromPlace; // 0x48
    ///   private SolDt solDt;                   // 0x50
    ///   private int ePLMvdPn;                  // 0x58
    ///   public string promoteToWhat;           // 0x60
    ///   private string bcPmtCtl;               // 0x68
    /// </summary>
    public class EnSonchalok
    {
        [SerializeField] private string curPositionList;
        [SerializeField] private string curMovesList;
        public bool chalatAahe;                // 0x20
        public string suruPHAN;                // 0x28
        public Player curMovingSide;           // 0x30
        public string movesToDo;               // 0x38
        public string inCheckFromPlace;        // 0x40
        private string possibleCheckFromPlace; // 0x48
        private int ePLMvdPn;                  // 0x58
        public string promoteToWhat;           // 0x60
        private string bcPmtCtl;               // 0x68

        public string curPositionListProp
        {
            get => curPositionList;
            set => curPositionList = value;
        }

        public string curMovesListProp
        {
            get => curMovesList;
            set => curMovesList = value;
        }

        public EnSonchalok()
        {
            chalatAahe = false;
            curMovingSide = Player.EK;
            movesToDo = "";
            inCheckFromPlace = "";
            promoteToWhat = "";
        }

        public void Init(string fen)
        {
            suruPHAN = fen;
            curPositionList = fen;
            curMovesList = "";
            chalatAahe = false;
            curMovingSide = Player.EK;
            movesToDo = "";
            inCheckFromPlace = "";
            promoteToWhat = "";
        }

        public void AppendMove(string moveCAN)
        {
            if (string.IsNullOrEmpty(curMovesList))
                curMovesList = moveCAN;
            else
                curMovesList += ";" + moveCAN;
        }

        public string GetLastMove()
        {
            if (string.IsNullOrEmpty(curMovesList)) return null;
            var moves = curMovesList.Split(';');
            return moves.Length > 0 ? moves[moves.Length - 1] : null;
        }

        public int GetMoveCount()
        {
            if (string.IsNullOrEmpty(curMovesList)) return 0;
            return curMovesList.Split(';').Length;
        }

        public void SwitchSide()
        {
            curMovingSide = curMovingSide == Player.EK ? Player.DO : Player.EK;
        }

        public void Reset()
        {
            curPositionList = suruPHAN;
            curMovesList = "";
            chalatAahe = false;
            curMovingSide = Player.EK;
            movesToDo = "";
            inCheckFromPlace = "";
            promoteToWhat = "";
        }

        // Original methods (RVA 0xECD250-0xECD4AC)
        private int CharCodeAt(string str, int at) => str[at];
        private static string FromCharCode(int cd) => char.ToString((char)cd);
        private int ParseInt(string str) => int.Parse(str);
        private string Convert88h8T(int vert, int horiz) => $"{(char)('a' + horiz)}{8 - vert}";
    }
}
