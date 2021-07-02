using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// only using UnityEditor in some sort of dev enviroment or something
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ExplosiveBarrelsManager : MonoBehaviour
{
    // having a script who knows all the other barrels that exists
    public static List<ExplosiveBarrel> allBarrels = new List<ExplosiveBarrel>();

    // sabe if in the funciton
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {

        // Dealing with how the lines works
        Handles.zTest = CompareFunction.LessEqual;

    foreach (ExplosiveBarrel barrel in allBarrels)
	{
            if (barrel.type == null) continue;

            Vector3 managerPos = transform.position;
            Vector3 barrelPos = barrel.transform.position;
            float halfHeight = (managerPos.y - barrelPos.y) * 0.5f;
            Vector3 tangentOffset = Vector3.up * halfHeight;

            Handles.DrawBezier(managerPos, barrelPos, managerPos - tangentOffset, barrelPos + tangentOffset, barrel.type.color, EditorGUIUtility.whiteTexture, 1f) ;

            // using Handles is better beacause
            // used when dragging things around
            // Handles is a editor only class 

            //Handles.DrawAAPolyLine(transform.position, barrel.transform.position);

            //Gizmos.DrawLine(transform.position, barrel.transform.position);
        }
        Handles.color = Color.white;

    }
    #endif

}
