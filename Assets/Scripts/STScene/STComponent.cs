using System;
using System.Collections.Generic;

using System.Security;
#if UNITY_EDITOR
using System.Xml;
#endif
using UnityEngine;


public abstract class STComponent:MonoBehaviour
{
    protected abstract STAttribute Attribute { get; }
    public abstract void UpdateAttribute();
    public abstract void SetAttribute();
#if UNITY_EDITOR
    public virtual XmlElement ToXml(XmlNode parent)
    {
        UpdateAttribute();
        return Attribute.ToXml(parent);
    }
#endif
    public abstract void ParseXml(SecurityElement node);

    public bool IsType(string tag)
    {
        return string.IsNullOrEmpty(tag) == false && tag == GetType().ToString();
    }
}

public abstract class STAttribute
{
#if UNITY_EDITOR
    public static XmlElement CreateXmlNode(XmlNode parent, string tag, Dictionary<string, string> attributes)
    {
        XmlDocument doc;
        if (parent.ParentNode == null)
        {
            doc = (XmlDocument)parent;
        }
        else
        {
            doc = parent.OwnerDocument;
        }
        XmlElement node = doc.CreateElement(tag);

        parent.AppendChild(node);

        foreach (var v in attributes)
        {
            //创建一个属性
            XmlAttribute attribute = doc.CreateAttribute(v.Key);
            attribute.Value = v.Value;
            //xml节点附件属性
            node.Attributes.Append(attribute);
        }

        return node;
    }

    public abstract XmlElement ToXml(XmlNode parent);
#endif
}