using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;

public class SnapperTool : EditorWindow {

    [MenuItem("Tools/Snapper Tool")]
    public static void OpenTheThing() => GetWindow<SnapperTool>("Snapper"); // single window 
    public float gridSize = 1f;
    public Vector3 point;

    SerializedObject so;
    SerializedProperty propGridSize;
    SerializedProperty propPoint;

    private void OnEnable() {
        so = new SerializedObject(this);
        propGridSize = so.FindProperty("gridSize");
        propPoint = so.FindProperty("point");

        Selection.selectionChanged += Repaint; // update the window when selection change
        SceneView.duringSceneGui += DuringSceneGUI;
    }
    private void OnDisable() {
        Selection.selectionChanged -= Repaint;
        SceneView.duringSceneGui -= DuringSceneGUI;
    }
    void DuringSceneGUI(SceneView scene) {

        so.Update();
        propPoint.vector3Value = Handles.PositionHandle(point, Quaternion.identity);
        so.ApplyModifiedProperties();

        //if (Event.current.type == EventType.Repaint) {
        Handles.zTest = CompareFunction.LessEqual;

        const float gridDrawExtent = 16;
        int lineCount = Mathf.RoundToInt((gridDrawExtent * 2) / gridSize);
        if (lineCount % 2 == 0) lineCount++; // make sure it's a odd number!
        int halfLineCount = lineCount / 2;

        for (int i = 0; i < lineCount; i++) {
            int intOffset = i - halfLineCount;

            float xCoord = intOffset * gridSize;
            float zCoord0 = halfLineCount * gridSize;
            float zCoord1 = -halfLineCount * gridSize;
            Vector3 p0 = new Vector3(xCoord, 0f, zCoord0);
            Vector3 p1 = new Vector3(xCoord, 0f, zCoord1);
            Handles.DrawAAPolyLine(p0, p1);
            p0 = new Vector3(zCoord0, 0f, xCoord);
            p1 = new Vector3(zCoord1, 0f, xCoord);
            Handles.DrawAAPolyLine(p0, p1);
        }

        //}
    }

    private void OnGUI() {

        so.Update();
        EditorGUILayout.PropertyField(propGridSize);
        so.ApplyModifiedProperties();

        //using (new GUILayout.HorizontalScope()) {
        //    GUILayout.Label( "Size of the snap");
        //    GUILayout.Label(gridSize.ToString());
        //    gridSize = Mathf.Round(GUILayout.HorizontalSlider(gridSize, 0.5f, 5f));
        //    GUILayout.Space(20);
        //}
        using (new EditorGUI.DisabledScope(Selection.gameObjects.Length == 0)) {
            if (GUILayout.Button("Snap selection")) {
                SnapSelection();
            }
        }
    }

    void SnapSelection() {
        foreach (GameObject go in Selection.gameObjects) {
            Undo.RecordObject(go.transform, "snap objects");
            go.transform.position = go.transform.position.Round(gridSize);
        }
    }
}
