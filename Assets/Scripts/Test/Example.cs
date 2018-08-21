using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 测试场景物体-实际应用中可以根据需求增加或修改，只需实现ISceneObject接口即可
/// </summary>
[System.Serializable]
public class TestSceneObject : ISeparateEntity
{
    [SerializeField]
    private Bounds mBounds;
    [SerializeField]
    private string mResPath;
    [SerializeField]
    private Vector3 mPosition;
    [SerializeField]
    private Vector3 mRotation;
    [SerializeField]
    private Vector3 mSize;

    private GameObject mLoadedPrefab;

    public Bounds Bounds
    {
        get { return mBounds; }
    }

    public void OnHide()
    {
        if (mLoadedPrefab)
        {
            Object.Destroy(mLoadedPrefab);
            mLoadedPrefab = null;
            TestResManager.UnLoad(mResPath);
        }
    }

    public bool OnShow(Transform parent)
    {
        if (mLoadedPrefab == null)
        {
            var obj = TestResManager.Load(mResPath);
            mLoadedPrefab = UnityEngine.Object.Instantiate<GameObject>(obj);
            mLoadedPrefab.transform.SetParent(parent);
            mLoadedPrefab.transform.position = mPosition;
            mLoadedPrefab.transform.eulerAngles = mRotation;
            mLoadedPrefab.transform.localScale = mSize;
            return true;
        }
        return false;
    }

    public TestSceneObject(Bounds bounds, Vector3 position, Vector3 rotation, Vector3 size, string resPath)
    {
        mBounds = bounds;
        mPosition = position;
        mRotation = rotation;
        mSize = size;
        mResPath = resPath;
    }


}

public class Example : MonoBehaviour
{
    public string desc;

    public List<TestSceneObject> loadObjects;

    public Bounds bounds;

    public bool asyn;

    public SeparateDetector detector;

    private SceneObjectLoadController mController;

    void Start()
    {
        mController = gameObject.GetComponent<SceneObjectLoadController>();
        if (mController == null)
            mController = gameObject.AddComponent<SceneObjectLoadController>();
        mController.Init(bounds.center, bounds.size, asyn, SeparateTreeType.QuadTree);


        for (int i = 0; i < loadObjects.Count; i++)
        {
            mController.AddSceneBlockObject(loadObjects[i]);
        }
    }

    void OnGUI()
    {
        GUI.color = Color.red;
        GUILayout.Label(desc);
    }
    
    void Update()
    {
        mController.RefreshDetector(detector);
    }

    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
        }
        else
        {
            bounds.DrawBounds(Color.green);
        }
    }
}
