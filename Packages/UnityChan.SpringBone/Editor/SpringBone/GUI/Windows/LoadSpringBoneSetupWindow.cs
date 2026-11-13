using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UTJ
{
    public class LoadSpringBoneSetupWindow : EditorWindow
    {
        /// <summary>打开弹簧骨骼配置导入窗口，并初始化根对象选择。</summary>
        public static void ShowWindow()
        {
            var editorWindow = GetWindow<LoadSpringBoneSetupWindow>(
                "导入弹簧骨骼配置");
            if (editorWindow != null)
            {
                editorWindow.SelectObjectsFromSelection();
            }
        }

        public static T DoObjectPicker<T>
        (
            string label,
            T currentObject,
            int uiWidth,
            int uiHeight,
            ref int yPos
        ) where T : UnityEngine.Object
        {
            var uiRect = new Rect(UISpacing, yPos, LabelWidth, uiHeight);
            GUI.Label(uiRect, label, SpringBoneGUIStyles.LabelStyle);
            uiRect.x = LabelWidth + UISpacing;
            uiRect.width = uiWidth - uiRect.x + UISpacing;
            yPos += uiHeight + UISpacing;
            return EditorGUI.ObjectField(uiRect, currentObject, typeof(T), true) as T;
        }

        // private

        /// <summary>播放模式下禁止导入配置时显示的提示。</summary>
        private const string StopPlayModeMessage = "请退出播放模式后再导入配置。";
        /// <summary>未指定弹簧骨骼根对象时显示的提示。</summary>
        private const string SelectObjectRootsMessage = "请选择弹簧骨骼根对象。";
        private const int UIRowHeight = 24;
        private const int UISpacing = 8;
        private const int LabelWidth = 200;

        private GameObject springBoneRoot;
        private DynamicsSetup.ImportSettings importSettings;

        private void SelectObjectsFromSelection()
        {
            springBoneRoot = null;
            if (Selection.objects.Length > 0)
            {
                springBoneRoot = Selection.objects[0] as GameObject;
            }

            if (springBoneRoot == null)
            {
                var characterRootComponentTypes = new System.Type[] {
                    typeof(SpringManager),
                    typeof(Animation),
                    typeof(Animator)
                };
                springBoneRoot = characterRootComponentTypes
                    .Select(type => FindObjectOfType(type) as Component)
                    .Where(component => component != null)
                    .Select(component => component.gameObject)
                    .FirstOrDefault();
            }
        }

        /// <summary>绘制导入选项。</summary>
        private void ShowImportSettingsUI(ref Rect uiRect)
        {
            if (importSettings == null)
            {
                importSettings = new DynamicsSetup.ImportSettings();
            }

            GUI.Label(uiRect, "导入选项", SpringBoneGUIStyles.HeaderLabelStyle);
            uiRect.y += uiRect.height;
            importSettings.ImportSpringBones = GUI.Toggle(uiRect, importSettings.ImportSpringBones, "弹簧骨骼", SpringBoneGUIStyles.ToggleStyle);
            uiRect.y += uiRect.height;
            importSettings.ImportCollision = GUI.Toggle(uiRect, importSettings.ImportCollision, "碰撞体", SpringBoneGUIStyles.ToggleStyle);
            uiRect.y += uiRect.height;
        }

        /// <summary>绘制弹簧骨骼根对象、导入选项和 CSV 导入按钮。</summary>
        private void OnGUI()
        {
            SpringBoneGUIStyles.ReacquireStyles();

            const int ButtonHeight = 30;

            var uiWidth = (int)position.width - UISpacing * 2;
            var yPos = UISpacing;
            springBoneRoot = DoObjectPicker("弹簧骨骼根对象", springBoneRoot, uiWidth, UIRowHeight, ref yPos);
            var buttonRect = new Rect(UISpacing, yPos, uiWidth, ButtonHeight);
            if (GUI.Button(buttonRect, "从所选对象获取根对象", SpringBoneGUIStyles.ButtonStyle))
            {
                SelectObjectsFromSelection();
            }
            yPos += ButtonHeight + UISpacing;
            buttonRect.y = yPos;

            ShowImportSettingsUI(ref buttonRect);

            string errorMessage;
            if (IsOkayToSetup(out errorMessage))
            {
                if (GUI.Button(buttonRect, "读取 CSV 并创建配置", SpringBoneGUIStyles.ButtonStyle))
                {
                    BrowseAndLoadSpringSetup();
                }
            }
            else
            {
                const int MessageHeight = 24;
                var uiRect = new Rect(UISpacing, buttonRect.y, uiWidth, MessageHeight);
                GUI.Label(uiRect, errorMessage, SpringBoneGUIStyles.HeaderLabelStyle);
            }
        }

        private bool IsOkayToSetup(out string errorMessage)
        {
            errorMessage = "";
            if (EditorApplication.isPlaying)
            {
                errorMessage = StopPlayModeMessage;
                return false;
            }

            if (springBoneRoot == null)
            {
                errorMessage = SelectObjectRootsMessage;
                return false;
            }
            return true;
        }

        private static T FindHighestComponentInHierarchy<T>(GameObject startObject) where T : Component
        {
            T highestComponent = null;
            if (startObject != null)
            {
                var transform = startObject.transform;
                while (transform != null)
                {
                    var component = transform.GetComponent<T>();
                    if (component != null) { highestComponent = component; }
                    transform = transform.parent;
                }
            }
            return highestComponent;
        }

        private class BuildDynamicsAction : SpringBoneSetupErrorWindow.IConfirmAction
        {
            public BuildDynamicsAction
            (
                DynamicsSetup newSetup,
                string newPath,
                GameObject newSpringBoneRoot
            )
            {
                setup = newSetup;
                path = newPath;
                springBoneRoot = newSpringBoneRoot;
            }

            /// <summary>构建配置、刷新资源数据库并记录导入结果。</summary>
            public void Perform()
            {
                setup.Build();
                AssetDatabase.Refresh();

                const string ResultFormat = "配置完成：{0}\n骨骼数量：{1} 碰撞体数量：{2}";
                var boneCount = springBoneRoot.GetComponentsInChildren<SpringBone>(true).Length;
                var colliderCount = SpringColliderSetup.GetColliderTypes()
                    .Sum(type => springBoneRoot.GetComponentsInChildren(type, true).Length);
                var resultMessage = string.Format(ResultFormat, path, boneCount, colliderCount);
                Debug.Log(resultMessage);
            }

            private DynamicsSetup setup;
            private string path;
            private GameObject springBoneRoot;
        }

        /// <summary>选择 CSV 或文本文件，解析并应用弹簧骨骼配置。</summary>
        private void BrowseAndLoadSpringSetup()
        {
            string checkErrorMessage;
            if (!IsOkayToSetup(out checkErrorMessage))
            {
                Debug.LogError(checkErrorMessage);
                return;
            }

            // var initialPath = "";
            var initialDirectory = ""; // System.IO.Path.GetDirectoryName(initialPath);
            var fileFilters = new string[] { "CSV 文件", "csv", "文本文件", "txt" };
            var path = EditorUtility.OpenFilePanelWithFilters(
                "导入弹簧骨骼配置", initialDirectory, fileFilters);
            if (path.Length == 0) { return; }

            var sourceText = FileUtil.ReadAllText(path);
            if (string.IsNullOrEmpty(sourceText)) { return; }

            var parsedSetup = DynamicsSetup.ParseFromRecordText(springBoneRoot, springBoneRoot, sourceText, importSettings);
            if (parsedSetup.Setup != null)
            {
                var buildAction = new BuildDynamicsAction(parsedSetup.Setup, path, springBoneRoot);
                if (parsedSetup.HasErrors)
                {
                    SpringBoneSetupErrorWindow.ShowWindow(springBoneRoot, springBoneRoot, path, parsedSetup.Errors, buildAction);
                }
                else
                {
                    buildAction.Perform();
                }
            }
            else
            {
                const string ErrorFormat =
                    "弹簧骨骼配置导入失败。\n"
                    + "源文件包含错误，或者\n"
                    + "配置与当前角色不匹配。\n"
                    + "请查看 Console 日志了解详情。\n\n"
                    + "角色：{0}\n\n"
                    + "路径：{1}";
                var resultErrorMessage = string.Format(ErrorFormat, springBoneRoot.name, path);
                EditorUtility.DisplayDialog("弹簧骨骼配置", resultErrorMessage, "确定");
                Debug.LogError("弹簧骨骼配置导入失败：" + springBoneRoot.name + "\n" + path);
            }
            Close();
        }
    }
}
