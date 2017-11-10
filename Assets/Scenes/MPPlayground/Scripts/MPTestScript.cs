﻿using MapGeneration;
using UnityEngine;

namespace MPPlayground
{
    public class MPTestScript : MonoBehaviour
    {
        private Map _map;

        void Start()
        {
            _map = MapBuilder.Instance.Generate();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                _map = MapBuilder.Instance.Generate();
            }
        }
    }
}