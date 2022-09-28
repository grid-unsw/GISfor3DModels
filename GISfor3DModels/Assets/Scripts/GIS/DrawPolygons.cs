using System.Collections;
using System.Collections.Generic;

using Esri.ArcGISMapsSDK.Components;
using Esri.ArcGISMapsSDK.Utils.GeoCoord;
using Esri.GameEngine.Geometry;
using Esri.HPFramework;

using UnityEngine;
using Unity.Mathematics;


public class DrawPolygons : MonoBehaviour
{
    private double[] latitude = new double[3] { 151.225962923, 151.23012222222, 151.2303933333 };
    private double[] longitude = new double[3] { -33.916431694, -33.9181111111, -33.9164444444 };
    private ArcGISMapComponent arcGISMapComponent;
    private double elevationOffset = 0.0;
    private List<GameObject> points = new List<GameObject>();
    public GameObject pointGO;

    void Start()
    {
        // We need this ArcGISMapComponent for the FromCartesianPosition Method
        // defined on the ArcGISMapComponent.View
        arcGISMapComponent = FindObjectOfType<ArcGISMapComponent>();
    }

    // Start is called before the first frame update
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(1))
        {
            StartCoroutine(CreatePointsIn());
        }
    }

    IEnumerator CreatePointsIn()
    {
        for (int i = 0; i < latitude.Length; i++)
        {
            var lat = latitude[i];
            var lon = longitude[i];
            points.Add(CreatePoints(lat, lon));

            yield return null;
            yield return null;
        }
        foreach (var point in points)
        {
            SetElevation(point);
        }

        // need a frame for location component updates to occur
        yield return null;
        yield return null;

    }

    private GameObject CreatePoints(double lat, double lon)
    {
        GameObject breadcrumb = Instantiate(pointGO, arcGISMapComponent.transform);

        breadcrumb.name = "Point";

        ArcGISLocationComponent location = breadcrumb.AddComponent<ArcGISLocationComponent>();
        location.Position = new ArcGISPoint(lat, lon, elevationOffset, new ArcGISSpatialReference(4326));

        return breadcrumb;
    }

    void SetElevation(GameObject breadcrumb)
    {
        // start the raycast in the air at an arbitrary to ensure it is above the ground
        var raycastHeight = 5000;
        var position = breadcrumb.transform.position;
        var raycastStart = new Vector3(position.x, position.y + raycastHeight, position.z);
        if (Physics.Raycast(raycastStart, Vector3.down, out RaycastHit hitInfo))
        {
            var location = breadcrumb.GetComponent<ArcGISLocationComponent>();
            location.Position = HitToGeoPosition(hitInfo, (float)elevationOffset);
        }
    }

    /// <summary>
    /// Return GeoPosition Based on RaycastHit; I.E. Where the user clicked in the Scene.
    /// </summary>
    /// <param name="hit"></param>
    /// <returns></returns>
    private ArcGISPoint HitToGeoPosition(RaycastHit hit, float yOffset = 0)
    {
        var worldPosition = math.inverse(arcGISMapComponent.WorldMatrix)
            .HomogeneousTransformPoint(hit.point.ToDouble3());

        var geoPosition = arcGISMapComponent.View.WorldToGeographic(worldPosition);
        var offsetPosition = new ArcGISPoint(geoPosition.X, geoPosition.Y, geoPosition.Z + yOffset,
            geoPosition.SpatialReference);

        return GeoUtils.ProjectToSpatialReference(offsetPosition, new ArcGISSpatialReference(4326));
    }
}


