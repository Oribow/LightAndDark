using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NavData2d
{
    public class DynamicObstructionHandler : MonoBehaviour
    {
        static DynamicObstructionHandler instance;
        DynamicObstructionHandler Instance { get { return instance; } }

        [SerializeField]
        NavigationData2D navData;
        [SerializeField]
        float navVertExtensionHeight;

        List<IDynamicObstructor> registeredObstructors;

        void Awake()
        {
            instance = this;
        }

        public void AddObstructor(IDynamicObstructor dynamicObstructor)
        {
            registeredObstructors.Add(dynamicObstructor);
        }

        public void RemoveObstructor(IDynamicObstructor dynamicObstructor)
        {
            dynamicObstructor.RemoveAllCreatedObstructions();
            registeredObstructors.Remove(dynamicObstructor);
        }

        void Update()
        {
            for (int iObstructor = 0; iObstructor < registeredObstructors.Count; iObstructor++)
            {
                if (registeredObstructors[iObstructor] == null)
                {
                    registeredObstructors.RemoveAt(iObstructor);
                    iObstructor--;
                    Debug.LogError("Registered Obstruction disappeared (became null). Remove them before destroying them. Will have dead obstructions on the navdata now.");
                }
                else if (registeredObstructors[iObstructor].NeedsUpdateFlag)
                {
                    RemapObstructor(registeredObstructors[iObstructor]);
                    registeredObstructors[iObstructor].NeedsUpdateFlag = false;
                }
#if UNITY_EDITOR
                registeredObstructors[iObstructor].Visualize();
#endif
            }
        }

        void RemapObstructor(IDynamicObstructor obstructor)
        {
            List<int> previouslyAffectedNodes = obstructor.RemoveAllCreatedObstructions();
            NavNode cNode;
            foreach (int pNode in previouslyAffectedNodes)
            {
                cNode = navData.nodes[pNode];

                if (!cNode.bounds.Intersects(obstructor.Bounds))
                    continue;

                RemapObstructor(obstructor, cNode, pNode);
            }

            for (int iNode = 0; iNode < navData.nodes.Length; iNode++)
            {
                foreach (int pNode in previouslyAffectedNodes)
                {
                    if (pNode == iNode)
                        goto NextNode;
                }
                cNode = navData.nodes[iNode];

                if (!cNode.bounds.Intersects(obstructor.Bounds))
                    continue;

                RemapObstructor(obstructor, cNode, iNode);

                NextNode:
                continue;
            }
        }

        void RemapObstructor(IDynamicObstructor obstructor, NavNode node, int navNodeIndex)
        {
            for (int iVert = 0; iVert < node.verts.Length - 1; iVert++)
            {
                float spanStart;
                float spanEnd;

                if (obstructor.FindIntersectionSpan(node.verts[iVert].PointB, node.verts[iVert + 1].PointB, navVertExtensionHeight, out spanStart, out spanEnd))
                {
                    DynamicObstruction dyObstr = obstructor.RecordObstruction(navNodeIndex, node.verts[iVert], spanStart, spanEnd);
                    node.verts[iVert].AddDynamicObstruction(dyObstr);
                }
            }
        }
    }
}
