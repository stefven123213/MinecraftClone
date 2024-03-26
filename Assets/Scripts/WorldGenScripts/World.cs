using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

public class World : MonoBehaviour
{
    #region singleton
    public static World world;
    private void Awake()
    {
        if (world == null)
            world = this;
        else if (world != this)
            Destroy(this);
    }
    #endregion

    public Material material;
    public BlockType[] blockTypes;

    public int renderDistance;

    public Dictionary<ChunkCoord, Chunk> chunks = new Dictionary<ChunkCoord, Chunk>();

    private void Start()
    {
        GenerateWorld();
    }

    void GenerateWorld()
    {
        for(int x = -renderDistance; x < renderDistance; x++)
        {
            for(int z = -renderDistance; z < renderDistance; z++)
            {
                CreateChunk(new ChunkCoord(x, z));
            }
        }
    }

    void CreateChunk(ChunkCoord coord)
    {
        chunks.Add(coord, new Chunk(coord));
    }
}
