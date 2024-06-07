using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Corbel.Builtin
{
    public interface ITimerSystem
    {
        public void AddTask(TimerTask task);
    }

    public class TimerSystem : ITimerSystem
    {
        protected ITimerScheduler scheduler;
        protected TimerSchedulerBehaviour behaviour;

        protected TimerSystem(ITimerScheduler scheduler, TimerSchedulerBehaviour behaviour)
        {
            this.scheduler = scheduler;
            this.behaviour = behaviour;
            behaviour.InitScheduler(scheduler);
        }

        public void AddTask(TimerTask task)
        {
            scheduler.AddTask(task);
        }

        public static ITimerSystem CreateTimerScheduler<TScheduler, TBehaviour>() where TScheduler : ITimerScheduler, new() where TBehaviour : TimerSchedulerBehaviour
        {
            GameObject gameObject = new("TimerSystem");
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            TimerSchedulerBehaviour behaviour = gameObject.AddComponent<TBehaviour>();
            ITimerScheduler scheduler = new TScheduler();
            return new TimerSystem(scheduler, behaviour);
        }

        public static ITimerSystem CreateTimerScheduler(Type schedulerType = null, Type behaviourType = null)
        {
            schedulerType ??= typeof(TimerScheduler);
            behaviourType ??= typeof(TimerSchedulerBehaviour);
            
            GameObject gameObject = new("TimerSystem");
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            TimerSchedulerBehaviour behaviour = gameObject.AddComponent(behaviourType) as TimerSchedulerBehaviour;
            ITimerScheduler scheduler = Activator.CreateInstance(schedulerType) as ITimerScheduler;
            return new TimerSystem(scheduler, behaviour);
        }

    }
}
