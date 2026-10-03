using UnityEngine;
using UnityEngine.Audio;
namespace LastKey
{
    public sealed class MusicLibrary:ScriptableObject
    {
        public AudioMixer Mixer;
        public AudioClip IronDominion,DeadCurrent,Mainframe;
    }
}
