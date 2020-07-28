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
    public float Frequency = 0.02f;


    private Terrain Terrain;
    private FastNoise NoiseGenerator;

    private JobMonitor JobMonitor;


    private float[,] HeightMap;
    private float MaxHeight { get; set; }
    private float MinHeight { get; set; }

    #endregion 


    // Start is called before the first frame update
    void Start()
    {
        #region Variable Initialization
        Terrain = GetComponent<Terrain>();
        NoiseGenerator = new FastNoise();

        Seed = Seed > 0 ? Seed : new System.Random().Next(1, 996954);

        JobMonitor = JobMonitor.CreateInstance();
        JobMonitor.Title = "Terrain Generation";
        JobMonitor.CreateInstance().Title = "test";
        JobMonitor.CreateInstance().Title = "jotain";

        #endregion

        NoiseGenerator.SetSeed(Seed);
        NoiseGenerator.SetNoiseType(FastNoise.NoiseType.Perlin);
        NoiseGenerator.SetFrequency(Frequency);

        HeightMap = new float[Terrain.terrainData.heightmapWidth, Terrain.terrainData.heightmapHeight];

        Color[,] HeightMapForMonitor = new Color[Terrain.terrainData.heightmapWidth, Terrain.terrainData.heightmapHeight];

        for (int x = 0; x < Terrain.terrainData.heightmapWidth; x++)
        {
            for (int y = 0; y < Terrain.terrainData.heightmapHeight; y++)
            {
                float noise = NoiseGenerator.GetNoise(x, y);
                MaxHeight = Math.Max(noise, MaxHeight);
                MinHeight = Math.Min(noise, MinHeight);
                HeightMap[x, y] = noise;
            }
        }

        HeightMap = ApplyCircularMask(HeightMap);
        HeightMap = NormalizeHeightMap(HeightMap);

        for (int x = 0; x < Terrain.terrainData.heightmapWidth; x++)
        {
            for (int y = 0; y < Terrain.terrainData.heightmapHeight; y++)
            {
                float noise = HeightMap[x, y];
                HeightMapForMonitor[x, y] = new Color(noise, noise, noise, 1);
            }
        }
        JobMonitor.ImageArray = HeightMapForMonitor;


        Terrain.terrainData.SetHeights(0, 0, HeightMap);
    }

    // Update is called once per frame
    void Update()
    {

    }


    #region Noise operations
    private float[,] NormalizeHeightMap(float[,] heightmap)
    {
        int Width = heightmap.GetLength(0);
        int Height = heightmap.GetLength(1);

        var ModifiedHeightMap = new float[Width, Height];

        for (var x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                float Noise = heightmap[x, y];

                ModifiedHeightMap[x, y] = (Noise - MinHeight) / (MaxHeight - MinHeight);

            }
        }

        return ModifiedHeightMap;
    }



    private float[,] ApplyCircularMask(float[,] heightmap)
    {

        int Width = heightmap.GetLength(0);
        int Height = heightmap.GetLength(1);


        float NewMaxHeight = 0f;
        float NewMinHeight = 0f;
        
        float OldMinHeight = MinHeight;

        var ModifiedHeightMap = new float[Width, Height];

        var Mask = new float[Width, Height];
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                float Noise = heightmap[x, y];
                float DistanceX = Math.Abs(x - Width * 0.5f);
                float DistanceY = Math.Abs(y - Width * 0.5f);
                float Distance = (float)Math.Sqrt(DistanceX * DistanceX + DistanceY * DistanceY);

                float MaxWidth = Width * 0.5f - 100f;
                float Delta = Distance / MaxWidth;
                float Gradient = Delta * Delta;

                float GradientResult = (Math.Max(0.0f, 1.0f - Gradient) * 20f + 1.0f);
                float NewNoise = 
                    (Noise + Math.Abs(OldMinHeight)) * GradientResult;
                Mask[x, y] = NewNoise;


                NewMaxHeight = Math.Max(NewNoise, NewMaxHeight);
                NewMinHeight = Math.Min(NewNoise, NewMinHeight);
            }
        }
        MaxHeight = NewMaxHeight;
        MinHeight = NewMinHeight;

        return Mask;
    }

    #endregion

}
