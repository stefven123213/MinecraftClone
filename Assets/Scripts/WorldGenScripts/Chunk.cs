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
    List<int> triangles = new List<int>();
    int vertexIndex = 0;

    byte[,,] voxelMap = new byte[VoxelData.chunkWidth, VoxelData.chunkHeight, VoxelData.chunkWidth];

    private void Start()
    {
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshCollider = gameObject.AddComponent<MeshCollider>();

        PopulateVoxelMap();
        CreateChunk();
        CreateMesh();
    }

    void PopulateVoxelMap()
    {
        for (int x = 0; x < VoxelData.chunkWidth; x++)
        {
            for (int z = 0; z < VoxelData.chunkWidth; z++)
            {
                for (int y = 0; y < VoxelData.chunkHeight; y++)
                {
                    voxelMap[x, y, z] = 0;
                }
            }
        }
    }
    void CreateChunk()
    {
        for(int x = 0; x < VoxelData.chunkWidth; x++)
        {
            for(int z = 0; z < VoxelData.chunkWidth; z++)
            {
                for(int y = 0; y < VoxelData.chunkHeight; y++)
                {
                    CreateVoxelData(new Vector3Int(x, y, z));
                }
            }
        }
    }
    void CreateVoxelData(Vector3Int pos)
    {
        for (int p = 0; p < 6; p++)
        {
            if (!CheckVoxel(pos + VoxelData.faceChecks[p]))
            {
                for (int i = 0; i < 4; i++)
                {
                    vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, i]]);
                    uvs.Add(new Vector2());
                }

                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 3);
                vertexIndex += 4;
            }
        }
    }
    void CreateMesh()
    {
        Mesh mesh = new Mesh()
        {
            vertices = vertices.ToArray(),
            triangles = triangles.ToArray(),
            uv = uvs.ToArray(),
        };
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
    }

    bool CheckVoxel(Vector3Int pos)
    {
        if (pos.x < 0 || pos.x > VoxelData.chunkWidth - 1 || pos.y < 0 || pos.y > VoxelData.chunkWidth - 1 || pos.z < 0 || pos.z > VoxelData.chunkWidth - 1)
            return false;
        else
            return voxelMap[pos.x, pos.y, pos.z] == 0;
    }
}
