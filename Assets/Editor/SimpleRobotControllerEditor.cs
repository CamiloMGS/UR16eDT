using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(SimpleRobotController))]
public class SimpleRobotControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SimpleRobotController script = (SimpleRobotController)target;

        GUILayout.Space(15);
        if (GUILayout.Button("Home Position"))
        {
            script.MoveRobotToHomePosition();
        }

    }
}
