using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{
    void OnSceneGUI()
    {
        FieldOfView fow = (FieldOfView)target;

        // ���� �þ� �� �׸���
        Handles.color = Color.white;
        Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.viewRadiusHorizontal);
        Vector3 viewAngleA = fow.DirFromAngle(-fow.viewAngleHorizontal / 2, false);
        Vector3 viewAngleB = fow.DirFromAngle(fow.viewAngleHorizontal / 2, false);

        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * fow.viewRadiusHorizontal);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.viewRadiusHorizontal);

        // ���� �þ� �� �׸���
        Handles.DrawWireArc(fow.transform.position, Vector3.right, Vector3.forward, 360, fow.viewRadiusVertical);
        Vector3 viewAngleC = fow.DirFromHorizontalAngle(-fow.viewAngleVertical / 2, false);
        Vector3 viewAngleD = fow.DirFromHorizontalAngle(fow.viewAngleVertical / 2, false);

        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleC * fow.viewRadiusVertical);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleD * fow.viewRadiusVertical);

        Handles.color = Color.red;
        foreach (Transform visible in fow.visibleTargets)
        {
            Handles.DrawLine(fow.transform.position, visible.position);
        }
    }

    // y�� ���Ϸ� ���� 3���� ���� ���ͷ� ��ȯ�Ѵ�.

}