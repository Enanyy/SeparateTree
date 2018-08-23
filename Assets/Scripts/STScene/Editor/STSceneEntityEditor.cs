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

        mTarget.localPosition = EditorGUILayout.Vector3Field("LocalPosition", mTarget.localPosition);
        mTarget.localRotation = EditorGUILayout.Vector3Field("LocalRotation", mTarget.localRotation);
        mTarget.localScale = EditorGUILayout.Vector3Field("LocalScale", mTarget.localScale);
        if(mTarget.mGo)
        {
            mTarget.mGo.transform.localPosition = mTarget.localPosition;
            mTarget.mGo.transform.localRotation = Quaternion.Euler(mTarget.localRotation);
            mTarget.mGo.transform.localScale = mTarget.localScale;
        }

        mTarget.Size(EditorGUILayout.Vector3Field("Bounds Size", mTarget.bounds.size));

        mTarget.path = EditorGUILayout.TextField("Resource Path", mTarget.path);
        if (string.IsNullOrEmpty(mTarget.path) == false)
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

