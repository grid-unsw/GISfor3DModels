using System.Collections;
using System.Collections.Generic;
using Esri.ArcGISMapsSDK.Components;
using Esri.GameEngine.Geometry;
using NetTopologySuite.Geometries;
using Npgsql;
using UnityEngine;

public static class DBquery
{
   public static void LoadPolyhedronTriangles(NpgsqlConnection connection, string tableName, MonoBehaviour handle, Material material, ArcGISMapComponent arcGISMapComponent)
   {
        var tableFields = "";
        DbCommonFunctions.CheckIfTableExistOrTruncate(tableName,connection, tableFields);

        //// Place this at the beginning of your program to use NetTopologySuite everywhere (recommended)
        //NpgsqlConnection.GlobalTypeMapper.UseNetTopologySuite();

        // Or to temporarily use NetTopologySuite on a single connection only
        connection.TypeMapper.UseNetTopologySuite();

        var sqlCentroids = $"select st_centroid((ST_Dump (geom)).geom), st_transform(st_setsrid(st_centroid((ST_Dump (geom)).geom),28356),4326) from {tableName}";

        var cmd = new NpgsqlCommand(sqlCentroids, connection);
        var polyhedronsCentroids = new List<(Point, Point)>();

        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                polyhedronsCentroids.Add(((Point)reader[0], (Point)reader[1]));
            }
        }

        var polyhedronsCount = polyhedronsCentroids.Count;

        var sqlPoints = $"select b.surface[2], (b.geom_pnt).geom from(SELECT(a.p_geom).path as surface, " +
                  $"ST_DumpPoints(st_tesselate((a.p_geom).geom)) As geom_pnt FROM (select ST_Dump(ST_Extrude(geom, 0, 0, 20)) as p_geom from {tableName}) as a) as b";

        cmd = new NpgsqlCommand(sqlPoints, connection);

        var indices = new List<int>();
        var vertices = new List<Vector3>();
        var i = 0;
        var i1 = 0;
        var previousSurfaceId = 1;
        var polyhedronId = 0;
        var polyhedronCentroid = polyhedronsCentroids[polyhedronId];
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                switch (i)
                {
                    case 0:
                        var surfaceId = (int) reader[0];
                        if (surfaceId < previousSurfaceId)
                        {
                            handle.StartCoroutine(InstantiatePolyhedron(handle.gameObject, material, arcGISMapComponent, polyhedronId, vertices, indices,
                                polyhedronCentroid));
                            indices = new List<int>();
                            vertices = new List<Vector3>();
                            i1 = 0;
                            polyhedronId++;
                            if (polyhedronsCount != polyhedronId + 1)
                            {
                                polyhedronCentroid = polyhedronsCentroids[polyhedronId];
                            }
                            previousSurfaceId = 1;
                        }
                        else
                        {
                            previousSurfaceId = surfaceId;
                        }

                        indices.Add(i1);
                        vertices.Add(GetShiftedVector3((Point) reader[1], polyhedronCentroid.Item1));
                        i++;
                        i1++;
                        break;
                    case 1:
                        indices.Add(i1);
                        vertices.Add(GetShiftedVector3((Point) reader[1], polyhedronCentroid.Item1));
                        i++;
                        i1++;
                        break;
                    case 2:
                        indices.Add(i1);
                        vertices.Add(GetShiftedVector3((Point) reader[1], polyhedronCentroid.Item1));
                        i++;
                        i1++;
                        break;
                    default:
                        i = 0;
                        break;
                }
            }
            handle.StartCoroutine(InstantiatePolyhedron(handle.gameObject, material, arcGISMapComponent, polyhedronId, vertices, indices,
                polyhedronCentroid));
        }
   }

   private static IEnumerator InstantiatePolyhedron(GameObject parent, Material material, ArcGISMapComponent arcGISMapComponent,
       int polyhedronId, List<Vector3> vertices, List<int> indices, (Point, Point) polyhedronCentroid)
   {
       var polyhedronGO = new GameObject($"Polyhedron{polyhedronId}", typeof(MeshFilter), typeof(MeshRenderer));
       var mesh = new Mesh
       {
           vertices = vertices.ToArray(),
           triangles = indices.ToArray()
       };
       mesh.RecalculateNormals();
       polyhedronGO.GetComponent<MeshFilter>().mesh = mesh;
       polyhedronGO.GetComponent<Renderer>().material = material;
       polyhedronGO.transform.parent = parent.transform;
       var location = polyhedronGO.AddComponent<ArcGISLocationComponent>();
       location.Position = new ArcGISPoint(polyhedronCentroid.Item2.X, polyhedronCentroid.Item2.Y, 100, new ArcGISSpatialReference(4326));
       // need a frame for location component updates to occur
       yield return null;
       yield return null;
       ArcGISFunctions.SetElevation(polyhedronGO, arcGISMapComponent, 20);
   }

   private static Vector3 GetShiftedVector3(Point point, Point shift)
   {
       return new Vector3((float) (point.X-shift.X), (float) (point.Y-shift.Y), (float) point.Z);
   }


}
