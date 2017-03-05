using UnityEngine;
using System.Collections;
using Pada1.BBCore;
using Entities;
using Pada1.BBCore.Framework;
using Pada1.BBCore.Tasks;
using LightSensing;
using System.Collections.Generic;

namespace BBUnity.Actions
{
    [Action("AIInteraction/FleeFromLight")]
    [Help("Moves in the oposite direction of the given lightsource")]
    public class FleeFromLight : BasePrimitiveAction
    {
        [InParam("AIEntity")]
        [Help("AIEntity to move")]
        public AIEntity entity;
        [InParam("TargetDistanceFromThreats")]
        [Help("After this distance is reached no further path will be calculated.")]
        float targetDistanceFromThreat;

        [InParam("LightSource")]
        [Help("The lightsource to run away from.")]
        List<LightMarker> marker;

        bool pathIsFound;
        Vector2[] threats;
        LightMarker[] markerArray;

        public override void OnStart()
        {
            if (marker == null || marker.Count == 0)
            {
                Debug.Log("Marker = null -> shouldn't happen some ref failure");
                marker = new List<LightMarker>();
                GlobalLightSensor.Instance.GetDynamicLightAt(entity.transform.position, ref marker);
                if (marker.Count == 0)
                {
                    pathIsFound = false;
                    return;
                }
            }
            InitPath();
        }

        private void OnPathComputationFinished(bool foundPath)
        {
            pathIsFound = foundPath;
        }

        public override TaskStatus OnUpdate()
        {
            if (!pathIsFound)
                return TaskStatus.FAILED;

            if (!entity.NavAgent.IsFollowingAPath)
            {
                entity.NavAgent.FleeFrom(threats, targetDistanceFromThreat, new NavAgent.OnPathComputationFinished(OnPathComputationFinished), true, markerArray);
                pathIsFound = true;
            }
            
            return TaskStatus.RUNNING;
        }

        public override void OnAbort()
        {
            entity.NavAgent.Stop();
        }

        void InitPath()
        {
            markerArray = marker.ToArray();
            threats = new Vector2[markerArray.Length];
            for (int i = 0; i < markerArray.Length; i++)
                threats[i] = marker[i].Center;

            entity.NavAgent.FleeFrom(threats, targetDistanceFromThreat, new NavAgent.OnPathComputationFinished(OnPathComputationFinished), true, markerArray);
            pathIsFound = true;
        }
    }
}
