using System.Collections.Generic;
using UnityEngine;
namespace LastKey
{
    // Reused free CC0 cues, with synthesised fallback for missing optional cues.
    public sealed class GameAudio:MonoBehaviour
    {
        public event System.Action<AudioClip> Emitted;
        public event System.Action<string> CuePlayed;
        public AudioClip Ambience {get{return clips["menu"];}}
        AudioSource effects,ambient,ui;
        public void Route(MusicManager music){effects.outputAudioMixerGroup=music.Group("SFX");ambient.outputAudioMixerGroup=music.Group("AMBIENCE");ui.outputAudioMixerGroup=music.Group("UI");}
        readonly Dictionary<string,AudioClip> clips=new Dictionary<string,AudioClip>();
        public void Initialize()
        {
            effects=gameObject.AddComponent<AudioSource>();effects.volume=.32f;
            ui=gameObject.AddComponent<AudioSource>();ui.volume=.32f;
            ambient=gameObject.AddComponent<AudioSource>();ambient.loop=true;ambient.volume=.09f;
            var cues=new[]{"typing","error","combo","warning","null","echo","elimination","round","victory","menu","ui"};
            for(int i=0;i<cues.Length;i++)clips[cues[i]]=Synth(cues[i],i==9?4:i==8?1.2f:i==6?.7f:.12f+i*.03f,130+i*65);
            foreach(var name in cues){var imported=Resources.Load<AudioClip>("Audio/VisualSfx/"+name);if(imported!=null)clips[name]=imported;}
            ambient.clip=clips["menu"];ambient.Play();
        }
        AudioClip Synth(string name,float duration,float frequency)
        {
            const int rate=44100;var samples=new float[Mathf.CeilToInt(duration*rate)];
            for(int i=0;i<samples.Length;i++)
            {
                float t=i/(float)rate,env=Mathf.Min(1,t*100)*Mathf.Clamp01((duration-t)*8);
                float f=name=="elimination"?frequency*(1-t/duration):name=="victory"?frequency*Mathf.Pow(2,Mathf.Floor(t*6)/12):frequency;
                samples[i]=name=="menu"?(Mathf.Sin(t*2*Mathf.PI*55)*.1f+Mathf.Sin(t*2*Mathf.PI*82.5f)*.08f):(Mathf.Sin(t*2*Mathf.PI*f)*.22f+Mathf.Sin(t*2*Mathf.PI*f*2.01f)*.06f)*env;
            }
            var clip=AudioClip.Create(name,samples.Length,1,rate,false);clip.SetData(samples,0);return clip;
        }
        public void Cue(string name){if(effects!=null&&clips.ContainsKey(name)){(name=="ui"?ui:effects).PlayOneShot(clips[name],name=="typing"?.8f:1.3f);Emitted?.Invoke(clips[name]);CuePlayed?.Invoke(name);}}
        public void StopSessionAudio(){foreach(var source in new[]{effects,ambient,ui})if(source!=null)source.Stop();}
        void OnApplicationQuit(){StopSessionAudio();}
        public void Menu(bool active){if(ambient!=null)ambient.volume=active?.09f:.025f;}
    }
}
