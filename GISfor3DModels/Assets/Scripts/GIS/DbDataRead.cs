using Esri.ArcGISMapsSDK.Components;
using UnityEngine;

public class DbDataRead : MonoBehaviour
{
    public string TableName;
    public Material Material;
    private ArcGISMapComponent arcGISMapComponent;

    public GameObject PointPrefab;
    [Range(0.01f, 0.5f)]
    public float PointSize = 0.1f;

    public void LoadDataFromDb()
    {
        arcGISMapComponent = FindObjectOfType<ArcGISMapComponent>();
        var connection = DbCommonFunctions.GetNpgsqlConnection();
        DBquery.LoadData(connection, TableName, this, PointPrefab, Material, arcGISMapComponent, PointSize);
    }
}
