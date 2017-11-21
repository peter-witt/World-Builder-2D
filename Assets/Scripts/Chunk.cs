﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using MapGeneration.Extensions;

namespace MapGeneration
{
    public enum ChunkType
    {
        Default, DeadEnd, Reward, Secret
    }

    /// <summary>
    /// Purpose:
    /// To Store the chunk's data.
    /// Creator:
    /// Mikkel Nielsen
    /// </summary>
    [ExecuteInEditMode]
    public class Chunk : MonoBehaviour
    {
        //This sections is for generel properties 
        [Header("Properties"), SerializeField]
        private int _width;//Tells the width of the chunk

        //Tells the height of the chunk
        [SerializeField] private int _height;
        
        //Tells the type of the chunk
        [SerializeField] private ChunkType _chunkType;

        //These fields tells what openings are open on the chunk
        [Header("Openings"), SerializeField] private bool _topOpen;
        [SerializeField] private bool _bottomOpen;
        [SerializeField] private bool _leftOpen;
        [SerializeField] private bool _rightOpen;

        //This is a list of TileFlags in the chunk
        [SerializeField] private List<TileFlag> _connections = new List<TileFlag>();

        [SerializeField] private List<TileFlag> _tileTileFlags = new List<TileFlag>();

        //This section is for refernces
        [Header("Refernces"), SerializeField] private ChunkBehavior _chunkBehavior;
        [SerializeField] private Tilemap _enviorment;
        [SerializeField] private ChunkHolder _chunkHolder;


        //Properties for generel properties
        public int Width{ get { return _width; } set { _width = value; }}
        public int Height{get { return _height; } set { _height = value; }}
        public ChunkType ChunkType{get { return _chunkType; } set { _chunkType = value; }}

        public ChunkHolder ChunkHolder
        {
            get { return _chunkHolder; }
            set { _chunkHolder = value; }
        }

        //Properties for references
        public ChunkBehavior ChunkBehavior
        {
            get
            {
                if (!_chunkBehavior)
                    _chunkBehavior = GetComponent<ChunkBehavior>();
                return _chunkBehavior;
            }
            set
            {
                _chunkBehavior = value;
            }
        }

        public Tilemap Enviorment { get { return _enviorment; } set { _enviorment = value; } }

        public string ID { get; set; }//A ID to indentify the Chunk

        //A list for the items in the chunk
        public List<GameObject> Items { get; set; }

        public List<TileFlag> Connections{get { return _connections; } set { _connections = value; }}
        public List<TileFlag> TileFlags{get { return _tileTileFlags; }set { _tileTileFlags = value; }}

       public void Start()
        {
            if(!_enviorment)
                Debug.LogWarning(gameObject.name + " dosen't have a refernec to a envierment tilemap");
        }

        

    }
}
