using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class raytest : MonoBehaviour
{
    // Attach this script to a camera and it will
    // draw a debug line pointing outward from the normal
    void Update()
    {
        // Only if we hit something, do we continue
        RaycastHit hit;


        if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            return;
        }

        // Just in case, also make sure the collider also has a renderer
        // material and texture
        BoxCollider hitCollider = hit.collider as BoxCollider;
        Mesh mesh = hitCollider.gameObject.GetComponent<MeshFilter>().sharedMesh;
        if (hitCollider == null)
        {
            return;
        }

        Vector3[] normals = mesh.normals;
        int[] triangles = mesh.triangles;

        // Extract local space normals of the triangle we hit
        Vector3 n0 = normals[triangles[0 * 3 + 0]];
        Vector3 n1 = normals[triangles[0 * 3 + 1]];
        Vector3 n2 = normals[triangles[0 * 3 + 2]];

        // interpolate using the barycentric coordinate of the hitpoint
        Vector3 baryCenter = hit.barycentricCoordinate;

        // Use barycentric coordinate to interpolate normal
        Vector3 interpolatedNormal = n0 * baryCenter.x + n1 * baryCenter.y + n2 * baryCenter.z;
        // normalize the interpolated normal
        interpolatedNormal = interpolatedNormal.normalized;

        // Transform local space normals to world space
        Transform hitTransform = hit.collider.transform;
        interpolatedNormal = hitTransform.TransformDirection(interpolatedNormal);

        // Display with Debug.DrawLine
        Debug.DrawRay(hit.point, interpolatedNormal);
    }
}