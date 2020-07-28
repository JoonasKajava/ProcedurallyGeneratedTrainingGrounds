using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeometryGeneration : MonoBehaviour
{

    #region Variables

    [Tooltip("Sets seed to be used in noise generation")]
    public int Seed;

    [Tooltip("Sets scale of the noise")]
    public float Scale = 2.0f;
    public float Frequency = 0.015f;
    public float IslandSize = 200.0f;

    public Material TerrainMaterial;

    private Terrain Terrain;
    private FastNoise NoiseGenerator;

    private JobMonitor JobMonitor;

    System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

    #endregion 


    // Start is called before the first frame update
    void Start()
    {
        #region Variable Initialization
        Terrain = GetComponent<Terrain>();
        NoiseGenerator = new FastNoise();

        Seed = Seed > 0 ? Seed : new System.Random().Next(1, 996954);

        JobMonitor = JobMonitor.CreateInstance();
        JobMonitor.Title = "Terrain Heightmap";

        #endregion


        GenerateTerrain();

    }

    public void GenerateTerrain()
    {
        stopwatch.Start();
        Heightmap map = new Heightmap()
        {
            Seed = Seed,
            Width = Terrain.terrainData.heightmapResolution,
            Height = Terrain.terrainData.heightmapResolution,
            Frequency = Frequency
        };
        map.Generate();
        map.Normalize();
        map.ApplyCircularMask(IslandSize);
        stopwatch.Stop();
        Debug.Log(string.Format("lasted: {0}", stopwatch.ElapsedMilliseconds));
        stopwatch.Reset();
        Terrain.terrainData.SetHeights(0, 0, map.Map);
        JobMonitor.ImageArray = map.GetColored(150, 150);
        var NewTexture = new Texture2D(map.Map.GetLength(0), map.Map.GetLength(1));
        NewTexture.SetPixels(map.GetColored(513, 513).ToFlat());
        NewTexture.Apply();
        TerrainMaterial.SetTexture("_MainTex", NewTexture);
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.mouseScrollDelta.y != 0)
        {
            IslandSize += Input.mouseScrollDelta.y;
            GenerateTerrain();
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            Seed = new System.Random().Next(1, 99999);
            GenerateTerrain();
        }

    }

}
