using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

public class GrimmCannon : EditorWindow {

    [MenuItem("Tools/Grimm Cannon")]
    public static void OpenGrimm() => GetWindow<GrimmCannon>();

    public float radius = 2f;
    public int spawnCount = 8;
    public GameObject spawnPrefab = null;
    public Material previewMaterial;

    SerializedObject so;
    SerializedProperty propRadius;
    SerializedProperty propSpawnCount;
    SerializedProperty propSpawnPrefab;
    SerializedProperty propPreviewMaterial;

    Vector2[] randPoints;


    private void OnEnable() {

        so = new SerializedObject(this);
        propRadius = so.FindProperty("radius");
        propSpawnCount = so.FindProperty("spawnCount");
        propSpawnPrefab = so.FindProperty("spawnPrefab");
        propPreviewMaterial = so.FindProperty("previewMaterial");
        GenerateRandomPoints();
        SceneView.duringSceneGui += DuringSceneGUI;
    }

    private void OnDisable() => SceneView.duringSceneGui -= DuringSceneGUI;

    void GenerateRandomPoints() {
        randPoints = new Vector2[spawnCount];
        for (int i = 0; i < spawnCount; i++) {
            randPoints[i] = Random.insideUnitCircle;
        }
    }

    void DrawSphere(Vector3 pos) {
        Handles.SphereHandleCap(-1, pos, Quaternion.identity, 0.1f, EventType.Repaint);
    }

    void TrySpawnObjects(List<Pose> poses) {
        if (spawnPrefab == null) return;

        foreach (Pose pose in poses) {
            // spawn prefab
            GameObject spawnedThing = (GameObject)PrefabUtility.InstantiatePrefab(spawnPrefab);
            Undo.RegisterCreatedObjectUndo(spawnedThing, "Spawned objects");
            spawnedThing.transform.position = pose.position;
            spawnedThing.transform.rotation = pose.rotation;
        }
        GenerateRandomPoints(); // update points


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

    //[Flags]
    //public enum Modifiers {
    //    Ctrl = 1,
    //    Alt = 2,
    //    Shift = 4,
    //    ShigtRight = 8,
    //    // 16
    //    // 32
    //}

    void DuringSceneGUI(SceneView sceneView) {
        Handles.zTest = CompareFunction.LessEqual;
        Transform camTf = sceneView.camera.transform;

        // repainting on mouse move
        if (Event.current.type == EventType.MouseMove) sceneView.Repaint();

        // checking if we are holding alt
        bool holdingAlt = (Event.current.modifiers & EventModifiers.Alt) != 0;

        // change the radius on scroll wheel
        if (Event.current.type == EventType.ScrollWheel && !holdingAlt)
        {
            float scrollDirection = Mathf.Sign(Event.current.delta.y); // -1 || 1 || 0

            so.Update();
            propRadius.floatValue *= 1 + scrollDirection * 0.05f;
            so.ApplyModifiedProperties();
            Repaint(); // repaint the editor window
            Event.current.Use(); // consume the event, don't let it fall through
        }

        // getting the mouse position
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        //Ray ray = new Ray(camTf.position, camTf.forward);

        if (Physics.Raycast(ray, out RaycastHit hit)) {

            // setting up tangent space
            Vector3 hitNormal= hit.normal;
            Vector3 hitTangent = Vector3.Cross(hitNormal, camTf.up).normalized;
            Vector3 hitBitangent = Vector3.Cross(hitNormal, hitTangent);

            Ray GetTangentRay(Vector2 tangentSpacePos) {
                Vector3 rayOrigin = hit.point + (hitTangent * tangentSpacePos.x + hitBitangent * tangentSpacePos.y) * radius;
                rayOrigin += hitNormal * 2; // offset margin 
                Vector3 rayDirection = -hitNormal;
                return new Ray(rayOrigin, rayDirection);
            }

            List<Pose> hitPoses = new List<Pose>();

            // drawing points
            foreach (Vector2 p in randPoints) {
                // create rau for this point
                Ray ptRay = GetTangentRay(p);
                // raycast to find point on surface

                if (Physics.Raycast(ptRay, out RaycastHit ptHit)) {

                    // calculate rotation and assign to pose together with position
                    float randomAngDeg = Random.value * 360;
                    Quaternion randRot = Quaternion.Euler(0f, 0f, randomAngDeg);
                    Quaternion rot = Quaternion.LookRotation(ptHit.normal) * (randRot * Quaternion.Euler(90f, 0f, 0f));
                    Pose pose = new Pose(ptHit.point, rot);
                    hitPoses.Add(pose);

                    // draw sphere and normal on surface
                    DrawSphere(ptHit.point);
                    Handles.DrawAAPolyLine(ptHit.point, ptHit.point + ptHit.normal);

                    // mesh 
                    MeshFilter[] filters = spawnPrefab.GetComponentsInChildren<MeshFilter>();
                    foreach (MeshFilter filter in filters) {
                        Mesh mesh = filter.sharedMesh;
                        Material mat = filter.GetComponent<MeshRenderer>().sharedMaterial;
                        mat.SetPass(0);
                        Graphics.DrawMeshNow(mesh, pose.position, pose.rotation);
                    }

                    //Mesh mesh = spawnPrefab.GetComponent<MeshFilter>().sharedMesh;
                    //Material mat = spawnPrefab.GetComponent<MeshRenderer>().sharedMaterial; // use material of the prefab
                    //mat.SetPass(0);
                    //Graphics.DrawMeshNow(mesh, pose.position, pose.rotation);
                }
            }

            // spawn on press
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Space) {
                // Debug.Log(Event.current.type);
                TrySpawnObjects(hitPoses);

            }

            Handles.color = Color.red;
            Handles.DrawAAPolyLine(6, hit.point, hit.point + hitTangent);
            Handles.color = Color.green;
            Handles.DrawAAPolyLine(6, hit.point, hit.point + hitBitangent);
            Handles.color = Color.blue;
            Handles.DrawAAPolyLine(6, hit.point, hit.point + hitNormal);

            // draw circle adptaded to the terrain
            const float TAU = 6.28318530718f;
            const int circleDetail = 64;
            Vector3[] ringPoints = new Vector3[circleDetail];
            for (int i = 0; i < circleDetail; i++) {
                float t = 1 / ((float)circleDetail-1); // go back to 0/1 position
                float angRad = t * TAU;
                Vector2 dir = new Vector2(Mathf.Cos(angRad), Mathf.Sin(angRad));
                Ray r = GetTangentRay(dir);
                if (Physics.Raycast(r, out RaycastHit cHit)) ringPoints[i] = cHit.point + cHit.normal * 0.02f;
                else ringPoints[i] = r.origin;
            }

            Handles.DrawAAPolyLine(ringPoints);
            //Handles.DrawWireDisc(hit.point, hit.normal, radius);
        }



    }

}
