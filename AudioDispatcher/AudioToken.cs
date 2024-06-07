using System.Collections.Generic;
using System;
using UnityEngine;

namespace Corbel.Builtin
{
    public class AudioToken
    {
        public bool Canceled { get; private set; } = false;

        public void Cancel()
        {
            Canceled = true;
        }

        public void Restart()
        {
            Canceled = false;
        }
    }
}
