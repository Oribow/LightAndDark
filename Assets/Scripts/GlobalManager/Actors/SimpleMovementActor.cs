﻿using UnityEngine;
using System.Collections;
using CC2D;

namespace Actors
{
    public class SimpleMovementActor : Actor
    {
        [SerializeField]
        CharacterController2D characterController2D;
        [SerializeField]
        Rigidbody2D rigidbody2d;
        [SerializeField]
        BoxCollider2D boxCollider2D;

        public Rigidbody2D Rigidbody2D { get { return rigidbody2d; } }
        public BoxCollider2D BoxCollider2D { get { return boxCollider2D; } }
        public CharacterController2D CharacterController2D { get { return characterController2D; } }

#if UNITY_EDITOR
        public override void Refresh()
        {
            base.Refresh();

            characterController2D = GetComponentInChildren<CharacterController2D>();
            rigidbody2d = GetComponentInChildren<Rigidbody2D>();
            boxCollider2D = GetComponentInChildren<BoxCollider2D>();

            rigidbody2d.isKinematic = true;
        }
#endif
    }
}