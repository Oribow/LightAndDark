using UnityEngine;
using Pada1.BBCore.Framework;
using System;
using Pada1.BBCore;
using Entities;
using System.Collections.Generic;
using AI.Brain;
using System.Collections;

namespace BehaviorBrick.Conditions
{
    [Condition("Perception/IsNPCInSight")]
    [Help("Checks whether an npc with a certain relationship is in sight.")]
    public class IsNPCInSight : ConditionBase
    {
        enum RequiredRelationship { Friendly, Hostile, Neutral, Any }

        [InParam("ActingEntity")]
        [Help("Entity that performs the check")]
        ActingEntity entity;

        [InParam("Eye")]
        [Help("The position from which the ray will be casted!")]
        Transform rayOrigin; // eyes

        [InParam("Eye Range")]
        [Help("How far the eye can look.")]
        float sightRange = 10;

        [InParam("Required Relationship")]
        [Help("How far the eye can look.")]
        RequiredRelationship relationship = RequiredRelationship.Hostile;

        [OutParam("Visible AIs")]
        [Help("List of detected npcs")]
        ResizableList<VisibleNPCs> visibleNpcs;

        List<Entity> queueryResultBuffer;
        EntityDatabase actorDatabase;

        public IsNPCInSight()
        {
            queueryResultBuffer = new List<Entity>(5);
            visibleNpcs = new ResizableList<VisibleNPCs>(5, 7);
            actorDatabase = EntityDatabase.GetInstance();
            
        }

        public override bool Check()
        {
            queueryResultBuffer.Clear();
            visibleNpcs.Clear();
            actorDatabase.Find<ActingEntity>(ref queueryResultBuffer);
            float sightRangeSquared = sightRange * sightRange;
            ActingEntity ae;
            foreach (var e in queueryResultBuffer)
            {
                if (e == this.entity)
                    continue;
                ae = (ActingEntity)e;
                if (!HasRequiredRelationship(entity.RelationshipMarker.GetRelationship(ae)))
                    continue;
                if (((Vector2)(e.transform.position - rayOrigin.position)).sqrMagnitude > sightRangeSquared)
                {
                    continue;
                }

                RaycastHit2D hit = Physics2D.Raycast(rayOrigin.position, e.transform.position - rayOrigin.position, sightRange);
                if (hit && hit.collider.gameObject == e.gameObject)
                {
                    visibleNpcs.Add(
                        (object[] args) => { return new VisibleNPCs((ActingEntity)args[0], (float)args[1]); },
                        (VisibleNPCs oldObj, object[] args) => { oldObj.distance = (float)args[1]; oldObj.otherEntity = (ActingEntity)args[0]; },
                        e, Vector2.Distance(rayOrigin.position, e.transform.position));
                }
            }
            return visibleNpcs.Count > 0;
        }

        bool HasRequiredRelationship(RelationshipType rel)
        {
            if (relationship == RequiredRelationship.Any)
                return true;
            switch (rel)
            {
                case RelationshipType.Friendly: return relationship == RequiredRelationship.Friendly;
                case RelationshipType.Hostile: return relationship == RequiredRelationship.Hostile;
                case RelationshipType.Neutral: return relationship == RequiredRelationship.Neutral;
            }
            return false;
        }

        public class VisibleNPCs
        {
            public ActingEntity otherEntity;
            public float distance;

            public VisibleNPCs(ActingEntity otherEntity, float distance)
            {
                this.otherEntity = otherEntity;
                this.distance = distance;
            }
        }

        public class ResizableList<T> : IEnumerable<T>
        {
            public delegate T ConstructNewObj (object[] args);
            public delegate void OverwriteOld (T oldObj, object[] args);

            List<T> _data;
            int fillCount;
            int upperLimit;

            public ResizableList(int capacity, int upperLimit)
            {
                _data = new List<T>(capacity);
                fillCount = 0;
                this.upperLimit = upperLimit;
            }

            public T this[int index]
            {
                get { return _data[index]; }
                set { _data[index] = value; }
            }

            public int Count { get { return fillCount; } }

            public void Add (ConstructNewObj ConstructNew, OverwriteOld overwriteOld, params object[] args)
            {
                if (fillCount >= _data.Count)
                    _data.Add(ConstructNew(args));
                else
                {
                    overwriteOld(_data[fillCount], args);
                }
                fillCount++;
            }

            public void Remove(T obj)
            {
                _data.Remove(obj);
                fillCount--;
            }

            public void Clear()
            {
                fillCount = 0;
                if (_data.Capacity > upperLimit)
                    _data.Capacity = upperLimit;
            }

            public IEnumerator<T> GetEnumerator()
            {
                return new ResizableListEnumerator(this);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new ResizableListEnumerator(this);
            }



            class ResizableListEnumerator : IEnumerator<T>
            {
                public T Current
                {
                    get
                    {
                        return src._data[index];
                    }
                }

                object IEnumerator.Current
                {
                    get
                    {
                        return src._data[index];
                    }
                }

                ResizableList<T> src;
                int index;

                public ResizableListEnumerator(ResizableList<T> src)
                {
                    this.src = src;
                    index = -1;
                }

                public void Dispose()
                {
                    src = null;
                }

                public bool MoveNext()
                {
                    if (index < src.fillCount - 1)
                    {
                        index++;
                        return true;
                    }
                    return false;
                }

                public void Reset()
                {
                    index = 0;
                }
            }
        }
    }
}
