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
    public PhysicMaterial mat;
    public BlockType[] blockTypes;

    public GameObject player;
    ChunkCoord playerChunkCoord, playerLastChunkCoord;
    public int chunkBuffer;

    public int renderDistance;
    public int minHeight, maxHeight;

    public Dictionary<ChunkCoord, Chunk> chunks = new Dictionary<ChunkCoord, Chunk>();
    Queue<Chunk> chunksToCreate = new Queue<Chunk>();

    private void Start()
    {
        player.transform.position = new Vector3(0f, maxHeight, 0f);
        playerLastChunkCoord = playerChunkCoord = new ChunkCoord(player.transform.position);

        GenerateWorld();
        StartCoroutine(InitChunks(2));
    }
    private void FixedUpdate()
    {
        playerChunkCoord = new ChunkCoord(player.transform.position);

        CheckChunks();
    }

    public void EditVoxel(Vector3Int _pos, byte index)
    {
        if (!isVoxelInWorld(_pos))
            return;
        ChunkCoord coord = new ChunkCoord(_pos);

        if (!chunks.ContainsKey(coord))
            return;
        if (!chunks[coord].isVoxelMapPopulated)
            return;

        Vector3Int pos = _pos - chunks[coord].position;
        chunks[coord].EditVoxel(pos, index);

        chunks[coord].CreateChunk();
        for(int i = 2; i < 6; i++)
        {
            ChunkCoord _coord = new ChunkCoord(_pos + VoxelData.faceChecks[i]);

            if (coord != _coord && chunks.ContainsKey(_coord) && chunks[_coord].isVoxelMapPopulated)
                chunks[_coord].CreateChunk();
        }
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
                CreateChunk(new ChunkCoord(x, z), true, true);
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

        int terrainHeight = GetTerrainHeight(pos);

        if (pos.y < terrainHeight - 3)
            return 2;
        else if (pos.y < terrainHeight)
            return 3;
        else if (pos.y == terrainHeight)
            return 4;
        else
            return 0;
    }
    public byte PopulateVoxel(Vector3Int pos, int terrainHeight)
    {
        if (!isVoxelInWorld(pos))
            return 0;

        if (pos.y == 0)
            return 1;

        if (pos.y < terrainHeight - 3)
            return 2;
        else if (pos.y < terrainHeight)
            return 3;
        else if (pos.y == terrainHeight)
            return 4;
        else
            return 0;
    }
    public byte CheckVoxel(Vector3Int _pos)
    {
        if (!isVoxelInWorld(_pos))
            return 0;

        ChunkCoord coord = new ChunkCoord(_pos);

        if(chunks.ContainsKey(coord) && chunks[coord].isVoxelMapPopulated)
        {
            Vector3Int pos = _pos - chunks[coord].position;
            return chunks[coord].voxelMap[pos.x, pos.y, pos.z];
        }

        return GetVoxel(_pos);
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

    public int GetTerrainHeight(Vector3Int pos)
    {
        return minHeight + Mathf.FloorToInt((maxHeight - minHeight) * Noise.Get2DPerlin(pos.x, pos.z, 0.25f, 123));
    }

    IEnumerator InitChunks(int populateBuffer)
    {
        while (true)
        {
            if (chunksToCreate.Count > 0)
            {
                Chunk c = chunksToCreate.Dequeue();

                if (!c.isVoxelMapPopulated)
                {
                    c.PopulateVoxelMap();
                }
                c.CreateChunk();
            }
            yield return null;
        }
    }
}
