using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SnapperTool : EditorWindow {

    [MenuItem("Tools/Snapper Tool")]
    public static void OpenTheThing() => GetWindow<SnapperTool>("Snapper"); // single window 

    private void OnEnable() {
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

        using (new EditorGUI.DisabledScope(Selection.gameObjects.Length == 0)) {
            if (GUILayout.Button("Snap selection")) {
                SnapSelection();
            }
        }
    }

    void SnapSelection() {
        foreach (GameObject go in Selection.gameObjects) {
            Undo.RecordObject(go.transform, "snap objects");
            go.transform.position = go.transform.position.Round();
        }
    }
}
