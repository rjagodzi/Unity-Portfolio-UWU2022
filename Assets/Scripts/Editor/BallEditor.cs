using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Ball))]
public class BallEditor : Editor
{
    public Ball script;

    public void OnEnable()
    {
        script = target as Ball;
    }

   

    public override void OnInspectorGUI()
    {
        script.DebugDirection = EditorGUILayout.Vector2Field("Direction", script.DebugDirection);
        script.DebugForce = EditorGUILayout.FloatField("Force", script.DebugForce);
        EditorGUILayout.LabelField("Velocity: " + script.RigidBody.velocity.ToString());
        EditorGUILayout.LabelField("AngularVelocity: " + script.RigidBody.angularVelocity.ToString());
        if(GUILayout.Button("Kick"))
        {
            script.Kick(script.DebugDirection, script.DebugForce, 0f);
        }
        if(GUILayout.Button("Stop"))
        {
            script.Stop();
        }
        if(GUILayout.Button("Release"))
        {
            script.Release();
        }
        DrawDefaultInspector();
    }

    
}
