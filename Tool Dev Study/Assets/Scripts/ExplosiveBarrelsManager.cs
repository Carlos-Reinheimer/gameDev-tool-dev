using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// only using UnityEditor in some sort of dev enviroment or something
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ExplosiveBarrelsManager : MonoBehaviour
{
    // having a script who knows all the other barrels that exists
    public static List<ExplosiveBarrel> allBarrels = new List<ExplosiveBarrel>();

    // sabe if in the funciton
    private void OnDrawGizmos()
    {
    foreach (ExplosiveBarrel barrel in allBarrels)
	{
            // using Handles is better beacause
            // used when dragging things around
            // Handles is a editor only class 
            #if UNITY_EDITOR
            Handles.DrawAAPolyLine(transform.position, barrel.transform.position);
            #endif

            //Gizmos.DrawLine(transform.position, barrel.transform.position);
        }

    }

}
