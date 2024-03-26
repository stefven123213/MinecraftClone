using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    MeshCollider meshCollider;

    List<Vector3> vertices = new List<Vector3>();
    List<Vector2> uvs = new List<Vector2>();
    List<int> triangles = new List<int>();
    int vertexIndex = 0;

    byte[,,] voxelMap = new byte[VoxelData.chunkWidth, VoxelData.chunkHeight, VoxelData.chunkWidth];

    public GameObject chunkObject;
    public Vector3Int position;
    public ChunkCoord coord;

    public Chunk(ChunkCoord coord)
    {
        this.coord = coord;

        Init();
    } 

    void Init()
    {
        chunkObject = new GameObject();
        chunkObject.name = "Chunk " + coord.x + ", " + coord.z;
        chunkObject.transform.position = position = new Vector3Int(coord.x * VoxelData.chunkWidth, 0, coord.z * VoxelData.chunkWidth);
        chunkObject.layer = LayerMask.NameToLayer("Chunk");
        chunkObject.transform.SetParent(World.world.transform);

        meshFilter = chunkObject.AddComponent<MeshFilter>();
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();
        meshCollider = chunkObject.AddComponent<MeshCollider>();

        meshRenderer.material = World.world.material;

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
                    voxelMap[x, y, z] = World.world.GetVoxel(new Vector3Int(x, y, z) + position);
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
                    if (World.world.blockTypes[voxelMap[x, y, z]].isSolid)
                        CreateVoxelData(new Vector3Int(x, y, z));
                }
            }
        }
    }
    void CreateVoxelData(Vector3Int pos)
    {
        for (byte p = 0; p < 6; p++)
        {
            if (!CheckVoxel(pos + VoxelData.faceChecks[p]))
            {
                for (byte i = 0; i < 4; i++)
                {
                    vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, i]]);
                }

                AddTexture(pos, p);

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
    void AddTexture(Vector3Int pos, byte faceIndex)
    {
        Vector2 uv = World.world.blockTypes[voxelMap[pos.x, pos.y, pos.z]].uvs[faceIndex];

        float x = uv.x / (float)VoxelData.textureSize;
        float y = uv.y / (float)VoxelData.textureSize;

        uvs.Add(new Vector2(x, y));
        uvs.Add(new Vector2(x, y + VoxelData.blockWidth));
        uvs.Add(new Vector2(x + VoxelData.blockWidth, y));
        uvs.Add(new Vector2(x + VoxelData.blockWidth, y + VoxelData.blockWidth));
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
            return World.world.blockTypes[World.world.GetVoxel(pos + position)].isSolid;
        else
            return World.world.blockTypes[voxelMap[pos.x, pos.y, pos.z]].isSolid;
    }
    public bool isActive
    {
        get
        {
            return chunkObject.activeSelf;
        }
        set
        {
            chunkObject.SetActive(value);
        }
    }
}
