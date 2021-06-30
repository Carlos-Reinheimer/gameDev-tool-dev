using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

    // adding it self to the list of allBarrels in the manager
    void OnEnable() => ExplosiveBarrelsManager.allBarrels.Add(this);
    // removing it self of the list of allBarrels in the manager
    void OnDisable() => ExplosiveBarrelsManager.allBarrels.Remove(this);
    
       

    


    //onDrawGizmos - helps who is using the gameObject to visualize what's is hapenning
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
