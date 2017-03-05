using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NavData2d;

namespace LightSensing
{
    public abstract class LightMarker : MonoBehaviour, IDynamicObstructor
    {
       //public abstract Color SampleLightAt(Vector2 pos);
        public abstract bool IsPointInsideMarker(Vector2 pos);
        public abstract Bounds Bounds { get; }
        public abstract Vector2 Center { get; }
        public abstract bool NeedsUpdateFlag { get; set; }
        public abstract Color SampleLightAt(Vector2 pos);
        public abstract int[] RemoveAllCreatedObstructions();
        public abstract void Visualize();
        public abstract DynamicObstruction RecordObstruction(int navNodeIndex, NavVert navVert, float start, float end);
        public abstract bool FindIntersectionSpan(Vector2 segmentA, Vector2 segmentB, float expansionHeight, out float spanStart, out float spanEnd);
    }
}
