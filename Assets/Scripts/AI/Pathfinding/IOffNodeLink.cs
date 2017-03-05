using UnityEngine;
using System.Collections;
using System;
using CC2D;

[Serializable]
public abstract class IOffNodeLink
{
    public NavData2d.NavPosition targetPos;
    public Vector2 startPoint;
    public float positionAlongPath;
    public float traversCosts;
}
