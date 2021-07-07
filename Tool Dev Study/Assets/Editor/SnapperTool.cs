using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;

public class SnapperTool : EditorWindow {

    public enum GridType {
        Cartesian,
        Polar
    }

    [MenuItem("Tools/Snapper Tool")]
    public static void OpenTheThing() => GetWindow<SnapperTool>("Snapper"); // single window 
    const float TAU = 6.28318530718f;
    public float gridSize = 1f;
    public GridType gridType = GridType.Cartesian;
    public int angularDivision = 24;

    public Vector3 point;

    SerializedObject so;
    SerializedProperty propGridSize;
    SerializedProperty propPoint;
    SerializedProperty propGridType;
    SerializedProperty propAngularDivision;

    private void OnEnable() {
        so = new SerializedObject(this);
        propGridSize = so.FindProperty("gridSize");
        propPoint = so.FindProperty("point");
        propGridType = so.FindProperty("gridType");
        propAngularDivision = so.FindProperty("angularDivision");

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

        if (gridType == GridType.Cartesian) DrawGridCartesian(gridDrawExtent);
        else DrawGridPolar(gridDrawExtent);


        //}
    }
    
    void DrawGridPolar(float gridDrawExtent) {
        int ringCount = Mathf.RoundToInt(gridDrawExtent / gridSize);
        float radiusOuter = (ringCount -1) * gridSize;

        // radial grid (rings)
        for (int i = 1; i < ringCount; i++) Handles.DrawWireDisc(Vector3.zero, Vector3.up, i * gridSize);

        // angular grid (lines)
        for (int i = 0; i < angularDivision; i++) {
            float t = i / (float)angularDivision;
            float angRad = t * TAU; // turns to radians
            float x = Mathf.Cos(angRad);
            float y = Mathf.Sin(angRad);
            Vector3 dir = new Vector3(x, 0f, y);

            Handles.DrawAAPolyLine(Vector3.zero, dir * radiusOuter);
        }
    }

    void DrawGridCartesian(float gridDrawExtent) {
        int lineCount = Mathf.RoundToInt((gridDrawExtent * 2) / gridSize);
        if (lineCount % 2 == 0) lineCount++; // make sure it's a odd number!
        int halfLineCount = lineCount / 2;

        for (int i = 0; i < lineCount; i++)
        {
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
    }

    private void OnGUI() {

        so.Update();
        EditorGUILayout.PropertyField(propGridType);
        EditorGUILayout.PropertyField(propGridSize);
        if (gridType == GridType.Polar) {
            EditorGUILayout.PropertyField(propAngularDivision);
            propAngularDivision.intValue = Mathf.Max(4, propAngularDivision.intValue);
        }
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
            go.transform.position = GetSnappedPosition(go.transform.position);
        }
    }

    Vector3 GetSnappedPosition(Vector3 posOriginal){
        if (gridType == GridType.Cartesian) return posOriginal.Round(gridSize);
        
        if (gridType == GridType.Polar) {
            Vector2 vec = new Vector2(posOriginal.x, posOriginal.z);
            float dist = vec.magnitude;
            float distSnapped = dist.Round(gridSize);

            float angRad = Mathf.Atan2(vec.y, vec.x); // 0 to TAU
            float angTurns = angRad / TAU; // 0 to 1
            float angSnapped = angTurns.Round(1f / angularDivision) * TAU;

            Vector2 dirSnapped = new Vector2(Mathf.Cos(angSnapped), Mathf.Sin(angSnapped));
            Vector2 snappedVector = distSnapped * dirSnapped;

            return new Vector3(snappedVector.x, posOriginal.y, snappedVector.y);
        }

        return default;
    }
}
