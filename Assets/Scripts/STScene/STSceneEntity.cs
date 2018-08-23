using UnityEngine;

[System.Serializable]
public class STSceneEntity:MonoBehaviour,IEntity
{
    [HideInInspector]
    [SerializeField]
    private Bounds mBounds = new Bounds(Vector3.zero, new Vector3(2,0,2));
    [HideInInspector]
    [SerializeField]
    public string path = "Prefabs/";

    [HideInInspector]
    public GameObject mGo;

    [HideInInspector]
    public Vector3 localPosition;
    [HideInInspector]
    public Vector3 localRotation;
    [HideInInspector]
    public Vector3 localScale = Vector3.one;

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
        if (mGo == null && string.IsNullOrEmpty(path)==false)
        {
            var obj = Resources.Load<GameObject>(path);
            if (obj)
            {
                mGo = Instantiate<GameObject>(obj);
                mGo.transform.SetParent(transform);
                mGo.transform.localPosition = localPosition;
                mGo.transform.localRotation = Quaternion.Euler(localRotation);
                mGo.transform.localScale = localScale;
                return true;
            }
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

