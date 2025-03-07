using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Corbel.Builtin
{
    public class ResourcesAudioDispatcher : AudioDispatcher
    {
        public ResourcesAudioDispatcher(AudioMixer audioMixer, int initialCount) : base(audioMixer, initialCount)
        {
        }

        public override AudioClip LoadAudioClip(string path)
        {
            return Resources.Load<AudioClip>(path);
        }
    }
}
