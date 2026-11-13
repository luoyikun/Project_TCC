using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UTJ
{
    public class SaveSpringBoneSetupWindow : EditorWindow
    {
        /// <summary>打开弹簧骨骼配置导出窗口，并初始化根对象选择。</summary>
        public static void ShowWindow()
        {
            var editorWindow = GetWindow<SaveSpringBoneSetupWindow>(
                "导出弹簧骨骼配置");
            if (editorWindow != null)
            {
                editorWindow.SelectObjectsFromSelection();
            }
        }

        // private

        private GameObject springBoneRoot;
        private SpringBoneSerialization.ExportSettings exportSettings;

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

        /// <summary>绘制导出选项。</summary>
        private void ShowExportSettingsUI(ref Rect uiRect)
        {
            if (exportSettings == null)
            {
                exportSettings = new SpringBoneSerialization.ExportSettings();
            }

            GUI.Label(uiRect, "导出选项", SpringBoneGUIStyles.HeaderLabelStyle);
            uiRect.y += uiRect.height;
            exportSettings.ExportSpringBones = GUI.Toggle(uiRect, exportSettings.ExportSpringBones, "弹簧骨骼", SpringBoneGUIStyles.ToggleStyle);
            uiRect.y += uiRect.height;
            exportSettings.ExportCollision = GUI.Toggle(uiRect, exportSettings.ExportCollision, "碰撞体", SpringBoneGUIStyles.ToggleStyle);
            uiRect.y += uiRect.height;
        }

        /// <summary>绘制弹簧骨骼根对象、导出选项和 CSV 保存按钮。</summary>
        private void OnGUI()
        {
            SpringBoneGUIStyles.ReacquireStyles();

            const int ButtonHeight = 30;
            const int UISpacing = 8;
            const int UIRowHeight = 24;

            var uiWidth = (int)position.width - UISpacing * 2;
            var yPos = UISpacing;

            springBoneRoot = LoadSpringBoneSetupWindow.DoObjectPicker(
                "弹簧骨骼根对象", springBoneRoot, uiWidth, UIRowHeight, ref yPos);
            var buttonRect = new Rect(UISpacing, yPos, uiWidth, ButtonHeight);
            if (GUI.Button(buttonRect, "从所选对象获取根对象", SpringBoneGUIStyles.ButtonStyle))
            {
                SelectObjectsFromSelection();
            }
            yPos += ButtonHeight + UISpacing;
            buttonRect.y = yPos;

            ShowExportSettingsUI(ref buttonRect);
            if (springBoneRoot != null)
            {
                if (GUI.Button(buttonRect, "保存 CSV", SpringBoneGUIStyles.ButtonStyle))
                {
                    BrowseAndSaveSpringSetup();
                }
            }
        }

        /// <summary>选择保存路径并导出弹簧骨骼配置。</summary>
        private void BrowseAndSaveSpringSetup()
        {
            if (springBoneRoot == null) { return; }

            var initialFileName = springBoneRoot.name + "_Dynamics.csv";

            var path = EditorUtility.SaveFilePanel(
                "导出弹簧骨骼配置", "", initialFileName, "csv");
            if (path.Length == 0) { return; }

            if (System.IO.File.Exists(path))
            {
                var overwriteMessage = "文件已存在，是否覆盖？\n\n" + path;
                if (!EditorUtility.DisplayDialog("保存弹簧骨骼配置", overwriteMessage, "覆盖", "取消"))
                {
                    return;
                }
            }

            var sourceText = SpringBoneSerialization.BuildDynamicsSetupString(springBoneRoot, exportSettings);
            if (FileUtil.WriteAllText(path, sourceText))
            {
                AssetDatabase.Refresh();
                Debug.Log("已保存：" + path);
            }
        }
    }
}
