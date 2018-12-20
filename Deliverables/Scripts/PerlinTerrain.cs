using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PerlinTerrain : MonoBehaviour
{

    private TerrainData myTerrainData;
    public Vector3 worldSize;
    public int resolution = 129;
    float[,] heightArray;

    // Use this for initialization
    void Start()
    {
        myTerrainData = gameObject.GetComponent<Terrain>().terrainData;
        worldSize = new Vector3(200, 50, 200);
        myTerrainData.size = worldSize;
        myTerrainData.heightmapResolution = resolution;
        heightArray = new float[resolution, resolution];
        Perlin();

        // Assign values from heightArray into the terrain object's heightmap
        myTerrainData.SetHeights(0, 0, heightArray);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Perlin()
    {
        float x = 0f, y = 0f;


        for (int i = 0; i < resolution; i++)
        {
            for (int j = 0; j < resolution; j++)
            {
                heightArray[i, j] = Mathf.PerlinNoise(x, y);
                y += 0.02f;
            }
            x += 0.01f;
            y = 0f; 
        }
    }
}