using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public static World world;
    private void Awake()
    {
        world = this;
    }

    public Material material;
    public BlockType[] blockTypes;

    public int minHeight, maxHeight;

    private void Start()
    {
        GenerateWorld();
    }

    void GenerateWorld()
    {
        for(int x = -5; x < 5; x++)
        {
            for(int z = -5; z < 5; z++)
            {
                Chunk chunk = new Chunk(new ChunkCoord(x, z));
            }
        }
    }

    public byte GetVoxel(Vector3Int pos, int terrainHeight)
    {
        if (!isVoxelInWorld(pos))
            return 0;

        if (pos.y == 0)
            return 1;
        else if (pos.y < terrainHeight)
            return 2;
        else if (pos.y < terrainHeight)
            return 3;
        else if (pos.y == terrainHeight)
            return 4;
        else
            return 0;
    }

    public bool isVoxelInWorld(Vector3Int pos)
    {
        if (pos.y < 0 || pos.y > VoxelData.chunkHeight - 1)
            return false;
        else
            return true;
    }

    public int GetTerrainHeight(Vector3Int pos)
    {
        return minHeight + Mathf.FloorToInt((maxHeight - minHeight) * Noise.Get2DPerlin(pos.x, pos.z, 0.25f, 1231));
    }
}
