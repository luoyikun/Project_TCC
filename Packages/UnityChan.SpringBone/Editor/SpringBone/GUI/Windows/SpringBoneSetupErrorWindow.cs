using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UTJ
{
    public class SpringBoneSetupErrorWindow : EditorWindow
    {
        public interface IConfirmAction
        {
            void Perform();
        }

        /// <summary>显示配置导入错误，并提供创建可用组件的确认操作。</summary>
        /// <param name="springBoneRoot">弹簧骨骼根对象。</param>
        /// <param name="colliderRoot">碰撞体根对象。</param>
        /// <param name="path">导入文件路径。</param>
        /// <param name="errors">解析过程中发现的错误。</param>
        /// <param name="onConfirm">用户确认后执行的操作。</param>
        public static void ShowWindow
        (
            GameObject springBoneRoot,
            GameObject colliderRoot,
            string path, 
            IEnumerable<DynamicsSetup.ParseMessage> errors, 
            IConfirmAction onConfirm
        )
        {
            var window = GetWindow<SpringBoneSetupErrorWindow>("动力学配置");
            window.springBoneRoot = springBoneRoot;
            window.colliderRoot = colliderRoot;
            window.filePath = path;
            window.onConfirmAction = onConfirm;
            window.errors = errors.ToArray();
        }

        // private

        private GameObject springBoneRoot;
        private GameObject colliderRoot;
        private string filePath;
        private IConfirmAction onConfirmAction;
        private DynamicsSetup.ParseMessage[] errors;
        private Vector2 scrollPosition;

        /// <summary>绘制导入错误、相关对象和确认/取消按钮。</summary>
        private void OnGUI()
        {
            EditorGUILayout.Space();
            GUILayout.Label("配置中发现部分错误。是否仍创建有效的组件？");
            EditorGUILayout.Space();
            EditorGUILayout.ObjectField("弹簧骨骼根对象", springBoneRoot, typeof(GameObject), true);
            EditorGUILayout.ObjectField("碰撞体根对象", colliderRoot, typeof(GameObject), true);
            EditorGUILayout.TextField("路径", filePath);
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("创建"))
            {
                onConfirmAction.Perform();
                Close();
            }
            if (GUILayout.Button("取消")) { Close(); }
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();

            GUILayout.Label("错误详情");
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);
            foreach (var error in errors)
            {
                var errorString = error.Message;
                if (!string.IsNullOrEmpty(error.SourceLine))
                {
                    errorString += "\n" + error.SourceLine;
                }
                GUILayout.Label(errorString);
            }
            GUILayout.EndScrollView();
        }
    }
}
