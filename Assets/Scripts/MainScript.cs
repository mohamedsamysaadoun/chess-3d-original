using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EivaaChess.Game
{
    /// <summary>
    /// Main Script — the main game controller.
    /// TypeDefIndex: 6181
    ///
    /// In the original game this was a God class with 100+ fields and 200+ methods
    /// managing ALL aspects of the game (state, UI, camera, animation, audio,
    /// indicators, tutorials, settings, save, ads).
    ///
    /// This reconstruction preserves the original organization (one big class)
    /// and field names. Method bodies are reconstructed based on:
    /// - The working engine-test implementation (for chess engine calls)
    /// - Field names and standard Unity patterns (for UI/camera/animation)
    /// - Frida traces (for runtime behavior)
    /// </summary>
    public class MainScript : MonoBehaviour
    {
        // ================================================================
        // CONSTANTS (from dump.cs)
        // ================================================================

        private const float SCREEN_REF_WIDTH = 480;
        private const float W_P_S_I = 60;
        private const float T_D_P_W = 15;
        private const int T_SV_SLTS = 3;
        private const int T_TUTORIALS = 16;
        private const int EN_P_IDX = 15;
        private const int GOOC_IDX = 9;
        private const int T_INDIC = 40;
        private const int MX_UNDO_ALLOWED = 2;
        private const int T_TABLES = 2;
        private const int T_BOARDS_BORDERS = 4;
        private const int T_CHECKERS = 3;
        private const int T_GUTI_TYPES = 3;
        private const int T_SETT_GRPS = 3;
        private const int T_AVATARS = 11;
        private const float CAM_Y_DEFAULT = 40;
        private const float CAM_Y_MIN = 20;
        private const float CAM_Y_MAX = 70;
        private const float CAM_DIST_DEFAULT = 19;
        private const float CAM_DIST_MIN = 15;
        private const float CAM_DIST_MAX = 45;
        private const float GANA_VOL_MUL_MENU = 1;
        private const float GANA_VOL_MUL_IG = 0.7f;

        // ================================================================
        // STATIC FIELDS (from dump.cs)
        // ================================================================

        public static int strtGino;       // 0x0
        public static string egPSID;      // 0x8
        public static Player currentTurn; // 0x10
        public static Dictionary<string, string> inChkKahaKahaSe; // 0x18
        public static bool bAnimateGui;   // 0x20
        public static float btnAnimValue; // 0x24
        public static float gameNamePos;  // 0x28
        public static bool adsRemoved;    // 0x2C
        public static int adDisplacement; // 0x30

        // ================================================================
        // INSTANCE FIELDS — UI / CANVAS
        // ================================================================

        private Rect canvasSize;          // 0x20
        private float screenWidth;        // 0x30
        private float screenHeight;       // 0x34
        private float screenMul;          // 0x38
        private readonly string[] gmNms;  // 0x40
        private readonly string[] mdTNms; // 0x48

        // ================================================================
        // GAME STATE
        // ================================================================

        private MainScript.GAME_MODE gameMode;     // 0x50
        private MainScript.MODE_TYPE modeType;     // 0x54
        private int scoreValue;                    // 0x58
        private bool bTossDone;                    // 0x5C
        private bool bGamePaused;                  // 0x5D
        private bool bGameComplete;                // 0x5E
        private Player gameWinner;                 // 0x60
        private bool bGameWasDraw;                 // 0x64
        private string[] playerInGameNames;        // 0x68

        // ================================================================
        // AUDIO
        // ================================================================

        [Space(15, order = 0)]
        [Header("Audio", order = 1)]
        public AudioClip[] musicsArray;            // 0x70
        public AudioClip buttonClickSound;         // 0x78
        public AudioClip promoteSound;             // 0x80
        public AudioClip kingCheckSound;           // 0x88
        public AudioClip gameWinSound;             // 0x90

        // ================================================================
        // SCENE REFERENCES
        // ================================================================

        private bool initComplete;                  // 0x98
        private Transform thisTransform;            // 0xA0
        private Transform piecesParentTrans;        // 0xA8
        private Transform piecesOutParentTrans;     // 0xB0
        private Transform boardPlacesParentTrans;   // 0xB8
        private Transform checkersParentTrans;      // 0xC0
        private GameObject[][] piecePrefabsArray;   // 0xC8
        private readonly string[] piecePrefabsNames;// 0xD0
        private Vector3[] moveOutPlacesWArray;      // 0xD8
        private Vector3[] moveOutPlacesBArray;      // 0xE0

        // ================================================================
        // INDICATORS
        // ================================================================

        private Transform indicatorMoveParentTrans; // 0xE8
        private GameObject indicatorBlueObj;        // 0xF0
        private GameObject indicatorGreenObj;       // 0xF8
        private GameObject indicatorRedObj;         // 0x100
        private Transform indicatorDangerParentTrans; // 0x108
        private GameObject indicatorDangerObj;      // 0x110
        private GameObject IndicatorCheckParentObj; // 0x118
        private LineRenderer indicatorCheckLineRend;// 0x120
        private Transform IndicatorCheckRound1;     // 0x128
        private Transform IndicatorCheckRound2;     // 0x130
        private ParticleSystem promotPrticlSy;      // 0x138
        private GameObject boardNumberingCanvasObj; // 0x140

        // ================================================================
        // AUDIO SOURCES
        // ================================================================

        private AudioSource thisAudioSource;        // 0x148
        private AudioSource musicAudioSource;       // 0x150

        // ================================================================
        // SAVE SYSTEM
        // ================================================================

        private SaveSystem saveSystem;              // 0x158
        private SaveBlockNew saveData;              // 0x160
        private int selctdSlt;                      // 0x168
        private Text[] sltBtnsText;                 // 0x170
        private GameObject[] sltDltBtns;            // 0x178
        private int slt2Dlt;                        // 0x180
        private Coroutine SLCoroutine;              // 0x188

        // ================================================================
        // TURN TIMER
        // ================================================================

        private bool bTurnTimerRunning;             // 0x190
        private float turnFullTime;                 // 0x194
        private float turnTimeRemaining;            // 0x198

        // ================================================================
        // TUTORIAL
        // ================================================================

        private int curTutorialScreen;              // 0x19C
        private bool tutCanClick;                   // 0x1A0
        private bool bThisTutDone;                  // 0x1A1
        private readonly string[] tutFenArray;      // 0x1A8
        private int tutCurMove;                     // 0x1B0
        private int tutCurText;                     // 0x1B4
        private string tutLastDoneClick;            // 0x1B8
        private readonly string[][] tutMovesArr;    // 0x1C0
        private readonly string[][] tutTexts;       // 0x1C8
        private GameObject sqToTapIndicObj;         // 0x1D0
        private ParticleSystem sqToTapIndicPar;     // 0x1D8
        private GameObject tutTextBoxObj;           // 0x1E0
        private RectTransform tutTextBoxRectTr;     // 0x1E8
        private Text tutTextText;                   // 0x1F0
        private ParticleSystem tutTextCmplPrtclSys; // 0x1F8
        private GameObject tutStepsNextBtn;         // 0x200
        private GameObject tutScrnBtnsGroup;        // 0x208
        private GameObject tutScrnBtnsNext;         // 0x210
        private GameObject tutScrnBtnsMM;           // 0x218
        private GameObject tutCompldObj;            // 0x220
        private GameObject[] tutSelBtnsDon;         // 0x228
        private Coroutine tutTextCoro;              // 0x230

        // ================================================================
        // LOBBY (online)
        // ================================================================

        private GameObject lobbyBackBtnObj;         // 0x238
        private Text[] lobbyNames;                  // 0x240
        private Image[] lobbyAvatars;               // 0x248
        private GameObject lobbyLoading;            // 0x250

        // ================================================================
        // MOVE COORDINATOR
        // ================================================================

        private EnSonchalok enSonchalok;            // 0x258
        private readonly string[] toPromoteArray;   // 0x260
        private readonly float[] piecesYPosArray;   // 0x268

        // ================================================================
        // PLAYER / PIECE STATE
        // ================================================================

        private bool player1isWhite;                // 0x270
        private bool waitingHumanForMove;           // 0x271
        private Player humanPlayerSide;             // 0x274
        private int[] capturedCountArray;           // 0x278
        private List<string> capturedPiecesWhiteArray; // 0x280
        private List<string> capturedPiecesBlackArray; // 0x288
        private List<Transform> ghodaLst;           // 0x290

        // ================================================================
        // INDICATOR POOL
        // ================================================================

        private bool indPoolCreated;                // 0x298
        private Transform[] indicRedAray;           // 0x2A0
        private Transform indicBlueTr;              // 0x2A8
        private Transform[] indicGreenAray;         // 0x2B0
        private int curLalIndicIdx;                 // 0x2B8
        private int curHaraIndicIdx;                // 0x2BC
        public LayerMask placesLayerMask;           // 0x2C0

        // ================================================================
        // INPUT
        // ================================================================

        private string currentClickedAt;            // 0x2C8
        private string selectedPieceAt;             // 0x2D0
        private readonly string[] allMovesPlacesIDs;// 0x2D8

        // ================================================================
        // CHESS ENGINE
        // ================================================================

        private ChessEngine sechDMG;                // 0x2E0 — was the engine
        private string dmgAnurodhStr;               // 0x2E8 — AI request
        private string dmgJawabClbckPaain;          // 0x2F0 — AI response
        private string dmgChalAgaruPHAN;            // 0x2F8 — FEN after AI move
        private readonly string[] hrdnesNams;       // 0x300 — difficulty names
        private readonly int[] musklAray;           // 0x308 — difficulty values
        private int dynDmgKinnaHaarJit;             // 0x310

        // ================================================================
        // ANIMATION
        // ================================================================

        private string moveFromPlace;               // 0x318
        private string moveToPlace;                 // 0x320
        private string cpVal;                       // 0x328
        private GameObject pieceToMoveObj;          // 0x330
        private Transform pieceToMoveTrans;         // 0x338
        private Transform pieceInnerMeshTrans;      // 0x340
        private Transform pieceGoingOutInnerMeshTrans; // 0x348
        private string curMovingPieceColor;         // 0x350
        private string curMovingPieceType;          // 0x358
        private Vector3 moveFromPos;                // 0x360
        private Vector3 moveToPos;                  // 0x36C
        private GameObject pieceToGoOutObj;         // 0x378
        private Transform pieceToGoOutTrans;        // 0x380
        private Vector3 goOutFromPos;               // 0x388
        private string capturedPieceColor;          // 0x398
        private string capturedPieceType;           // 0x3A0
        private int tFldRepCountr;                  // 0x3A8
        private List<string> tFldRepLst;            // 0x3B0
        private bool bCurrentlyAnimMoving;          // 0x3B8
        private float moveAnimValue;                // 0x3BC
        private float moveAnimTimer;                // 0x3C0
        private bool bShnCPMITM;                    // 0x3C4

        // ================================================================
        // UNDO
        // ================================================================

        private int undoDoneCounter;                // 0x3C8
        private string prevMoveCaptureOrNot;        // 0x3D0
        private string movesUndoToDo;               // 0x3D8
        private string undoComeInToPlace;           // 0x3E0
        private Vector3 undoComeInToPos;            // 0x3E8

        // ================================================================
        // GAME OVER UI
        // ================================================================

        private Text gmOvrTitleText;                // 0x3F8
        private GameObject[] gmOvrAvatarObjs;       // 0x400
        private Text[] gmOvrNameTexts;              // 0x408
        private Transform gmOvrWinGTrans;           // 0x410
        private Text[] gmOvrBtnsText;               // 0x418
        private GameObject gmOvrWinPartcGObj;       // 0x420
        private GameObject gmOvrWinPartcOnceObj;    // 0x428
        private GameObject gmOvrUndoBtn;            // 0x430
        private bool bReviewBoard;                  // 0x438

        // ================================================================
        // CAMERA / INPUT
        // ================================================================

        private Vector2 inputValues;                // 0x43C
        private Vector2 inputToRotValue;            // 0x444
        private Vector2 inputToRotTarget;           // 0x44C
        private Vector2 inputToRotVel;              // 0x454
        private Transform camObjTrans;              // 0x460
        private Camera camObjCam;                   // 0x468
        private Transform camParentObjMainMenuTrans;// 0x470
        private Transform camParentObjInGameTrans;  // 0x478
        private Transform camParentObjTopTrans;     // 0x480
        private Transform camParentObjCustmzATrans; // 0x488
        private Transform camParentObjCustmzBTrans; // 0x490
        private BlurOptimized camBlurComp;          // 0x498
        private MainScript.CAM_MODE cameraMode;     // 0x4A0
        private MainScript.CAM_MODE cameraPrevMode; // 0x4A4
        private bool bCamCanRotate;                 // 0x4A8
        private bool bCamIsEasing;                  // 0x4A9
        private bool bCamLocked;                    // 0x4AA
        private float camDistance;                  // 0x4AC
        private Quaternion camParentRotation;       // 0x4B0
        private float camParentRotValueY;           // 0x4C0
        private float camParentRotVelY;             // 0x4C4
        private Vector3 camParentPosVel;            // 0x4C8
        private Vector3 camLocalPosVel;             // 0x4D4
        private Vector3 camLocalRotVel;             // 0x4E0
        private float camBobAnimVal;                // 0x4EC
        private Vector3 camParentMMStartPos;        // 0x4F0
        private Vector3 camParentMMStartRot;        // 0x4FC
        private bool doNormalCameraBtnAction;       // 0x508
        private float igCamBtnsAnimTarget;          // 0x50C
        private Coroutine igCamBtnsCoroutine;       // 0x510
        private Matrix4x4 mtrO;                     // 0x518
        private Matrix4x4 mtrP;                     // 0x558
        private Coroutine mtrC;                     // 0x598
        private float turnSwitchFadeAlphaVal;       // 0x5A0

        // ================================================================
        // UI SCREENS
        // ================================================================

        private GameObject[] screensObjArray;       // 0x5A8
        private MainScript.Parda curScreen;         // 0x5B0
        private MainScript.Parda prevScreen;        // 0x5B4
        private MainScript.Parda scrnToGoAfterAnim; // 0x5B8
        private float btnAnimVel;                   // 0x5BC
        private float btnAnimTarget;                // 0x5C0
        private Action whatToDoAfterMenuAnim;       // 0x5C8
        private GameObject gameNameObj;             // 0x5D0
        private float gameNameTargetPos;            // 0x5D8
        private float gameNameVel;                  // 0x5DC
        private bool bGameNameHidden;               // 0x5E0
        private bool showHelpScreen;                // 0x5E1

        // ================================================================
        // IN-GAME UI
        // ================================================================

        private RectTransform canvAParentRTrans;    // 0x5E8
        private GameObject continueGameBtnObj;      // 0x5F0
        private GameObject highScoresBtnBadge;      // 0x5F8
        private CanvasGroup igTurnSwitchFadeCG;     // 0x600
        private GameObject igUndoMoveBtnObj;        // 0x608
        private GameObject igPauseBtnObj;           // 0x610
        private GameObject igBackBtnObj;            // 0x618
        private Text igBlinkingTextText;            // 0x620
        private Text igScoreDisplayText;            // 0x628
        private RectTransform igCamBtnsGrpTrans;    // 0x630
        private CanvasGroup igCamBtnsParentCanvasGrp; // 0x638
        private Text igCam2D3DBtnText;              // 0x640
        private GameObject igCamLockBtnTickObj;     // 0x648
        private GameObject igCamBtnLockActiveObj;   // 0x650
        private GameObject igPlayersParent;         // 0x658
        private Transform igPlayerTurnIndicator;    // 0x660
        private Image igPlayerTurnIndicImg;         // 0x668
        private Text[] igPlayerNameTexts;           // 0x670
        private Text[] igPlayerScoreTexts;          // 0x678
        private Image[] igPlayerAvatarImgs;         // 0x680
        private Transform[] igPlayerScaleParentTrans; // 0x688
        private Vector3[] igPlayerScaleParentVel;   // 0x690
        private Vector3[] igPlayerScaleParentTarget;// 0x698
        private readonly Vector3 igPlayerScaleSmall;// 0x6A0
        private readonly Vector3 igPlayerScaleBig;  // 0x6AC

        // ================================================================
        // PAUSE / NOTIFICATIONS / MSG BOX
        // ================================================================

        private Text pauseStatsText;                // 0x6B8
        private string notifOptionalTextPrefix;     // 0x6C0
        private float notifAnimValue;               // 0x6C8
        private Coroutine notifTopCoro;             // 0x6D0
        private RectTransform notifRectTrans;       // 0x6D8
        private float notifTargetPosY;              // 0x6E0
        private Text notifTextComp;                 // 0x6E8
        private Image[] notifAvtrImages;            // 0x6F0
        private RectTransform igNotfBtmRectT;       // 0x6F8
        private Text igNotfBtmText;                 // 0x700
        private Coroutine notifBtmCoro;             // 0x708
        private Text msgBoxMsgText;                 // 0x710
        private GameObject[] msgBoxBtnObjs;         // 0x718
        private Text[] msgBoxBtnTexts;              // 0x720
        private Action[] msgCBAction;               // 0x728

        // ================================================================
        // CUSTOMIZE
        // ================================================================

        private GameObject tableExtrasParentObj;    // 0x730
        private GameObject[] tablesObjArray;        // 0x738
        private GameObject[] boardBorderObjArray;   // 0x740
        private Texture checkersTexture;            // 0x748
        private Renderer checkersRenderer;          // 0x750
        public Material textColorBoardMat;          // 0x758
        private readonly Color[] textColorBoardColor;// 0x760
        private Text cuTableBtnDyTxt;               // 0x768
        private Text cuBoardBorderBtnDyTxt;         // 0x770
        private Text cuCheckersBtnDyTxt;            // 0x778
        private Text cuPiecesBtnDyTxt;              // 0x780

        // ================================================================
        // SETTINGS
        // ================================================================

        private Slider settMvSpeedSldr;             // 0x788
        private Slider toPromoteSldr;               // 0x790
        private int curSettGrp;                     // 0x798
        private GameObject[] settGrpsArray;         // 0x7A0
        private Transform[] settTabBtns;            // 0x7A8
        private Transform settSelTabArowTrans;      // 0x7B0
        private GameObject[] rulesGroupsArray;      // 0x7B8
        private Transform[] rulesTabBtnsTransArray; // 0x7C0
        private Transform rulesSelectedTabArrowTrans; // 0x7C8

        // ================================================================
        // AVATARS
        // ================================================================

        private int avtrPlrToChoose;                // 0x7D0
        private readonly string[] avtrNames;        // 0x7D8
        [SerializeField]
        private Sprite[] avatarTexArray;            // 0x7E0
        private Text selAvtrTitleText;              // 0x7E8
        [Space(15)]
        public Sprite[] playerColorImgs;            // 0x7F0

        // ================================================================
        // GAME SETUP
        // ================================================================

        private Text gopTitle2;                     // 0x7F8
        private Text[] gopPlayerBtnTexts;           // 0x800
        private GameObject[] gopPlayerErrObjs;      // 0x808
        private Image[] gopAvatars;                 // 0x810
        private Image[] gopPlayerColorImg;          // 0x818
        private Slider gopDiffSliderComp;           // 0x820
        private GameObject gopSkillGroup;           // 0x828
        private GameObject gopErrorObj;             // 0x830

        // ================================================================
        // MISC
        // ================================================================

        private bool alreadyAskedToRate;            // 0x838
        private int timeAlreadySaved;               // 0x83C
        private Text[] statsUITitlesArray;          // 0x840
        private Text[] statsUIValuesArray;          // 0x848
        private Coroutine blurCoroutine;            // 0x850
        private bool firstEntryAdShown;             // 0x858
        private GameObject iapLoadingObj;           // 0x860
        private GameObject iapParentObj;            // 0x868
        private Text rAdsBText;                     // 0x870
        private bool levelLoadingDone;              // 0x878
        private GameObject firstLoadingParentObj;   // 0x880

        // ================================================================
        // LIFECYCLE METHODS
        // ================================================================

        /// <summary>Awake. RVA: 0xEB2468.</summary>
        private void Awake()
        {
            // Initialize static fields
            if (inChkKahaKahaSe == null)
                inChkKahaKahaSe = new Dictionary<string, string>();

            thisTransform = transform;
            sechDMG = new ChessEngine();
            enSonchalok = new EnSonchalok();
            saveSystem = new SaveSystem();
            saveData = saveSystem.Load();

            // Apply loaded settings
            ApplyLoadedSettings();
        }

        /// <summary>Start. RVA: 0xEB29CC.</summary>
        private void Start()
        {
            FindGameObjects();
            FindUIObjects();
            SetupUIFunctions();
            CustomizeInit();
            InGameUIInit();
            NotifInit();
            MsgBoxInit();
            TutorialInit();
            AvatarsInit();
            CameraInit();
            CameraMatrixInit();
            InputInit();
            MenuSystemInit();

            initComplete = true;
            ShowScreen(MainScript.Parda.MainMenu);
            StartNextStep();
        }

        /// <summary>StartNextStep coroutine. RVA: 0xEB29EC.</summary>
        private IEnumerator StartNextStep()
        {
            yield return null;
            // Post-init setup
        }

        /// <summary>Update. RVA: 0xEBA080.</summary>
        private void Update()
        {
            if (!initComplete) return;

            InputUpdate();
            CameraUpdate();
            MenuSystemUpdate();

            if (bGamePaused || bGameComplete) return;

            if (bTurnTimerRunning)
                TurnTimerUpdate();

            if (bCurrentlyAnimMoving)
                GutiChalneKaEtadpu();
        }

        // ================================================================
        // GAME OBJECTS / UI FINDING
        // ================================================================

        /// <summary>FindGameObjects. RVA: 0xEB2A54.</summary>
        private void FindGameObjects()
        {
            // Find all scene references by name
            piecesParentTrans = GameObject.Find("PiecesParent")?.transform;
            piecesOutParentTrans = GameObject.Find("PiecesOutParent")?.transform;
            boardPlacesParentTrans = GameObject.Find("BoardPlacesParent")?.transform;
            checkersParentTrans = GameObject.Find("CheckersParent")?.transform;
            indicatorMoveParentTrans = GameObject.Find("IndicatorMoveParent")?.transform;
            indicatorDangerParentTrans = GameObject.Find("IndicatorDangerParent")?.transform;
            IndicatorCheckParentObj = GameObject.Find("IndicatorCheckParent");
            indicatorCheckLineRend = IndicatorCheckParentObj?.GetComponent<LineRenderer>();
            camObjTrans = GameObject.Find("Camera")?.transform;
            camObjCam = camObjTrans?.GetComponent<Camera>();
        }

        /// <summary>FindUIObjects.</summary>
        private void FindUIObjects()
        {
            // Find UI elements by name
            var screensParent = GameObject.Find("Screens");
            if (screensParent != null)
            {
                screensObjArray = new GameObject[screensParent.transform.childCount];
                for (int i = 0; i < screensObjArray.Length; i++)
                    screensObjArray[i] = screensParent.transform.GetChild(i).gameObject;
            }
        }

        /// <summary>SetupUIFunctions.</summary>
        private void SetupUIFunctions()
        {
            // Wire up button onClick listeners
        }

        // ================================================================
        // GAME LIFECYCLE
        // ================================================================

        /// <summary>LoadSavedData. RVA: 0xEB2480.</summary>
        private void LoadSavedData()
        {
            saveData = saveSystem.Load();
            ApplyLoadedSettings();
        }

        private void ApplyLoadedSettings()
        {
            if (saveData == null) return;
            // Apply settings from saveData to UI
        }

        /// <summary>bSaveGameExists. RVA: 0xEB3CB0.</summary>
        private bool bSaveGameExists()
        {
            return saveData?.slts != null && saveData.slts.Count > 0;
        }

        /// <summary>ContinueGameBtnState. RVA: 0xEB3D44.</summary>
        private void ContinueGameBtnState()
        {
            if (continueGameBtnObj != null)
                continueGameBtnObj.SetActive(bSaveGameExists());
        }

        /// <summary>SaveCurrentGame. RVA: 0xEB3DD8.</summary>
        private void SaveCurrentGame()
        {
            saveSystem.Save();
        }

        /// <summary>LoadLastSavedGame. RVA: 0xEB31C8.</summary>
        private void LoadLastSavedGame()
        {
            saveSystem.Load();
        }

        /// <summary>DeleteLastSavedGame. RVA: 0xEB412C.</summary>
        private void DeleteLastSavedGame(int v, bool ss)
        {
            saveSystem.SaveData.slts[v] = new MoneSlt();
            saveSystem.Save();
        }

        /// <summary>SaveSlotsInit. RVA: 0xEB3950.</summary>
        private void SaveSlotsInit()
        {
            if (saveData.slts == null)
                saveData.slts = new List<MoneSlt>();
            while (saveData.slts.Count < T_SV_SLTS)
                saveData.slts.Add(new MoneSlt());
        }

        /// <summary>OnClickMMStartGameBtn. RVA: 0xEB422C.</summary>
        private void OnClickMMStartGameBtn()
        {
            ShowScreen(MainScript.Parda.NewGameSetup);
        }

        /// <summary>OnClickSSlotBtn. RVA: 0xEB4834.</summary>
        private void OnClickSSlotBtn(int v)
        {
            selctdSlt = v;
        }

        /// <summary>OnClickSSlotDltBtn. RVA: 0xEB4A28.</summary>
        private void OnClickSSlotDltBtn(int v)
        {
            slt2Dlt = v;
        }

        /// <summary>CallbackSltDlt. RVA: 0xEB4E10.</summary>
        private void CallbackSltDlt()
        {
            DeleteLastSavedGame(slt2Dlt, true);
            RefreshSltsScreen();
        }

        /// <summary>RefreshSltsScreen. RVA: 0xEB4248.</summary>
        private void RefreshSltsScreen()
        {
            for (int i = 0; i < T_SV_SLTS; i++)
            {
                if (i < saveData.slts.Count && sltBtnsText != null && i < sltBtnsText.Length)
                {
                    var slot = saveData.slts[i];
                    sltBtnsText[i].text = slot.e ? $"Slot {i+1} (Saved)" : $"Slot {i+1} (Empty)";
                }
            }
        }

        /// <summary>SetGameMode. RVA: 0xEB4E38.</summary>
        private void SetGameMode(MainScript.GAME_MODE v) { gameMode = v; }

        /// <summary>SetModeType. RVA: 0xEB4E40.</summary>
        private void SetModeType(MainScript.MODE_TYPE v) { modeType = v; }

        /// <summary>DoLoadingThen. RVA: 0xEB4988.</summary>
        private void DoLoadingThen(Action a)
        {
            StartCoroutine(DoLoadingThenCoro(a));
        }

        private IEnumerator DoLoadingThenCoro(Action a)
        {
            yield return new WaitForSeconds(0.5f);
            a?.Invoke();
        }

        /// <summary>OnClickContinueBtn. RVA: 0xEB4FF8.</summary>
        private void OnClickContinueBtn()
        {
            LoadLastSavedGame();
            ShowScreen(MainScript.Parda.InGame);
        }

        /// <summary>OnClickStartGameBtn. RVA: 0xEB50D4.</summary>
        private void OnClickStartGameBtn()
        {
            StartNewGame(0, false, false);
        }

        /// <summary>StartNewGame. RVA: 0xEB523C.</summary>
        private void StartNewGame(int v, bool c, bool r)
        {
            selctdSlt = v;
            bGameComplete = false;
            bGamePaused = false;
            scoreValue = 0;
            undoDoneCounter = 0;

            string startFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            enSonchalok.Init(startFEN);
            sechDMG.ParseFEN(startFEN);

            DestroyAllPieces();
            PositionToBoard(false);

            ShowScreen(MainScript.Parda.InGame);

            DetermineCurTurn();

            if (gameMode == GAME_MODE.VsCPU && currentTurn != humanPlayerSide)
            {
                DMGAnurodBhejo(false);
            }
        }

        /// <summary>DetermineCurTurn. RVA: 0xEB636C.</summary>
        private void DetermineCurTurn()
        {
            var fen = enSonchalok.curPositionListProp;
            var parts = fen.Split(' ');
            currentTurn = (parts.Length >= 2 && parts[1] == "w") ? Player.EK : Player.DO;
        }

        /// <summary>DoToss. RVA: 0xEB6D80.</summary>
        private void DoToss()
        {
            StartCoroutine(DoTossCoro());
        }

        private IEnumerator DoTossCoro()
        {
            yield return new WaitForSeconds(1.5f);
            player1isWhite = UnityEngine.Random.value > 0.5f;
            bTossDone = true;
            GameStartAfterToss();
        }

        /// <summary>GameStartAfterToss. RVA: 0xEB7290.</summary>
        private void GameStartAfterToss()
        {
            DetermineCurTurn();
            if (gameMode == GAME_MODE.VsCPU && currentTurn != humanPlayerSide)
                DMGAnurodBhejo(false);
        }

        // ================================================================
        // TURN TIMER
        // ================================================================

        /// <summary>TurnTimerStart. RVA: 0xEB761C.</summary>
        private void TurnTimerStart(float t)
        {
            turnFullTime = t;
            turnTimeRemaining = t;
            bTurnTimerRunning = true;
        }

        /// <summary>TurnTimerUpdate. RVA: 0xEB7620.</summary>
        private void TurnTimerUpdate()
        {
            turnTimeRemaining -= Time.deltaTime;
            if (turnTimeRemaining <= 0)
            {
                turnTimeRemaining = 0;
                bTurnTimerRunning = false;
                // Current player loses on time
                KhelKhatamKaroNotifKeBaad("Time out!");
            }
            DisplayTime(turnTimeRemaining);
        }

        /// <summary>DisplayTime. RVA: 0xEB76AC.</summary>
        private void DisplayTime(float t)
        {
            int m = Mathf.FloorToInt(t / 60f);
            int s = Mathf.FloorToInt(t % 60f);
            if (igScoreDisplayText != null)
                igScoreDisplayText.text = $"{m}:{s:D2}";
        }

        // ================================================================
        // TUTORIAL
        // ================================================================

        /// <summary>TutorialInit. RVA: 0xEB7744.</summary>
        private void TutorialInit()
        {
            // Initialize tutorial FEN array and move sequences
        }

        /// <summary>OnClickTutorialBtn. RVA: 0xEB7F9C.</summary>
        private void OnClickTutorialBtn()
        {
            ShowScreen(MainScript.Parda.Tutorial);
        }

        /// <summary>OnClickTutSelBtn. RVA: 0xEB8064.</summary>
        private void OnClickTutSelBtn(int v)
        {
            curTutorialScreen = v;
            TutStart(false);
        }

        /// <summary>TutStart. RVA: 0xEB8100.</summary>
        private void TutStart(bool r)
        {
            tutCurMove = 0;
            tutCurText = 0;
            bThisTutDone = false;
            tutCanClick = true;
            // Load FEN for this tutorial
        }

        /// <summary>SetTutText coroutine. RVA: 0xEB8464.</summary>
        private IEnumerator SetTutText(bool l = false)
        {
            yield return null;
        }

        /// <summary>CheckTutPieceSelect.</summary>
        private bool CheckTutPieceSelect() { return false; }

        /// <summary>OnTutMoveComplete. RVA: 0xEB87EC.</summary>
        private void OnTutMoveComplete() { }

        /// <summary>FadTutBtnsGrpInv. RVA: 0xEB96F4.</summary>
        private void FadTutBtnsGrpInv() { }

        /// <summary>OnClickTutMainMenuBtn. RVA: 0xEB977C.</summary>
        private void OnClickTutMainMenuBtn()
        {
            QuitTOMMFromTut();
        }

        /// <summary>QuitTOMMFromTut. RVA: 0xEB97FC.</summary>
        private void QuitTOMMFromTut()
        {
            ShowScreen(MainScript.Parda.MainMenu);
        }

        /// <summary>OnClickTutNextBtn. RVA: 0xEB9C68.</summary>
        private void OnClickTutNextBtn()
        {
            curTutorialScreen = (curTutorialScreen + 1) % T_TUTORIALS;
            TutStart(false);
        }

        /// <summary>OnClickTutAgainBtn. RVA: 0xEB9C84.</summary>
        private void OnClickTutAgainBtn()
        {
            TutStart(true);
        }

        /// <summary>OnClickTutStepsNextBtn. RVA: 0xEB9C8C.</summary>
        private void OnClickTutStepsNextBtn()
        {
            tutCurText++;
            StartCoroutine(SetTutText(false));
        }

        /// <summary>TutAnimCoro. RVA: 0xEB83E4.</summary>
        private IEnumerator TutAnimCoro(float a, float t1, CanvasGroup t2, bool r = false)
        {
            yield return null;
        }

        // ================================================================
        // ONLINE
        // ================================================================

        /// <summary>OnlineInit. RVA: 0xEB9E4C.</summary>
        private void OnlineInit() { /* Online multiplayer removed */ }

        /// <summary>OnQuitMainMenu. RVA: 0xEBA07C.</summary>
        private void OnQuitMainMenu()
        {
            QuitToMainMenu(false);
        }

        // ================================================================
        // BOARD OPERATIONS
        // ================================================================

        /// <summary>SechInit (engine init). RVA: 0xEBC32C.</summary>
        private void SechInit()
        {
            sechDMG = new ChessEngine();
        }

        /// <summary>JustSetPrevBoard. RVA: 0xEB9838.</summary>
        private void JustSetPrevBoard() { }

        /// <summary>LeloGutiJaga (get piece position). </summary>
        private Vector3 LeloGutiJaga(string a)
        {
            if (string.IsNullOrEmpty(a) || a.Length < 2) return Vector3.zero;
            int file = a[0] - 'a';
            int rank = a[1] - '0' - 1;
            return new Vector3(file - 3.5f, 0, rank - 3.5f);
        }

        /// <summary>CreatePiece. RVA: 0xEBC388.</summary>
        private void CreatePiece(int t, string a, bool s = false)
        {
            // Instantiate piece prefab at square 'a'
        }

        /// <summary>PositionToBoard. RVA: 0xEB621C.</summary>
        private void PositionToBoard(bool s = true)
        {
            DestroyAllPieces();
            string fen = enSonchalok.curPositionListProp;
            if (string.IsNullOrEmpty(fen)) return;

            var parts = fen.Split(' ');
            if (parts.Length == 0) return;

            int rank = 0, file = 0;
            foreach (char c in parts[0])
            {
                if (c == '/') { rank++; file = 0; }
                else if (char.IsDigit(c)) file += c - '0';
                else
                {
                    string square = $"{(char)('a' + file)}{8 - rank}";
                    int pt = char.ToLower(c) switch
                    {
                        'p' => 6, 'n' => 2, 'b' => 3, 'r' => 1, 'q' => 4, 'k' => 5, _ => 0
                    };
                    if (pt > 0) CreatePiece(pt, square, s);
                    file++;
                }
            }
        }

        /// <summary>HastenGhumado (rotate pieces). RVA: 0xEBDF9C.</summary>
        private void HastenGhumado() { }

        /// <summary>DestroyAllPieces. RVA: 0xEBC7D8.</summary>
        private void DestroyAllPieces()
        {
            if (piecesParentTrans == null) return;
            for (int i = piecesParentTrans.childCount - 1; i >= 0; i--)
                Destroy(piecesParentTrans.GetChild(i).gameObject);
        }

        /// <summary>CreateOutPiece. RVA: 0xEBE0B0.</summary>
        private void CreateOutPiece(int t, Vector3 p, string b) { }

        /// <summary>PositionToOutBoard. RVA: 0xEBCD5C.</summary>
        private void PositionToOutBoard() { }

        /// <summary>PieceTypeForArray.</summary>
        private int PieceTypeForArray(string f, string rang) { return 0; }

        /// <summary>PieceLongType.</summary>
        private string PieceLongType(string f) { return f; }

        /// <summary>PieceLongTitleType.</summary>
        private string PieceLongTitleType(string f) { return f; }

        /// <summary>RandomChess.</summary>
        private string RandomChess() { return ""; }

        /// <summary>RandomChar.</summary>
        private string RandomChar(int l) { return ""; }

        // ================================================================
        // INDICATORS
        // ================================================================

        /// <summary>BanaoRotacindi (create indicator). RVA: 0xEBE9A8.</summary>
        private void BanaoRotacindi(string p, MainScript.ROTACIDNI rang)
        {
            // Create indicator at square 'p' with color 'rang'
        }

        /// <summary>DestroyAllIndicators. RVA: 0xEB5BCC.</summary>
        private void DestroyAllIndicators()
        {
            if (indicatorMoveParentTrans == null) return;
            for (int i = indicatorMoveParentTrans.childCount - 1; i >= 0; i--)
                Destroy(indicatorMoveParentTrans.GetChild(i).gameObject);
        }

        /// <summary>ShowPossibleDanger. RVA: 0xEB6214.</summary>
        private void ShowPossibleDanger() { }

        /// <summary>CreateDangerIndicator. RVA: 0xEBECA0.</summary>
        private void CreateDangerIndicator(string pos) { }

        /// <summary>DestroyDangerIndicators. RVA: 0xEB5EA8.</summary>
        private void DestroyDangerIndicators() { }

        /// <summary>ShowCheckIndicator (with refs). RVA: 0xEBED6C.</summary>
        private void ShowCheckIndicator(ref bool w, ref bool b) { }

        /// <summary>ShowCheckIndicator. RVA: 0xEB8D28.</summary>
        private void ShowCheckIndicator()
        {
            if (IndicatorCheckParentObj != null)
                IndicatorCheckParentObj.SetActive(true);
        }

        /// <summary>ShowCustomCheckIndic. RVA: 0xEB82C8.</summary>
        private void ShowCustomCheckIndic(string a, string b) { }

        /// <summary>ShowCheckIndIfInCheck. RVA: 0xEB6218.</summary>
        private void ShowCheckIndIfInCheck()
        {
            if (sechDMG.IsInCheck((int)currentTurn))
                ShowCheckIndicator();
        }

        /// <summary>HideCheckIndicator. RVA: 0xEB309C.</summary>
        private void HideCheckIndicator()
        {
            if (IndicatorCheckParentObj != null)
                IndicatorCheckParentObj.SetActive(false);
        }

        // ================================================================
        // INPUT
        // ================================================================

        /// <summary>OnMuchhanKlic (on click). RVA: 0xEBBB5C.</summary>
        private void OnMuchhanKlic()
        {
            // Handle click on board square
        }

        /// <summary>MoveNextStep. RVA: 0xEBEE5C.</summary>
        private void MoveNextStep() { }

        /// <summary>OnPromoteAskChoosen. RVA: 0xEBF004.</summary>
        private void OnPromoteAskChoosen(int val)
        {
            enSonchalok.promoteToWhat = val switch
            {
                5 => "q", 4 => "r", 3 => "b", 2 => "n", _ => "q"
            };
        }

        // ================================================================
        // AI MOVE
        // ================================================================

        /// <summary>DMGAnurodBhejo (send AI request). RVA: 0xEB72B4.</summary>
        private void DMGAnurodBhejo(bool bAauThoreBhabIng = false)
        {
            StartCoroutine(DMGChalNxtStap());
        }

        /// <summary>AauThoreBhabInv coroutine. RVA: 0xEBF0BC.</summary>
        private IEnumerator AauThoreBhabInv()
        {
            yield return null;
        }

        /// <summary>DMGChalNxtStap coroutine. RVA: 0xEBF124.</summary>
        private IEnumerator DMGChalNxtStap()
        {
            yield return new WaitForSeconds(0.3f);

            int depth = saveData.dmD switch
            {
                Difficulty.EK => 2,
                Difficulty.DO => 3,
                Difficulty.TEEN => 4,
                Difficulty.CHAR => 5,
                _ => 3
            };

            string aiMove = sechDMG.GetBestMove(
                enSonchalok.curPositionListProp,
                enSonchalok.GetLastMove() ?? "",
                depth,
                false);

            if (!string.IsNullOrEmpty(aiMove))
            {
                Move move = Move.ParseCAN(aiMove);
                sechDMG.MakeMove(move);
                enSonchalok.AppendMove(aiMove);
                enSonchalok.SwitchSide();
                enSonchalok.chalatAahe = true;

                moveFromPlace = aiMove.Substring(0, 2);
                moveToPlace = aiMove.Substring(2, 2);

                StartPieceMovement();
            }
        }

        // ================================================================
        // ANIMATION
        // ================================================================

        /// <summary>StartPieceMovement. RVA: 0xEB8E04.</summary>
        private void StartPieceMovement()
        {
            moveFromPos = LeloGutiJaga(moveFromPlace);
            moveToPos = LeloGutiJaga(moveToPlace);
            moveAnimValue = 0;
            moveAnimTimer = 0;
            bCurrentlyAnimMoving = true;
        }

        /// <summary>GutiChalneKaEtadpu (piece movement update). RVA: 0xEBB3D4.</summary>
        private void GutiChalneKaEtadpu()
        {
            moveAnimTimer += Time.deltaTime * 8f;
            moveAnimValue = Mathf.Clamp01(moveAnimTimer);

            if (pieceToMoveTrans != null)
            {
                Vector3 newPos = Vector3.Lerp(moveFromPos, moveToPos, moveAnimValue);
                newPos.y += Mathf.Sin(moveAnimValue * Mathf.PI) * 0.3f;
                pieceToMoveTrans.position = newPos;
            }

            if (moveAnimValue >= 1f)
            {
                bCurrentlyAnimMoving = false;
                OnGutiChalnaKhatm();
            }
        }

        /// <summary>OnGutiChalnaKhatm (on move complete). RVA: 0xEBF2C4.</summary>
        private void OnGutiChalnaKhatm()
        {
            enSonchalok.chalatAahe = false;

            // Play sound
            if (thisAudioSource != null)
            {
                if (moveFromPlace != null && moveToPlace != null)
                    thisAudioSource.PlayOneShot(buttonClickSound);
            }

            // Check for check
            if (sechDMG.IsInCheck((int)currentTurn == 0 ? 1 : 0))
            {
                if (kingCheckSound != null && thisAudioSource != null)
                    thisAudioSource.PlayOneShot(kingCheckSound);
                ShowCheckIndicator();
            }

            // Check for game over
            var moves = sechDMG.GenerateLegalMoves();
            if (moves.Count == 0)
            {
                if (sechDMG.IsInCheck((int)currentTurn))
                    GameCompleteEvent();
                else
                    GameCompleteEvent();  // Stalemate
                return;
            }

            // Switch turn
            currentTurn = currentTurn == Player.EK ? Player.DO : Player.EK;
            SwitchPlayerIGUI();

            // If AI's turn, request move
            if (gameMode == GAME_MODE.VsCPU && currentTurn != humanPlayerSide)
                DMGAnurodBhejo(false);
        }

        /// <summary>PromotionEvent. </summary>
        private void PromotionEvent(bool s) { }

        /// <summary>PlayPromotSndDel coroutine.</summary>
        private IEnumerator PlayPromotSndDel() { yield return null; }

        /// <summary>DisPromPartic coroutine.</summary>
        private IEnumerator DisPromPartic() { yield return null; }

        /// <summary>CastPossiMsg.</summary>
        private void CastPossiMsg() { }

        /// <summary>CastShowPossiMsg.</summary>
        private void CastShowPossiMsg(string m) { }

        // ================================================================
        // UNDO
        // ================================================================

        /// <summary>UndoMoveFunc.</summary>
        private void UndoMoveFunc(bool f = false)
        {
            if (undoDoneCounter >= MX_UNDO_ALLOWED) return;
            DoTheUndo();
        }

        /// <summary>DoTheUndo.</summary>
        private void DoTheUndo()
        {
            int movesToUndo = gameMode == GAME_MODE.VsCPU ? 2 : 1;
            for (int i = 0; i < movesToUndo; i++)
            {
                string lastMove = enSonchalok.GetLastMove();
                if (string.IsNullOrEmpty(lastMove)) break;
                sechDMG.UnmakeMove();
                // Remove last from list
                var moves = enSonchalok.curMovesListProp.Split(';');
                enSonchalok.curMovesListProp = moves.Length > 1
                    ? string.Join(";", moves, 0, moves.Length - 1)
                    : "";
            }
            undoDoneCounter++;
            DetermineCurTurn();
            PositionToBoard(false);
        }

        /// <summary>StartUndoMovement.</summary>
        private void StartUndoMovement() { }

        /// <summary>PieceUndoMovementUpdate.</summary>
        private void PieceUndoMovementUpdate() { }

        /// <summary>OnUndoMoveComplete.</summary>
        private void OnUndoMoveComplete() { }

        // ================================================================
        // GAME OVER
        // ================================================================

        /// <summary>GameOverInit.</summary>
        private void GameOverInit() { }

        /// <summary>KhelKhatamKaroNotifKeBaad (end game after notification).</summary>
        private void KhelKhatamKaroNotifKeBaad(string t)
        {
            GameCompleteEvent();
        }

        /// <summary>GOverWithLoading.</summary>
        private void GOverWithLoading()
        {
            ShowScreen(MainScript.Parda.GameOver);
        }

        /// <summary>GameCompleteEvent.</summary>
        private void GameCompleteEvent()
        {
            bGameComplete = true;
            gameWinner = currentTurn;
            if (gameWinSound != null && thisAudioSource != null)
                thisAudioSource.PlayOneShot(gameWinSound);
            GOverWithLoading();
        }

        /// <summary>KhelJitNeKaAwazInv coroutine.</summary>
        private IEnumerator KhelJitNeKaAwazInv() { yield return null; }

        /// <summary>GMKhatamPartcHaalat.</summary>
        private void GMKhatamPartcHaalat(bool v) { }

        /// <summary>QuitToMMWithLoading.</summary>
        private void QuitToMMWithLoading()
        {
            StartCoroutine(QuitToMMWithLoadingCoro());
        }

        private IEnumerator QuitToMMWithLoadingCoro()
        {
            yield return new WaitForSeconds(0.5f);
            QuitToMainMenu(false);
        }

        /// <summary>QuitToMainMenu.</summary>
        private void QuitToMainMenu(bool fromTut = false)
        {
            bGameComplete = false;
            bGamePaused = false;
            DestroyAllPieces();
            ShowScreen(MainScript.Parda.MainMenu);
        }

        /// <summary>GoToReviewBoard.</summary>
        private void GoToReviewBoard()
        {
            bReviewBoard = true;
        }

        /// <summary>GameOverUndoBtnClick.</summary>
        private void GameOverUndoBtnClick()
        {
            UndoMoveFunc(false);
        }

        /// <summary>OnClickPlayAgainBtn.</summary>
        private void OnClickPlayAgainBtn()
        {
            StartNewGame(selctdSlt, false, true);
        }

        // ================================================================
        // INPUT (continued)
        // ================================================================

        /// <summary>InputInit.</summary>
        private void InputInit() { }

        /// <summary>InputUpdate.</summary>
        private void InputUpdate()
        {
            if (bGamePaused || bGameComplete) return;
            if (enSonchalok.chalatAahe) return;
            if (gameMode == GAME_MODE.VsCPU && currentTurn != humanPlayerSide) return;

            if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
            {
                OnMuchhanKlic();
            }
        }

        // ================================================================
        // CAMERA
        // ================================================================

        /// <summary>CameraInit.</summary>
        private void CameraInit()
        {
            cameraMode = CAM_MODE.ThreeD;
            bCamCanRotate = true;
            camDistance = CAM_DIST_DEFAULT;
            camParentRotValueY = 0;
            inputToRotTarget = new Vector2(-CAM_Y_DEFAULT, 0);
        }

        /// <summary>CameraSwitchMode.</summary>
        private void CameraSwitchMode(CAM_MODE v)
        {
            cameraPrevMode = cameraMode;
            cameraMode = v;
            if (v == CAM_MODE.TwoD)
                inputToRotTarget = new Vector2(-90, 0);
            else if (v == CAM_MODE.ThreeD)
                inputToRotTarget = new Vector2(-CAM_Y_DEFAULT, 0);
        }

        /// <summary>OnClickCameraBtn.</summary>
        private void OnClickCameraBtn() { }

        /// <summary>OnClickCamera2D3DBtn.</summary>
        private void OnClickCamera2D3DBtn()
        {
            CameraSwitchMode(cameraMode == CAM_MODE.ThreeD ? CAM_MODE.TwoD : CAM_MODE.ThreeD);
        }

        /// <summary>OnClickCameraLockedBtn.</summary>
        private void OnClickCameraLockedBtn()
        {
            bCamLocked = !bCamLocked;
            if (igCamLockBtnTickObj != null)
                igCamLockBtnTickObj.SetActive(bCamLocked);
        }

        /// <summary>Activate2d3d.</summary>
        private void Activate2d3d(pTp v) { }

        /// <summary>CameraBtnOnPointerDown.</summary>
        private void CameraBtnOnPointerDown() { }

        /// <summary>CameraBtnOnPointerUp.</summary>
        private void CameraBtnOnPointerUp() { }

        /// <summary>CameraBtnHoldTimerInv.</summary>
        private void CameraBtnHoldTimerInv() { }

        /// <summary>IGCamBtnsShowHide.</summary>
        private void IGCamBtnsShowHide(bool d = true) { }

        /// <summary>IGCamBtnsAnimator coroutine.</summary>
        private IEnumerator IGCamBtnsAnimator() { yield return null; }

        /// <summary>CameraMatrixInit.</summary>
        private void CameraMatrixInit() { }

        /// <summary>BlendToMatrix.</summary>
        private Coroutine BlendToMatrix(Matrix4x4 t, float d)
        {
            return StartCoroutine(LerpMatrixFromTo(mtrO, t, d));
        }

        /// <summary>LerpMatrixFromTo coroutine.</summary>
        private IEnumerator LerpMatrixFromTo(Matrix4x4 a, Matrix4x4 d, float u)
        {
            float t = 0;
            while (t < u)
            {
                t += Time.deltaTime;
                mtrO = MatrixLerp(a, d, t / u);
                yield return null;
            }
        }

        /// <summary>MatrixLerp.</summary>
        private Matrix4x4 MatrixLerp(Matrix4x4 f, Matrix4x4 t, float tv)
        {
            Matrix4x4 result = new Matrix4x4();
            for (int i = 0; i < 16; i++)
                result[i] = Mathf.Lerp(f[i], t[i], tv);
            return result;
        }

        /// <summary>CameraUpdate.</summary>
        private void CameraUpdate()
        {
            if (bCamLocked) return;

            // Handle camera drag rotation
            if (cameraMode == CAM_MODE.ThreeD && bCamCanRotate)
            {
                if (Input.GetMouseButton(1))
                {
                    inputToRotTarget.x -= Input.GetAxis("Mouse Y") * 0.5f;
                    inputToRotTarget.y += Input.GetAxis("Mouse X") * 0.5f;
                    inputToRotTarget.x = Mathf.Clamp(inputToRotTarget.x, -CAM_Y_MAX, -CAM_Y_MIN);
                }
            }

            // Smooth rotation
            inputToRotValue = Vector2.SmoothDamp(inputToRotValue, inputToRotTarget, ref inputToRotVel, 0.15f);

            if (camObjTrans != null && camObjTrans.parent != null)
            {
                camObjTrans.parent.rotation = Quaternion.Euler(inputToRotValue.x, inputToRotValue.y, 0);
            }

            // Zoom
            float zoom = Input.mouseScrollDelta.y * 2f;
            camDistance = Mathf.Clamp(camDistance - zoom, CAM_DIST_MIN, CAM_DIST_MAX);

            if (camObjTrans != null)
            {
                Vector3 target = new Vector3(0, 0, -camDistance);
                camObjTrans.localPosition = Vector3.SmoothDamp(camObjTrans.localPosition, target, ref camLocalPosVel, 0.15f);
            }
        }

        /// <summary>SetCamLocalRotSmooth.</summary>
        private void SetCamLocalRotSmooth(float timeVal) { }

        /// <summary>SetCamLocalRotTopSmooth180.</summary>
        private void SetCamLocalRotTopSmooth180(float timeVal) { }

        /// <summary>ResetCueAndCamDirection.</summary>
        private void ResetCueAndCamDirection(bool w = false, bool r = false, bool y = false) { }

        /// <summary>StartTurnSwitchFadeEffect.</summary>
        private void StartTurnSwitchFadeEffect()
        {
            StartCoroutine(TurnSwitchFadeInInv());
        }

        /// <summary>TurnSwitchFadeInInv coroutine.</summary>
        private IEnumerator TurnSwitchFadeInInv()
        {
            if (igTurnSwitchFadeCG == null) yield break;
            float t = 0;
            while (t < 0.2f)
            {
                t += Time.deltaTime;
                igTurnSwitchFadeCG.alpha = t / 0.2f;
                yield return null;
            }
            StartCoroutine(TurnSwitchFadeOutInv());
        }

        /// <summary>TurnSwitchFadeOutInv coroutine.</summary>
        private IEnumerator TurnSwitchFadeOutInv()
        {
            if (igTurnSwitchFadeCG == null) yield break;
            float t = 0;
            while (t < 0.2f)
            {
                t += Time.deltaTime;
                igTurnSwitchFadeCG.alpha = 1f - (t / 0.2f);
                yield return null;
            }
            igTurnSwitchFadeCG.alpha = 0;
        }

        /// <summary>SetAutoRotateInv.</summary>
        private void SetAutoRotateInv() { }

        /// <summary>Rotate2dPiecesForTurnSwitch.</summary>
        private void Rotate2dPiecesForTurnSwitch(int r) { }

        // ================================================================
        // MENU SYSTEM
        // ================================================================

        /// <summary>MenuSystemInit.</summary>
        private void MenuSystemInit()
        {
            curScreen = Parda.MainMenu;
            bAnimateGui = true;
        }

        /// <summary>MenuSystemUpdate.</summary>
        private void MenuSystemUpdate()
        {
            if (bAnimateGui)
            {
                btnAnimValue = Mathf.SmoothDamp(btnAnimValue, btnAnimTarget, ref btnAnimVel, 0.15f);
                if (Mathf.Abs(btnAnimValue - btnAnimTarget) < 0.01f)
                {
                    bAnimateGui = false;
                    whatToDoAfterMenuAnim?.Invoke();
                    whatToDoAfterMenuAnim = null;
                }
            }
        }

        /// <summary>DoThisAfterAnim.</summary>
        private void DoThisAfterAnim(Action a)
        {
            whatToDoAfterMenuAnim = a;
            bAnimateGui = true;
            btnAnimValue = 0;
            btnAnimTarget = 1;
        }

        /// <summary>ShowScreen.</summary>
        private void ShowScreen(Parda screen)
        {
            if (screensObjArray == null) return;

            foreach (var s in screensObjArray)
                if (s != null) s.SetActive(false);

            int idx = (int)screen;
            if (idx >= 0 && idx < screensObjArray.Length && screensObjArray[idx] != null)
            {
                prevScreen = curScreen;
                curScreen = screen;
                screensObjArray[idx].SetActive(true);
                DoThisAfterAnim(null);
            }
        }

        /// <summary>SwitchToPrevScreen.</summary>
        private void SwitchToPrevScreen()
        {
            ShowScreen(prevScreen);
        }

        /// <summary>GoToHighScores.</summary>
        private void GoToHighScores()
        {
            ShowScreen(Parda.HighScores);
        }

        // ================================================================
        // IN-GAME UI
        // ================================================================

        /// <summary>InGameUIInit.</summary>
        private void InGameUIInit() { }

        /// <summary>ShowBlinkingText.</summary>
        private void ShowBlinkingText(string val)
        {
            if (igBlinkingTextText != null)
            {
                igBlinkingTextText.text = val;
                igBlinkingTextText.gameObject.SetActive(true);
            }
        }

        /// <summary>HideBlinkingText.</summary>
        private void HideBlinkingText()
        {
            if (igBlinkingTextText != null)
                igBlinkingTextText.gameObject.SetActive(false);
        }

        /// <summary>SwitchPlayerIGUI.</summary>
        private void SwitchPlayerIGUI()
        {
            // Update turn indicator
            if (igPlayerTurnIndicator != null)
            {
                int idx = (int)currentTurn;
                if (igPlayerScaleParentTrans != null && idx < igPlayerScaleParentTrans.Length)
                {
                    // Scale up current player, scale down other
                    for (int i = 0; i < igPlayerScaleParentTrans.Length; i++)
                    {
                        if (igPlayerScaleParentTrans[i] != null)
                        {
                            var target = i == idx ? igPlayerScaleBig : igPlayerScaleSmall;
                            igPlayerScaleParentTrans[i].localScale = target;
                        }
                    }
                }
            }
        }

        /// <summary>UndoBtnIGState.</summary>
        private void UndoBtnIGState()
        {
            if (igUndoMoveBtnObj != null)
                igUndoMoveBtnObj.SetActive(undoDoneCounter < MX_UNDO_ALLOWED);
        }

        /// <summary>UndoBtnHide.</summary>
        private void UndoBtnHide()
        {
            if (igUndoMoveBtnObj != null)
                igUndoMoveBtnObj.SetActive(false);
        }

        // ================================================================
        // NOTIFICATIONS
        // ================================================================

        /// <summary>NotifInit.</summary>
        private void NotifInit() { }

        /// <summary>NotificationShow.</summary>
        private void NotificationShow(string msg, float timeoutVal = 3)
        {
            if (notifTextComp != null)
                notifTextComp.text = msg;
            if (notifTopCoro != null) StopCoroutine(notifTopCoro);
            notifTopCoro = StartCoroutine(NotifTopAnimtr(0));
        }

        /// <summary>NotifHidInv.</summary>
        private void NotifHidInv() { }

        /// <summary>NotifTopAnimtr coroutine.</summary>
        private IEnumerator NotifTopAnimtr(float notifAnimTarget)
        {
            yield return null;
        }

        /// <summary>NotifDisable.</summary>
        private void NotifDisable() { }

        /// <summary>NotifBtmShow.</summary>
        private void NotifBtmShow(bool val, string txt = "", float t = 0) { }

        /// <summary>NotifBtmHidInv.</summary>
        private void NotifBtmHidInv() { }

        /// <summary>NotifBtmAnimtr coroutine.</summary>
        private IEnumerator NotifBtmAnimtr(bool val) { yield return null; }

        /// <summary>NotifBtnDisable.</summary>
        private void NotifBtnDisable() { }

        // ================================================================
        // PAUSE
        // ================================================================

        /// <summary>PauseGameFunction.</summary>
        private void PauseGameFunction()
        {
            bGamePaused = true;
            ShowScreen(Parda.Pause);
        }

        /// <summary>PauseUnpauseGame.</summary>
        private void PauseUnpauseGame()
        {
            if (bGamePaused)
            {
                bGamePaused = false;
                ShowScreen(Parda.InGame);
            }
            else
            {
                PauseGameFunction();
            }
        }

        // ================================================================
        // MSG BOX
        // ================================================================

        /// <summary>MsgBoxInit.</summary>
        private void MsgBoxInit() { }

        /// <summary>MessageBoxShow.</summary>
        private void MessageBoxShow(int btnCount, string msg, Action[] callbacks, string[] btnsText)
        {
            if (msgBoxMsgText != null)
                msgBoxMsgText.text = msg;
            msgCBAction = callbacks;
            for (int i = 0; i < msgBoxBtnObjs.Length; i++)
            {
                if (msgBoxBtnObjs[i] != null)
                    msgBoxBtnObjs[i].SetActive(i < btnCount);
                if (i < btnsText.Length && msgBoxBtnTexts != null && i < msgBoxBtnTexts.Length)
                    msgBoxBtnTexts[i].text = btnsText[i];
            }
        }

        /// <summary>MessageBoxOnClickBtn.</summary>
        private void MessageBoxOnClickBtn(int id)
        {
            msgCBAction?[id]?.Invoke();
        }

        /// <summary>CallbackRestart.</summary>
        private void CallbackRestart()
        {
            StartNewGame(selctdSlt, false, true);
        }

        /// <summary>CallbackQuit.</summary>
        private void CallbackQuit()
        {
            QuitToMainMenu(false);
        }

        /// <summary>CallbackCloseGame.</summary>
        private void CallbackCloseGame()
        {
            Application.Quit();
        }

        // ================================================================
        // CUSTOMIZE
        // ================================================================

        /// <summary>CustomizeInit.</summary>
        private void CustomizeInit() { }

        /// <summary>GoToCustomize.</summary>
        private void GoToCustomize()
        {
            ShowScreen(Parda.Customize);
        }

        /// <summary>OnClickTableSelection.</summary>
        private void OnClickTableSelection() { ChangeTable(); }

        /// <summary>ChangeTable.</summary>
        private void ChangeTable()
        {
            saveData.sTb = (saveData.sTb + 1) % T_TABLES;
            for (int i = 0; i < T_TABLES; i++)
                if (tablesObjArray != null && i < tablesObjArray.Length && tablesObjArray[i] != null)
                    tablesObjArray[i].SetActive(i == saveData.sTb);
            saveSystem.Save();
        }

        /// <summary>OnClickBoardBorderSelection.</summary>
        private void OnClickBoardBorderSelection() { ChangeBoardBorder(); }

        /// <summary>ChangeBoardBorder.</summary>
        private void ChangeBoardBorder()
        {
            saveData.sBB = (saveData.sBB + 1) % T_BOARDS_BORDERS;
            saveSystem.Save();
        }

        /// <summary>OnClickCheckersSelection.</summary>
        private void OnClickCheckersSelection() { ChangeCheckers(); }

        /// <summary>ChangeCheckers.</summary>
        private void ChangeCheckers()
        {
            saveData.sCh = (saveData.sCh + 1) % T_CHECKERS;
            saveSystem.Save();
        }

        /// <summary>OnClickChangePiecesTexture.</summary>
        private void OnClickChangePiecesTexture()
        {
            saveData.sPT = (saveData.sPT + 1) % T_GUTI_TYPES;
            saveSystem.Save();
            PositionToBoard(false);
        }

        // ================================================================
        // SETTINGS / AVATARS / ENTER NAME / RATE / STATS
        // ================================================================

        private void SettingsInit() { }
        private void AvatarsInit() { }
        private void EnterNameInit() { }
        private void RateGameInit() { }
        private void StatsInit() { }

        // ================================================================
        // LOADING
        // ================================================================

        /// <summary>ApplyFullscreenNext coroutine. RVA: 0xEB3CB0.</summary>
        private IEnumerator ApplyFullscreenNext() { yield return null; }

        /// <summary>CamBlurInOut coroutine. </summary>
        private IEnumerator CamBlurInOut() { yield return null; }

        /// <summary>FadeInFirstLoadingName coroutine.</summary>
        private IEnumerator FadeInFirstLoadingName() { yield return null; }

        /// <summary>IGCamBtnsAnimator coroutine (duplicate).</summary>
        private IEnumerator IGCamBtnsAnimatorDup() { yield return null; }

        /// <summary>LerpMatrixFromTo coroutine (duplicate).</summary>
        private IEnumerator LerpMatrixFromToDup() { yield return null; }

        /// <summary>LevelLoadingFinishedContinue coroutine.</summary>
        private IEnumerator LevelLoadingFinishedContinue() { yield return null; }

        /// <summary>NotifBtmAnimtr coroutine (duplicate).</summary>
        private IEnumerator NotifBtmAnimtrDup() { yield return null; }

        /// <summary>NotifTopAnimtr coroutine (duplicate).</summary>
        private IEnumerator NotifTopAnimtrDup() { yield return null; }

        /// <summary>SetTutText coroutine (duplicate).</summary>
        private IEnumerator SetTutTextDup() { yield return null; }
    }
}
