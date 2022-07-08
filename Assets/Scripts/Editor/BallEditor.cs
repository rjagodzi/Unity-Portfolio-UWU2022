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

    private float m_DebugForce;
    private Vector2 m_DebugDirection;

    public override void OnInspectorGUI()
    {
        m_DebugDirection = EditorGUILayout.Vector2Field("Direction", m_DebugDirection);
        m_DebugForce = EditorGUILayout.FloatField("Force", m_DebugForce);
        if(GUILayout.Button("Kick"))
        {
            script.Kick(m_DebugDirection, m_DebugForce);
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
