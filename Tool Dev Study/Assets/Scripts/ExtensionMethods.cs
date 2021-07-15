using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods {

    public static Vector3 Round(this Vector3 v) { // extension methods --- make the function kinda "Exists" inside the thing
        v.x = Mathf.Round(v.x);
        v.y = Mathf.Round(v.y);
        v.z = Mathf.Round(v.z);
        return v;
    }

    public static Vector3 Round(this Vector3 v, float gridSize) {
        return (v / gridSize).Round() * gridSize;
    }

    public static float Round(this float v, float gridSize) {
        return Mathf.Round(v / gridSize) * gridSize;
    }

    public static float AtLeast(this float v, float min) => Mathf.Max(v, min);
    public static int AtLeast(this int v, int min) => Mathf.Max(v, min);
}








public struct SpawnData
{
    public Vector2 pointInDisc;
    public float randAngleDeg;
    public GameObject prefab;

    public void SetRandomValues(List<GameObject> prefabs)
    {
        pointInDisc = Random.insideUnitCircle;
        randAngleDeg = Random.value * 360;
        prefab = prefabs[Random.Range(0, prefabs.Count)];
    }
}

public class SpawnPoint
{
    public SpawnData spawnData;
    public Vector3 position;
    public Quaternion rotation;
    public bool isValid = false;

    public Vector3 Up => rotation * Vector3.up;
    public SpawnPoint(Vector3 position, Quaternion rotation, SpawnData spawnData) {
        this.spawnData = spawnData;
        this.position = position;
        this.rotation = rotation;

        // check if this mesh can be placed/fit current location
        SpawnablePrefabs spawnablePrefab = spawnData.prefab.GetComponent<SpawnablePrefabs>();
        if (spawnablePrefab == null) isValid = true;
        else {
            float h = spawnablePrefab.height;
            Ray ray = new Ray(position, Up);
            isValid = Physics.Raycast(ray, h) == false;
        }
    }
}


[MenuItem("Tools/Grimm Cannon")]
public static void OpenGrimm() => GetWindow<GrimmCannon>();

public float radius = 2f;
public int spawnCount = 8;
public List<GameObject> spawnPrefabs = new List<GameObject>();

public Material materialInvalid;

public Material previewMaterial;

SerializedObject so;
SerializedProperty propRadius;
SerializedProperty propSpawnCount;
SerializedProperty propSpawnPrefab;
SerializedProperty propPreviewMaterial;

[SerializeField] bool[] prefabSelectionStates;
SpawnData[] randPoints;
GameObject[] prefabs;

private void OnEnable() {

    so = new SerializedObject(this);
    propRadius = so.FindProperty("radius");
    propSpawnCount = so.FindProperty("spawnCount");
    propSpawnPrefab = so.FindProperty("spawnPrefab");
    propPreviewMaterial = so.FindProperty("previewMaterial");
    GenerateRandomPoints();
    SceneView.duringSceneGui += DuringSceneGUI;

    Shader sh = Shader.Find("Unlit/InvalidSpawn")
    materialInvalid = new Material(sh);

    // load spawn prefabs
    string[] guids = AssetDatabase.FindAssets("t:prefab", new[] { "Assets/Prefabs" }); // find all Global Unique Identifiers
    IEnumerable<string> paths = guids.Select(AssetDatabase.GUIDToAssetPath);
    prefabs = paths.Select(AssetDatabase.LoadAssetAtPath<GameObject>).ToArray();
    if (prefabSelectionStates == null || prefabSelectionStates.Length != prefabs.Length) {
        prefabSelectionStates = new bool[prefabs.Length];
    }
}

private void OnDisable() {
    SceneView.duringSceneGui -= DuringSceneGUI;
    DestroyImmediate(materialInvalid);
}

void GenerateRandomPoints() {
    randPoints = new SpawnData[spawnCount];
    for (int i = 0; i < spawnCount; i++)
    {
        randPoints[i].SetRandomValues(spawnPrefabs);
    }
}

private void OnGUI() { // from the window
    so.Update();
    EditorGUILayout.PropertyField(propRadius);
    propRadius.floatValue = propRadius.floatValue.AtLeast(1f); // make sure it stops at 1
    EditorGUILayout.PropertyField(propSpawnCount);
    propSpawnCount.intValue = propSpawnCount.intValue.AtLeast(1); // make sure it stops at 1
    EditorGUILayout.PropertyField(propSpawnPrefab);
    EditorGUILayout.PropertyField(propPreviewMaterial);

    if (so.ApplyModifiedProperties()) {
        GenerateRandomPoints();
        SceneView.RepaintAll();
    }

    // if you clicked left mouse button in the editor window
    if (Event.current.type == EventType.MouseDown && Event.current.button == 0) {
        GUI.FocusControl(null);
        Repaint(); // repaint on the editor window ui
    }
}

void TrySpawnObjects (List<SpawnPoint> spawnPoints) {
    if (spawnPrefabs.Count == 0) return;
    foreach (SpawnPoint spawnPoint in spawnPoints) {
        if (spawnPoint.isValid == false) continue;
        // spawn prefab
        GameObject spawnedThing = (GameObject)PrefabUtility.InstantiatePrefab(spawnPoint.spawnData.prefab);
        Undo.RegisterCreatedObjectUndo(spawnedThing, "Spawn objects");
        spawnedThing.transform.position = spawnPoint.position;
        spawnedThing.transform.rotation = spawnPoint.rotation;
    }

    GenerateRandomPoints(); //update points
}

bool TryRaycastFromCamera(Vector2 cameraUp, out Matrix4x4 tangentToWorldMtx) {
    Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
    if (Physics.Raycast(ray, out RaycastHit hit)) {
        // setting up tangent space
        Vector3 hitNormal = hit.normal;
        Vector3 hitTangent = Vector3.Cross(hitNormal, camTf.up).normalized;
        Vector3 hitBitangent = Vector3.Cross(hitNormal, hitTangent);
        tangentToWorldMtx = Matrix4x4.TRS(hit.point, Quaternion.LookRotation(hitNormal, hitBitangent), Vector3.one);
        return true;
    }

    tangentToWorldMtx = default;
    return false;
}

void DuringSceneGUI(SceneView sceneView) {
    Handles.BeginGUI();
    Rect rect = new Rect(8, 8, 64, 64);
    for (int i = 0; i < prefabs.Length; i++) {
        // GUI.Button(rect, new GUIContent(icon)) - NORMAL BUTTON
        GameObject prefab = prefabs[i];
        Texture icon = AssetPreview.GetAssetPreview(prefab);
        EditorGUI.BeginChangeCheck();
        prefabSelectionStates[i] = GUI.Toggle(rect, prefabSelectionStates[i], new GUIContent(icon));
        if (EditorGUI.EndChangeCheck()) {
            // update selection list
            spawnPrefabs.Clear();
            for (int j = 0; j < prefabs.Length; j++) {
                if (prefabSelectionStates[j]) spawnPrefabs.Add(prefabs[j]);
            }
        }
        //spawnPrefabs = prefab;
        rect.y += rect.height + 2;
    }
    Handles.EndGUI();

    Handles.zTest = CompareFunction.LessEqual;
    Transform camTf = sceneView.camera.transform;

    // repainting on mouse move
    if (Event.current.type == EventType.MouseMove) sceneView.Repaint();

    // checking if we are holding alt
    bool holdingAlt = (Event.current.modifiers & EventModifiers.Alt) != 0;
    if (Event.current.type == EventType.ScrollWheel && !holdingAlt) { // change the radius on scroll wheel
        float scrollDirection = Mathf.Sign(Event.current.delta.y); // -1 || 1 || 0

        so.Update();
        propRadius.floatValue *= 1 + scrollDirection * 0.05f;
        so.ApplyModifiedProperties();
        Repaint(); // repaint the editor window
        Event.current.Use(); // consume the event, don't let it fall through
    }

    // if the cursor is pointing on valid ground
    if (TryRaycastFromCamera(camTf.up, out Matrix4x4 tangentToWorld)) {
        List<Pose> spawnPoses = GetSpawnPoses(tangentToWorld);
        if (Event.current.type == EventType.Repaint) {
            DrawCircleRegion(tangentToWorld);
            DrawSpawnPreviews(spawnPoses, sceneView.camera);
        }
    }

    // spawn on press
    if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Space) TrySpawnObjects(spawnPoses);
}

void DrawSpawnPreviews (List<SpawnPoint> spawnPoints, Camera cam) {
    foreach (SpawnPoint spawnPoint in spawnPoints) {
        if (spawnPoint.spawnData.prefab != null) {
            // draw preview of all meshes in prefab
            Matrix4x4 poseToWorld = Matrix4x4.TRS(spawnPoint.position, spawnPoint.rotation, Vector3.one);
            DrawPrefab(spawnPoint.spawnData.prefab, poseToWorld, cam, spawnPoint.isValid);
        } else {
            // prefab missing, draw sphere and normal on surface instead
            Handles.SphereHandleCap(-1, spawnPoint.position, Quaternion.identity, 0.1f, EventType.Repaint);
            Handles.DrawAAPolyline(spawnPoint.position, spawnPoint.position + spawnPoint.Up);
        }
    }
}

 void DrawPrefab (GameObject prefab, Matrix4x4 poseToWorld, Camera cam, bool valid) {
    MeshFilter[] filters = prefab.GetComponentsInChildren<MeshFilter>();
    foreach (MeshFilter filter in filters) {
        Matrix4x4 childToPose = filter.transform.localToWorldMatrix;
        Matrix4x4 childToWorldMtx = poseToWorld * childToPose;
        Mesh mesh = filter.sharedMesh;
        Material mat = valid ? filter.GetComponent<MeshRenderer>().sharedMaterial : materialInvalid;
        //mat.SetPass(0); // global setting Graphics.X commands
        Graphics.DrawMesh(mesh, childToWorldMtx, mat, 0, cam);
    }
}

List<SpawnPoint> GetSpawnPoints (Matrix4x4 tangentToWorld){
    List<SpawnPoint> hitSpawnPoints = new List<SpawnPoint>;
    foreach (SpawnData rndDataPoint  in hitSpawnPoints) {
        // create ray for this point
        Ray ptRay = GetCircleRay(tangentToWorld, rndDataPoint.pointInDisc);
        // raycast to find point on surface
        if (Physics.Raycast(ptRay, out RaycastHit ptHit)) {
            // calculate rotation and assign to pose together with position
            Quaternion randRot = Quaternion.Euler(0f, 0f, rndDataPoint.randAngleDeg);
            Quaternion rot = Quaternion.LookRotation(ptHit.point, rot, rndDataPoint);
            SpawnPoint spawnPoint = new SpawnPoint(ptHit.point, rot, rndDataPoint);
            hitSpawnPoints.Add(spawnPoint);
        }
    }
}

Ray GetCircleRay(Matrix4x4 tangentToWorld, Vector2 pointInCircle) {

}

void DrawCircleRegion(Matrix4x4 localToWorld) {

}

void DrawAxes (Matrix4x4 localToWorld) {

}