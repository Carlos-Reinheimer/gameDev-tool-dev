using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SnapperTool : EditorWindow {

    [MenuItem("Tools/Snapper Tool")]
    public static void OpenTheThing() => GetWindow<SnapperTool>("Snapper"); // single window 
    public float gridSize = 1f;

    SerializedObject so;
    SerializedProperty propGridSize;

    private void OnEnable() {
        so = new SerializedObject(this);
        propGridSize = so.FindProperty("gridSize");

        Selection.selectionChanged += Repaint; // update the window when selection change
        SceneView.duringSceneGui += DuringSceneGUI;
    }
    private void OnDisable() {
        Selection.selectionChanged -= Repaint;
        SceneView.duringSceneGui -= DuringSceneGUI;
    }
    void DuringSceneGUI(SceneView scene) {
        Handles.DrawLine(Vector3.zero, Vector3.up);
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
