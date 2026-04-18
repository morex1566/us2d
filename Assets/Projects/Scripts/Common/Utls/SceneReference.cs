#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


/// <summary>
/// 씬 에셋 레퍼런스
/// </summary>
[System.Serializable]
public struct SceneReference
{
    [SerializeField] public string SceneName;

    public static implicit operator string(SceneReference sceneReference)
        => sceneReference.SceneName;

#if UNITY_EDITOR
    [SerializeField] private Object sceneAsset;

    // 리플렉션 데이터
    public static string NameOfEditorSceneAsset => nameof(sceneAsset);
    public static string NameOfSceneName => nameof(SceneName);
#endif
}

#if UNITY_EDITOR
/// <summary>
/// 씬 에셋을 인스펙터에서 바인딩하면, 씬 에셋 대신에 씬 이름을 저장하게 해주는 런타임 클래스 <br/>
/// </summary>
[CustomPropertyDrawer(typeof(SceneReference))]
public class SceneReferenceDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // SceneReference을 리플렉션
        SerializedProperty sceneName = property.FindPropertyRelative(SceneReference.NameOfSceneName);
        SerializedProperty sceneAsset = property.FindPropertyRelative(SceneReference.NameOfEditorSceneAsset);

        // 인스펙터 변경 감지
        EditorGUI.BeginChangeCheck();

        // 인스펙터에서 수정한 리소스 정보를 GET
        Object newObject
            = EditorGUI.ObjectField(position, label, sceneAsset.objectReferenceValue, typeof(Object), allowSceneObjects: false);

        // 인스펙터 변경 감지됨
        if (EditorGUI.EndChangeCheck())
        {
            if (newObject)
            {
                // 씬 에셋인가?
                SceneAsset newSceneAsset = newObject as SceneAsset;
                if (!newSceneAsset)
                {
                    Debug.LogError($"{newObject.name}은 씬 에셋이 아닙니다.");
                    EditorGUI.EndProperty();
                    return;
                }

                sceneName.stringValue = newSceneAsset.name;
                sceneAsset.objectReferenceValue = newSceneAsset;
            }
            else
            {
                sceneName.stringValue = "";
                sceneAsset.objectReferenceValue = null;
            }

            // 변경 사항 저장
            property.serializedObject.ApplyModifiedProperties();
        }

        EditorGUI.EndProperty();
    }
}
#endif
