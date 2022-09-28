using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DbManager))]
public class DbManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var dbManager = (DbManager)target;

        if (GUILayout.Button("Load Polyhedron Data"))
        {
            dbManager.LoadPolyhedronData();;
        }
    }
}
