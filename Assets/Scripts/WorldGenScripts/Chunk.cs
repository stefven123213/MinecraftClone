using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    MeshCollider meshCollider;

    List<Vector3> vertices = new List<Vector3>();
    List<Vector2> uvs = new List<Vector2>();
    List<Color> colors = new List<Color>();
    List<int> triangles = new List<int>();
    int vertexIndex = 0;

    private void Start()
    {
        Init();
    }

    void Init()
    {
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshCollider = gameObject.AddComponent<MeshCollider>();

        CreateVoxelData();
        CreateMesh();
    }
    void CreateVoxelData()
    {
        for(int p = 0; p < 6; p++)
        {
            for (int i = 0; i < 4; i++)
            {
                vertices.Add(VoxelData.voxelVerts[VoxelData.voxelTris[p, i]]);
                uvs.Add(new Vector2()); // TODO: implement texturing, remove this line after implementation
                colors.Add(Color.white);
            }

            for(int i = 0; i < 6; i++)
            {
                triangles.Add(vertexIndex + VoxelData.triAdd[i]);
            }

            vertexIndex += 4;
        }
    }
    void CreateMesh()
    {
        Mesh mesh = new Mesh()
        {
            vertices = vertices.ToArray(),
            triangles = triangles.ToArray(),
            uv = uvs.ToArray(),
            colors = colors.ToArray(),
        };
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
    }
}
