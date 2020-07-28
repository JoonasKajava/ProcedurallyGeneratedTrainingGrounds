using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class Extensions
{

    public static Color[] ToFlat(this Color[,] colors)
    {
        int width = colors.GetLength(0);
        int height = colors.GetLength(1);
        Color[] flat = new Color[width * height];
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                flat[width * y + x] = colors[x, y];
            }
        }
        return flat;
    }
}