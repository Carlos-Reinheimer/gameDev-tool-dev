using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects] // allow select multiple objects
[CustomEditor(typeof(BarrelType))] // set as CustomEditor specifying the type
public class BarrelTypeEditor : Editor {

    //public float thing0; // serialized, visible & public
    //float thing1; //not serialized, hidden & private
    //[SerializeField] float thing2; // serialized, visible & private
    //[HideInInspector] public float thing3; // serialized, hidden, public

    SerializedObject so;
    SerializedProperty propRadius;
    SerializedProperty propDamage;
    SerializedProperty propColor;

    private void OnEnable() { // intialize things in the editor        
        so = serializedObject;
        propRadius = so.FindProperty("radius");
        propDamage = so.FindProperty("damage");
        propColor = so.FindProperty("color");
    }


    public override void OnInspectorGUI() {

        so.Update();
        EditorGUILayout.PropertyField(propRadius);
        EditorGUILayout.PropertyField(propDamage);
        EditorGUILayout.PropertyField(propColor);
        if (so.ApplyModifiedProperties()) { // if something changed
            ExplosiveBarrelsManager.UpdateAllBarrelColors();
        }



        // --------- BAD WAY ----------
        //BarrelType barrel = target as BarrelType; // reference to the object in the inspector     // targets - for multiple selection
        //float newRadius = EditorGUILayout.FloatField("radius", barrel.radius);
        //if (newRadius != barrel.radius) {
        //    Undo.RecordObject(barrel, "change barrel radius");
        //    barrel.radius = newRadius;
        //}

        //barrel.damage = EditorGUILayout.FloatField("damage", barrel.damage);
        //barrel.color = EditorGUILayout.ColorField("color", barrel.color);
    }




    //public override void OnInspectorGUI() {

    //    using (new GUILayout.VerticalScope(EditorStyles.helpBox)) {

    //        using (new GUILayout.HorizontalScope()) { // safer way, kinda new
    //            //GUILayout.BeginHorizontal();
    //            things = (Things)EditorGUILayout.EnumPopup(things);
    //            someValue = GUILayout.HorizontalSlider(someValue, -1f, 1f);
    //            //GUILayout.EndHorizontal();
    //    }
    //        GUILayout.Label("Things", GUI.skin.button);
    //        GUILayout.Space(40);
    //    }


    //    GUILayout.Label("Things", EditorStyles.boldLabel);

    //    EditorGUILayout.ObjectField("Assign here", null, typeof(Transform), true);

    //    // ----- explicit positioning using Rect -----
    //    // GUI
    //    // EditorGUI

    //    // ----- implicit positioning, auto-layout -----
    //    // GUILayout
    //    // EditorGUILayout



    //}

}
