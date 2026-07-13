using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace EivaaChess.Game
{
    /// <summary>
    /// Save System. TypeDefIndex: 6189.
    ///
    /// Original fields:
    ///   private const string FL_NMo = "/save.dat";
    ///   private const string FL_NMn = "/mone.tik";
    ///
    /// Original methods:
    ///   Save(SaveBlockNew d)         — RVA 0xED5548
    ///   Load() → SaveBlockNew        — RVA 0xED57D4
    ///   Etargim2(SaveBlockNew, SaveBlockOld) — RVA 0xED5C5C (migration)
    ///   GPID() → string              — RVA 0xED5B74 (get player ID)
    /// </summary>
    public class SaveSystem
    {
        private const string FL_NMo = "/save.dat";
        private const string FL_NMn = "/mone.tik";

        public SaveBlockNew saveData;

        public SaveSystem()
        {
            saveData = new SaveBlockNew();
            Load();
        }

        private string SavePath => Application.persistentDataPath + FL_NMo;
        private string SlotPath => Application.persistentDataPath + FL_NMn;

        /// <summary>
        /// Save game state. Original: Save(SaveBlockNew d).
        /// </summary>
        public void Save(SaveBlockNew d)
        {
            saveData = d;
            Save();
        }

        public void Save()
        {
            try
            {
                string json = JsonUtility.ToJson(saveData, true);
                File.WriteAllText(SavePath, json);
            }
            catch (Exception e)
            {
                Debug.LogError($"Save failed: {e.Message}");
            }
        }

        /// <summary>
        /// Load game state. Original: Load().
        /// </summary>
        public SaveBlockNew Load()
        {
            try
            {
                if (File.Exists(SavePath))
                {
                    string json = File.ReadAllText(SavePath);
                    saveData = JsonUtility.FromJson<SaveBlockNew>(json);
                    if (saveData == null) saveData = new SaveBlockNew();
                }
                else
                {
                    saveData = new SaveBlockNew();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Load failed: {e.Message}");
                saveData = new SaveBlockNew();
            }
            return saveData;
        }

        /// <summary>
        /// Migrate from old format to new. Original: Etargim2(SaveBlockNew fst, SaveBlockOld snd).
        /// </summary>
        public void Etargim2(SaveBlockNew fst, SaveBlockOld snd)
        {
            if (snd == null) return;
            fst.eid = snd.egPlayerID;
            fst.pN = snd.playerNames;
            fst.cAv = snd.chosenAvatar;
            fst.tPVl = snd.toPromoteVal;
            fst.dmD = snd.aiDifficulty;
            fst.udOn = snd.undoOptionOn;
            fst.snV = snd.soundEnabled ? 1f : 0f;
            fst.msV = snd.musicVolVal;
            fst.senV = snd.sensitivityValue;
            fst.tnOr = snd.turnOrientationOn;
            fst.fsOn = snd.fullscreenOn;
            fst.sLegM = snd.showLegalMoves;
            fst.sLstM = snd.showLastMove;
            fst.pMvS = snd.pieceMoveSpeed;
            fst.sTb = snd.selectedTable;
            fst.sBB = snd.selectedBoardBorder;
            fst.sCh = snd.selectedCheckers;
            fst.sPT = snd.selectedPieceType;
            fst.bNO = snd.boardNumberingOn;
            fst.rmE = snd.roomEnabled;
            fst.tExO = snd.tableExtrasOn;
            fst.ttP = snd.timePlayed;
            fst.gPVC = snd.gamesPlayedVsCPU;
            fst.gWVC = snd.gamesWonVsCPU;
            fst.gPVH = snd.gamesPlayedVsHuman;
            fst.gWVH = snd.gamesWonVsHuman;
            fst.tDr = snd.tDraws;
            fst.tEnP = snd.tEnPassant;
            fst.tPr = snd.tPromotion;
            fst.tCTO = snd.tCheckToOpponent;
            fst.tCBO = snd.tCheckByOpponent;
            fst.bSHlp = snd.bShowHelp;
        }

        /// <summary>
        /// Get player ID. Original: GPID().
        /// </summary>
        public string GPID()
        {
            if (!string.IsNullOrEmpty(saveData.eid))
                return saveData.eid;
            saveData.eid = Guid.NewGuid().ToString();
            Save();
            return saveData.eid;
        }
    }

    /// <summary>
    /// Save block (old format). TypeDefIndex: 6187.
    /// </summary>
    [Serializable]
    public class SaveBlockOld
    {
        public string egPlayerID;       // 0x10
        public string[] playerNames;    // 0x18
        public int[] chosenAvatar;      // 0x20
        public int toPromoteVal;        // 0x28
        public Difficulty aiDifficulty; // 0x2C
        public bool undoOptionOn;       // 0x30
        public bool soundEnabled;       // 0x31
        public float musicVolVal;       // 0x34
        public float sensitivityValue;  // 0x38
        public bool turnOrientationOn;  // 0x3C
        public bool fullscreenOn;       // 0x3D
        public bool showLegalMoves;     // 0x3E
        public bool showLastMove;       // 0x3F
        public float pieceMoveSpeed;    // 0x40
        public int selectedTable;       // 0x44
        public int selectedBoardBorder;// 0x48
        public int selectedCheckers;    // 0x4C
        public int selectedPieceType;   // 0x50
        public bool boardNumberingOn;   // 0x54
        public bool roomEnabled;        // 0x55
        public bool tableExtrasOn;      // 0x56
        public int timePlayed;          // 0x58
        public int gamesPlayedVsCPU;    // 0x5C
        public int gamesWonVsCPU;       // 0x60
        public int gamesPlayedVsHuman;  // 0x64
        public int gamesWonVsHuman;     // 0x68
        public int tDraws;              // 0x6C
        public int tEnPassant;          // 0x70
        public int tPromotion;          // 0x74
        public int tCheckToOpponent;    // 0x78
        public int tCheckByOpponent;    // 0x7C
        public bool bShowHelp;          // 0x80
    }

    /// <summary>
    /// Save block (new format). TypeDefIndex: 6188.
    /// </summary>
    [Serializable]
    public class SaveBlockNew
    {
        public string eid;          // 0x10 — player ID
        public string[] pN;         // 0x18 — player names
        public int[] cAv;           // 0x20 — chosen avatars
        public int tPVl;            // 0x28 — to promote value
        public Difficulty dmD;      // 0x2C — AI difficulty
        public bool udOn;           // 0x30 — undo option on
        public float snV;           // 0x34 — sound volume
        public float msV;           // 0x38 — music volume
        public float senV;          // 0x3C — sensitivity
        public bool tnOr;           // 0x40 — turn orientation on
        public bool fsOn;           // 0x41 — fullscreen on
        public bool sLegM;          // 0x42 — show legal moves
        public bool sLstM;          // 0x43 — show last move
        public float pMvS;          // 0x44 — piece move speed
        public int sTb;             // 0x48 — selected table
        public int sBB;             // 0x4C — selected board border
        public int sCh;             // 0x50 — selected checkers
        public int sPT;             // 0x54 — selected piece type
        public bool bNO;            // 0x58 — board numbering on
        public bool rmE;            // 0x59 — room enabled
        public bool tExO;           // 0x5A — table extras on
        public int ttP;             // 0x5C — total time played
        public int gPVC;            // 0x60 — games played vs CPU
        public int gWVC;            // 0x64 — games won vs CPU
        public int gPVH;            // 0x68 — games played vs human
        public int gWVH;            // 0x6C — games won vs human
        public int tDr;             // 0x70 — total draws
        public int tEnP;            // 0x74 — total en passant
        public int tPr;             // 0x78 — total promotions
        public int tCTO;            // 0x7C — total check to opponent
        public int tCBO;            // 0x80 — total check by opponent
        public bool bSHlp;          // 0x84 — show help
        public bool kR;             // 0x85 — keep rating
        public System.Collections.Generic.List<int> lsD;     // 0x88 — list of slots done
        public int cSlt;            // 0x90 — current slot
        public System.Collections.Generic.List<MoneSlt> slts;// 0x98 — slots
    }

    /// <summary>
    /// Per-slot save data. TypeDefIndex: 6190.
    /// </summary>
    [Serializable]
    public class MoneSlt
    {
        public bool e;              // 0x10 — exists
        public string FN;           // 0x18 — FEN
        public string mvL;          // 0x20 — move list
        public MainScript.GAME_MODE gM; // 0x28 — game mode
        public MainScript.MODE_TYPE mdT; // 0x2C — mode type
        public Difficulty dmD;      // 0x30 — difficulty
        public bool p1W;            // 0x34 — player 1 is white
        public int[] cC;            // 0x38 — captured count
        public System.Collections.Generic.List<string> cPW; // 0x40 — captured pieces white
        public System.Collections.Generic.List<string> cPB; // 0x48 — captured pieces black
        public string pMC;          // 0x50 — promo choice
        public long d;              // 0x58 — date

        public MoneSlt()
        {
            e = false;
            FN = "";
            mvL = "";
            gM = MainScript.GAME_MODE.VsCPU;
            mdT = MainScript.MODE_TYPE.Classic;
            dmD = Difficulty.DO;
            p1W = true;
            cC = new int[2];
            cPW = new System.Collections.Generic.List<string>();
            cPB = new System.Collections.Generic.List<string>();
            pMC = "";
            d = 0;
        }
    }
}
