using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;

namespace HISPlayerShader
{
    public class HISPlayer360ShaderGUI : ShaderGUI
    {
        class Styles
        {
            public static GUIContent Get3DLayoutContent(MaterialProperty property)
            {
                if (s_Stereo3DLayoutContent == null)
                    s_Stereo3DLayoutContent = EditorGUIUtility.TrTextContent(
                        property.displayName,
                        "Layout of 3D content in the source. Only meaningful when stereoscopic render is used.");

                return s_Stereo3DLayoutContent;
            }

            private static GUIContent s_Stereo3DLayoutContent;
        }

        //readonly AnimBool m_ShowFlipVertically = new AnimBool();
        readonly AnimBool m_ShowLatLongLayout = new AnimBool();
        readonly AnimBool m_ShowMirrorOnBack = new AnimBool();

        bool m_Initialized = false;

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            if (!m_Initialized)
            {
                m_ShowLatLongLayout.valueChanged.AddListener(materialEditor.Repaint);
                m_ShowMirrorOnBack.valueChanged.AddListener(materialEditor.Repaint);
                m_Initialized = true;
            }

            // Allow the default implementation to set widths for consistency for common properties.
            float lw = EditorGUIUtility.labelWidth;
            materialEditor.SetDefaultGUIWidths();
            ShowProp(materialEditor, FindProperty("_Tint", props));
            ShowProp(materialEditor, FindProperty("_Exposure", props));
            ShowProp(materialEditor, FindProperty("_Rotation", props));
            ShowProp(materialEditor, FindProperty("_FlipVertically", props));

            ShowProp(materialEditor, FindProperty("_MainTex", props));
            EditorGUIUtility.labelWidth = lw;

            m_ShowLatLongLayout.target = ShowProp(materialEditor, FindProperty("_Mapping", props)) == 1;

            if (EditorGUILayout.BeginFadeGroup(m_ShowLatLongLayout.faded))
            {
                m_ShowMirrorOnBack.target = ShowProp(materialEditor, FindProperty("_ImageType", props)) == 1;
                if (EditorGUILayout.BeginFadeGroup(m_ShowMirrorOnBack.faded))
                {
                    EditorGUI.indentLevel++;
                    ShowProp(materialEditor, FindProperty("_MirrorOnBack", props));
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndFadeGroup();
                ShowProp(materialEditor, FindProperty("_Layout", props), Styles.Get3DLayoutContent);
            }
            EditorGUILayout.EndFadeGroup();

            // Let the default implementation add the extra shader properties at the bottom.
            materialEditor.PropertiesDefaultGUI(new MaterialProperty[0]);
        }

        private delegate GUIContent ContentGenerator(MaterialProperty property);
        private float ShowProp(
            MaterialEditor materialEditor, MaterialProperty prop,
            ContentGenerator contentGenerator = null)
        {
            if (contentGenerator != null)
                materialEditor.ShaderProperty(prop, contentGenerator(prop));
            else
                materialEditor.ShaderProperty(prop, prop.displayName);
            return prop.floatValue;
        }
    }
}
