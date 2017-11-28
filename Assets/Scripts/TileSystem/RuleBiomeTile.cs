﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MapGeneration;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MapGeneration
{

    [CreateAssetMenu(fileName = "RuleBiomeTile", menuName = "MapGeneration/Tiles/RuleBiomeTile")]
    public class RuleBiomeTile : BiomeTile
    {
        private enum TileNeighborType
        {
            Middle,
            top,
            bottom,
            left,
            Right
        }


        [SerializeField] private List<RuleBiomeSprites> _biomesSprites 
            = new List<RuleBiomeSprites>();

        private RuleBiomeSprites _currentSprites;
        private TileNeighborType _neighborType;

        protected override void RefreshBiomeValues(Vector3Int position, ITilemap tilemap)
        {
            base.RefreshBiomeValues(position, tilemap);

                //if a chunk is found, then get the current biome id and use it to get sprites
                if (_chunk)
                {
                    _currentSprites = _biomesSprites.Count > 0
                        ? _biomesSprites.FirstOrDefault(x => x.iD == _biome)
                        : null;
                }
            //This checks the tiles neighbor status
            _neighborType = GetNeightbotType(position, tilemap);
        }

        public override void GetTileData(Vector3Int position, ITilemap tilemap,
            ref TileData tileData)
        {
            base.GetTileData(position,tilemap,ref tileData);

            //This refeshes the values needed for the tile
            RefreshBiomeValues(position, tilemap);

            if (_currentSprites != null)
            {
                switch (_neighborType)
                {
                    case TileNeighborType.Middle:
                        tileData.sprite = _currentSprites.middleSprite;
                        break;
                    case TileNeighborType.top:
                        tileData.sprite = _currentSprites.topSprite;
                        break;
                    case TileNeighborType.left:
                        tileData.sprite = _currentSprites.leftSprite;
                        break;
                    case TileNeighborType.Right:
                        tileData.sprite = _currentSprites.rightSprite;
                        break;
                    case TileNeighborType.bottom:
                        tileData.sprite = _currentSprites.middleSprite;
                        break;

                }
                /*TODO Figure out why setting the tiledata.color dosen't change the color of the tile 
                and why this does.*/
                tilemap.GetComponent<Tilemap>().SetColor(position, _currentSprites.tint);
            }
            else
                Debug.LogWarning("Tile is missing sprites for the current biome");

        }


        private TileNeighborType GetNeightbotType(Vector3Int position, ITilemap tilemap)
        {
            TileNeighborType type = 0;
            TileBase top = tilemap.GetTile(position + new Vector3Int(0, 1, 0));
            TileBase left = tilemap.GetTile(position + new Vector3Int(-1, 0, 0));
            TileBase right = tilemap.GetTile(position + new Vector3Int(1, 0, 0));

            if (top && (left || right))
                type = TileNeighborType.Middle;

            else if (!top && right && !left)
                type = TileNeighborType.left;

            else if (!top && left && !right)
                type = TileNeighborType.Right;

            else if (top)
                type = TileNeighborType.bottom;

            else if (!top)
                type = TileNeighborType.top;

            return type;
        }

        
    }
}