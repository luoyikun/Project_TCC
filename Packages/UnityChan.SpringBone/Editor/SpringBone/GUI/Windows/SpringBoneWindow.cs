using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UTJ
{
    public class SpringBoneWindow : EditorWindow
    {
        /// <summary>打开弹簧骨骼工具窗口。</summary>
        [MenuItem("UTJ/弹簧骨骼工具")]
        public static void ShowWindow()
        {
            var window = GetWindow<SpringBoneWindow>("弹簧骨骼");
            window.OnShow();
        }

        // private

        private GUIElements.Column mainUI;
        private Vector2 scrollPosition;

        private Texture headerIcon;
        private Texture newDocumentIcon;
        private Texture openDocumentIcon;
        private Texture saveDocumentIcon;
        private Texture deleteIcon;
        private Texture pivotIcon;
        private Texture sphereIcon;
        private Texture capsuleIcon;
        private Texture panelIcon;

        private SpringBoneSettings settings;

        /// <summary>从指定目录加载工具栏图标。</summary>
        private static Texture LoadIcon(string iconDirectory, string filename)
        {
            var iconPath = PathUtil.CombinePath(iconDirectory, filename);
            var iconTexture = AssetDatabase.LoadAssetAtPath<Texture>(iconPath);
            if (iconTexture == null)
            {
                Debug.LogWarning("图标加载失败：\n" + iconPath);
            }
            return iconTexture;
        }

        private static string FindIconAssetDirectory()
        {
            // Try to find the icons in a way such that the user can put the Dynamics folder anywhere
            return DirectoryUtil.GetFilesRecursively(Application.dataPath, "SpringCapsuleIcon.tga")
                .Select(path => PathUtil.NormalizePath(path))
                .Where(path => path.ToLowerInvariant().Contains("editor/springbone/gui/icons/"))
                .Select(path => PathUtil.SystemPathToAssetPath(System.IO.Path.GetDirectoryName(path)))
                .FirstOrDefault();
        }

        /// <summary>加载窗口所需的图标资源。</summary>
        private void InitializeIcons()
        {
            if (headerIcon != null) { return; }

            var iconDirectory = FindIconAssetDirectory();
            if (iconDirectory == null)
            {
                Debug.LogWarning("未找到弹簧骨骼工具窗口的图标目录。");
                return;
            }

            headerIcon = LoadIcon(iconDirectory, "SpringIcon.tga");
            newDocumentIcon = LoadIcon(iconDirectory, "NewDocumentHS.png");
            openDocumentIcon = LoadIcon(iconDirectory, "OpenHH.bmp");
            saveDocumentIcon = LoadIcon(iconDirectory, "SaveHH.bmp");
            deleteIcon = LoadIcon(iconDirectory, "Delete.png");
            pivotIcon = LoadIcon(iconDirectory, "Pivot.png");
            sphereIcon = LoadIcon(iconDirectory, "SpringSphereIcon.tga");
            capsuleIcon = LoadIcon(iconDirectory, "SpringCapsuleIcon.tga");
            panelIcon = LoadIcon(iconDirectory, "SpringPanelIcon.tga");
        }

        /// <summary>创建窗口中的弹簧骨骼和碰撞体操作控件。</summary>
        private void InitializeButtonGroups()
        {
            if (mainUI != null) { return; }

            const float BigButtonHeight = 60f;

            System.Func<GUIStyle> headerLabelStyleProvider = () => SpringBoneGUIStyles.HeaderLabelStyle;
            System.Func<GUIStyle> buttonLabelStyleProvider = () => SpringBoneGUIStyles.MiddleLeftJustifiedLabelStyle;

            mainUI = new GUIElements.Column(new GUIElements.IElement[]
            {
                new GUIElements.Column(new GUIElements.IElement[]
                {
                    new GUIElements.Label("动力学 CSV", headerLabelStyleProvider),
                    new GUIElements.Row(new GUIElements.IElement[]
                    {
                        new GUIElements.Button("导入", LoadSpringBoneSetupWindow.ShowWindow, openDocumentIcon, buttonLabelStyleProvider),
                        new GUIElements.Button("保存", SaveSpringBoneSetupWindow.ShowWindow, saveDocumentIcon, buttonLabelStyleProvider)
                    },
                    BigButtonHeight)
                }),

                new GUIElements.Column(new GUIElements.IElement[]
                {
                    new GUIElements.Label("弹簧骨骼", headerLabelStyleProvider),
                    new GUIElements.Row(new GUIElements.IElement[]
                    {
                        new GUIElements.Button("添加弹簧骨骼", SpringBoneEditorActions.AssignSpringBonesRecursively, headerIcon, buttonLabelStyleProvider),
                        new GUIElements.Button("创建基点", SpringBoneEditorActions.CreatePivotForSpringBones, pivotIcon, buttonLabelStyleProvider)
                    },
                    BigButtonHeight),
                    new GUIElements.Button("创建/更新管理器", SpringBoneEditorActions.AddToOrUpdateSpringManagerInSelection, newDocumentIcon, buttonLabelStyleProvider),
                    //new GUIElements.Button("执行初始设置", SpringBoneAutoSetupWindow.ShowWindow, newDocumentIcon, buttonLabelStyleProvider),
                    //new GUIElements.Button("与初始骨骼列表对齐", SpringBoneEditorActions.PromptToUpdateSpringBonesFromList, null, buttonLabelStyleProvider),
                    new GUIElements.Separator(),
                    new GUIElements.Button("镜像弹簧骨骼", MirrorSpringBoneWindow.ShowWindow, null, buttonLabelStyleProvider),
                    new GUIElements.Button("选择所选对象及其子级中的弹簧骨骼", SpringBoneEditorActions.SelectChildSpringBones, null, buttonLabelStyleProvider),
                    new GUIElements.Button("删除所选弹簧骨骼", SpringBoneEditorActions.DeleteSelectedBones, deleteIcon, buttonLabelStyleProvider),
                    new GUIElements.Button("删除所选对象及其子级中的管理器和骨骼", SpringBoneEditorActions.DeleteSpringBonesAndManagers, deleteIcon, buttonLabelStyleProvider),
                }),

                new GUIElements.Column(new GUIElements.IElement[]
                {
                    new GUIElements.Label("碰撞体", headerLabelStyleProvider),
                    new GUIElements.Row(new GUIElements.IElement[]
                    {
                        new GUIElements.Button("球体", SpringColliderEditorActions.CreateSphereColliderBeneathSelectedObjects, sphereIcon, buttonLabelStyleProvider),
                        new GUIElements.Button("胶囊体", SpringColliderEditorActions.CreateCapsuleColliderBeneathSelectedObjects, capsuleIcon, buttonLabelStyleProvider),
                        new GUIElements.Button("板", SpringColliderEditorActions.CreatePanelColliderBeneathSelectedObjects, panelIcon, buttonLabelStyleProvider),
                    },
                    BigButtonHeight),
                    new GUIElements.Button("将胶囊体对齐到父级", SpringColliderEditorActions.AlignSelectedCapsulesToParents, capsuleIcon, buttonLabelStyleProvider),
                    new GUIElements.Button("移除弹簧骨骼上的碰撞体", SpringColliderEditorActions.DeleteCollidersFromSelectedSpringBones, deleteIcon, buttonLabelStyleProvider),
                    new GUIElements.Button("删除所选对象及其子级中的碰撞体", SpringColliderEditorActions.DeleteAllChildCollidersFromSelection, deleteIcon, buttonLabelStyleProvider),
                    new GUIElements.Button("清理", SpringColliderEditorActions.CleanUpDynamics, deleteIcon, buttonLabelStyleProvider)
                })
            },
            false,
            0f);
        }

        private Rect GetScrollContentsRect()
        {
            const int ScrollbarWidth = 24;
            var width = position.width - GUIElements.Spacing - ScrollbarWidth;
            var height = mainUI.Height;
            return new Rect(0f, 0f, width, height);
        }

        /// <summary>绘制弹簧骨骼工具窗口及其滚动内容。</summary>
        private void OnGUI()
        {
            if (settings == null) { LoadSettings(); }

            SpringBoneGUIStyles.ReacquireStyles();
            InitializeIcons();
            InitializeButtonGroups();

            var xPos = GUIElements.Spacing;
            var yPos = GUIElements.Spacing;
            var scrollContentsRect = GetScrollContentsRect();
            yPos = ShowHeaderUI(xPos, yPos, scrollContentsRect.width);
            var scrollViewRect = new Rect(0f, yPos, position.width, position.height - yPos);
            scrollPosition = GUI.BeginScrollView(scrollViewRect, scrollPosition, scrollContentsRect);
            mainUI.DoUI(GUIElements.Spacing, 0f, scrollContentsRect.width);
            GUI.EndScrollView();

            ApplySettings();
        }

        private static void DrawHeaderIcon
        (
            ref Rect containerRect,
            Texture iconTexture,
            int iconDrawSize,
            int spacing = 4
        )
        {
            if (iconTexture != null
                && containerRect.width >= iconDrawSize * 3)
            {
                var iconYPosition = containerRect.y + (containerRect.height - iconDrawSize) / 2;
                var iconRect = new Rect(containerRect.x, iconYPosition, iconDrawSize, iconDrawSize);
                GUI.DrawTexture(iconRect, iconTexture);

                var xOffset = iconDrawSize + spacing;
                containerRect.x += xOffset;
                containerRect.width -= xOffset;
            }
        }

        /// <summary>绘制场景视图显示选项，并返回主面板的起始纵坐标。</summary>
        private float ShowHeaderUI(float xPos, float yPos, float uiWidth)
        {
            var needToRepaint = false;
            System.Func<GUIStyle> headerLabelStyleProvider = () => SpringBoneGUIStyles.HeaderLabelStyle;
            System.Func<GUIStyle> toggleStyleProvider = () => SpringBoneGUIStyles.ToggleStyle;
            var headerColumn = new GUIElements.Column(
                new GUIElements.IElement[] {
                    new GUIElements.Label("显示", headerLabelStyleProvider),
                    new GUIElements.Row(new GUIElements.IElement[]
                        {
                            new GUIElements.Toggle("仅显示选中的骨骼", () => settings.onlyShowSelectedBones, newValue => { settings.onlyShowSelectedBones = newValue; needToRepaint = true; }, toggleStyleProvider),
                            new GUIElements.Toggle("显示骨骼碰撞体", () => settings.showBoneSpheres, newValue => { settings.showBoneSpheres = newValue; needToRepaint = true; }, toggleStyleProvider),
                        },
                        GUIElements.RowHeight),
                    new GUIElements.Row(new GUIElements.IElement[]
                        {
                            new GUIElements.Toggle("仅显示选中的碰撞体", () => settings.onlyShowSelectedColliders, newValue => { settings.onlyShowSelectedColliders = newValue; needToRepaint = true; }, toggleStyleProvider),
                            new GUIElements.Toggle("显示骨骼名称", () => settings.showBoneNames, newValue => { settings.showBoneNames = newValue; needToRepaint = true; }, toggleStyleProvider)
                        },
                        GUIElements.RowHeight),
                },
                true, 4f, 0f);
            headerColumn.DoUI(xPos, yPos, uiWidth);
            if (needToRepaint)
            {
                ApplySettings();
                SaveSettings();
                SceneView.RepaintAll();
            }

            return yPos + headerColumn.Height + GUIElements.Spacing;
        }

        private void ApplySettings()
        {
            SpringManager.onlyShowSelectedBones = settings.onlyShowSelectedBones;
            SpringManager.showBoneSpheres = settings.showBoneSpheres;
            SpringManager.onlyShowSelectedColliders = settings.onlyShowSelectedColliders;
            SpringManager.showBoneNames = settings.showBoneNames;
        }

#if false
        private static string GetSettingsFilePath()
        {
            const string SettingsFileName = "SpringBoneWindow.json";
            return ProjectPaths.GetUserPreferencesPath(SettingsFileName);
        }

        private void LoadSettings()
        {
            var settingPath = GetSettingsFilePath();
            if (System.IO.File.Exists(settingPath))
            {
                var settingText = FileUtil.ReadAllText(settingPath);
                if (settingText.Length > 0)
                {
                    settings = JsonUtility.FromJson<SpringBoneSettings>(settingText);
                }
            }
            if (settings == null)
            {
                settings = SpringBoneSettings.GetDefaultSettings();
            }
        }

        private void SaveSettings()
        {
            if (settings == null) { return; }
            var settingText = JsonUtility.ToJson(settings);
            FileUtil.WriteAllText(GetSettingsFilePath(), settingText);
        }

        private void OnDestroy()
        {
            SaveSettings();
        }
#else
        // Todo: Get a good settings path
        private void LoadSettings()
        {
            if (settings == null)
            {
                settings = SpringBoneSettings.GetDefaultSettings();
            }
        }

        private void SaveSettings()
        {
            // NYI
        }
#endif

        private void OnShow()
        {
            LoadSettings();
        }

        [System.Serializable]
        private class SpringBoneSettings
        {
            public bool onlyShowSelectedBones;
            public bool onlyShowSelectedColliders;
            public bool showBoneSpheres;
            public bool showBoneNames;

            public static SpringBoneSettings GetDefaultSettings()
            {
                return new SpringBoneSettings
                {
                    onlyShowSelectedBones = true,
                    onlyShowSelectedColliders = true,
                    showBoneSpheres = true,
                    showBoneNames = false
                };
            }
        }
    }
}
