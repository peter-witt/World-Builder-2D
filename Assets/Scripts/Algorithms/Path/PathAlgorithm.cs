﻿using System;
using System.Collections.Generic;
using System.Linq;
using MapGeneration.Extensions;
using UnityEngine;

namespace MapGeneration.Algorithm
{
    /// <summary>
    /// Purpose: Base class for all path algortihms.
    /// Creator: MP & NJ
    /// </summary>
    public class PathAlgorithm : MapGenerationAlgorithm
    {
        //Compass directions used for choosing where to go next.
        [Flags]
        public enum CardinalDirections
        {
            Top, Bottom, Left, Right
        }

        protected CardinalDirections NextDirection;
        protected Queue<ChunkHolder> MarkedChunks = new Queue<ChunkHolder>();
        protected Queue<KeyValuePair<ChunkHolder, CardinalDirections?>> Road = new Queue<KeyValuePair<ChunkHolder, CardinalDirections?>>();
        protected List<CardinalDirections> DirectionCandidates;

        public override void Process(Map map, List<Chunk> usableChunks)
        {
            base.Process(map, usableChunks);
            Reset();
        }

        /// <summary>
        /// Backtraks both queues
        /// </summary>
        /// <param name="chunks">Chunks to backtrack</param>
        /// <param name="directions">Directions to backtrack</param>
        protected void BackTrackChunks(Queue<ChunkHolder> chunks, Queue<CardinalDirections> directions)
        {
            CardinalDirections currentDirection = CardinalDirections.Bottom;
            while (chunks.Count > 0)
            {
                ChunkHolder currentChunk = chunks.Dequeue();

                if (directions.Count > 0)
                    currentDirection = directions.Dequeue();

                if (currentChunk.Instance && chunks.Count > 0)
                    SetChunkConnections(currentDirection, currentChunk, chunks.First());
            }
        }

        /// <summary>
        /// Backtraks both queues
        /// </summary>
        /// <param name="road"></param>
        protected void BackTrackChunks(Queue<KeyValuePair<ChunkHolder, CardinalDirections?>> road)
        {
            if (road.Count < 2)
                return;

            CardinalDirections currentDirection = CardinalDirections.Bottom;

            ChunkHolder current = road.Dequeue().Key;

            while (road.Count > 0)
            {
                KeyValuePair<ChunkHolder, CardinalDirections?> currentChunk = road.Dequeue();

                if (currentChunk.Value != null)
                    currentDirection = currentChunk.Value.Value;

                SetChunkConnections(currentDirection, current, currentChunk.Key);

                current = currentChunk.Key;
            }
        }

        /// <summary>
        /// Resets direction candidates back to a list full of directions.
        /// </summary>
        protected void ResetDirectionCandidates()
        {
            DirectionCandidates = ((CardinalDirections[])Enum.GetValues(typeof(CardinalDirections))).ToList();
        }

        /// <summary>
        /// Tries to find a suitable chunk from a position.
        /// </summary>
        /// <param name="map">Map it works on.</param>
        /// <param name="usableChunks">All usable chunks.</param>
        /// <param name="currentPos"></param>
        /// <returns>Returns true if it found one, false if it dident.</returns>
        protected KeyValuePair<ChunkHolder, CardinalDirections?>? FindNextChunk(Map map, List<Chunk> usableChunks, ref Vector2Int currentPos)
        {
            //find the next direction among the candidates.
            NextDirection = DirectionCandidates[map.Random.Range(0, DirectionCandidates.Count)];

            //Get the next position
            Vector2Int? nextPosition = CheckNextPosition(currentPos, NextDirection, map);

            //if the position is valid continue the process
            if (nextPosition != null)
            {
                ChunkHolder nextChunk = map.Grid[nextPosition.Value.x, nextPosition.Value.y];

                //if the next chunk isnt marked, continue the process
                if (!MarkedChunks.Contains(nextChunk))
                {
                    //set current position to the next position
                    currentPos = nextPosition.Value;

                    //enqueue the next chunk, so we know it is used.
                    MarkedChunks.Enqueue(nextChunk);

                    //Change the prefab on the found chunk to another one. TODO: Find another way to mark marked chunks.
                    nextChunk.Prefab = usableChunks.FirstOrDefault();
                    //Reset candidates.

                    ResetDirectionCandidates();
                    map.EndChunk = MarkedChunks.LastOrDefault();
                    return new KeyValuePair<ChunkHolder, CardinalDirections?>(nextChunk, NextDirection);
                }

                DirectionCandidates.Remove(NextDirection);
            }
            else
                DirectionCandidates.Remove(NextDirection);

            return null;
        }

        protected override void Reset()
        {
            base.Reset();
            MarkedChunks.Clear();
            ResetDirectionCandidates();
            Road.Clear();
        }
    }
}