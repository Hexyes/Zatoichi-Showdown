using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (FOV_scrip))]
public class FieldOfViewEditor : Editor {

	void OnSceneGUI()
    {     
        FOV_scrip fow = (FOV_scrip)target;
        Handles.color = Color.red;
        Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.viewRadius);

        Vector3 viewAngleA = fow.DirFromAngle(-fow.viewAngle / 2, false);
        Vector3 viewAngleB = fow.DirFromAngle(fow.viewAngle / 2, false);
        Handles.DrawLine(fow.transform.position, fow.transform.position+viewAngleA* fow.viewRadius);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.viewRadius);

        Handles.color = Color.black;
        foreach (Transform visibleTargets in fow.visibleTargets)
        {
            if (visibleTargets == null) continue;
            Handles.DrawLine(fow.transform.position, visibleTargets.position);
        }
    }
}
