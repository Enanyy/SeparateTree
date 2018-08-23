using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(STSceneEntity))]
public class STSceneEntityEditor:Editor
{
    private STSceneEntity mTarget;

    void OnEnable()
    {
        mTarget = target as STSceneEntity;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (mTarget == null)
        {
            return;
        }

        mTarget.attribute.localPosition = EditorGUILayout.Vector3Field("LocalPosition", mTarget.attribute.localPosition);
        mTarget.attribute.localRotation = EditorGUILayout.Vector3Field("LocalRotation", mTarget.attribute.localRotation);
        mTarget.attribute.localScale = EditorGUILayout.Vector3Field("LocalScale", mTarget.attribute.localScale);
        if(mTarget.mGo)
        {
            mTarget.mGo.transform.localPosition = mTarget.attribute.localPosition;
            mTarget.mGo.transform.localRotation = Quaternion.Euler(mTarget.attribute.localRotation);
            mTarget.mGo.transform.localScale = mTarget.attribute.localScale;
        }

        mTarget.attribute.bounds.size = EditorGUILayout.Vector3Field("Bounds Size", mTarget.attribute.bounds.size);

        mTarget.attribute.path = EditorGUILayout.TextField("Resource Path", mTarget.attribute.path);
        if (string.IsNullOrEmpty(mTarget.attribute.path) == false)
        {
            if (GUILayout.Button("Load"))
            {
                if (mTarget.mGo)
                {
                    DestroyImmediate(mTarget.mGo);
                }

                mTarget.OnShow();
            }
        }

        if (mTarget.mGo != null)
        {
            if (GUILayout.Button("Clear"))
            {
                if (mTarget.mGo)
                {
                    DestroyImmediate(mTarget.mGo);
                }

            }
        }
    }
}

