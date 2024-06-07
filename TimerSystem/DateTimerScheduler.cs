using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Corbel.Builtin
{

    public class DateSchedulerBehaviour : TimerSchedulerBehaviour
    {
        private static readonly DateTime Start = new Lazy<DateTime>(() => DateTime.UtcNow).Value;

        public override void InitScheduler(ITimerScheduler scheduler)
        {
            this.scheduler = scheduler;
            scheduler.CalibrateTime(Time.time, GetTime());
        }

        protected override void OnUpdate()
        {
            scheduler.CalibrateTime(Time.time, GetTime());
            scheduler.Tick();
        }

        public static float GetTime()
        {
            float seconds = (float)(DateTime.UtcNow - Start).TotalSeconds;
            return seconds;
        }
    }
}
