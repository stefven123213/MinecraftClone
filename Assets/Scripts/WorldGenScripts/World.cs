using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public static World world;
    private void Awake()
    {
        if (world == null)
            world = this;
        else if (world != this)
            Destroy(this);
    }

    public Material material;
    public BlockType[] blockTypes;
}
