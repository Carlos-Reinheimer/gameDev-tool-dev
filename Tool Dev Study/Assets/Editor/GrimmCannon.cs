using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GrimmCannon : EditorWindow {

    [MenuItem("Tools/Grimm Cannon")]
    public static void OpenGrimm() => GetWindow<GrimmCannon>();

    public float radius = 2f;
    public int spawnCount = 8;

    SerializedObject so;
    SerializedProperty propRadius;
    SerializedProperty propSpawnCount;

    Vector2[] randPoints;


    private void OnEnable() {

        so = new SerializedObject(this);
        propRadius = so.FindProperty("radius");
        propSpawnCount = so.FindProperty("spawnCount");
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

    private void OnGUI() { // from the window
        so.Update();
        EditorGUILayout.PropertyField(propRadius);
        propRadius.floatValue = propRadius.floatValue.AtLeast(1f); // make sure it stops at 1
        EditorGUILayout.PropertyField(propSpawnCount);
        propSpawnCount.intValue = propSpawnCount.intValue.AtLeast(1); // make sure it stops at 1

        if (so.ApplyModifiedProperties())
        {
            GenerateRandomPoints();
            SceneView.RepaintAll();
        }

    }

    void DuringSceneGUI(SceneView sceneView) {
        Transform camTf = sceneView.camera.transform;

        Ray ray = new Ray(camTf.position, camTf.forward);
        if (Physics.Raycast(ray, out RaycastHit hit)) {

            // setting up tangent space
            Vector3 hitNormal= hit.normal;
            Vector3 hitTangent = Vector3.Cross(hitNormal, camTf.up).normalized;
            Vector3 hitBitangent = Vector3.Cross(hitNormal, hitTangent);

            foreach (Vector2 p in randPoints) {
                Vector3 worldPos = hit.point + (hitTangent * p.x + hitBitangent * p.y) * radius;
                DrawSphere(worldPos);
            }

            Handles.color = Color.red;
            Handles.DrawAAPolyLine(6, hit.point, hit.point + hitTangent);
            Handles.color = Color.green;
            Handles.DrawAAPolyLine(6, hit.point, hit.point + hitBitangent);
            Handles.color = Color.blue;
            Handles.DrawAAPolyLine(6, hit.point, hit.point + hitNormal);

            Handles.DrawAAPolyLine(6, hit.point, hit.point + hit.normal);
            Handles.DrawWireDisc(hit.point, hit.normal, radius);
        }
    }

}
