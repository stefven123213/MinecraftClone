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

    public GameObject player;
    ChunkCoord playerChunkCoord, playerLastChunkCoord;
    public int chunkBuffer;

    public int renderDistance;

    public Dictionary<ChunkCoord, Chunk> chunks = new Dictionary<ChunkCoord, Chunk>();
    Queue<Chunk> chunksToCreate = new Queue<Chunk>();

    private void Start()
    {
        playerLastChunkCoord = playerChunkCoord = new ChunkCoord(player.transform.position);

        GenerateWorld();
        StartCoroutine(InitChunks(2));
    }
    private void FixedUpdate()
    {
        playerChunkCoord = new ChunkCoord(player.transform.position);

        CheckChunks();
    }

    void CheckChunks()
    {
        if (playerChunkCoord != playerLastChunkCoord)
        {
            for (int x = playerChunkCoord.x - renderDistance - chunkBuffer; x < playerChunkCoord.x + renderDistance + chunkBuffer; x++)
            {
                for (int z = playerChunkCoord.z - renderDistance - chunkBuffer; z < playerChunkCoord.z + renderDistance + chunkBuffer; z++)
                {
                    ChunkCoord coord = new ChunkCoord(x, z);

                    if (isChunkInRenderDistance(coord))
                    {
                        if (!chunks.ContainsKey(coord))
                            CreateChunk(coord, false, false);

                        if (!chunks[coord].isActive)
                            chunks[coord].isActive = true;
                    }
                    else if (chunks.ContainsKey(coord) && chunks[coord].isActive)
                        chunks[coord].isActive = false;
                }
            }
            playerLastChunkCoord = playerChunkCoord;
        }
    }
    void GenerateWorld()
    {
        for(int x = -renderDistance; x < renderDistance; x++)
        {
            for(int z = -renderDistance; z < renderDistance; z++)
            {
                CreateChunk(new ChunkCoord(x, z), true, false);
            }
        }
    }

    void CreateChunk(ChunkCoord coord, bool init, bool create)
    {
        chunks.Add(coord, new Chunk(coord));

        if(init)
        {
            chunks[coord].PopulateVoxelMap();

            if(create)
            {
                chunks[coord].CreateChunk();
                return;
            }
        }
        chunksToCreate.Enqueue(chunks[coord]);
    }

    public byte GetVoxel(Vector3Int pos)
    {
        if (!isVoxelInWorld(pos))
            return 0;

        if (pos.y == 0)
            return 1;

        int terrainHeight = 15;

        if (pos.y < terrainHeight - 3)
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
    public bool isChunkInRenderDistance(ChunkCoord coord)
    {
        if (coord.x < playerChunkCoord.x - renderDistance ||
           coord.x > playerChunkCoord.x + renderDistance ||
           coord.z < playerChunkCoord.z - renderDistance ||
           coord.z > playerChunkCoord.z + renderDistance)
            return false;
        else
            return true;
    }

    IEnumerator InitChunks(int populateBuffer)
    {
        int i = 0;
        while (true)
        {
            if (chunksToCreate.Count > 0)
            {
                i++;

                if (i > populateBuffer)
                {
                    i = 0;
                    yield return null;
                }

                Chunk c = chunksToCreate.Dequeue();

                if (!c.isVoxelMapPopulated)
                {
                    c.PopulateVoxelMap();
                }
                c.CreateChunk();
            }
            else
                yield return null;
        }
    }
}
