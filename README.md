# Real Chess 3D - Original-Style Reconstruction

A faithful reconstruction of the **Real Chess 3D** mobile game (com.eivaagames.RealChess3DFree v1.35)
with the **same code organization as the original developer** — one big `MainScript.cs` God class,
all scripts in a flat `Assets/Scripts/` folder, all original class names preserved (using the
decoded Marathi/Hindi → English obfuscation map).

## What This Is

This is a **best-effort reconstruction** of the original source code. The original game was
compiled with IL2CPP, which strips C# source code and converts it to native ARM64 assembly.
We cannot recover the exact original code, but we CAN reconstruct:

- ✅ **Same file organization** — flat `Assets/Scripts/` folder, one `.cs` per class
- ✅ **Same class names** — using decoded names from `obfuscation_map_full.json` (91 mappings)
- ✅ **Same field names** — preserved exactly from IL2CPP dump
- ✅ **Same method signatures** — preserved exactly from `dump_decoded.cs`
- ✅ **Best-effort method implementations** — based on:
  - `engine-test/ChessEngine.cs` (working, 10/10 tests) for chess engine
  - Field names and standard Unity patterns for UI/camera/animation
  - Frida traces for runtime behavior

## Project Structure (matches original)

```
Assets/
├── Scripts/                       ← All scripts in flat folder (like original)
│   ├── MainScript.cs              ← God class (100+ fields, 200+ methods)
│   ├── ChessEngine.cs             ← Sealed chess engine class
│   ├── EnSonchalok.cs             ← Move coordinator
│   ├── OpeningBook.cs             ← Opening book
│   ├── Move.cs                    ← Move + ScoredMove + HistoryMove structs
│   ├── SaveSystem.cs              ← SaveSystem + SaveBlockNew + SaveBlockOld + MoneSlt
│   ├── AdMobScript.cs             ← Ad service (stubbed)
│   ├── HelperScripts.cs           ← All helper classes (IAPScript, NetworkScript,
│   │                                HighScoresScript, FPS, Logo, PingPongAnim,
│   │                                BlurOptimized, etc.)
│   └── Enums.cs                   ← Player, Difficulty, GAME_MODE, MODE_TYPE,
│                                    Parda, CAM_MODE, ROTACIDNI, pTp
├── Scenes/
├── Prefabs/
├── Sprites/                       ← 90 sprites
├── Materials/                     ← 100 materials
├── Meshes/                        ← 98 OBJ meshes
├── Sounds/
├── Textures/                      ← 552 textures (incl. 2 cubemaps)
├── Animations/
└── Resources/
```

## Classes (matches original)

### Chess Engine (namespace `EivaaChess.Game`)

| Original Name | Decoded Name | TypeDefIndex | Description |
|---|---|---|---|
| `SechDMG` | `ChessEngine` | 6232 | Main chess engine (sealed class) |
| `SechChal` | `Move` | 6233 | Move struct |
| `KeemtiChal` | `ScoredMove` | 6234 | Scored move struct |
| — | `HistoryMove` | 6235 | History move struct |
| `KhaataKhol` | `OpeningBook` | 6231 | Opening book |

### Game Controllers

| Original Name | Decoded Name | TypeDefIndex | Description |
|---|---|---|---|
| — | `MainScript` | 6181 | God class — game controller |
| `EnSonchalok` | `EnSonchalok` | 6150 | Move coordinator |
| — | `SaveSystem` | 6189 | Save/load system |
| — | `SaveBlockNew` | 6188 | New save format |
| — | `SaveBlockOld` | 6187 | Old save format (for migration) |
| `MoneSlt` | `MoneSlt` | 6190 | Per-slot save data |
| — | `AdMobScript` | 6204 | Ad service (stubbed) |
| — | `IAPScript` | 6215 | In-app purchases (stubbed) |
| — | `NetworkScript` | 6182 | Online multiplayer (stubbed) |
| — | `HighScoresScript` | 6214 | Leaderboard (stubbed) |

### Enums

| Original Name | Decoded Name | Values |
|---|---|---|
| `PALI` | `Player` | `EK=0` (White), `DO=1` (Black) |
| `DMG_MUSKL` | `Difficulty` | `EK=0, DO=1, TEEN=2, CHAR=3` |
| — | `MainScript.GAME_MODE` | VsCPU, VsHuman, Online |
| — | `MainScript.MODE_TYPE` | Classic, Timed, Tournament |
| `Parda` | `MainScript.Parda` | 19 UI panels |
| — | `MainScript.CAM_MODE` | ThreeD, TwoD, Locked |
| `ROTACIDNI` | `MainScript.ROTACIDNI` | Red, Green, Blue, Danger |

### Helper Classes

| Class | TypeDefIndex | Description |
|---|---|---|
| `BlurOptimized` | 6226 | Camera blur post-effect |
| `PostEffectsBase` | 6227 | Base class for post-effects |
| `FPS` | 6211 | FPS counter |
| `Logo` | 6221 | Splash screen |
| `KeyboardScript` | 6217 | In-game keyboard |
| `PingPongAnim` | 6183 | Ping-pong animation |
| `PlayerPrefsX` | 6184 | Extended PlayerPrefs |
| `PromoteAnimScale` | 6185 | Promotion animation |
| `RotateObject` | 6186 | Object rotation |
| `SetTextFromVariable` | 6191 | Set text from variable |
| `SineTest` | 6192 | Sine test animation |
| `UIAnimator` | 6196 | UI animator |
| `UIBlinkingText` | 6197 | Blinking text |
| `UIBobAnim` | 6198 | Bob animation |
| `UIPingPongAnim` | 6199 | Ping-pong UI animation |
| `UITabAnimate` | 6200 | Tab animation |
| `UIToggleOnEnabled` | 6201 | Toggle on enable |
| `BowLineRend` | 6147 | Border line renderer |
| `ConsyaScript` | 6205 | Consent script |
| `DiffuseReflection` | 6206 | Diffuse reflection |
| `MultiDim` | 6228 | Multi-dim array helper |
| `NewGamePromo` | 6224 | New game promo |
| `StandaloneStart` | 6193 | Standalone start |
| `WebData` | 6194 | Web data |
| `StartDataWeb` | 6195 | Start data web |

## What Works

- ✅ Chess engine — fully functional (alpha-beta + quiescence + PST eval)
- ✅ FEN parsing + CAN notation
- ✅ Castling, en passant, pawn promotion
- ✅ Move generation + validation
- ✅ 4 AI difficulty levels
- ✅ Save/load system (JSON-based, backward compatible)
- ✅ All class structures preserved
- ✅ All field names preserved
- ✅ All method signatures preserved

## What's Stubbed

- ❌ AdMobScript — all ad methods are no-ops (ad-free build)
- ❌ IAPScript — all IAP methods are no-ops
- ❌ NetworkScript — online multiplayer removed
- ❌ HighScoresScript — leaderboard removed

## What Needs Manual Work in Unity Editor

1. **Build `Main.unity` scene** — use `reference/scene-data/scene_hierarchy.json` (1,243 GameObjects)
2. **Create prefabs** — 18 piece prefabs (6 pieces × 3 sets) + UI prefabs + indicator prefabs
3. **Wire up SerializeField references** — connect scene GameObjects to MainScript fields
4. **Test** — press Play, verify game flow

## References

- Original game: Real Chess 3D v1.35 by Eivaa Games
- Reverse engineering: [chess-3d-re](https://github.com/mohamedsamysaadoun/chess-3d-re)
- Decomposed version (12 controllers): [chess-3d-reconstructed](https://github.com/mohamedsamysaadoun/chess-3d-reconstructed)

## License

MIT
