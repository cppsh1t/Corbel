using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Corbel.Builtin
{

    public static class TimerSchedulerExtension
    {
        public static void AddTask(this ITimerSystem self, params TimerTask[] tasks)
        {
            for (int i = 0; i < tasks.Length; i++)
            {
                self.AddTask(tasks[i]);
            }
        }
    }

    public interface ITimerScheduler
    {
        void CalibrateTime(float currentEngineTime, float currentRealTime);
        void AddTask(TimerTask task);
        void Tick();
    }

    public class TimerScheduler : ITimerScheduler
    {
        protected readonly PriorityQueue<TimerTask, float> engineTimeQueue = new();
        protected readonly PriorityQueue<TimerTask, float> realTimeQueue = new();
        protected float currentEngineTime;
        protected float currentRealTime;

        public void AddTask(TimerTask task)
        {
            if (task.InRealTime)
            {
                float targetTime = task.Duration + currentRealTime;
                realTimeQueue.Enqueue(task, targetTime);
            }
            else
            {
                float targetTime = task.Duration + currentEngineTime;
                engineTimeQueue.Enqueue(task, targetTime);
            }
        }

        public void CalibrateTime(float currentEngineTime, float currentRealTime)
        {
            this.currentEngineTime = currentEngineTime;
            this.currentRealTime = currentRealTime;
        }

        public void Tick()
        {
            DoTickOnTargetQueue(engineTimeQueue, currentEngineTime);
            DoTickOnTargetQueue(realTimeQueue, currentRealTime);
        }

        protected void DoTickOnTargetQueue(PriorityQueue<TimerTask, float> targetQueue, float currentTime)
        {
            if (targetQueue.TryPeek(out TimerTask _, out float targetTime))
            {
                if (currentTime >= targetTime)
                {
                    TimerTask task = targetQueue.Dequeue();
                    task.TargetCallback.Invoke();

                    if (task.TaskType == TaskType.Repeat && !task.Canceled)
                    {
                        AddTask(task);
                    }
                }
            }
        }
    }
}
