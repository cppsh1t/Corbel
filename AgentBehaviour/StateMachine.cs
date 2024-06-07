using UnityEngine;
using Corbel.Extension;
using System.Linq;
using System;
using System.Collections.Generic;

namespace Corbel.AgentBehaviour
{
    public interface IStateChanger
    {
        void ChangeState(string name);
        void RevertToPreviousState();
    }

    public class StateMachine<T> : IStateChanger
    {
        public State<T> CurrentState { get; protected set; }
        public State<T> PreviousState { get; private set; }
        private readonly Dictionary<string, State<T>> stateMap = new();

        public StateMachine() { }

        public StateMachine(params State<T>[] states)
        {
            AddState(states);
        }


        public void Enable()
        {
            CurrentState.Enter();
        }

        public void Update()
        {
            CurrentState.LogicLoop();
        }

        public void FixedUpdate()
        {
            CurrentState.PhysicsLoop();
        }

        public void Init(T owner, State<T> currentState, State<T> previousState = null)
        {
            CurrentState = currentState;
            previousState ??= new EmptyState<T>();
            PreviousState = previousState;

            State<T>[] states = stateMap.Values.ToArray();
            for (int i = 0; i < states.Length; i++)
            {
                State<T> state = states[i];
                state.stateChanger = this;
                state.SetOwner(owner);
                state.Init();
            }
        }

        public void AddState(params State<T>[] states)
        {
            for (int i = 0; i < states.Length; i++)
            {
                State<T> state = states[i];
                stateMap.TryAdd(state.Name, state);
            }
        }

        public void ChangeState(string name)
        {
            if (stateMap.ContainsKey(name))
            {
                State<T> newState = stateMap[name];
                ChangeState(newState);
            }
            else
            {
                
                Debug.LogWarning($"{name} state not found!");
                
            }
        }

        private void ChangeState(State<T> newState)
        {
            CurrentState.Exit();
            PreviousState = CurrentState;
            CurrentState = newState;
            CurrentState.Enter();
        }

        public void RevertToPreviousState()
        {
            ChangeState(PreviousState);
        }

        public bool IsInState(string name)
        {
            return CurrentState.Name == name;
        }

    }

}
