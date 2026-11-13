using System;
using Unity.TinyCharacterController;
using Unity.TinyCharacterControllerEditor;
using UnityEditor;
using UnityEngine;

namespace TinyCharacterControllerEditor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CharacterSettings))]
    public class CharacterSettingsEditor : UnityEditor.Editor
    {
        private readonly TccCondition _condition = new ();
        private GUIContent _title;
        private Vector2 _scrollPosition;

        private void Awake()
        {
            _title = new GUIContent("Condition Check");
            OnUpdate();
        }
        
        public override bool HasPreviewGUI() => _condition.HasErrorMessage;

        public override GUIContent GetPreviewTitle() => _title;

        public override void OnPreviewGUI(Rect previewArea, GUIStyle background)
        {
            OnUpdate();

            GUILayout.BeginArea(previewArea);
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            foreach (var message in _condition.ErrorMessages)
            {
                EditorGUILayout.HelpBox(message, MessageType.Warning);
            }
            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        /// <summary>
        /// Update components
        /// </summary>
        private void OnUpdate()
        {
            if (Application.isPlaying)
                return;

            _condition.GatherErrorMessage((Component)target);
        }
    }
}
