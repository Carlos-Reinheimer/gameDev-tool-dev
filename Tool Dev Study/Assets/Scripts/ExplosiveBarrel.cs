using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


// ExecuteAlways - all the functions are also gonna be called in the editor (like onEnable())
[ExecuteAlways]
public class ExplosiveBarrel : MonoBehaviour
{

    public BarrelType type;

    MaterialPropertyBlock mpb;

    // Optimize with PropertyToId witch converts a string to a integer (?)
    static readonly int shPropColor = Shader.PropertyToID("_Color");
    public MaterialPropertyBlock Mpb
    {
        get
        {
            if (mpb == null) mpb = new MaterialPropertyBlock();
            return mpb;
        }
    }

    //private void Awake()
    //{
    //    // ------------------------------------------------------------------------------------------
    //    Shader shader = Shader.Find("Default/Diffuse");
    //    // This is an asset, witch will exist until you say to destroy it. It can cause memory leaks 
    //    Material material = new Material(shader){ hideFlags = HideFlags.HideAndDontSave };
    //    // ------------------------------------------------------------------------------------------


    //    // ---------- DON'T DO ----------

    //    // This is will duplicate the material/mesh witch is bad
    //    //GetComponent<MeshRenderer>().material.color = Color.red;
    //    //GetComponent<MeshFilter>().mesh.vertices = ...

    //    // This is will modify the proper material/mesh, witch is also bad
    //    //GetComponent<MeshRenderer>().sharedMaterial.color = Color.red;
    //    //GetComponent<MeshFilter>().sharedMesh.vertices = ...
    //}

    // adding it self to the list of allBarrels in the manager
    void OnEnable() => ExplosiveBarrelsManager.allBarrels.Add(this);
    // removing it self of the list of allBarrels in the manager
    void OnDisable() => ExplosiveBarrelsManager.allBarrels.Remove(this);
   
    //onDrawGizmos - helps who is using the gameObject to visualize what's is hapenning
    private void OnDrawGizmos()
    {
        if (type == null) return;

        Handles.color = type.color;
        Handles.DrawWireDisc(transform.position, transform.up, type.radius);
        Handles.color = Color.white; // reset back to the default

        //Gizmos.DrawWireSphere(transform.position, radius);
    }

    public void TryApplyColor()
    {
        if (type == null) return;
        MeshRenderer rnd = GetComponent<MeshRenderer>();
        Mpb.SetColor(shPropColor, type.color);
        rnd.SetPropertyBlock(Mpb); // setting the Propertie Block to the mesh

        //rnd.material.SetColor("_Color", color); ==== rnd.material.color(color);
    }
    private void OnValidate()
    {
        // This function is called every time some value has changed in the inspector
        TryApplyColor();
    }
}
