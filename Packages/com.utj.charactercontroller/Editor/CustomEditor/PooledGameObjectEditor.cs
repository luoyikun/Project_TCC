using System.Collections;
using System.Collections.Generic;
using Unity.TinyCharacterController.Utility;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TinyCharacterControllerEditor
{
    [CustomEditor(typeof(PooledGameObject))]
    public class PooledGameObjectEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            // 构建 UI。
            BuildUi(root, 
                out var isUseLifetime, 
                out var lifeTime, 
                out var onReleaseByLifeTime);

            root.Bind(serializedObject);

            // set initial state and callbacks.
            isUseLifetime.RegisterValueChangedCallback(c => UpdateVisibility(c.newValue));
            UpdateVisibility(serializedObject.FindProperty("_isUseLifetime").boolValue);

            return root;

            void UpdateVisibility(bool value)
            {
                var visibility = value ? DisplayStyle.Flex : DisplayStyle.None;
                lifeTime.style.display = visibility;
                onReleaseByLifeTime.style.display = visibility;
            }
        }

        /// <summary>
        /// 构建 UI。
        /// </summary>
        /// <param name="root">要设置的根对象。</param>
        /// <param name="isUseLifetime">IsUseLifeTime 对应的界面元素。</param>
        /// <param name="lifeTime">lifeTime 对应的界面元素。</param>
        /// <param name="onReleaseByLifeTime">onReleaseByLifeTime 对应的界面元素。</param>
        private static void BuildUi(in VisualElement root, 
            out Toggle isUseLifetime, out PropertyField lifeTime, out PropertyField onReleaseByLifeTime)
        {
            isUseLifetime = new Toggle();
            lifeTime = new PropertyField();
            onReleaseByLifeTime = new PropertyField();

            isUseLifetime.label = "Is Use Lifetime";
            isUseLifetime.bindingPath = "_isUseLifetime";
            lifeTime.bindingPath = "_lifeTime";
            onReleaseByLifeTime.bindingPath = "OnReleaseByLifeTime";

            root.Add(isUseLifetime);
            root.Add(lifeTime);
            root.Add(onReleaseByLifeTime);
        }
    }
}
