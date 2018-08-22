using UnityEngine;

[System.Serializable]
public class SceneEntity:MonoBehaviour,IEntity
{
    [HideInInspector]
    [SerializeField]
    private Bounds mBounds = new Bounds(Vector3.zero, Vector3.one * 2);
    [HideInInspector]
    [SerializeField]
    public string mPath = "Prefabs/";

    [HideInInspector]
    public GameObject mGo;

    public Bounds bounds
    {
        get {
            mBounds.center = transform.parent.TransformPoint(transform.localPosition);
            return mBounds; }
        set { mBounds = value; }
    }

    public SeparateTreeNode node { get; set; }

    public void Size(Vector3 size)
    {
        mBounds.size = size;
    }


    public void OnHide()
    {
        if (mGo)
        {
            Object.Destroy(mGo);
            mGo = null;
        }
    }

    public bool OnShow()
    {
        if (mGo == null && string.IsNullOrEmpty(mPath)==false)
        {
            var obj = Resources.Load<GameObject>(mPath);
            mGo = Instantiate<GameObject>(obj);
            mGo.transform.SetParent(transform);
            mGo.transform.localPosition = Vector3.zero;
            mGo.transform.localRotation = Quaternion.identity;
            mGo.transform.localScale = Vector3.one;
            return true;
        }
        return false;
    }
#if UNITY_EDITOR
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
#endif
}

