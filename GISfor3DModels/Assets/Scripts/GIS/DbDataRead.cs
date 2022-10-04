using Esri.ArcGISMapsSDK.Components;
using UnityEngine;

public class DbDataRead : MonoBehaviour
{
    public string TableName;
    public Material Material;
    private ArcGISMapComponent arcGISMapComponent;
    public void LoadPolyhedronData()
    {
        arcGISMapComponent = FindObjectOfType<ArcGISMapComponent>();
        var connection = DbCommonFunctions.GetNpgsqlConnection();
        DBquery.LoadPolyhedronTriangles(connection, TableName, this, Material, arcGISMapComponent);
    }
}
