using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// kinda an asset
[CreateAssetMenu]
public class BarrelType : ScriptableObject
{
    // define variables that are gonna be used 
    // TIP: use a slider to make sure the person who is gonna use it (like a level designer) doesn't make mistakes
    [Range(1f, 8f)]
    public float radius = 4f;

    public float damage = 10f;
    public Color color = Color.red;

    //public List<MyClass> thing = new List<MyClass>();
}

//[Serializable]
//public class MyClass
//{
//    public Vector3 pos;
//    public Color color;
//}

//[Serializable]
//public class MyOtherClass: MyClass
//{
//    public Quaternion rot;
//}

