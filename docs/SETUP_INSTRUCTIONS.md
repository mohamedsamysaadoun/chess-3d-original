# Setup Instructions (مهم جداً)

## 1. متطلبات قبل البدء
- Unity Hub: https://unity.com/download
- Unity Editor 2022.3.20f1 (LTS) — نفس اللي استخدمناه في ProjectVersion.txt
- بيئة تطوير C# (Visual Studio Community أو VS Code)

## 2. فتح المشروع لأول مرة

1. **افتح Unity Hub**
2. اضغط **Add** → **Add project from disk**
3. اختر فولدر `chess-3d-original/`
4. لو سألك عن إصدار Unity، اختار **2022.3.20f1**
5. انتظر التحميل (5-15 دقيقة أول مرة)

## 3. تنظيم المشروع

```
chess-3d-original/
├── Assets/
│   ├── Scripts/           ← كل أكواد C# (9 ملفات)
│   ├── Editor/            ← Editor scripts (3 ملفات)
│   ├── Meshes/            ← 49 ملف .obj
│   ├── Textures/          ← 204 صورة .png
│   ├── Materials/         ← 95 ملف .mat
│   ├── Sounds/            ← 5 ملفات صوت .wav
│   ├── Shaders/           ← 59 ملف shader
│   ├── Fonts/             ← 4 خطوط .ttf
│   ├── Sprites/           ← 90 صورة sprite .png
│   ├── Cubemaps/          ← 2 cubemaps
│   ├── Prefabs/
│   │   ├── Pieces/        ← 36 piece prefabs
│   │   ├── Indicators/    ← 4 indicator prefabs
│   │   ├── UI/            ← 19 UI panel prefabs
│   │   └── Board/         ← 10 board prefabs
│   ├── Scenes/
│   │   └── Main.unity     ← المشهد الأساسي
│   └── Resources/
├── ProjectSettings/       ← كل إعدادات Unity
├── Packages/
│   ├── manifest.json      ← Unity packages
│   └── packages-lock.json
├── reference/             ← الـ RE artifacts الأصلية (مش في build)
├── docs/                  ← التوثيق
└── README.md
```

## 4. أول مرة تفتح المشروع

لما المشروع يفتح، روح لقائمة **Chess** في الـ menu bar:

1. **Chess > Generate All Meta Files** — بيولّد .meta files لأي asset ناقصة
2. **Chess > Build Scene From JSON** — بيـ rebuild المشهد من scene_hierarchy.json (اختياري لو المشهد الحالي مش بيفتح)
3. **Chess > Generate All Prefabs** — بيولّد الـ prefabs من الـ meshes

## 5. ربط الـ SerializeFields في MainScript

افتح `Assets/Scenes/Main.unity`، بعدين:
1. اعمل GameObject فاضي اسمه `MainController`
2. drag-drop سكريبت `MainScript.cs` عليه
3. في الـ Inspector هتلاقي 100+ field فاضية
4. اربط:
   - `piecesParentTrans` ← GameObject باسم "PiecesParent" (اعمله فاضي)
   - `piecesOutParentTrans` ← GameObject باسم "PiecesOutParent"
   - `boardPlacesParentTrans` ← GameObject باسم "BoardPlacesParent"
   - `musicsArray` ← اسحب chess001.wav من Assets/Sounds/
   - `buttonClickSound` ← اسحب buttonClick.wav
   - `kingCheckSound` ← اسحب pleasant_interface_32.wav أو strikeCheers3.wav
   - `gameWinSound` ← اسحب strikeCheers3.wav
   - ... الخ

## 6. بناء الـ APK

```
File > Build Settings > Android > Build
```

## مشاكل معروفة و حلولها

| المشكلة | الحل |
|---|---|
| MainScript بيـ throw errors للـ AdMob namespace | امسح `using GoogleMobileAds.Api;` من AdMobScript.cs (احنا عملنا stub) |
| Missing references في المشهد | استخدم Chess > Build Scene From JSON لإعادة بناء المشهد |
| مفيش صوت في الـ game | تأكد إن الـ AudioClips مربوطة في الـ Inspector |
| مفيش piece prefabs تظهر | استخدم Chess > Generate Piece Prefabs |
| فيه مشكلة في الـ Build | File > Build Settings > Player Settings > Other Settings > تفعيل Internet permission لو محتاج |

## لو الـ AdMob namespace ضايقك

افتح `Assets/Scripts/AdMobScript.cs` وغيّر السطر:
```csharp
using GoogleMobileAds.Api;
```
إلى:
```csharp
// using GoogleMobileAds.Api;  // Commented - ads removed
namespace GoogleMobileAds.Api {
    // Stub types
    public class BannerView { public void Destroy() {} }
    public class InterstitialAd { public void Destroy() {} }
    public class RewardedAd { public void Destroy() {} }
    public class AdRequest { }
}
```
