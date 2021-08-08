using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomShapeGenerator : MonoBehaviour
{
    private MeshFilter m_MeshFilter;
    private MeshRenderer m_MeshRenderer;

    private void Awake()
    {
        m_MeshFilter = GetComponent<MeshFilter>();
        m_MeshRenderer = GetComponent<MeshRenderer>();

        m_MeshRenderer.sortingOrder = 5;
    }

    public void GenerateNewShape(Bounds[] bounds)
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[4 * bounds.Length];
        int[] triangles = new int[6 * bounds.Length];
        int index = 0;

        foreach(Bounds b in bounds)
        {
            vertices[4 * index] = new Vector3(b.min.x, b.min.y);
            vertices[4 * index + 1] = new Vector3(b.min.x, b.max.y);
            vertices[4 * index + 2] = new Vector3(b.max.x, b.max.y);
            vertices[4 * index + 3] = new Vector3(b.max.x, b.min.y);

            triangles[6 * index] = (4 * index);
            triangles[6 * index + 1] = (4 * index + 1);
            triangles[6 * index + 2] = (4 * index + 2);
            triangles[6 * index + 3] = (4 * index);
            triangles[6 * index + 4] = (4 * index + 2);
            triangles[6 * index + 5] = (4 * index + 3);

            index++;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        m_MeshFilter.mesh = mesh;
    }
}
