using System.Linq;
using MapGeneration.Algorithm;
using UnityEngine;

namespace MapGeneration.ChunkSystem
{
    /// <summary>
    /// Example validationEntry that needs en empty space in a given direction, then if that is the
    /// case, place another chunk in that spot
    /// </summary>
    [CreateAssetMenu(fileName = "New Validate Neighbor", menuName = "2D Map Generation/Conditional Chunks/Validate Neighbor")]
    public class ValidateNeighbor : ValidationEntry
    {
        //The direction it checks.
        [SerializeField]
        private PathAlgorithm.CardinalDirections _direction;

        //What chunk type are we looking for in the direction.
        [SerializeField, Tooltip("Leave empty if neighbor should be empty")]
        private GameObject __chunkToCheck;

        [SerializeField, Header("On Approved")]
        private Chunk _chunkToSpawn;

        /// <summary>
        /// Validated the space in the given direction
        /// </summary>
        /// <param name="map">Map</param>
        /// <param name="chunkHolder">Chunkholder</param>
        /// <returns></returns>
        public override bool Validate(Map map, ChunkHolder chunkHolder)
        {
            var neighbors = map.GetNeighbor(chunkHolder, _direction);

            if (!neighbors.Any())
                return false;

            foreach (var item in neighbors)
            {
                if (item.Prefab != __chunkToCheck ||
                    !item.ChunkOpenings.IsMatching(_chunkToSpawn.ChunkOpenings) ||
                    item.ChunkType != _chunkToSpawn.ChunkType)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Spawns the other chunk if validation was successful
        /// </summary>
        /// <param name="map">Current map</param>
        /// <param name="chunkHolder">Current chunkholder</param>
        public override void Approved(Map map, ChunkHolder chunkHolder)
        {
            base.Approved(map, chunkHolder);

            var newPos = map.GetChunkPos(chunkHolder);

            switch (_direction)
            {
                case PathAlgorithm.CardinalDirections.Top:
                    newPos = new Vector2Int(newPos.x, newPos.y + 1);
                    break;

                case PathAlgorithm.CardinalDirections.Bottom:
                    newPos = new Vector2Int(newPos.x, newPos.y - 1);
                    break;

                case PathAlgorithm.CardinalDirections.Left:
                    newPos = new Vector2Int(newPos.x + 1, newPos.y);
                    break;

                case PathAlgorithm.CardinalDirections.Right:
                    newPos = new Vector2Int(newPos.x - 1, newPos.y);
                    break;
            }

            var newHolder = map.Grid[newPos.x, newPos.y];

            chunkHolder.ChunkOpenings.SetConnectionAuto(_direction, newHolder);

            map.Place(newHolder, _chunkToSpawn);
        }
    }
}