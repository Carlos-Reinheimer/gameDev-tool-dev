using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// set as CustomEditor specifying the type
[CustomEditor(typeof(BarrelType))]
public class BarrelTypeEditor : Editor {

    public float thing0; // serialized, visible & public
    float thing1; //not serialized, hidden & private
    [SerializeField] float thing2; // serialized, visible & private
    [HideInInspector] public float thing3; // serialized, hidden, public
    
    
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();


    }

}
