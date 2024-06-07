using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Corbel.Builtin
{
    public enum TaskType
    {
        Once,
        Repeat,
    }

    public class TimerTask
    {
        public float Duration { get; private set; }
        public bool Canceled { get; private set; } = false;
        public bool InRealTime { get; private set; } = false;
        public TaskType TaskType { get; private set; }

        private readonly Action Callback;
        private readonly Action CancelCallback;

        public Action TargetCallback => Canceled ? CancelCallback : Callback;

        public TimerTask(float duration, Action callback, Action cancelCallback = null, bool inRealTime = false, TaskType taskType = TaskType.Once)
        {
            Duration = duration;
            InRealTime = inRealTime;
            TaskType = taskType;
            Callback += callback;
            cancelCallback ??= () => { };
            CancelCallback += cancelCallback;
        }

        public void Cancel()
        {
            Canceled = true;
        }

        public void Enable()
        {
            Canceled = false;
        }
    }
}
