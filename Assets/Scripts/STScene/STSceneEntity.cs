using System;
#if UNITY_EDITOR
using System.Xml;
#endif
using System.Collections.Generic;
using UnityEngine;
using System.Security;

[System.Serializable]
public class STSceneEntity:MonoBehaviour,IEntity,ISTAttribute
{
    #region Attribute
    [System.Serializable]
    public class STSceneEntityAttribute : STAttribute
    {
        public string path = "Prefabs/";
        public Vector3 position;
        public Vector3 rotation;

        public Bounds bounds = new Bounds(Vector3.zero, new Vector3(2, 0, 2));
        public Vector3 localPosition;
        public Vector3 localRotation;
        public Vector3 localScale;
#if UNITY_EDITOR
        public override XmlElement ToXml(XmlNode parent)
        {
            Dictionary<string, string> attributes = new Dictionary<string, string>();
            attributes.Add("position", position.ToString());
            attributes.Add("rotation", rotation.ToString());
            attributes.Add("bounds", bounds.ToStringEx());
            attributes.Add("localPosition", localPosition.ToString());
            attributes.Add("localRotation", localRotation.ToString());
            attributes.Add("localScale", localScale.ToString());
            attributes.Add("path", path);


            return CreateXmlNode(parent,typeof(STSceneEntity).ToString(), attributes);
        }
#endif
    }
    [SerializeField][HideInInspector]
    public STSceneEntityAttribute attribute = new STSceneEntityAttribute();
    public void UpdateAttribute()
    {
        attribute.position = transform.position;
        attribute.rotation = transform.rotation.eulerAngles;
    }
   
#if UNITY_EDITOR
    public XmlElement ToXml(XmlNode parent)
    {
        UpdateAttribute();
        return attribute.ToXml(parent);
    }
#endif

    public void SetAttribute()
    {
        transform.position = attribute.position;
        transform.rotation = Quaternion.Euler(attribute.rotation);
    }

    public void ParseXml(SecurityElement node)
    {
        if(node == null)
        {
            return;
        }

        if(node.Tag == typeof(STSceneEntity).ToString())
        {
            attribute.position = node.Attribute("position").ToVector3Ex();
            attribute.rotation = node.Attribute("rotation").ToVector3Ex();
            attribute.localPosition = node.Attribute("localPosition").ToVector3Ex();
            attribute.localRotation = node.Attribute("localRotation").ToVector3Ex();
            attribute.localScale = node.Attribute("localScale").ToVector3Ex();
            attribute.bounds = node.Attribute("bounds").ToBoundsEx();
            attribute.path = node.Attribute("path");
        }

        SetAttribute();
    }

    #endregion

    [HideInInspector]
    public GameObject mGo;

   

    public Bounds bounds
    {
        get {
            attribute.bounds.center = transform.parent.TransformPoint(transform.localPosition);
            return attribute.bounds; }
    }

    public SeparateTreeNode node { get; set; }

    public void OnHide()
    {
        if (mGo)
        {
            GameObject.Destroy(mGo);
            mGo = null;
        }
    }

    public bool OnShow()
    {
        if (mGo == null && string.IsNullOrEmpty(attribute.path)==false)
        {
            var obj = Resources.Load<GameObject>(attribute.path);
            if (obj)
            {
                mGo = Instantiate<GameObject>(obj);
                mGo.transform.SetParent(transform);
                mGo.transform.localPosition = attribute.localPosition;
                mGo.transform.localRotation = Quaternion.Euler(attribute.localRotation);
                mGo.transform.localScale = attribute.localScale;
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
            bounds.DrawEx(Color.green);
        }
    }

   
#endif
}

