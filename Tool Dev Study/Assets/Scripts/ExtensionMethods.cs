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
