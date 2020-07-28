using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Heightmap
{
    #region Variables
    public float Scale = 2.0f;
    public float Frequency = 0.02f;
    public int Seed = 1337;
    public FastNoise.NoiseType NoiseType = FastNoise.NoiseType.Perlin;

    public int Width;
    public int Height;

    public float[,] Map;


    public FastNoise NoiseGenerator;

    System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

    #endregion
    public Heightmap()
    {
        NoiseGenerator = new FastNoise();
    }

    public float[,] Generate()
    {
        NoiseGenerator.SetSeed(Seed);
        NoiseGenerator.SetNoiseType(FastNoise.NoiseType.Perlin);
        NoiseGenerator.SetFrequency(Frequency);
        Map = new float[Width, Height];

        for(int x = 0; x < Width; x++)
        {
            for(int y = 0; y < Height; y++)
            {
                Map[x, y] = NoiseGenerator.GetNoise(x, y);
            }
        }
        return Map;
    }

    public float[,] Normalize()
    {
        var minMax = GetMinMax();

        float[,] normalizedMap = new float[Width, Height];
        for(var x = 0; x < Width; x++)
        {
            for(var y = 0; y < Height; y++)
            {
                normalizedMap[x, y] = (Map[x, y] - minMax.Min) / (minMax.Max - minMax.Min);
            }
        }
        Map = normalizedMap;
        return Map;

    }

    private (float Min, float Max) GetMinMax()
    {
        float min = 0;
        float max = 0;
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                min = Math.Min(Map[x, y], min);
                max = Math.Max(Map[x, y], max);
            }
        }
        return (min, max);
    }

    public Color[,] GetColored(int width, int height)
    {
        Color[,] colors = new Color[width, height];
        float xRatio = 1.0f / width;
        float yRatio = 1.0f / height;

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                float noise = Map[(int)Math.Round(y * yRatio * Map.GetLength(1)), (int)Math.Round(x * xRatio * Map.GetLength(0))];
                Color color = new Color(noise, noise, noise, 1);
                if (noise < 0.2f)
                {
                    color = Color.yellow;
                }else if(noise < 0.5f)
                {
                    color = Color.green;
                }else
                {
                    color = Color.white;
                }

                colors[x, y] = color;
            }
        }
        return colors;
    }

    public float[,] ApplyCircularMask(float islandSize)
    {
        float[,] mask = new float[Width, Height];
        for(int x = 0; x < Width; x++)
        {
            for(int y = 0; y < Height; y++)
            {
                float noise = Map[x, y];
                float distanceX = Math.Abs(x - Width * 0.5f);
                float distanceY = Math.Abs(y - Width * 0.5f);
                float distance = (float)Math.Sqrt(distanceX * distanceX + distanceY * distanceY);
                float maxWidth = Width * 0.5f - (Width / 2 + islandSize);
                float delta = distance / maxWidth;
                float gradient = delta * delta;
                mask[x,y] = noise * Math.Max(0.0f, 1.0f - gradient);
            }
        }
        Map = mask;
        return Map;
    }
}
