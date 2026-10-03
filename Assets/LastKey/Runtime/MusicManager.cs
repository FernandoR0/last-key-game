using System;
using UnityEngine;
using UnityEngine.Audio;
namespace LastKey
{
    public enum MusicState {MENU,CHARACTER_SELECT,WAKE_UP,ROUND_PREPARE,GAMEPLAY,HIGH_DANGER,ROUND_RESULT,FINAL_RESULT,VICTORY}
    // Provisional musical segments. Auditory acceptance is documented separately from runtime QA.
    public sealed class MusicManager:MonoBehaviour
    {
        public const float GameplayOffset=16f;
        public const float IntroSeconds=GameplayOffset-3f;
        public MusicState State {get;private set;}
        public float Volume {get;private set;}
        public string TrackName=>sources==null||sources[active].clip==null?"":sources[active].clip.name;
        public float TrackTime=>sources==null?0:sources[active].time;
        public int DangerTier {get;private set;}
        LastKeyGame host;MusicLibrary library;AudioSource[] sources;
        int active;float loopStart,loopEnd,fadeStart,fadeSeconds=1,duckUntil,duckDb;
        bool fading,paused,ready;float[] gains={0,0};
        public void Initialize(LastKeyGame game)
        {
            host=game;library=Resources.Load<MusicLibrary>("Audio/MusicLibrary");
            if(library==null)throw new InvalidOperationException("Music library not prepared.");
            Volume=Mathf.Clamp01(PlayerPrefs.GetFloat("LAST_KEY_MUSIC_VOLUME",.7f));
            sources=new AudioSource[2];var group=library.Mixer.FindMatchingGroups("MUSIC")[0];
            for(int i=0;i<2;i++){var go=new GameObject("MusicSource"+(i==0?"A":"B"));go.transform.SetParent(transform);sources[i]=go.AddComponent<AudioSource>();sources[i].playOnAwake=false;sources[i].loop=false;sources[i].outputAudioMixerGroup=group;sources[i].volume=0;}
            host.Sound.CuePlayed+=OnCue;ready=true;SetFlow(ExperienceState.MainMenu,false);
        }
        public AudioMixerGroup Group(string name)=>library.Mixer.FindMatchingGroups(name)[0];
        public void SetVolume(float value){Volume=Mathf.Clamp01(value);PlayerPrefs.SetFloat("LAST_KEY_MUSIC_VOLUME",Volume);PlayerPrefs.Save();}
        public void SetFlow(ExperienceState flow,bool resume)
        {
            if(!ready)return;
            paused=flow==ExperienceState.Paused;
            if(paused||resume)return;
            DangerTier=0;
            switch(flow)
            {
                case ExperienceState.RoundIntro:State=MusicState.WAKE_UP;Begin(library.IronDominion,0,16,60,1);break;
                case ExperienceState.Countdown:break; // Continue the soft opening; TYPE arrives at the musical entry.
                case ExperienceState.Gameplay:State=MusicState.GAMEPLAY;if(TrackName!=library.IronDominion.name||TrackTime<GameplayOffset-.2f)Begin(library.IronDominion,GameplayOffset,16,60,.5f);break;
                case ExperienceState.RoundResult:State=MusicState.ROUND_RESULT;Change(library.Mainframe,0,0,16,1);break;
                case ExperienceState.FinalResult:State=MusicState.VICTORY;Change(library.IronDominion,40,40,60,1.2f);break;
                case ExperienceState.CharacterSelect:State=MusicState.CHARACTER_SELECT;Change(library.Mainframe,0,0,16,1);break;
                case ExperienceState.MatchSetup:State=MusicState.ROUND_PREPARE;Change(library.Mainframe,0,0,16,1);break;
                case ExperienceState.Boot:case ExperienceState.Loading:case ExperienceState.MainMenu:case ExperienceState.ModeSelect:State=MusicState.MENU;Change(library.Mainframe,0,0,16,1);break;
            }
        }
        void OnCue(string cue)
        {
            if(cue=="null"||cue=="echo")Duck(4,1.1f);
            else if(cue=="elimination")Duck(8,1.5f);
            else if(cue=="warning")Duck(3,.8f);
        }
        public void Duck(float db,float seconds){duckDb=Mathf.Max(db,Time.time<duckUntil?duckDb:0);duckUntil=Mathf.Max(duckUntil,Time.time+seconds);}
        void Change(AudioClip clip,float offset,float start,float end,float seconds)
        {
            // Re-selecting a menu or preview keeps its musical position.
            if(sources[active].clip==clip&&Mathf.Approximately(loopStart,start)&&Mathf.Approximately(loopEnd,end)&&sources[active].isPlaying)return;
            Begin(clip,offset,start,end,seconds);
        }
        void Begin(AudioClip clip,float offset,float start,float end,float seconds)
        {
            int next=1-active;sources[next].Stop();sources[next].clip=clip;
            sources[next].Play();sources[next].timeSamples=Mathf.Clamp(Mathf.RoundToInt(offset*clip.frequency),0,clip.samples-1);
            sources[next].volume=0;active=next;loopStart=start;loopEnd=Mathf.Min(end,clip.length-.05f);
            fadeStart=Time.time;fadeSeconds=seconds;fading=true;
            Debug.Log("MUSIC_TRANSITION "+State+" track="+clip.name+" offset="+offset+" loop="+loopStart+".."+loopEnd);
        }
        void Update()
        {
            if(!ready)return;
            if(host.Flow==ExperienceState.Gameplay)
            {
                float danger=host.Game.Players[host.HumanIndex].Danger;
                int next=DangerTier==2?(danger<75?(danger>=50?1:0):2):DangerTier==1?(danger>=80?2:danger<45?0:1):(danger>=80?2:danger>=50?1:0);
                if(next!=DangerTier){DangerTier=next;State=next==0?MusicState.GAMEPLAY:MusicState.HIGH_DANGER;
                    if(next==0)Change(library.IronDominion,16,16,60,1);
                    else Change(library.DeadCurrent,next==2?28:16,next==2?28:16,48,1.2f);}
            }
            if(!fading&&sources[active].clip!=null&&sources[active].time>=loopEnd-fadeSeconds)Begin(sources[active].clip,loopStart,loopStart,loopEnd,1);
            float t=Mathf.Clamp01((Time.time-fadeStart)/fadeSeconds);
            gains[active]=fading?Mathf.Sin(t*Mathf.PI*.5f):1;gains[1-active]=fading?Mathf.Cos(t*Mathf.PI*.5f):0;
            if(fading&&t>=1){fading=false;sources[1-active].Stop();}
            float duck=Time.time<duckUntil?Mathf.Pow(10,-duckDb/20):1;
            float level=Volume*.45f*(paused?.45f:1)*duck;
            for(int i=0;i<2;i++)sources[i].volume=Mathf.MoveTowards(sources[i].volume,gains[i]*level,Time.deltaTime*1.5f);
        }
        void OnDestroy(){if(host!=null&&host.Sound!=null)host.Sound.CuePlayed-=OnCue;}
    }
}
