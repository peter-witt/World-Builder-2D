﻿using System.Collections.Generic;
using MapGeneration.Extensions;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MapGeneration.Algorithm
{
    /// <summary>
    /// Purpose: 
    /// Set biome on the generated map with perlin noise
    /// Creator:
    /// Niels Justesen
    /// </summary>
    [CreateAssetMenu(fileName = "New Perlin Noise", menuName = "MapGeneration/Algorithms/Perlin Noise Biome")]
    public class PerlinNoiseBiome : MapGenerationAlgorithm
    {
        private float[,] _noiseGrid;
        private int _width;
        private int _heigt;
        [SerializeField] private int _variation = 1;

        private float xOffset;
        private float yOffset;

        public override bool Process(Map map, List<Chunk> usableChunks)
        {
            _width = map.Grid.GetLength(0) * map.MapBlueprint.ChunkSize.x;
            _heigt = map.Grid.GetLength(1) * map.MapBlueprint.ChunkSize.y;

            xOffset = map.Random.Range(0f, 999f);
            yOffset = map.Random.Range(0f, 999f);

            _noiseGrid = new float[_width, _heigt];

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _heigt; y++)
                {
                    _noiseGrid[x, y] = CalculateNoise(x, y);
                }
            }

            return base.Process(map, usableChunks);
        }

        public override bool PostProcess(Map map, List<Chunk> usableChunks)
        {
            foreach (ChunkHolder chunk in map.Grid)
            {
                if (chunk.Instance != null)
                {
                    int width = chunk.Instance.Width;
                    int height = chunk.Instance.Height;

                    for (int x = 0; x < chunk.Instance.Width; x++)
                    {
                        for (int y = 0; y < chunk.Instance.Height; y++)
                        {
                            chunk.Instance.BiomeValues[x, y] = Mathf.Clamp01(_noiseGrid[x + width * chunk.Position.x, y + height * chunk.Position.y]);
                            
                        }
                    }
                    chunk.Instance.RefreshTilemaps();
                }
            }
            
            return base.PostProcess(map, usableChunks);
        }

        private float CalculateNoise(int x, int y)
        {
            float xCoord = (float)x / _width * _variation + xOffset;
            float yCoord = (float)y / _heigt * _variation + yOffset;
            return Mathf.PerlinNoise(xCoord, yCoord);
        }
    }
}