using Core.Atributes;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ShowOnlyAttribute))]
public class ShowOnlyDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
	{
		string valueStr;
 
		switch (prop.propertyType)
		{
			case SerializedPropertyType.Integer:
				valueStr = prop.intValue.ToString();
				break;
			case SerializedPropertyType.Boolean:
				valueStr = prop.boolValue.ToString();
				break;
			case SerializedPropertyType.Float:
				valueStr = prop.floatValue.ToString("0.00000");
				break;
			case SerializedPropertyType.String:
				valueStr = prop.stringValue;
				break;
			case SerializedPropertyType.Enum:
				valueStr = prop.enumNames[prop.enumValueIndex];
				break;
			default:
				valueStr = prop.objectReferenceValue==null?"null":prop.objectReferenceValue.ToString();
				break;
		}
 
		EditorGUI.LabelField(position,label.text, valueStr,new GUIStyle(){ fontStyle = FontStyle.Bold });
	}
}