using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Corbel.AgentBehaviour
{
    public abstract class State<T>
    {
        internal IStateChanger stateChanger;
        protected T owner;
        internal virtual void SetOwner(T owner)
        {
            this.owner = owner;
        }
        public abstract string Name { get; }
        public abstract void Enter();
        public abstract void LogicLoop();
        public abstract void PhysicsLoop();
        public abstract void Exit();

        internal virtual void Init()
        {
            
        }
    }

    
    public abstract class BehaviourState<T> : State<T> where T : MonoBehaviour
    {
        protected Transform transform;

        internal override void Init()
        {
            transform = owner.transform;
        }
    } 
   

    
    public abstract class ActorState<T> : BehaviourState<T> where T : Actor
    {
        protected PropertyModel propertyModel;

        internal override void Init()
        {
            base.Init();
            propertyModel = owner.GetPropertyModel();
        }
    }
    

    public class HierarchyState<T> : State<T>, IStateChanger
    {
        public State<T> CurrentState { get; protected set; }
        public State<T> PreviousState { get; private set; }

        public override string Name => name;
        private readonly string name;

        private readonly Dictionary<string, State<T>> stateMap = new();

        public HierarchyState(string name) => this.name = name;

        public HierarchyState(string name, params State<T>[] states) : this(name)
        {
            AddState(states);
        }

        internal override void SetOwner(T owner)
        {
            State<T>[] states = stateMap.Values.ToArray();
            for (int i = 0; i < states.Length; i++)
            {
                State<T> state = states[i];
                state.SetOwner(owner);
            }
        }

        public void Init(State<T> currentState, State<T> previousState = null)
        {
            CurrentState = currentState;
            previousState ??= new EmptyState<T>();
            PreviousState = previousState;
        }

        public void AddState(params State<T>[] states)
        {
            for (int i = 0; i < states.Length; i++)
            {
                State<T> state = states[i];
                bool addSuccess = stateMap.TryAdd(state.Name, state);
                if (addSuccess)
                {
                    state.stateChanger = this;
                }
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

        public void ChangeStateOnFather(string name)
        {
            stateChanger.ChangeState(name);
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

        public override void Enter()
        {
            CurrentState.Enter();
        }

        public override void LogicLoop()
        {
            CurrentState.LogicLoop();
        }

        public override void PhysicsLoop()
        {
            CurrentState.PhysicsLoop();
        }

        public override void Exit()
        {
            CurrentState.Exit();
        }
    }


    public class EmptyState<T> : State<T>
    {
        public override string Name => "empty";

        public override void Enter()
        {

        }

        public override void Exit()
        {

        }

        public override void LogicLoop()
        {

        }

        public override void PhysicsLoop()
        {

        }
    }
}