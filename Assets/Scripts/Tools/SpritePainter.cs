using UnityEngine;
using UnityEditor;

public class SpritePainter : EditorWindow
{
    public SpritePropLayer[] propLayers;
    public float brushSize = 5f;
    public int density = 10;
    public Transform parentContainer;
    public GameObject prefabToMakeLayerFrom;

    [MenuItem("Tools/Advanced 2D Sprite Painter")]
    static void Init() => GetWindow<SpritePainter>("2D Sprite Painter");

    void OnEnable() => SceneView.duringSceneGui += OnSceneGUI;
    void OnDisable() => SceneView.duringSceneGui -= OnSceneGUI;

    void OnGUI()
    {
        SerializedObject so = new SerializedObject(this);
        SerializedProperty layersProp = so.FindProperty("propLayers");

        EditorGUILayout.PropertyField(layersProp, true);
        prefabToMakeLayerFrom = (GameObject)EditorGUILayout.ObjectField("New Paintable Prefab", prefabToMakeLayerFrom, typeof(GameObject), false);

        if (prefabToMakeLayerFrom != null)
        {
            CreateAndAddLayer(prefabToMakeLayerFrom);
            prefabToMakeLayerFrom = null;
        }

        EditorGUILayout.Space();
        brushSize = EditorGUILayout.FloatField("Brush Size", brushSize);
        density = EditorGUILayout.IntSlider("Density", density, 1, 200);
        parentContainer = (Transform)EditorGUILayout.ObjectField("Parent Container", parentContainer, typeof(Transform), true);

        so.ApplyModifiedProperties();
    }

    void OnSceneGUI(SceneView sceneView)
    {
        Event e = Event.current;
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Handles.color = new Color(0, 1, 0, 0.25f);
            Handles.DrawSolidDisc(hit.point, Vector3.up, brushSize);

            if (e.type == EventType.MouseDown && e.button == 0 && !e.alt)
            {
                for (int i = 0; i < density; i++)
                {
                    Vector2 offset = Random.insideUnitCircle * brushSize;
                    Vector3 spawnPos = hit.point + new Vector3(offset.x, 0f, offset.y);

                    if (Physics.Raycast(spawnPos + Vector3.up * 100f, Vector3.down, out RaycastHit groundHit))
                    {
                        SpawnPropsAt(groundHit.point, groundHit.normal);
                    }
                }

                e.Use();
            }
        }
    }

    void SpawnPropsAt(Vector3 position, Vector3 normal)
    {
        foreach (var layer in propLayers)
        {
            if (layer == null || layer.propPrefabs.Length == 0 || Random.value > layer.spawnChance)
                continue;

            GameObject prefab = layer.propPrefabs[Random.Range(0, layer.propPrefabs.Length)];
            GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

            Undo.RegisterCreatedObjectUndo(obj, "Paint Prop");
            obj.transform.position = position;
            obj.transform.up = normal;

            float scale = Random.Range(layer.scaleRange.x, layer.scaleRange.y);
            obj.transform.localScale = new Vector3(scale, scale, scale);

            if (obj.TryGetComponent(out SpriteRenderer sr))
            {
                sr.color = layer.colorVariation.Evaluate(Random.value);
            }

            if (parentContainer != null)
            {
                obj.transform.SetParent(parentContainer);
            }
        }
    }

    void CreateAndAddLayer(GameObject prefab)
    {
        if (propLayers == null) propLayers = new SpritePropLayer[0];

        foreach (var layer in propLayers)
        {
            if (layer != null && layer.propPrefabs != null && System.Array.Exists(layer.propPrefabs, p => p == prefab))
            {
                Debug.LogWarning("Prefab already exists in a layer.");
                return;
            }
        }

        string dirPath = "Assets/Resources/SpritePainterLayers";
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            AssetDatabase.CreateFolder("Assets", "Resources");
        if (!AssetDatabase.IsValidFolder(dirPath))
            AssetDatabase.CreateFolder("Assets/Resources", "SpritePainterLayers");

        string prefabName = prefab.name;
        string assetPath = AssetDatabase.GenerateUniqueAssetPath($"{dirPath}/{prefabName}_Layer.asset");

        SpritePropLayer newLayer = ScriptableObject.CreateInstance<SpritePropLayer>();
        newLayer.layerName = prefabName;
        newLayer.propPrefabs = new GameObject[] { prefab };
        newLayer.scaleRange = new Vector2(1f, 1f);
        newLayer.spawnChance = 1f;

        Gradient g = new Gradient();
        g.colorKeys = new[] { new GradientColorKey(Color.white, 0), new GradientColorKey(Color.white, 1) };
        newLayer.colorVariation = g;

        AssetDatabase.CreateAsset(newLayer, assetPath);
        AssetDatabase.SaveAssets();

        var newList = new SpritePropLayer[propLayers.Length + 1];
        propLayers.CopyTo(newList, 0);
        newList[^1] = newLayer;
        propLayers = newList;

        Debug.Log($"Created new SpritePropLayer: {prefabName} at {assetPath}");
    }
}
