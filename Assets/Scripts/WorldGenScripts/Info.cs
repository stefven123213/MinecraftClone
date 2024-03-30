using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region classes
public static class VoxelData
{
    public static readonly int chunkWidth = 16;
    public static readonly int chunkHeight = 256;

    public static readonly int textureSize = 16;
    public static float blockWidth
    {
        get
        {
            return 1f / textureSize;
        }
    }

    public static readonly Vector3[] voxelVerts = new Vector3[8]
    {
        new Vector3(0f, 0f, 0f),
        new Vector3(1f, 0f, 0f),
        new Vector3(1f, 1f, 0f),
        new Vector3(0f, 1f, 0f),
        new Vector3(0f, 0f, 1f),
        new Vector3(1f, 0f, 1f),
        new Vector3(1f, 1f, 1f),
        new Vector3(0f, 1f, 1f),
    };
    public static readonly int[,] voxelTris = new int[6, 4]
    {
        {3, 7, 2, 6},
        {4, 0, 5, 1},
        {0, 3, 1, 2},
        {5, 6, 4, 7},
        {4, 7, 0, 3},
        {1, 2, 5, 6},
    };
    public static readonly Vector3Int[] faceChecks = new Vector3Int[6]
    {
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, -1, 0),
        new Vector3Int(0, 0, -1),
        new Vector3Int(0, 0, 1),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(1, 0, 0),
    };
    public static Vector3Int FloorToInt(Vector3 pos)
    {
        return new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
    }
}
public static class Noise
{
    public static float Get2DPerlin(int x, int z, float scale, float offset)
    {
        return Mathf.PerlinNoise(((x + 0.1f) / VoxelData.chunkWidth) * scale + offset, ((z + 0.1f) / VoxelData.chunkWidth) * scale + offset);
    }
}
#endregion

#region structs
[System.Serializable]
public struct BlockType
{
    public string blockName;
    public bool isSolid;
    public Vector2[] uvs;
    public Color[] colors;
}
public struct ChunkCoord
{
    public int x, z;

    public ChunkCoord(int x, int z)
    {
        this.x = x;
        this.z = z;
    }
    public ChunkCoord(Vector3 pos)
    {
        x = Mathf.FloorToInt(pos.x / VoxelData.chunkWidth);
        z = Mathf.FloorToInt(pos.z / VoxelData.chunkWidth);
    }

    public static bool operator ==(ChunkCoord a, ChunkCoord b)
    {
        return a.x == b.x && a.z == b.z;
    }
    public static bool operator !=(ChunkCoord a, ChunkCoord b)
    {
        return !(a == b);
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return base.ToString();
    }
}
#endregion