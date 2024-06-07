using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Corbel.Builtin
{
    public class TimerSchedulerBehaviour : MonoBehaviour
    {
        protected ITimerScheduler scheduler;

        public virtual void InitScheduler(ITimerScheduler scheduler)
        {
            this.scheduler = scheduler;
            scheduler.CalibrateTime(Time.time, Time.realtimeSinceStartup);
        }

        protected virtual void OnUpdate()
        {
            scheduler.CalibrateTime(Time.time, Time.realtimeSinceStartup);
            scheduler.Tick();
        }

        void Update()
        {
            OnUpdate();
        }
    }
}
