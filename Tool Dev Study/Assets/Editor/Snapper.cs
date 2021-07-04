using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class Snapper {

    const string UNDO_STR_SNAP = "snap objects";

    [MenuItem("My tab/Functions/Snap Selected Objects %&S", isValidateFunction:true)]
    public static bool SnapTheThingValidated() {
        return Selection.gameObjects.Length > 0;
    }

    [MenuItem("My tab/Functions/Snap Selected Objects %&S")] // hotkeys | % - control   # - shift    & - alt
    public static void SnapTheThing() {
        foreach (GameObject go in Selection.gameObjects) { // all the game objects you have selected
            Undo.RecordObject(go.transform, UNDO_STR_SNAP ); // this is really important to Unity recognize as a change, otherwise it will not work properly
            go.transform.position = go.transform.position.Round();
        }
    }

    public static Vector3 Round(this Vector3 v) { // extension methods --- make the function kinda "Exists" inside the thing
        v.x = Mathf.Round(v.x);
        v.y = Mathf.Round(v.y);
        v.z = Mathf.Round(v.z);
        return v;
    }

}
