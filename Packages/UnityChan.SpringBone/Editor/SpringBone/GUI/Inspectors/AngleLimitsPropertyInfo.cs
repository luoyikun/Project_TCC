using UnityEditor;
using UnityEngine;

namespace UTJ
{
    namespace Inspector
    {
        public class AngleLimitPropertyInfo : PropertyInfo
        {
            /// <summary>创建角度限制属性，并设置默认的下限与上限滑块。</summary>
            public AngleLimitPropertyInfo(string newName, string labelText)
                : base(newName, labelText)
            {
                minSlider = new FloatSlider("最小值", 0f, -180f);
                maxSlider = new FloatSlider("最大值", 0f, 180f);
            }

            /// <summary>绘制角度限制滑块及对称调整按钮。</summary>
            public override void Show()
            {
                GUILayout.Space(14f);

                GUILayout.BeginVertical("box");

                var propertyIterator = serializedProperty.Copy();

                if (propertyIterator.NextVisible(true))
                {
                    EditorGUILayout.PropertyField(propertyIterator, label, true, null);
                }

                SerializedProperty minProperty = null;
                SerializedProperty maxProperty = null;
                if (propertyIterator.NextVisible(true))
                {
                    minProperty = propertyIterator.Copy();
                }

                if (propertyIterator.NextVisible(true))
                {
                    maxProperty = propertyIterator.Copy();
                }

                if (minProperty != null
                    && maxProperty != null)
                {
                    const float SubSpacing = 3f;
                    GUILayout.Space(SubSpacing);
                    var minChanged = minSlider.Show(minProperty);
                    GUILayout.Space(SubSpacing);
                    var maxChanged = maxSlider.Show(maxProperty);
                    GUILayout.Space(SubSpacing);
                    GUILayout.BeginHorizontal();

                    updateValuesTogether = GUILayout.Toggle(updateValuesTogether, "同步修改");
                    if (updateValuesTogether)
                    {
                        if (minChanged)
                        {
                            maxProperty.floatValue = -minProperty.floatValue;
                        }
                        else if (maxChanged)
                        {
                            minProperty.floatValue = -maxProperty.floatValue;
                        }
                    }

                    if (GUILayout.Button("按最小值对称"))
                    {
                        maxProperty.floatValue = -minProperty.floatValue;
                    }

                    if (GUILayout.Button("按最大值对称"))
                    {
                        minProperty.floatValue = -maxProperty.floatValue;
                    }

                    if (GUILayout.Button("反转"))
                    {
                        var minValue = minProperty.floatValue;
                        minProperty.floatValue = -maxProperty.floatValue;
                        maxProperty.floatValue = -minValue;
                    }

                    GUILayout.EndHorizontal();
                }

                GUILayout.EndVertical();
            }

            private FloatSlider minSlider;
            private FloatSlider maxSlider;
            private bool updateValuesTogether = false;
        }
    }
}
