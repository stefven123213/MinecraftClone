using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Chunk
{
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    MeshCollider meshCollider;

    List<Vector3> vertices = new List<Vector3>();
    List<Vector2> uvs = new List<Vector2>();
    List<Color> colors = new List<Color>();
    List<int> triangles = new List<int>();
    int vertexIndex = 0;

    public GameObject chunkObject;
    public ChunkCoord coord;
    public Vector3Int position;

    byte[,,] voxelMap = new byte[VoxelData.chunkWidth, VoxelData.chunkHeight, VoxelData.chunkWidth];

    public Chunk(ChunkCoord coord)
    {
        this.coord = coord;
        Init();
    }

    void Init()
    {
        chunkObject = new GameObject();

        chunkObject.transform.position = new Vector3(coord.x * VoxelData.chunkWidth, 0, coord.z * VoxelData.chunkWidth);
        position = new Vector3Int(coord.x * VoxelData.chunkWidth, 0, coord.z * VoxelData.chunkWidth);

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
                int terrainHeight = World.world.GetTerrainHeight(new Vector3Int(x, 0, z) + position);
                for (int y = 0; y < VoxelData.chunkHeight; y++)
                {
                    voxelMap[x, y, z] = World.world.GetVoxel(new Vector3Int(x, y, z) + position, terrainHeight);
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
                for (int i = 0; i < 4; i++)
                {
                    vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, i]]);
                }

                AddTexture(pos, p);

                for (int i = 0; i < 6; i++)
                {
                    triangles.Add(vertexIndex + VoxelData.triAdd[i]);
                }

                vertexIndex += 4;
            }
        }
    }
    void AddTexture(Vector3Int pos, byte faceIndex)
    {
        Vector2 uv = World.world.blockTypes[voxelMap[pos.x, pos.y, pos.z]].uvs[faceIndex];

        float x = uv.x / VoxelData.textureSize;
        float y = uv.y / VoxelData.textureSize;

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
            colors = colors.ToArray(),
        };
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh; //TODO: after implementing water,
                                        //create a separate mesh for the collidable and
                                        //non collidable blocks, only apply the colliding blocks mesh to the collider
    }

    bool CheckVoxel(Vector3Int pos)
    {
        if (pos.x < 0 || pos.x > VoxelData.chunkWidth - 1 || pos.y < 0 || pos.y > VoxelData.chunkHeight - 1 || pos.z < 0 || pos.z > VoxelData.chunkWidth - 1)
            return World.world.blockTypes[World.world.GetVoxel(pos + position, World.world.GetTerrainHeight(pos + position))].isSolid;
        else
            return World.world.blockTypes[voxelMap[pos.x, pos.y, pos.z]].isSolid; //TODO: add blockTypes, after implementation, change this to check for is block solid
    }
}
