using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NavData2d
{
    public interface IDynamicObstructor
    {
        bool NeedsUpdateFlag { get; set; }
        int[] RemoveAllCreatedObstructions();
        DynamicObstruction RecordObstruction(int navNodeIndex, NavVert navVert, float start, float end);
        bool FindIntersectionSpan(Vector2 segmentA, Vector2 segmentB, float expansionHeight, out float spanStart, out float spanEnd);
        Bounds Bounds { get; }
        void Visualize();
    }
}
