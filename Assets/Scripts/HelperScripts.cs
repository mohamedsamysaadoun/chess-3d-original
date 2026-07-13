using System;
using UnityEngine;
using UnityEngine.Purchasing;

namespace EivaaChess.Game
{
    /// <summary>
    /// IAP Script (In-App Purchases). TypeDefIndex: 6215.
    ///
    /// In the original this managed Google Play billing for removing ads.
    /// In this reconstruction, all methods are stubbed — no IAP.
    /// </summary>
    public class IAPScript : MonoBehaviour, IDetailedStoreListener, IStoreListener
    {
        private static IStoreController storeController;
        private static IExtensionProvider storeExtensionProvider;

        public void Init()
        {
            // No-op — IAP removed
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            storeController = controller;
            storeExtensionProvider = extensions;
        }

        public void OnInitializeFailed(InitializationFailureReason error) { }
        public void OnInitializeFailed(InitializationFailureReason error, string message) { }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason) { }
        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription) { }
    }

    /// <summary>
    /// Network Script. TypeDefIndex: 6182.
    /// Online multiplayer — removed in this reconstruction.
    /// </summary>
    public class NetworkScript : MonoBehaviour
    {
        public void Init() { /* No-op — online removed */ }
        public void Connect() { }
        public void Disconnect() { }
    }

    /// <summary>
    /// High Scores Script. TypeDefIndex: 6214.
    /// Online leaderboard — removed.
    /// </summary>
    public class HighScoresScript : MonoBehaviour
    {
        public void SubmitScore(int score) { /* No-op */ }
        public void LoadLeaderboard() { }
    }

    /// <summary>
    /// ConsyaScript (Consent Script). TypeDefIndex: 6205.
    /// GDPR consent — not needed without ads.
    /// </summary>
    public class ConsyaScript
    {
        internal bool bChotOn; // 0x10
    }

    /// <summary>
    /// Keyboard Script. TypeDefIndex: 6217.
    /// In-game keyboard for name entry.
    /// </summary>
    public class KeyboardScript : MonoBehaviour
    {
        public void ShowKeyboard() { }
        public void HideKeyboard() { }
    }

    /// <summary>
    /// FPS Counter. TypeDefIndex: 6211.
    /// </summary>
    public class FPS : MonoBehaviour
    {
        private float deltaTime = 0f;

        private void Update()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        }

        private void OnGUI()
        {
            int w = Screen.width, h = Screen.height;
            GUIStyle style = new GUIStyle();
            Rect rect = new Rect(0, 0, w, h * 2 / 100);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 2 / 100;
            style.normal.textColor = Color.white;
            float msec = deltaTime * 1000f;
            float fps = 1f / deltaTime;
            string text = $"{msec:0.0} ms ({fps:0.} fps)";
            GUI.Label(rect, text, style);
        }
    }

    /// <summary>
    /// Logo splash screen. TypeDefIndex: 6221.
    /// </summary>
    public class Logo : MonoBehaviour
    {
        private void Start()
        {
            // Show logo, then transition to main menu
            StartCoroutine(ShowLogo());
        }

        private System.Collections.IEnumerator ShowLogo()
        {
            yield return new WaitForSeconds(2f);
            // Transition to main menu
        }
    }

    /// <summary>
    /// New Game Promo. TypeDefIndex: 6224.
    /// </summary>
    public class NewGamePromo : MonoBehaviour { }

    /// <summary>
    /// Diffuse Reflection shader helper. TypeDefIndex: 6206.
    /// </summary>
    public class DiffuseReflection : MonoBehaviour { }

    /// <summary>
    /// BowLineRend (border line renderer). TypeDefIndex: 6147.
    /// </summary>
    public class BowLineRend : MonoBehaviour { }

    /// <summary>
    /// PingPong animation. TypeDefIndex: 6183.
    /// </summary>
    public class PingPongAnim : MonoBehaviour
    {
        public Vector3 from = Vector3.zero;
        public Vector3 to = Vector3.one;
        public float speed = 1f;

        private void Update()
        {
            float t = Mathf.PingPong(Time.time * speed, 1f);
            transform.localScale = Vector3.Lerp(from, to, t);
        }
    }

    /// <summary>
    /// Player Prefs Extended. TypeDefIndex: 6184.
    /// </summary>
    public class PlayerPrefsX : MonoBehaviour
    {
        public static bool SetBool(string name, bool value)
        {
            PlayerPrefs.SetInt(name, value ? 1 : 0);
            return true;
        }

        public static bool GetBool(string name)
        {
            return PlayerPrefs.GetInt(name) == 1;
        }

        public static bool GetBool(string name, bool defaultValue)
        {
            return PlayerPrefs.GetInt(name, defaultValue ? 1 : 0) == 1;
        }
    }

    /// <summary>
    /// Promotion animation scale. TypeDefIndex: 6185.
    /// </summary>
    public class PromoteAnimScale : MonoBehaviour
    {
        public float targetScale = 1.5f;
        public float duration = 0.5f;

        private void Start()
        {
            StartCoroutine(Animate());
        }

        private System.Collections.IEnumerator Animate()
        {
            Vector3 original = transform.localScale;
            Vector3 target = original * targetScale;
            float t = 0;
            while (t < duration)
            {
                t += Time.deltaTime;
                transform.localScale = Vector3.Lerp(original, target, t / duration);
                yield return null;
            }
            transform.localScale = original;
        }
    }

    /// <summary>
    /// Rotate object. TypeDefIndex: 6186.
    /// </summary>
    public class RotateObject : MonoBehaviour
    {
        public Vector3 rotationSpeed = new Vector3(0, 30, 0);

        private void Update()
        {
            transform.Rotate(rotationSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Set Text From Variable. TypeDefIndex: 6191.
    /// </summary>
    public class SetTextFromVariable : MonoBehaviour
    {
        public string variableName;
        public UnityEngine.UI.Text textComponent;
    }

    /// <summary>
    /// Sine test animation. TypeDefIndex: 6192.
    /// </summary>
    public class SineTest : MonoBehaviour
    {
        public float frequency = 1f;
        public float amplitude = 1f;

        private void Update()
        {
            transform.position += Vector3.up * Mathf.Sin(Time.time * frequency) * amplitude * Time.deltaTime;
        }
    }

    /// <summary>
    /// Standalone start. TypeDefIndex: 6193.
    /// </summary>
    public class StandaloneStart : MonoBehaviour { }

    /// <summary>
    /// Web data. TypeDefIndex: 6194.
    /// </summary>
    public class WebData { }

    /// <summary>
    /// Start data web. TypeDefIndex: 6195.
    /// </summary>
    public class StartDataWeb { }

    /// <summary>
    /// UI Animator. TypeDefIndex: 6196.
    /// </summary>
    public class UIAnimator : MonoBehaviour
    {
        public Vector3 fromPosition;
        public Vector3 toPosition;
        public float duration = 0.3f;

        public void Play()
        {
            StartCoroutine(Animate());
        }

        private System.Collections.IEnumerator Animate()
        {
            float t = 0;
            while (t < duration)
            {
                t += Time.deltaTime;
                transform.localPosition = Vector3.Lerp(fromPosition, toPosition, t / duration);
                yield return null;
            }
        }
    }

    /// <summary>
    /// UI blinking text. TypeDefIndex: 6197.
    /// </summary>
    public class UIBlinkingText : MonoBehaviour
    {
        public float blinkSpeed = 1f;
        private UnityEngine.UI.Text text;

        private void Start()
        {
            text = GetComponent<UnityEngine.UI.Text>();
        }

        private void Update()
        {
            if (text != null)
                text.color = new Color(text.color.r, text.color.g, text.color.b,
                    Mathf.PingPong(Time.time * blinkSpeed, 1f));
        }
    }

    /// <summary>
    /// UI bob animation. TypeDefIndex: 6198.
    /// </summary>
    public class UIBobAnim : MonoBehaviour
    {
        public float speed = 2f;
        public float amplitude = 5f;

        private void Update()
        {
            var pos = transform.localPosition;
            pos.y = Mathf.Sin(Time.time * speed) * amplitude;
            transform.localPosition = pos;
        }
    }

    /// <summary>
    /// UI ping pong animation. TypeDefIndex: 6199.
    /// </summary>
    public class UIPingPongAnim : MonoBehaviour
    {
        public Vector3 from = Vector3.zero;
        public Vector3 to = Vector3.one;
        public float speed = 1f;

        private void Update()
        {
            float t = Mathf.PingPong(Time.time * speed, 1f);
            transform.localScale = Vector3.Lerp(from, to, t);
        }
    }

    /// <summary>
    /// UI tab animate. TypeDefIndex: 6200.
    /// </summary>
    public class UITabAnimate : MonoBehaviour { }

    /// <summary>
    /// UI toggle on enabled. TypeDefIndex: 6201.
    /// </summary>
    public class UIToggleOnEnabled : MonoBehaviour { }

    /// <summary>
    /// Blur optimized (post-effect). TypeDefIndex: 6226.
    /// </summary>
    public class BlurOptimized : PostEffectsBase
    {
        [Range(0, 2)] public int downsample = 1;
        [Range(0.0f, 10.0f)] public float blurSize = 3.0f;
        [Range(1, 4)] public int blurIterations = 2;

        public enum BlurType
        {
            StandardGauss = 0,
            SgxGauss = 1
        }

        [Range(0, 1)] public int blurType = (int)BlurType.StandardGauss;

        public Shader blurShader;
        private Material blurMaterial;

        public override bool CheckResources()
        {
            CheckSupport(false);
            blurMaterial = CheckShaderAndCreateMaterial(blurShader, blurMaterial);
            if (!isSupported) ReportAutoDisable();
            return isSupported && blurMaterial != null;
        }

        private void OnDisable()
        {
            if (blurMaterial) DestroyImmediate(blurMaterial);
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (!CheckResources())
            {
                Graphics.Blit(source, destination);
                return;
            }

            float widthMod = 1.0f / (1.0f * (1 << downsample));
            blurMaterial.SetVector("_Parameter", new Vector4(blurSize * widthMod, -blurSize * widthMod, 0, 0));

            int rtW = source.width >> downsample;
            int rtH = source.height >> downsample;
            RenderTexture rt = RenderTexture.GetTemporary(rtW, rtH, 0, source.format);
            rt.filterMode = FilterMode.Bilinear;
            Graphics.Blit(source, rt, blurMaterial, 0);

            for (int i = 0; i < blurIterations; i++)
            {
                float iterationOffs = i * 1.0f;
                blurMaterial.SetVector("_Parameter",
                    new Vector4(blurSize * widthMod + iterationOffs, -blurSize * widthMod - iterationOffs, 0, 0));
                RenderTexture rt2 = RenderTexture.GetTemporary(rtW, rtH, 0, source.format);
                rt2.filterMode = FilterMode.Bilinear;
                Graphics.Blit(rt, rt2, blurMaterial, 1);
                RenderTexture.ReleaseTemporary(rt);
                rt = rt2;

                rt2 = RenderTexture.GetTemporary(rtW, rtH, 0, source.format);
                rt2.filterMode = FilterMode.Bilinear;
                Graphics.Blit(rt, rt2, blurMaterial, 2);
                RenderTexture.ReleaseTemporary(rt);
                rt = rt2;
            }

            Graphics.Blit(rt, destination);
            RenderTexture.ReleaseTemporary(rt);
        }
    }

    /// <summary>
    /// Post effects base. TypeDefIndex: 6227.
    /// </summary>
    public class PostEffectsBase : MonoBehaviour
    {
        protected bool supportHDR = true;
        protected bool supportDX11 = false;
        protected bool isSupported = true;

        protected void CheckSupport(bool needDepth) { CheckSupport(needDepth, true); }

        protected bool CheckSupport(bool needDepth, bool needHdr)
        {
            isSupported = true;
            supportHDR = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf);
            supportDX11 = SystemInfo.graphicsShaderLevel >= 50;
            return isSupported;
        }

        protected Material CheckShaderAndCreateMaterial(Shader s, Material m2Create)
        {
            if (!s) { return null; }
            if (s.isSupported && m2Create && m2Create.shader == s) return m2Create;
            if (!s.isSupported) return null;
            var m = new Material(s);
            m.hideFlags = HideFlags.DontSave;
            return m;
        }

        public virtual bool CheckResources() { return false; }
        protected void ReportAutoDisable() { }
        protected void NotSupported() { enabled = false; }
        protected void DrawBorder(RenderTexture dest, Material material) { }
    }

    /// <summary>
    /// Multi-dimensional array helper. TypeDefIndex: 6228.
    /// </summary>
    public class MultiDim
    {
        public static int[][] JaggedInt(int a) => new int[a][];
        public static float[][] JaggedFloat(int a) => new float[a][];
        public static string[][] JaggedString(int a) => new string[a][];
    }
}
