using UnityEngine;

public class DbDataWrite : MonoBehaviour
{
    public string TableName;
    public bool Truncate;
    public void WritePolyhedronData()
    {
        var connection = DbCommonFunctions.GetNpgsqlConnection();

        var gameObjectsInChildren = new GameObject[gameObject.transform.childCount];
        for (var i = 0; i < gameObject.transform.childCount; i++)
        {
            gameObjectsInChildren[i] = gameObject.transform.GetChild(i).gameObject;
        }

        DBexport.ExportMesheseAsPolyhedrons(gameObjectsInChildren, connection, TableName, Truncate);
    }
}
