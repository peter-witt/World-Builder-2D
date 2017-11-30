﻿using MapGeneration.Algorithm;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;
using MapGeneration.Extensions;

namespace MapGeneration.Editor
{
    /// <summary>
    /// Purpose: Draws a custom inspector for map blueprints.
    /// Creator: MP
    /// </summary>
    [CustomEditor(typeof(MapBlueprint))]
    public class MapBlueprintEditor : UnityEditor.Editor
    {
        private MapBlueprint _context;
        private SerializedProperty _whitelistedChunks;
        private SerializedProperty _blacklistedChunks;

        private SerializedProperty _currentSelectedAlgorithm;
        private ReorderableList _algorithmStack;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            //Auto settings
            EditorGUILayout.LabelField("Automation Settings:", EditorStyles.boldLabel);
            _context.FillEmptySpaces = EditorGUILayout.Toggle("Fill Empty Spaces", _context.FillEmptySpaces);
            _context.FindValidChunks = EditorGUILayout.Toggle("Find Valid Chunks", _context.FindValidChunks);
            _context.OpenConnections = EditorGUILayout.Toggle("Open Connections", _context.OpenConnections);

            //Algorithm stack
            if (_algorithmStack != null)
            {
                GUILayout.Space(20);
                EditorGUILayout.LabelField("Algorithm Stack:", EditorStyles.boldLabel);
                _algorithmStack.DoLayoutList();
                GUILayout.Space(10);
                EditorExtension.DrawSerializedProperty(_currentSelectedAlgorithm);
            }
             
            GUILayout.Space(20);
            EditorGUILayout.LabelField("Size Settings:", EditorStyles.boldLabel);
            _context.GridSize = EditorGUILayout.Vector2IntField("Grid", _context.GridSize);
            _context.ChunkSize = EditorGUILayout.Vector2IntField("Chunk", _context.ChunkSize);

            GUILayout.Space(20);
            EditorGUILayout.LabelField("Chunk Conditions:", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_whitelistedChunks, true);
            EditorGUILayout.PropertyField(_blacklistedChunks, true);

            EditorUtility.SetDirty(_context);
            serializedObject.ApplyModifiedProperties();
        }

        private void OnEnable()
        {
            _context = target as MapBlueprint;
            _whitelistedChunks = serializedObject.FindProperty("WhitelistedChunks"); 
            _blacklistedChunks = serializedObject.FindProperty("BlacklistedChunks");

            //Instantiate a new reorderablelist
            _algorithmStack = new ReorderableList(
                serializedObject,
                serializedObject.FindProperty("AlgorithmStack"),
                true, false, true, true);

            //We need to hook some methods to events on the new reorderable list.
            _algorithmStack.onAddDropdownCallback = OnAlgorithmStackAddElement;
            _algorithmStack.drawElementCallback = OnAlgorithmStackDrawElement;
        }

        /// <summary>
        /// Called when a new element is about to get added to the algorithm stack, 
        /// meaning the plus button is clicked.
        /// </summary>
        private void OnAlgorithmStackAddElement(Rect buttonRect, ReorderableList l)
        {
            var menu = new GenericMenu(); //Create a clean context menu

            //Lets find all the assets under the type MapGenerationAlgorithm.
            var algorithms = AssetDatabaseExtension.FindAssetsByType<MapGenerationAlgorithm>();

            //Add each of the found algorithms to the newly made context menu.
            foreach (var algorithm in algorithms)
            {
                menu.AddItem(new GUIContent(algorithm.name), false, data =>
                {
                    _context.AlgorithmStack.Add(new AlgorithmStorage(data as MapGenerationAlgorithm));
                }, algorithm);  
            }

            menu.ShowAsContext();
        }

        /// <summary>
        /// Called when the reorderable list draws an element.
        /// </summary>
        private void OnAlgorithmStackDrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            //Lets create some offset for the elements.
            rect.y += 2;

            //First we grab an element from the algorithm stack
            var targetObject = _algorithmStack.serializedProperty.GetArrayElementAtIndex(index);
             
            var algorithmValue = targetObject.FindPropertyRelative("Algorithm");
            var toggleValue = targetObject.FindPropertyRelative("IsActive");

            float toggleWidth = 15;

            //Then we create an object field for the object.
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width - (toggleWidth + 5), EditorGUIUtility.singleLineHeight),
                algorithmValue, GUIContent.none);

            EditorGUI.PropertyField(new Rect(rect.width + toggleWidth, rect.y, toggleWidth, EditorGUIUtility.singleLineHeight),
                toggleValue, GUIContent.none);

            //If this element is active we make it the current active algorithm.
            if (isActive)
                _currentSelectedAlgorithm = algorithmValue;

            //If this element is in focus and active we make it go away with a key press.
            if (isFocused && isActive && Event.current.keyCode == KeyCode.Delete && Event.current.type == EventType.KeyDown)
            {
                _context.AlgorithmStack.RemoveAt(index);
                if (_currentSelectedAlgorithm == algorithmValue)
                    _currentSelectedAlgorithm = null;
            }
        }
    }
}