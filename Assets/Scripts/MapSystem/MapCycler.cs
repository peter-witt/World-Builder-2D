﻿using System;
using System.Collections.Generic;
using System.Linq;
using MapGeneration.SaveSystem;
using UnityEngine;

namespace MapGeneration
{
    /// <summary>
    /// Purpose: Creates a game flow
    /// Creator: MP & NJ
    /// </summary>
    public class MapCycler : Singleton<MapCycler>
    {
        [SerializeField] private GameObject __player;

        public GameObject Player { get; set; }
        public LinkedList<Guid> Maps { get; set; }
        public LinkedListNode<Guid> CurrentMap { get; set; }

        protected override void Awake()
        {
            base.Awake();
            Maps = new LinkedList<Guid>();
        }

        /// <summary>
        /// Loads the next map in the linked list.
        /// </summary>
        public void LoadNextMap()
        {
            //If we havent generated our first map
            if (CurrentMap == null || CurrentMap.Next == null)
            {
                Generate(Guid.Empty, true);
            }
            else
            {
                Generate(CurrentMap.Next.Value, true);
            }
        }

        /// <summary>
        /// Loads the previous map in the linked list.
        /// </summary>
        public void LoadPreviousMap()
        {
            //If we're trying to load a map that isnt' there, dont.
            if (Maps == null || (Maps != null && CurrentMap.Previous == null))
            {
                Debug.LogWarning(string.Format("MapCycler: {0} tried to load a previous map, " +
                                               "but it isn't there.", name), this);
                return;
            }

            if (CurrentMap.Previous != null) Generate(CurrentMap.Previous.Value, false);
        }

        /// <summary>
        /// Generates a new map if it needs to load a non-existing map.
        /// </summary>
        /// <param name="id">ID of a preexisting map if any.</param>
        /// <param name="isStartChunk">Should it place the player in start chunk?</param>
        public void Generate(Guid id, bool isStartChunk)
        {
            MapDataSaver foundMap = MapBuilder.Instance.SavedMaps.FirstOrDefault(saver => saver.MapId == id);
            if (foundMap != null)
            {
                CurrentMap = Maps.Find(id);
                GrabPlayer(MapBuilder.Instance.Generate(foundMap), isStartChunk);
            }
            else
            {
                var newMap = MapBuilder.Instance.Generate();
                CurrentMap = Maps.AddLast(newMap.ID);
            }
        }

        /// <summary>
        /// Takes a map and places a player on it.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="isStartChunk">Should it place the player in start chunk?</param>
        public void GrabPlayer(Map map, bool isStartChunk)
        {
            if (isStartChunk)
            {
                //TODO: Find the start chunk in the map and call grab player on that door it finds.
            }
            else
            {
                //TODO: Find the end chunk in the map and call grab player on that door it finds.
            }
        }
    }
}