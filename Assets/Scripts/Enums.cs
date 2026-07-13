using System;

namespace EivaaChess.Game
{
    /// <summary>
    /// Player enum. Original: enum Player { EK=0, DO=1 } (Hindi 1, 2).
    /// TypeDefIndex: 6151
    /// </summary>
    public enum Player
    {
        EK = 0,  // White (Hindi "one")
        DO = 1   // Black (Hindi "two")
    }

    /// <summary>
    /// Difficulty enum. Original: enum Difficulty { EK, DO, TEEN, CHAR } (Hindi 1-4).
    /// TypeDefIndex: 6152
    /// </summary>
    public enum Difficulty
    {
        EK = 0,    // Easy
        DO = 1,    // Medium
        TEEN = 2,  // Hard
        CHAR = 3   // Expert
    }

    /// <summary>
    /// Game mode enum. TypeDefIndex: 6153
    /// </summary>
    public enum MainScript.GAME_MODE
    {
        VsCPU = 0,
        VsHuman = 1,
        Online = 2
    }

    /// <summary>
    /// Mode type enum. TypeDefIndex: 6154
    /// </summary>
    public enum MainScript.MODE_TYPE
    {
        Classic = 0,
        Timed = 1,
        Tournament = 2
    }

    /// <summary>
    /// UI Panel enum. Original: enum MainScript.Parda (Parda = curtain in Hindi).
    /// TypeDefIndex: 6155-6158 (multiple Parda enums existed)
    /// </summary>
    public enum MainScript.Parda
    {
        FirstLoading = 0,
        MainMenu = 1,
        NewGameSetup = 2,
        GameLobby = 3,
        InGame = 4,
        Pause = 5,
        PromotePrompt = 6,
        UndoConfirm = 7,
        GameOver = 8,
        Settings = 9,
        Customize = 10,
        Stats = 11,
        Help = 12,
        RateUs = 13,
        IAP = 14,
        Tutorial = 15,
        TutorialComplete = 16,
        HighScores = 17,
        AvatarSelect = 18
    }

    /// <summary>
    /// Camera mode enum. TypeDefIndex: 6157
    /// </summary>
    public enum MainScript.CAM_MODE
    {
        ThreeD = 0,
        TwoD = 1,
        Locked = 2
    }

    /// <summary>
    /// Indicator color enum. Original: ROTACIDNI (Indicator spelled backwards).
    /// </summary>
    public enum MainScript.ROTACIDNI
    {
        Red = 0,      // capture / danger
        Green = 1,    // possible move
        Blue = 2,     // selected piece
        Danger = 3    // king in danger
    }

    /// <summary>
    /// 2D/3D piece toggle enum.
    /// </summary>
    public enum MainScript.pTp
    {
        TwoD = 0,
        ThreeD = 1
    }
}
