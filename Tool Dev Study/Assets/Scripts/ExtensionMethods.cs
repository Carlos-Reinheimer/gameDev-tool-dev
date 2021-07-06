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
}
