using UTJ.GameObjectExtensions;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UTJ
{
    public static class SpringBoneEditorActions
    {
        public static void ShowSpringBoneWindow()
        {
            SpringBoneWindow.ShowWindow();
        }

        /// <summary>为所选对象及其子级添加弹簧骨骼组件并刷新管理器列表。</summary>
        public static void AssignSpringBonesRecursively()
        {
            if (Application.isPlaying)
            {
                Debug.LogError("请先退出播放模式。");
                return;
            }

            if (Selection.gameObjects.Length < 1)
            {
                Debug.LogError("请至少选择一个对象。");
                return;
            }

            var springManagers = new HashSet<SpringManager>();
            foreach (var gameObject in Selection.gameObjects)
            {
                SpringBoneSetup.AssignSpringBonesRecursively(gameObject.transform);
                var manager = gameObject.GetComponentInParent<SpringManager>();
                if (manager != null)
                {
                    springManagers.Add(manager);
                }
            }

            foreach (var manager in springManagers)
            {
                SpringBoneSetup.FindAndAssignSpringBones(manager, true);
            }

            AssetDatabase.Refresh();
        }

        /// <summary>为所选弹簧骨骼创建基点节点。</summary>
        public static void CreatePivotForSpringBones()
        {
            if (Application.isPlaying)
            {
                Debug.LogError("请先退出播放模式。");
                return;
            }

            if (Selection.gameObjects.Length < 1)
            {
                Debug.LogError("请至少选择一个对象。");
                return;
            }

            var selectedSpringBones = Selection.gameObjects
                .Select(gameObject => gameObject.GetComponent<SpringBone>())
                .Where(bone => bone != null);
            foreach (var springBone in selectedSpringBones)
            {
                SpringBoneSetup.CreateSpringPivotNode(springBone);
            }
        }

        /// <summary>为所选对象创建管理器或更新其弹簧骨骼列表。</summary>
        public static void AddToOrUpdateSpringManagerInSelection()
        {
            if (Application.isPlaying)
            {
                Debug.LogError("请先退出播放模式。");
                return;
            }

            if (Selection.gameObjects.Length <= 0)
            {
                Debug.LogError("请至少选择一个对象。");
                return;
            }

            foreach (var gameObject in Selection.gameObjects)
            {
                var manager = gameObject.GetComponent<SpringManager>();
                if (manager == null) { manager = gameObject.AddComponent<SpringManager>(); }
                SpringBoneSetup.FindAndAssignSpringBones(manager, true);
            }
        }

        public static void SelectChildSpringBones()
        {
            var springBoneObjects = Selection.gameObjects
                .SelectMany(gameObject => gameObject.GetComponentsInChildren<SpringBone>(true))
                .Select(bone => bone.gameObject)
                .Distinct()
                .ToArray();
            Selection.objects = springBoneObjects;
        }

        /// <summary>删除所选根对象及其子级上的弹簧骨骼和管理器。</summary>
        public static void DeleteSpringBonesAndManagers()
        {
            if (Application.isPlaying)
            {
                Debug.LogError("请先退出播放模式。");
                return;
            }

            if (Selection.gameObjects.Length != 1)
            {
                Debug.LogError("请选择且仅选择一个根对象。");
                return;
            }

            var rootObject = Selection.gameObjects.First();
            var queryMessage = "确定删除此对象及其子级上的所有弹簧骨骼和管理器吗？\n\n"
                + rootObject.name;
            if (EditorUtility.DisplayDialog(
                "删除弹簧骨骼和管理器", queryMessage, "删除", "取消"))
            {
                SpringBoneSetup.DestroySpringManagersAndBones(rootObject);
                AssetDatabase.Refresh();
            }
        }

        /// <summary>删除所选弹簧骨骼，并更新相关管理器。</summary>
        public static void DeleteSelectedBones()
        {
            var springBonesToDelete = GameObjectUtil.FindComponentsOfType<SpringBone>()
                .Where(bone => Selection.gameObjects.Contains(bone.gameObject))
                .ToArray();
            var springManagersToUpdate = GameObjectUtil.FindComponentsOfType<SpringManager>()
                .Where(manager => manager.springBones.Any(bone => springBonesToDelete.Contains(bone)))
                .ToArray();
            Undo.RecordObjects(springManagersToUpdate, "删除所选弹簧骨骼");
            foreach (var boneToDelete in springBonesToDelete)
            {
                Undo.DestroyObjectImmediate(boneToDelete);
            }
            foreach (var manager in springManagersToUpdate)
            {
                manager.FindSpringBones(true);
            }
        }

        /// <summary>确认后根据骨骼列表更新唯一的弹簧骨骼管理器。</summary>
        public static void PromptToUpdateSpringBonesFromList()
        {
            if (Application.isPlaying)
            {
                Debug.LogError("播放模式下无法更新。");
                return;
            }

            var selectedSpringManagers = Selection.gameObjects
                .Select(gameObject => gameObject.GetComponent<SpringManager>())
                .Where(manager => manager != null)
                .ToArray();
            if (!selectedSpringManagers.Any())
            {
                selectedSpringManagers = GameObjectUtil.FindComponentsOfType<SpringManager>().ToArray();
            }

            if (selectedSpringManagers.Count() != 1)
            {
                Debug.LogError("请选择且仅选择一个 SpringManager。");
                return;
            }

            var springManager = selectedSpringManagers.First();
            var queryMessage = "要根据骨骼列表更新弹簧骨骼吗？\n\n"
                + "列表中不存在的 SpringBone 信息将被删除，\n"
                + "模型中尚未登记的 SpringBone 信息将被添加。\n\n"
                + "SpringManager: " + springManager.name;
            if (EditorUtility.DisplayDialog("从骨骼列表更新", queryMessage, "更新", "取消"))
            {
                AutoSpringBoneSetup.UpdateSpringManagerFromBoneList(springManager);
            }
        }
    }
}
