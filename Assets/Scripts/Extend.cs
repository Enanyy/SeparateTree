using System;
using System.Collections.Generic;
using UnityEngine;


public static class Extend
{
    public static bool ToBoolEx(this string text)
    {
        bool result = false;

        if (string.IsNullOrEmpty(text))
        {
            return result;
        }

        bool.TryParse(text, out result);

        return result;
    }
   
    public static string ToStringEx(this Bounds bounds)
    {
        return string.Format("({0},{1},{2},{3},{4},{5})", bounds.center.x, bounds.center.y, bounds.center.z, bounds.size.x, bounds.size.y, bounds.size.z);
    }

    public static Bounds ToBoundsEx(this string text)
    {
        if(string.IsNullOrEmpty(text))
        {
            return new Bounds();
        }
        string v = text.Substring(1, text.Length - 2);
        string[] values = v.Split(new string[] { "," }, StringSplitOptions.None);
        if (values.Length == 6)
        {
            Vector3 center = new Vector3(Convert.ToSingle(values[0]), Convert.ToSingle(values[1]), Convert.ToSingle(values[2]));
            Vector3 size = new Vector3(Convert.ToSingle(values[3]), Convert.ToSingle(values[4]), Convert.ToSingle(values[5]));
            return new Bounds(center, size);
        }
        return new Bounds();
    }

    public static Vector3 ToVector3Ex(this string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return new Vector3();
        }
        string v = text.Substring(1, text.Length - 2);
        string[] values = v.Split(new string[] { "," }, StringSplitOptions.None);
        if (values.Length == 3)
        {
            return new Vector3(Convert.ToSingle(values[0]), Convert.ToSingle(values[1]), Convert.ToSingle(values[2]));
        }
        return new Vector3();
    }
}

