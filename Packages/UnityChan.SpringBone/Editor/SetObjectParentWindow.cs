using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UTJ
{
    public class SetObjectParentWindow : EditorWindow
    {
        /// <summary>打开父子级设置窗口。</summary>
        [MenuItem("UTJ/设置父子级")]
        public static void ShowWindow()
        {
            GetWindow<SetObjectParentWindow>("设置父子级");
        }

        // private

        private Transform newParent;

        private void ReparentSelectedObjects()
        {
            var newChildren = Selection.gameObjects
                .Where(item => item.transform != newParent)
                .Select(gameObject => gameObject.transform)
                .ToArray();
            foreach (var child in newChildren)
            {
                Undo.SetTransformParent(child, newParent, "Set Parent");
            }
        }

        /// <summary>绘制父级对象选择框和执行按钮。</summary>
        private void OnGUI()
        {
            EditorGUILayout.Space();
            newParent = EditorGUILayout.ObjectField("新父级", newParent, typeof(Transform), true) as Transform;
            EditorGUILayout.Space();
            if (GUILayout.Button("设置父子级"))
            {
                ReparentSelectedObjects();
            }
        }
   }
}
