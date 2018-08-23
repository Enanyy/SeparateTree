using System;
using System.Collections.Generic;
using UnityEngine;


public static class Helper
{
   
    public static string BoundsToString(Bounds bounds)
    {
        return string.Format("({0},{1},{2},{3},{4},{5})", bounds.center.x, bounds.center.y, bounds.center.z, bounds.size.x, bounds.size.y, bounds.size.z);
    }
  
}

