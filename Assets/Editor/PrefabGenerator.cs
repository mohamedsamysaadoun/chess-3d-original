#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

namespace EivaaChess.Game
{
    /// <summary>
    /// Prefab Generator — بيولّد piece prefabs من الـ meshes
    /// من قائمة: Chess > Generate Piece Prefabs
    /// </summary>
    public class PrefabGenerator : MonoBehaviour
    {
        const string MESHES_DIR = "Assets/Meshes";
        const string PREFAB_DIR = "Assets/Prefabs/Pieces";

        [MenuItem("Chess/Generate Piece Prefabs")]
        public static void GeneratePiecePrefabs()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            if (!AssetDatabase.IsValidFolder("Assets/Prefabs/Pieces"))
                AssetDatabase.CreateFolder("Assets/Prefabs", "Pieces");

            // Piece types
            string[] pieceTypes = { "pawn", "knight", "bishop", "rook", "queen", "king" };
            string[] sets = { "1", "2", "3" };
            string[] colors = { "White", "Black" };

            int created = 0;
            foreach (string set in sets)
            {
                foreach (string piece in pieceTypes)
                {
                    foreach (string color in colors)
                    {
                        string meshName = $"{piece}Mesh";
                        string[] meshAssets = AssetDatabase.FindAssets(meshName, new[] { MESHES_DIR });
                        if (meshAssets.Length == 0) continue;

                        Mesh mesh = AssetDatabase.LoadAssetAtPath<Mesh>(
                            AssetDatabase.GUIDToAssetPath(meshAssets[0]));
                        if (mesh == null) continue;

                        // Create GameObject with mesh
                        GameObject go = new GameObject($"{set}{color}{piece}");
                        MeshFilter mf = go.AddComponent<MeshFilter>();
                        mf.sharedMesh = mesh;
                        MeshRenderer mr = go.AddComponent<MeshRenderer>();

                        // Try to find material
                        string matName = $"{set}{color}Wood{char.ToUpper(piece[0])}{piece.Substring(1)}";
                        string[] matAssets = AssetDatabase.FindAssets(matName, new[] { "Assets/Materials" });
                        if (matAssets.Length > 0)
                        {
                            Material mat = AssetDatabase.LoadAssetAtPath<Material>(
                                AssetDatabase.GUIDToAssetPath(matAssets[0]));
                            if (mat != null)
                                mr.sharedMaterial = mat;
                        }

                        // Save as prefab
                        string prefabPath = $"{PREFAB_DIR}/{set}{color}{piece}.prefab";
                        PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
                        DestroyImmediate(go);
                        created++;
                    }
                }
            }

            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("Success", $"Created {created} piece prefabs in {PREFAB_DIR}", "OK");
        }

        [MenuItem("Chess/Generate Indicator Prefabs")]
        public static void GenerateIndicatorPrefabs()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Prefabs/Indicators"))
                AssetDatabase.CreateFolder("Assets/Prefabs", "Indicators");

            // Red indicator (capture)
            CreateIndicator("RedIndicator", new Color(1, 0.2f, 0.2f, 0.5f));
            // Green indicator (possible move)
            CreateIndicator("GreenIndicator", new Color(0.2f, 1, 0.2f, 0.5f));
            // Blue indicator (selection)
            CreateIndicator("BlueIndicator", new Color(0.2f, 0.4f, 1, 0.5f));
            // Danger indicator (king in danger)
            CreateIndicator("DangerIndicator", new Color(1, 0.6f, 0, 0.7f));

            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("Success", "Created 4 indicator prefabs", "OK");
        }

        static void CreateIndicator(string name, Color color)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Quad);
            go.name = name;
            go.transform.rotation = Quaternion.Euler(90, 0, 0);  // Flat on ground
            go.transform.localScale = new Vector3(0.9f, 0.9f, 1);

            // Create material
            Material mat = new Material(Shader.Find("Transparent/Diffuse"));
            mat.color = color;
            go.GetComponent<MeshRenderer>().sharedMaterial = mat;

            // Save material
            string matPath = $"Assets/Materials/{name}_Mat.mat";
            AssetDatabase.CreateAsset(mat, matPath);

            // Save prefab
            string prefabPath = $"Assets/Prefabs/Indicators/{name}.prefab";
            PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
            DestroyImmediate(go);
        }

        [MenuItem("Chess/Generate UI Panel Prefabs")]
        public static void GenerateUIPanels()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Prefabs/UI"))
                AssetDatabase.CreateFolder("Assets/Prefabs", "UI");

            string[] panels = {
                "FirstLoading", "MainMenu", "NewGameSetup", "GameLobby",
                "InGame", "Pause", "PromotePrompt", "UndoConfirm",
                "GameOver", "Settings", "Customize", "Stats",
                "Help", "RateUs", "IAP", "Tutorial",
                "TutorialComplete", "HighScores", "AvatarSelect"
            };

            foreach (string panel in panels)
            {
                GameObject go = new GameObject(panel);
                RectTransform rt = go.AddComponent<RectTransform>();
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;

                CanvasRenderer cr = go.AddComponent<CanvasRenderer>();
                Image img = go.AddComponent<Image>();
                img.color = new Color(0, 0, 0, 0.85f);

                string prefabPath = $"Assets/Prefabs/UI/{panel}.prefab";
                PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
                DestroyImmediate(go);
            }

            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("Success", $"Created {panels.Length} UI panel prefabs", "OK");
        }

        [MenuItem("Chess/Generate All Prefabs")]
        public static void GenerateAll()
        {
            GeneratePiecePrefabs();
            GenerateIndicatorPrefabs();
            GenerateUIPanels();
            EditorUtility.DisplayDialog("All Done", "All prefabs generated successfully!", "OK");
        }
    }
#endif
