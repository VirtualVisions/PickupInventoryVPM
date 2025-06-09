#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif

using UnityEngine;
using UnityEngine.UIElements;

namespace Vowgan.Inventory
{
    /// <summary>
    /// Disables editing during Inspector preview.
    /// This conveys that the value is meant to be viewed exclusively for readout.
    /// </summary>
    public class ReadoutOnlyAttribute : PropertyAttribute { }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ReadoutOnlyAttribute))]
    public class ReadoutAttributeDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            PropertyField field = new PropertyField(property);
            field.SetEnabled(false);
            return field;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(position, property, label);
            EditorGUI.EndDisabledGroup();
        }
    }
#endif
}