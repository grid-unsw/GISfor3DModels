using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DbDataWrite))]
public class DbDataWriteEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var dbManager = (DbDataWrite)target;

        if (GUILayout.Button("Load Polyhedron Data"))
        {
            dbManager.WritePolyhedronData();;
        }
    }
}
