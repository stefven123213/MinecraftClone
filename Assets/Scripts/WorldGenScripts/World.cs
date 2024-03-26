using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Start()
    {
        Chunk newChunk = new Chunk(new ChunkCoord());
    }
}
