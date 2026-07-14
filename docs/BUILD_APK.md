# كيفية بناء APK من الريpo ده

## الطريقة الأولى: GitHub Actions (مجانًا، بدون تثبيت Unity)

### المطلوب:
1. حساب Unity مجاني (سجّل من https://unity.com)
2. حساب GitHub (عندك بالفعل)

### الخطوات:

#### 1. احصل على Unity License File (.ulf)

```bash
# على جهازك (أي جهاز فيه Unity مثبت):
# افتح Unity Hub → Login بحسابك
# أو نزّل Unity 2022.3.20f1 من Unity Hub
```

لو مش معاك Unity Hub، استخدم السكريبت ده:
```bash
# تنزيل Unity License Activation Tool
docker run -it --rm \
  -e UNITY_USERNAME=your_email \
  -e UNITY_PASSWORD=your_password \
  -v $(pwd):/workdir \
  unityci/editor:2022.3.20f1-android-1 \
  /bin/bash -c "xvfb-run /opt/unity/Editor/Unity \
    -quit \
    -batchmode \
    -nographics \
    -username '$UNITY_USERNAME' \
    -password '$UNITY_PASSWORD' \
    -createManualActivationFile && \
    mv *.alf /workdir/"
```

ثم فعّل الملف على https://license.unity3d.com/manual

#### 2. أضف Secrets للريpo

افتح: https://github.com/mohamedsamysaadoun/chess-3d-original/settings/secrets/actions

أضف الـ secrets دي:
- `UNITY_LICENSE` → محتوى ملف .ulf كامل
- `UNITY_EMAIL` → إيميل حساب Unity بتاعك
- `UNITY_PASSWORD` → باسورد حساب Unity

#### 3. شغّل الـ Workflow

افتح: https://github.com/mohamedsamysaadoun/chess-3d-original/actions

- اختار "Build Android APK" workflow
- اضغط "Run workflow"
- استنى 30-60 دقيقة
- APK هينزل في Artifacts

#### 4. نزّل الـ APK

في صفحة الـ workflow run، انزل تحت لـ "Artifacts" → نزّل `RealChess3D-Reconstructed`

---

## الطريقة الثانية: Build محلي على جهازك

### المطلوب:
- Unity Hub: https://unity.com/download
- Unity Editor 2022.3.20f1 (مع Android Build Support)
- ~10 GB مساحة فاضية

### الخطوات:

1. **نزّل الريpo**:
```bash
git clone https://github.com/mohamedsamysaadoun/chess-3d-original.git
```

2. **افتح Unity Hub → Add → اختر فولدر المشروع**

3. **انتظر 5-15 دقيقة** لاستيراد الـ assets

4. **من قائمة Chess** (في الأعلى):
   - `Chess > Generate All Meta Files`
   - `Chess > Build Scene From JSON`

5. **اربط الـ AudioClips** في MainScript (شوف docs/SETUP_INSTRUCTIONS.md)

6. **Build APK**:
   - `File > Build Settings > Android > Build`

---

## مشاكل معروفة

| المشكلة | الحل |
|---|---|
| GitHub Actions fails on Unity activation | تأكد إن .ulf file صالح ومش منتهي |
| Build fails على missing references | شغّل Chess > Build Scene From JSON الأول |
| APK ما يفتحش على الموبايل | تأكد إنك uninstall أي version قديم الأول |
| "App not installed" | الـ signature مختلفة — uninstall الأصلي |

---

## إذا ما عندكش Unity account

ممكن تستخدم Unity Personal License مجانًا (مش محتاج credit card):
1. سجّل على https://unity.com/products/unity-personal
2. هتلاقي خيار "Manual Activation" في Unity Hub
3. اتبع الخطوات للحصول على .ulf file
