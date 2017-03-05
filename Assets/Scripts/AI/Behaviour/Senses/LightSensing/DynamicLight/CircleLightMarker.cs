#define DEBUG_SHOW_GIZMOS

using UnityEngine;
using System.Collections;
using System;
using NavData2d;
using System.Collections.Generic;

namespace LightSensing
{
    public class CircleLightMarker : LightMarker
    {
        [SerializeField]
        float radius;
        [SerializeField]
        Vector2 centerOffset;

        [SerializeField]
        Color centerColor;
        [SerializeField]
        Color edgeColor;
        [SerializeField]
        Color pathfindingColor;

        [SerializeField]
        bool squaredInterpolation;

        List<LightObstructionRecord> obstructionRecord;
        float sqrRadius;
        Bounds chachedBounds;

        public override Bounds Bounds
        {
            get
            {
                if (transform.hasChanged)
                {
                    chachedBounds = new Bounds(transform.position, new Vector3(radius * transform.lossyScale.x * 2, radius * transform.lossyScale.y * 2));
                    transform.hasChanged = false;
                }
                return chachedBounds;
            }
        }

        public override Vector2 Center
        {
            get
            {
                return transform.TransformPoint(centerOffset);
            }
        }

        public override bool NeedsUpdateFlag
        {
            get
            {
                return transform.hasChanged;
            }

            set
            {
                transform.hasChanged = value;
            }
        }

        public override bool IsPointInsideMarker(Vector2 pos)
        {
            pos = transform.InverseTransformPoint(pos);
            float dx = Mathf.Abs(pos.x - centerOffset.x);
            float dy = Mathf.Abs(pos.y - centerOffset.y);

            if (dx > radius)
                return false;
            if (dy > radius)
                return false;
            if (dx + dy <= radius)
                return true;
            if (dx * dx + dy * dy <= sqrRadius)
                return true;
            return false;
        }

        const float LineCircle_FudgeFactor = 0.00001f;
       /* public override bool IsTraversable(LightSkin skin, out float traverseCostsMulitplier)
        {
            return skin.IsTraversable(pathfindingColor, out traverseCostsMulitplier);
        }

        public override bool OverlapsSegment(Vector2 segmentA, Vector2 segmentB)
        {
            if (!Utility.ExtendedGeometry.DoesLineIntersectBounds(segmentA, segmentB, Bounds))
            {
                return false;
            }
            segmentA = transform.InverseTransformPoint(segmentA);
            segmentB = transform.InverseTransformPoint(segmentB);

            Vector2 dir = (segmentB - segmentA).normalized;
            float t = dir.x * (centerOffset.x - segmentA.x) + dir.y * (centerOffset.y - segmentA.y);
            Vector2 tangent = t * dir + segmentA;
            float distToCenter = (tangent - centerOffset).magnitude;

            if (distToCenter <= radius)
            {
                return true;
            }
            else
            {
                return false;
            }
        }*/

        public override Color SampleLightAt(Vector2 pos)
        {
            pos = transform.InverseTransformPoint(pos);
            float dist = (pos - centerOffset).magnitude;
            return SampleColorAt(dist);
        }

        private Color SampleColorAt(float distanceFromCenter)
        {
            distanceFromCenter /= radius;
            if (squaredInterpolation)
            {
                distanceFromCenter = Mathf.Sqrt(distanceFromCenter);
            }
            return Color.Lerp(centerColor, edgeColor, distanceFromCenter);
        }

        void Awake()
        {
            radius = Mathf.Max(0.001f, radius);
            sqrRadius = radius * radius;
            transform.hasChanged = true;
        }

#if DEBUG_SHOW_GIZMOS
        void OnDrawGizmos()
        {
            Awake();
            if (!squaredInterpolation)
            {
                float stepSize = 0.5f / radius;
                for (float iRad = 0; iRad <= 1; iRad += stepSize)
                {
                    DebugExtension.DrawCircle(centerOffset, Vector3.forward, Color.Lerp(centerColor, edgeColor, iRad), transform.localToWorldMatrix, iRad * radius);
                }
            }
            else
            {
                float stepSize = 0.5f / radius;
                for (float iRad = 0; iRad <= 1; iRad += stepSize)
                {
                    DebugExtension.DrawCircle(centerOffset, Vector3.forward, Color.Lerp(centerColor, edgeColor, Mathf.Sqrt(iRad)), transform.localToWorldMatrix, iRad * radius);
                }
            }
            DebugExtension.DrawCircle(centerOffset, Vector3.forward, edgeColor, transform.localToWorldMatrix, radius);
            DebugExtension.DrawPoint(transform.TransformPoint(centerOffset), centerColor);
        }

        public override int[] RemoveAllCreatedObstructions()
        {
            int[] touchedNavNodes = new int[obstructionRecord.Count];
            for (int iRecord = 0; iRecord < obstructionRecord.Count; iRecord++)
            {
                touchedNavNodes[iRecord] = obstructionRecord[iRecord].navNodeIndex;
                obstructionRecord[iRecord].registeredVert.dynamicObstructions.Remove(obstructionRecord[iRecord].obstruction);
            }
            return touchedNavNodes;
        }

        public override void Visualize()
        {
            foreach (var rec in obstructionRecord)
            {
                rec.Visualize();
            }
        }

        public override DynamicObstruction RecordObstruction(int navNodeIndex, NavVert navVert, float start, float end)
        {
            DynamicObstruction obstr = new LightObstruction(start, end, pathfindingColor);
            obstructionRecord.Add(new LightObstructionRecord(obstr, navNodeIndex, navVert);
            return obstr;
        }

        public override bool FindIntersectionSpan(Vector2 segmentA, Vector2 segmentB, float expansionHeight, out float spanStart, out float spanEnd)
        {
            Vector2 expandedA = segmentA + Vector2.up * expansionHeight;
            Vector2 expandedB = segmentB + Vector2.up * expansionHeight;

            expandedA = transform.InverseTransformPoint(expandedA);
            expandedB = transform.InverseTransformPoint(expandedB);
            segmentA = transform.InverseTransformPoint(segmentA);
            segmentB = transform.InverseTransformPoint(segmentB);

            Vector2 ac = centerOffset - segmentA;
            Vector2 ab = segmentB - segmentA;
            Vector2 ad = expandedA - segmentA;

            float dotAcAb = Vector2.Dot(ac, ab);
            float dotAcAd = Vector2.Dot(ac, ad);

            //Check if center of circle is in rect
            if (0 <= dotAcAb && dotAcAb <= Vector2.Dot(ab, ab) && 0 <= dotAcAd && dotAcAd <= Vector2.Dot(ad, ad))
            {
                //Center is in rectangle
                if (ab.sqrMagnitude <= radius * radius)
                {//Doesn't matter where the circle is, will overlap complete edge

                }
            }
            else
            {//Check for line intersections

            }
        }
#endif
    }
}
