﻿using UnityEngine;
using System.Collections;
using System;
using Entities;

namespace Manager
{
    public class GamePlayStateHandler : MonoBehaviour, IGameState
    {    
        [AssignEntityAutomaticly]
        GameStateEntity actor;

        public GameStateType StateType
        {
            get
            {
                return GameStateType.Play;
            }
        }

        void Start()
        {
            GameStateManager.Instance.AssignDefaultState(this);
        }

        public void OnStateActive()
        {
            Debug.Log("act");
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("esc");
                GameStateManager.Instance.StartNewState(actor.PauseMenuStateHandler);
            }
        }

        public void OnStateStart()
        {
            //Do nothing, cause this is the default state. Every other state will revert to this state when it loses focus.
        }

        public void OnStateEnd()
        {
            //Do nothing, cause this is the default state. Every other state will revert to this state when it loses focus.
        }

        public override string ToString()
        {
            return "GamePlayState";
        }
    }
}
