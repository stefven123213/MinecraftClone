using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VoxelData
{
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
}