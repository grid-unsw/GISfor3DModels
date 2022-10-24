using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DbDataRead))]
public class DbDataReadEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var dbManager = (DbDataRead)target;

        if (GUILayout.Button("Load Data"))
        {
            dbManager.LoadDataFromDb();;
        }
    }
}
