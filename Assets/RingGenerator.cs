using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class RingGenerator : MonoBehaviour {

 //   private Mesh mesh;
    private List<Vector3> vertices;
    private List<int> triangles;
    private Vector3[] points;
    private int pointCount = 0;

    [Range(0.1f,1.0f)]
    public float ringHeight = 0.1f;

    [Range(10, 20)]
    public int numSegments = 15;

    [Range(5f, 15f)]
    public float ringRadius = 8f;

    private Vector3 c = Vector3.zero;
    private float segmentSize;

    // Use this for initialization
    void Start () {
        segmentSize = 360f / numSegments;
        vertices = new List<Vector3>();
        triangles = new List<int>();
        points = new Vector3[numSegments * 2];

        //generate a new vertex for each part of the ring
        for (int i = 0; i < numSegments; i++) {
            //angle we are creating the point at (in the circle)
            float a = segmentSize * i;

            //random x offset of the point on the circle
            float rx = Random.Range(-1f, 1f);

            //random y offset of the point on the circle
            float ry = Random.Range(-1f, 1f);

            //the above two points give us the jagged inner parts of the circle that we want

            Vector3 point = GeneratePoint(a, ringRadius, 0f, rx, ry);
            points[i] = point;
            vertices.Add(point);
        }

        List<Vector3> perQuadVerts = new List<Vector3>();
        perQuadVerts.Add(vertices[0] + new Vector3(0f, 0f, -ringHeight));
        perQuadVerts.Add(vertices[0]);
        perQuadVerts.Add(vertices[1] + new Vector3(0f, 0f, -ringHeight));
        perQuadVerts.Add(vertices[1]);

        GameObject section = new GameObject("RingSection");
        section.transform.parent = gameObject.transform;
        section.tag = "Ring";
        section.AddComponent<MeshRenderer>();
        section.AddComponent<MeshFilter>();
        section.AddComponent<BoxCollider>();
        Mesh mesh = section.GetComponent<MeshFilter>().mesh;
        mesh.Clear();

        //ok, so we're going to do this by creating a generic plane and box collider at the transform origin, and then moving it into position in local space

        //first, get the distance between the two points on the circle (that make up the start and end of the segment
        float distance = Vector3.Distance(points[0], points[1]);

        //so now we construct the quad using this value (centred at the origin)
        List<Vector3> quadVerts = new List<Vector3>();
        quadVerts.Add(new Vector3(0f, 0f - (float)(distance / 2f), 0f - (float)(ringHeight / 2f)));
        quadVerts.Add(new Vector3(0f, 0f - (float)(distance / 2f), 0f + (float)(ringHeight / 2f)));
        quadVerts.Add(new Vector3(0f, 0f + (float)(distance / 2f), 0f - (float)(ringHeight / 2f)));
        quadVerts.Add(new Vector3(0f, 0f + (float)(distance / 2f), 0f + (float)(ringHeight / 2f)));
        //centre it at the origin so it's easier to deal with in the future (And also because then the collider will be automatically centred)

        List<int> quadTris = new List<int>();
        quadTris.Add(0);
        quadTris.Add(1);
        quadTris.Add(2);
        quadTris.Add(1);
        quadTris.Add(3);
        quadTris.Add(2);

        mesh.vertices = quadVerts.ToArray();
        mesh.triangles = quadTris.ToArray();
        mesh.RecalculateNormals();

        //at this point, we should have a quad of the correct length that faces the right direction, sat at our parent transform origin
        //while we're here, lets fix the box collider
        BoxCollider coll = section.GetComponent<BoxCollider>() as BoxCollider;
        coll.size = new Vector3(0.1f, distance, ringHeight);

        //so now all we need to do is translate the entire part to where it's supposed to be


        /*      MeshFilter filter = GetComponent<MeshFilter>();
              mesh = filter.mesh;
              mesh.Clear();


              //generate top and bottom points on ring (segments)
              for (int i = 0; i < numSegments; i++) {
                  float a = segmentSize * i;
                  float rx = Random.Range(-1f, 1f);
                  float rz = Random.Range(-1f, 1f);
                  Vector3 top = GeneratePoint(a, ringRadius, 0f, rx, rz);
                  points[pointCount] = top;
                  Vector3 bottom = GeneratePoint(a, ringRadius, -ringHeight, rx, rz);
                  points[pointCount + 1] = bottom;
                  pointCount += 2;
                  GenerateMeshData(top, bottom);
              }

              //tie the last point to the first
              GenerateMeshData(points[0], points[1]);
              pointCount += 2;

              //add the vertices and the triangles to the mesh
              mesh.vertices = vertices.ToArray();
              mesh.triangles = triangles.ToArray();
              mesh.triangles = mesh.triangles.Reverse().ToArray();
              mesh.RecalculateNormals();

              GetComponent<MeshCollider>().sharedMesh = filter.sharedMesh; */

        GenerateExampleMesh();
    }

    private void GenerateMeshData(Vector3 top, Vector3 bottom) {
        vertices.Add(bottom);
        vertices.Add(top);

        if(vertices.Count >= 4) {
            int start = vertices.Count - 4;
            triangles.Add(start+0);
            triangles.Add(start+1);
            triangles.Add(start+2);
            triangles.Add(start+1);
            triangles.Add(start+3);
            triangles.Add(start+2);
        }
    }

    private Vector3 GeneratePoint(float angle, float radius, float z, float rx, float ry) {
        float x = rx + (float)(radius * Mathf.Cos(angle * Mathf.PI / 180F)) + c.x;
        float y = ry + (float)(radius * Mathf.Sin(angle * Mathf.PI / 180F)) + c.y;
        return new Vector3(x, y, z);
    }

    private void GenerateExampleMesh() {
        GameObject section = new GameObject("RingSection");
        section.transform.parent = gameObject.transform;
        section.tag = "Ring";
        section.AddComponent<MeshRenderer>();
        section.AddComponent<MeshFilter>();
        section.AddComponent<BoxCollider>();
        Mesh mesh = section.GetComponent<MeshFilter>().mesh;
        mesh.Clear();

        //so now we construct the quad using this value
        List<Vector3> quadVerts = new List<Vector3>();
        quadVerts.Add(points[0] - new Vector3(0,0,ringHeight));
        quadVerts.Add(points[0]);
        quadVerts.Add(points[1] - new Vector3(0,0,ringHeight));
        quadVerts.Add(points[1]);

        List<int> perQuadTris = new List<int>();
                 perQuadTris.Add(0);
                 perQuadTris.Add(1);
                 perQuadTris.Add(2);
                 perQuadTris.Add(1);
                 perQuadTris.Add(3);
                 perQuadTris.Add(2);

                 mesh.vertices = quadVerts.ToArray();
                 mesh.triangles = perQuadTris.ToArray();
                 mesh.RecalculateNormals();
    }
}
