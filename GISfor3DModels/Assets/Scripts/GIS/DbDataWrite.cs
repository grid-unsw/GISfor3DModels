using Esri.ArcGISMapsSDK.Components;
using Esri.ArcGISMapsSDK.Utils.GeoCoord;
using Esri.GameEngine.Geometry;
using UnityEngine;

public class DbDataWrite : MonoBehaviour
{
    public string TableName;
    public bool Truncate;
    public void WritePolyhedronData()
    {
        var location = GetComponent<ArcGISLocationComponent>();
        if (location == null)
        {
            Debug.Log($"{gameObject.name} does not have ArcGISLocation component attached.");
            return;
        }

        var meshFiltersInChildren = GetComponentsInChildren<MeshFilter>();

        if (meshFiltersInChildren.Length == 0)
        {
            Debug.Log("No meshes detected.");
            return;
        }
        var connection = DbCommonFunctions.GetNpgsqlConnection();
        var reprojectedLocation = GeoUtils.ProjectToSpatialReference(location.Position, new ArcGISSpatialReference(28356));
        var centroid = new ArcGISPoint(reprojectedLocation.X- gameObject.transform.position.x, reprojectedLocation.Y - gameObject.transform.position.z, reprojectedLocation.Z - gameObject.transform.position.y);

        DBexport.ExportMeshesAsPolyhedrons(meshFiltersInChildren, connection, centroid, TableName, Truncate);
    }
}
