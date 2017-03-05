using UnityEngine;
using System.Collections;
using NavData2d;
using Entities;
using System;

namespace LightSensing
{
    public class LightObstruction : DynamicObstruction
    {
        Color lightColor;

        public LightObstruction(float start, float end, Color lightColor)
        {
            this.lightColor = lightColor;
            this.startPositionAlongPath = start;
            this.endPositionAlongPath = end;
        }

        public override bool IsObstructing(AIEntity aiEntity)
        {
            throw new NotImplementedException();
        }
    }

    internal class LightObstructionRecord
    {
        public DynamicObstruction obstruction;
        public NavVert registeredVert;
        public int navNodeIndex;

        public LightObstructionRecord(DynamicObstruction obstruction, int navNodeIndex, NavVert navVert)
        {
            this.navNodeIndex = navNodeIndex;
            this.obstruction = obstruction;
            this.registeredVert = navVert;
        }

        public void Visualize()
        {
            Debug.DrawLine(registeredVert.PointB + registeredVert.edgeDir * obstruction.startPositionAlongPath,
                registeredVert.PointB + registeredVert.edgeDir * obstruction.endPositionAlongPath);
        }
    }
}
