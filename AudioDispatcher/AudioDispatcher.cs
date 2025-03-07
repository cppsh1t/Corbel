using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Audio;
using System.Linq;

namespace Corbel.Builtin
{
    public abstract class AudioDispatcher
    {
        private readonly Dictionary<string, AudioOption> optionMap = new();
        private readonly HashSet<AudioClip> audioSet = new();
        protected AudioPool audioPool;
        private readonly AudioMixer audioMixer;

        public AudioDispatcher(AudioMixer audioMixer, int initialCount)
        {
            this.audioMixer = audioMixer;
            audioPool = new AudioPool(initialCount);
            audioPool.Preload();
        }

        public void AddOption(params AudioOption[] options)
        {
            for (int i = 0; i < options.Length; i++)
            {
                AudioOption option = options[i];
                if (!optionMap.TryAdd(option.Path, option))
                {
                    Debug.LogWarning($"Can't Add Option: {option.Path} because of already added");
                }
            }
        }

        public abstract AudioClip LoadAudioClip(string path);

        public virtual void InitAudioSource(AudioPlayer player, AudioClip audioClip, AudioOption option)
        {
            //Init by Option
            AudioSource audioSource = player.AudioSource;
            audioSource.clip = audioClip;
            audioSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups(option.AudioMixerName).FirstOrDefault()
                ?? audioMixer.FindMatchingGroups("Msater").First();

            audioSource.mute = option.Mute;
            audioSource.bypassEffects = option.BypassEffects;
            audioSource.bypassListenerEffects = option.BypassListenerEffects;
            audioSource.bypassReverbZones = option.BypassReverbZones;
            audioSource.playOnAwake = option.PlayOnAwake;
            audioSource.loop = option.Loop;
            audioSource.priority = option.Priority;
            audioSource.volume = option.Volume;
            audioSource.pitch = option.Pitch;
            audioSource.panStereo = option.PanStereo;
            audioSource.spatialBlend = option.SpatialBlend;
            audioSource.reverbZoneMix = option.ReverbZoneMix;
            audioSource.dopplerLevel = option.DopplerLevel;
            audioSource.spread = option.Spread;
            audioSource.rolloffMode = option.RolloffMode;
            audioSource.minDistance = option.MinDistance;
            audioSource.maxDistance = option.MaxDistance;
            audioSource.gamepadSpeakerOutputType = option.GamepadSpeakerOutputType;
            audioSource.ignoreListenerPause = option.IgnoreListenerPause;
            audioSource.ignoreListenerVolume = option.IgnoreListenerVolume;
            audioSource.spatialize = option.Spatialize;
            audioSource.spatializePostEffects = option.SpatializePostEffects;
            audioSource.timeSamples = option.TimeSamples;
            audioSource.velocityUpdateMode = option.VelocityUpdateMode;
            audioSource.SetTime(option.StartTime);
        }

        public AudioPlayer GetPlayer(string path, bool autoRecycle, bool onceOnly)
        {
            AudioClip audioClip = LoadAudioClip(path);
            if (audioClip == null)
            {
                Debug.LogWarning($"Can't Find audio which path: {path}");
                return null;
            }

            if (onceOnly)
            {
                bool exist = audioSet.Contains(audioClip);
                if (exist) return null;
            }

            AudioPlayer player = audioPool.Release();
            if (onceOnly)
            {
                player.checkOnlySet = audioSet;
                audioSet.Add(audioClip);
            }

            AudioOption option = optionMap.GetValueOrDefault(path, AudioOption.DefaultOption);
            player.autoRecycle = autoRecycle;
            InitAudioSource(player, audioClip, option);
            return player;
        }

        public void Play(string path, AudioToken token = null, bool onceOnly = false)
        {
            AudioPlayer player = GetPlayer(path, true, onceOnly);
            player?.Play(token);
        }

        public void PlayOnPosition(string path, Vector3 position, AudioToken token = null, bool onceOnly = false)
        {
            AudioPlayer player = GetPlayer(path, true, onceOnly);
            player?.PlayOnPosition(position, token);
        }

        public void PlayOnTransform(string path, Transform transform, AudioToken token = null, bool onceOnly = false)
        {
            AudioPlayer player = GetPlayer(path, true, onceOnly);
            player?.PlayOnTransform(transform, token);
        }

        public void PlayDelayed(string path, float delay, AudioToken token = null, bool onceOnly = false)
        {
            AudioPlayer player = GetPlayer(path, true, onceOnly);
            player?.PlayDelayed(delay, token);
        }

        public void PlayDelayedOnPosition(string path, float delay, Vector3 position, AudioToken token = null, bool onceOnly = false)
        {
            AudioPlayer player = GetPlayer(path, true, onceOnly);
            player?.PlayDelayedOnPosition(delay, position, token);
        }

        public void PlayDelayedOnTransform(string path, float delay, Transform transform, AudioToken token = null, bool onceOnly = false)
        {
            AudioPlayer player = GetPlayer(path, true, onceOnly);
            player?.PlayDelayedOnTransform(delay, transform, token);
        }
    }

    static class AudioSourceExtension
    {
        public static void SetTime(this AudioSource self, float time)
        {
            self.time = time;
        }
    }
}