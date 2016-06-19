﻿using UnityEngine;

namespace FakePhysics
{
    [RequireComponent(typeof(PositionHolder2D))]
    public class MovingPlattform : MonoBehaviour
    {
        enum MovemenType {
            OneShot,
            Loop,
            PingPong
        }
        [SerializeField]
        float timeForOneTravel;

        //After the platform arrives at the last platform, it will stop moving.
        [SerializeField]
        MovemenType movemenType;

        [SerializeField]
        float targetRadius = 0.5f;

        new Rigidbody2D rigidbody;
        float plattformSpeed;
        int nextPointToReach;
        //If set to false, the platform will not move.
        bool shouldMove = true;
        bool backwards;
        float targetRadiusSqr;
        Vector3 velocity;
        PositionHolder2D posHolder;

        void Awake()
        {
            posHolder = GetComponent<PositionHolder2D>();
            rigidbody = GetComponent<Rigidbody2D>();

            //Square the targetrad, to avoid root calculation later
            targetRadiusSqr = targetRadius * targetRadius;

            //Calc the platform speed
            plattformSpeed = CalcTotalTravelLength() / timeForOneTravel;
        }

        float CalcTotalTravelLength()
        {
            float totalDistance = 0;
            for (int i = 0; i < posHolder.positions.Count - 1; i++)
                totalDistance += Vector3.Distance(posHolder.positions[i], posHolder.positions[i + 1]);
            return totalDistance;
        }

        void FixedUpdate()
        {
            if (!shouldMove)
                return;

            //Calc the direction
            velocity = (Vector3)posHolder.positions[nextPointToReach] - transform.position;

            //Did we arrive at our target?
            if (velocity.sqrMagnitude < targetRadiusSqr)
            {
                AdvancePointCycle();
            }

            //We didn't, so move on
            velocity.Normalize();
            rigidbody.velocity = velocity * plattformSpeed;
        }

        void AdvancePointCycle()
        {
            if (backwards)
                nextPointToReach--;
            else
                nextPointToReach++;

            //Finished a complete travel to all points
            if (nextPointToReach == posHolder.positions.Count)
            {
                if (movemenType == MovemenType.PingPong)
                {
                    backwards = true;
                    nextPointToReach = posHolder.positions.Count - 2;
                }
                else if (movemenType == MovemenType.OneShot)
                    shouldMove = false;
                else
                    nextPointToReach = 0;
            }
            else if (nextPointToReach == -1)
            {
                nextPointToReach = 1;
                backwards = false;
            }
        }
    }
}
