using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RingGenerator : MonoBehaviour {

 //   private Mesh mesh;
    private List<Vector3> vertices;
    private Vector3[] points;

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

            Vector3 point = GeneratePoint(a, ringRadius, rx, ry);
            points[i] = point;
            vertices.Add(point);
			if(i >= 1)
			GenerateQuad (i);
        }

		//case for last quad
		GenerateQuad(numSegments);
    }

	private void GenerateQuad(int segment) {
		List<Vector3> perQuadVerts = new List<Vector3>();
		perQuadVerts.Add (vertices [segment - 1] - new Vector3 (0f, 0f, ringHeight));
		perQuadVerts.Add (vertices [segment - 1]);
		if (segment == numSegments) {
			perQuadVerts.Add (vertices [0] - new Vector3 (0f, 0f, ringHeight));
			perQuadVerts.Add (vertices [0]);
		} else {
			perQuadVerts.Add (vertices [segment] - new Vector3 (0f, 0f, ringHeight));
			perQuadVerts.Add (vertices [segment]);
		}


		GameObject section = new GameObject("RingSection");
		section.transform.parent = gameObject.transform;
		section.tag = "Ring";
		section.layer = LayerMask.NameToLayer ("Ring");
		section.AddComponent<MeshRenderer>();
		section.AddComponent<MeshFilter>();
		section.AddComponent<BoxCollider>();
		section.AddComponent<RingCollider>();
		Mesh mesh = section.GetComponent<MeshFilter>().mesh;
		mesh.Clear();

		//ok, so we're going to do this by creating a generic plane and box collider at the transform origin, and then moving it into position in local space

		//first, get the distance between the two points on the circle (that make up the start and end of the segment
		float distance = Vector3.Distance(perQuadVerts[1], perQuadVerts[3]);

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
		mesh.triangles = mesh.triangles.Reverse ().ToArray ();
		mesh.RecalculateNormals();

		//at this point, we should have a quad of the correct length that faces the right direction, sat at our parent transform origin
		//while we're here, lets fix the box collider
		BoxCollider coll = section.GetComponent<BoxCollider>() as BoxCollider;
		coll.size = new Vector3(0.1f, distance, ringHeight);

		//so now all we need to do is translate the entire part to where it's supposed to be
		Vector3 newCentre = Vector3.Lerp(perQuadVerts[1], perQuadVerts[3], 0.5f);

		//float angle = Quaternion.FromToRotation(Vector3.up, perQuadVerts[3] - perQuadVerts[1]).eulerAngles.z;
		float angle = Vector3.Angle(Vector3.up, perQuadVerts[1] - perQuadVerts[3]);
		int sign = Vector3.Cross (Vector3.up, perQuadVerts [1] - perQuadVerts [3]).z < 0 ? -1 : 1;

		//determine rotation
		section.transform.localPosition = newCentre;
		section.transform.localRotation = Quaternion.Euler (0f, 0f, sign * angle);
	}

    private Vector3 GeneratePoint(float angle, float radius, float rx, float ry) {
        float x = rx + (float)(radius * Mathf.Cos(angle * Mathf.PI / 180F)) + c.x;
        float y = ry + (float)(radius * Mathf.Sin(angle * Mathf.PI / 180F)) + c.y;
        return new Vector3(x, y, 0f);
    }
}
