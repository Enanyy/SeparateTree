using System;
using System.Collections.Generic;

using System.Security;
#if UNITY_EDITOR
using System.Xml;
#endif
using UnityEngine;

public abstract class STAttribute
{
#if UNITY_EDITOR
    public static XmlElement CreateXmlNode(XmlNode parent, string tag, Dictionary<string, string> attributes)
    {
        XmlDocument doc;
        if (parent.ParentNode == null) {
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

public interface ISTAttribute
{
    void UpdateAttribute();
    void SetAttribute();
#if UNITY_EDITOR
    XmlElement ToXml(XmlNode parent);
#endif
    void ParseXml(SecurityElement node);
}
