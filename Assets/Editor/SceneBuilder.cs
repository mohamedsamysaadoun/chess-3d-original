#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace EivaaChess.Game
{
    /// <summary>
    /// Scene Builder — يبني مشهد Main.unity من scene_hierarchy.json
    /// ينفع يشتغل لما المشروع يتفتح في Unity Editor
    /// من قائمة: Chess > Build Scene From JSON
    /// </summary>
    public class SceneBuilder : MonoBehaviour
    {
        const string SCENE_DATA_PATH = "reference/scene-data/scene_hierarchy.json";
        const string SCENE_OUTPUT_PATH = "Assets/Scenes/Main.unity";

        [MenuItem("Chess/Build Scene From JSON")]
        public static void BuildScene()
        {
            string projectPath = System.IO.Path.GetDirectoryName(Application.dataPath);
            string jsonPath = System.IO.Path.Combine(projectPath, SCENE_DATA_PATH);

            if (!File.Exists(jsonPath))
            {
                EditorUtility.DisplayDialog("Error", $"Scene data not found at:\n{jsonPath}", "OK");
                return;
            }

            EditorUtility.DisplayProgressBar("Building Scene", "Loading JSON...", 0.1f);

            string json = File.ReadAllText(jsonPath);
            SceneHierarchyData data = JsonUtility.FromJson<SceneHierarchyData>(json);
            if (data == null || data.gameObjects == null)
            {
                // Maybe it's a wrapped array
                var wrapper = JsonUtility.FromJson<GameObjectArray>(json);
                if (wrapper?.items != null)
                    data = new SceneHierarchyData { gameObjects = wrapper.items };
                else
                {
                    // Try as raw array
                    json = "{\"gameObjects\":" + json + "}";
                    data = JsonUtility.FromJson<SceneHierarchyData>(json);
                }
            }

            if (data?.gameObjects == null || data.gameObjects.Length == 0)
            {
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("Error", "Failed to parse scene hierarchy JSON", "OK");
                return;
            }

            // Create new scene
            EditorUtility.DisplayProgressBar("Building Scene", "Creating new scene...", 0.2f);
            Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // Build hierarchy
            Dictionary<int, GameObject> idToObj = new Dictionary<int, GameObject>();
            int total = data.gameObjects.Length;
            int created = 0;

            foreach (var goData in data.gameObjects)
            {
                EditorUtility.DisplayProgressBar("Building Scene",
                    $"Creating {goData.name} ({created}/{total})",
                    0.3f + (0.6f * created / total));

                GameObject go = new GameObject(goData.name ?? "GameObject");
                go.SetActive(goData.active);

                // Apply transform
                if (goData.position != null && goData.position.Length >= 3)
                    go.transform.position = new Vector3(goData.position[0], goData.position[1], goData.position[2]);

                if (goData.rotation != null && goData.rotation.Length >= 4)
                    go.transform.rotation = new Quaternion(goData.rotation[0], goData.rotation[1], goData.rotation[2], goData.rotation[3]);

                if (goData.scale != null && goData.scale.Length >= 3)
                    go.transform.localScale = new Vector3(goData.scale[0], goData.scale[1], goData.scale[2]);

                // Apply components
                if (goData.components != null)
                {
                    foreach (var comp in goData.components)
                    {
                        AddComponent(go, comp);
                    }
                }

                idToObj[goData.path_id] = go;
                created++;
            }

            // Set parent-child relationships
            EditorUtility.DisplayProgressBar("Building Scene", "Setting up hierarchy...", 0.95f);
            // Note: scene_hierarchy.json doesn't have explicit parent IDs, but
            // we can infer from path patterns in component data

            // Save scene
            EditorUtility.DisplayProgressBar("Building Scene", "Saving scene...", 0.99f);
            Directory.CreateDirectory("Assets/Scenes");
            EditorSceneManager.SaveScene(newScene, SCENE_OUTPUT_PATH);

            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("Success",
                $"Scene built with {created} GameObjects.\nSaved to: {SCENE_OUTPUT_PATH}", "OK");
        }

        static void AddComponent(GameObject go, ComponentData comp)
        {
            if (comp == null || string.IsNullOrEmpty(comp.type)) return;

            switch (comp.type)
            {
                case "Camera":
                    if (!go.GetComponent<Camera>())
                        go.AddComponent<Camera>();
                    break;
                case "AudioListener":
                    if (!go.GetComponent<AudioListener>())
                        go.AddComponent<AudioListener>();
                    break;
                case "AudioSource":
                    if (!go.GetComponent<AudioSource>())
                        go.AddComponent<AudioSource>();
                    break;
                case "Light":
                    if (!go.GetComponent<Light>())
                        go.AddComponent<Light>();
                    break;
                case "MeshFilter":
                    if (!go.GetComponent<MeshFilter>())
                        go.AddComponent<MeshFilter>();
                    break;
                case "MeshRenderer":
                    if (!go.GetComponent<MeshRenderer>())
                        go.AddComponent<MeshRenderer>();
                    break;
                case "BoxCollider":
                    if (!go.GetComponent<BoxCollider>())
                        go.AddComponent<BoxCollider>();
                    break;
                case "LineRenderer":
                    if (!go.GetComponent<LineRenderer>())
                        go.AddComponent<LineRenderer>();
                    break;
                case "ParticleSystem":
                    if (!go.GetComponent<ParticleSystem>())
                        go.AddComponent<ParticleSystem>();
                    break;
                case "Canvas":
                    if (!go.GetComponent<Canvas>())
                        go.AddComponent<Canvas>();
                    break;
                case "CanvasGroup":
                    if (!go.GetComponent<CanvasGroup>())
                        go.AddComponent<CanvasGroup>();
                    break;
                case "CanvasRenderer":
                    if (!go.GetComponent<CanvasRenderer>())
                        go.AddComponent<CanvasRenderer>();
                    break;
                case "RectTransform":
                    if (!go.GetComponent<RectTransform>())
                        go.AddComponent<RectTransform>();
                    break;
                case "SpriteRenderer":
                    if (!go.GetComponent<SpriteRenderer>())
                        go.AddComponent<SpriteRenderer>();
                    break;
                // MainScript is the main game controller
                case "MonoBehaviour":
                    // We'd need to map script_path_id to script type
                    // For now, add MainScript if name suggests it's the controller
                    break;
            }
        }

        [MenuItem("Chess/Add MainScript to Scene")]
        public static void AddMainScript()
        {
            Scene scene = EditorSceneManager.GetActiveScene();
            GameObject mainObj = new GameObject("MainController");
            mainObj.AddComponent<MainScript>();
            EditorSceneManager.MarkSceneDirty(scene);
            Debug.Log("MainScript added to scene");
        }

        [System.Serializable]
        public class SceneHierarchyData
        {
            public GameObjectData[] gameObjects;
        }

        [System.Serializable]
        public class GameObjectArray
        {
            public GameObjectData[] items;
        }

        [System.Serializable]
        public class GameObjectData
        {
            public string name;
            public bool active;
            public int path_id;
            public float[] position;
            public float[] rotation;
            public float[] scale;
            public ComponentData[] components;
        }

        [System.Serializable]
        public class ComponentData
        {
            public string type;
            public int path_id;
            public int gameobject_path_id;
            public int script_path_id;
        }
    }
}
#endif
