using System;
using System.Collections.Generic;
using System.Security;
#if UNITY_EDITOR
using System.Xml;
#endif
using UnityEngine;
public class STSceneGround :STComponent
{
#region Attribute
    [Serializable]
    public class STSceneGroundAttribute : STAttribute
    {
        public string path;
        public Vector3 localPosition;
        public Vector3 localRotation;
        public Vector3 localScale = Vector3.one;
#if UNITY_EDITOR

        public override XmlElement ToXml(XmlNode parent)
        {
            Dictionary<string, string> attributes = new Dictionary<string, string>();
          
            attributes.Add("localPosition", localPosition.ToString());
            attributes.Add("localRotation", localRotation.ToString());
            attributes.Add("localScale", localScale.ToString());
            attributes.Add("path", path);

            return CreateXmlNode(parent, typeof(STSceneGround).ToString(), attributes);
        }
#endif

    }


    public override void ParseXml(SecurityElement node)
    {
        if(node == null)
        {
            return;
        }

        if(node.Tag == typeof(STSceneGround).ToString())
        {
            attribute.localPosition = node.Attribute("localPosition").ToVector3Ex();
            attribute.localRotation = node.Attribute("localRotation").ToVector3Ex();
            attribute.localScale = node.Attribute("localScale").ToVector3Ex();
            attribute.path = node.Attribute("path");
        }
        SetAttribute();
    }

    public override void SetAttribute()
    {
        if(mGo)
        {
            DestroyImmediate(mGo);
        }
        if(string.IsNullOrEmpty(attribute.path)==false)
        {
            var obj = Resources.Load<GameObject>(attribute.path);
            if (obj)
            {
                mGo = Instantiate<GameObject>(obj);
                mGo.transform.SetParent(transform);
                mGo.transform.localPosition = attribute.localPosition;
                mGo.transform.localRotation = Quaternion.Euler(attribute.localRotation);
                mGo.transform.localScale = attribute.localScale;
            }
        }
    }
#if UNITY_EDITOR
    public override XmlElement ToXml(XmlNode parent)
    {
        UpdateAttribute();
        return attribute.ToXml(parent);
    }
#endif
    public override void UpdateAttribute()
    {
        if(mGo)
        {
            attribute.localPosition = mGo.transform.localPosition;
            attribute.localRotation = mGo.transform.localRotation.eulerAngles;
            attribute.localScale = mGo.transform.localScale;
        }
    }
    #endregion

    [SerializeField][HideInInspector]
    public STSceneGroundAttribute attribute = new STSceneGroundAttribute();

    [HideInInspector]
    public GameObject mGo;
}

