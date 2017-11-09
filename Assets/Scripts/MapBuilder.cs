﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MapGeneration.SaveSystem;
using UnityEngine;
using Random = System.Random;

namespace MapGeneration
{
    public class MapBuilder : Singleton<MapBuilder>
    {
        [SerializeField] private MapBlueprint _currentBlueprint;

        public Map ActiveMap { get; set; }
        public List<MapDataSaver> Maps { get; set; }

        protected override void Awake()
        {
            base.Awake();
            Maps = new List<MapDataSaver>();
        }

        /// <summary>
        /// Generates a map from a specific blueprint
        /// </summary>
        /// <param name="mapBlueprint">blueprint</param>
        /// <returns>Map</returns>
        public Map Generate(MapBlueprint mapBlueprint)
        {
            //If the seed has been defined in the blueprint use that instead.
            var seed = mapBlueprint.UserSeed != 0 ? 
                mapBlueprint.UserSeed : 
                DateTime.Now.Millisecond;

            //Creating the new map.
            Map map = new GameObject(mapBlueprint.name).AddComponent<Map>();
            map.Initialize(seed, mapBlueprint);

            //Save the new map.
            Save(map);

            //Start the blueprint process.
            mapBlueprint.Generate(map);

            //Now that the map is fully made, spawn it.
            Spawn(map);

            return map;
        }

        public Map Generate(MapDataSaver existingMap)
        {
            //Creating the new map.
            Map map = new GameObject(existingMap.MapBlueprint.name).AddComponent<Map>();

            //If save data from active map before making this new one.
            if (ActiveMap.MapDataSaver == existingMap)
                existingMap.SavePersistentData();

            //Update MapDataSaver with the new map reference.
            existingMap.Map = map;

            //Initialize the map with the existing data saver.
            map.Initialize(existingMap.Seed, existingMap.MapBlueprint, existingMap);

            //Start the blueprint process.
            existingMap.MapBlueprint.Generate(map);

            //Now that the map is fully made, spawn it.
            Spawn(map);

            return map;
        }

        /// <summary>
        /// Generates a map form current blueprint
        /// </summary>
        /// <returns>Map</returns>
        public Map Generate()
        {
            return Generate(_currentBlueprint);
        }

        /// <summary>
        /// Saves a map
        /// </summary>
        /// <param name="map">map</param>
        public void Save(Map map)
        {
            Maps.Add(map.MapDataSaver);
        }

        /// <summary>
        /// Spawns a map as instances
        /// </summary>
        /// <param name="map">map</param>
        public void Spawn(Map map)
        {
            //Remember if we have a already active map.
            Map oldMap = ActiveMap;

            //Set the new map as active.
            ActiveMap = map;

            //Lets destroy the old map if there was one.
            if (oldMap)
                Despawn(oldMap);

            float chunkSizeX = map.MapBlueprint.ChunkSize.x;
            float chunkSizeY = map.MapBlueprint.ChunkSize.y;

            for (int x = 0; x < map.MapBlueprint.GridSize.x; x++)
            {
                for (int y = 0; y < map.MapBlueprint.GridSize.y; y++)
                {
                    float xPosition = map.transform.position.x + chunkSizeX * x;
                    float yPOsition = map.transform.position.y + chunkSizeY * y;

                    if (map.Grid[x, y] != null && map.Grid[x,y].Prefab != null) 
                        map.Grid[x, y].Instantiate(new Vector2(xPosition, yPOsition), map.transform);
                }
            }

            //Start the post process
            map.MapBlueprint.StartPostProcess(map);

            map.MapDataSaver.LoadPersistentData();
        }

        /// <summary>
        /// Despawns a map from the world
        /// </summary>
        /// <param name="map">map</param>
        public void Despawn(Map map)
        {
            //If the new map isn't the same as the old one, save its data before despawning.
            if (map && map.MapDataSaver != ActiveMap.MapDataSaver)
                map.MapDataSaver.SavePersistentData();

            //Destroying all instances of the spawned chunks
            Destroy(map.gameObject);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Generate(Maps[0]);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Generate(Maps[1]);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Generate(Maps[2]);
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                Generate(Maps[3]);
            }

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                Generate(Maps[4]);
            }

            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                Generate(Maps[5]);
            }
        }
    }
}
