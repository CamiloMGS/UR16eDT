using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(RobotGripperController))]
public class RobotGripperControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RobotGripperController script = (RobotGripperController)target;

        GUILayout.Space(15);
        if (GUILayout.Button("Open Gripper"))
        {
            script.OpenGripper();
        }
        if (GUILayout.Button("Close Gripper"))
        {
            script.CloseGripper();
        }

    }
}
