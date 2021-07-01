using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


// ExecuteAlways - all the functions are also gonna be called in the editor (like onEnable())
[ExecuteAlways]
public class ExplosiveBarrel : MonoBehaviour
{
    // define variables that are gonna be used 

    // TIP: use a slider to make sure the person who is gonna use it (like a level designer) doesn't make mistakes
    [Range(1f, 8f)]
    public float radius = 4f;
    
    public float damage = 10f;
    public Color color = Color.red;

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

    void ApplyColor()
    {
        MeshRenderer rnd = GetComponent<MeshRenderer>();
        Mpb.SetColor(shPropColor, color);
        rnd.SetPropertyBlock(Mpb); // setting the Propertie Block to the mesh

        //rnd.material.SetColor("_Color", color); ==== rnd.material.color(color);
    }
    private void OnValidate()
    {
        // This function is called every time some value has changed in the inspector
        ApplyColor();
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
    private void OnDrawGizmosSelected()
    {
        Handles.DrawWireDisc(transform.position, transform.up, radius);

        //Gizmos.DrawWireSphere(transform.position, radius);
    }
}
