using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;
namespace LastKey
{
    // Runtime diagnostics only: no screenshots from this controlled test enter gameplay evidence.
    public sealed class MusicRuntimeQa:MonoBehaviour
    {
        LastKeyGame host;float originalVolume;
        void Check(bool result,string name)
        {
            Debug.Log((result?"MUSIC_QA_PASS ":"MUSIC_QA_FAIL ")+name);
            if(!result){host.Music.SetVolume(originalVolume);
#if UNITY_EDITOR
                UnityEditor.EditorApplication.Exit(1);
#endif
                throw new InvalidOperationException(name);}
        }
        void Flow(ExperienceState value)=>typeof(LastKeyGame).GetMethod("Switch",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(host,new object[]{value});
        AudioSource[] Sources()=>GetComponentsInChildren<AudioSource>().Where(s=>s.name.StartsWith("MusicSource")).ToArray();
        IEnumerator Start()
        {
            host=GetComponent<LastKeyGame>();originalVolume=host.Music.Volume;
            yield return new WaitForSeconds(4);
            Check(Sources().Length==2,"exactly two persistent music sources");
            Check(host.Music.TrackName=="Mainframe","menu has supplied track");
            host.Music.SetVolume(.37f);Check(Mathf.Abs(PlayerPrefs.GetFloat("LAST_KEY_MUSIC_VOLUME")-.37f)<.001f,"volume survives PlayerPrefs save");host.Music.SetVolume(originalVolume);
            host.OpenCharacterSelect();host.ChooseCharacter(0);host.ConfirmCharacter();host.BeginIntro();yield return new WaitForSeconds(1);
            Check(host.Music.TrackName=="Iron Dominion"&&host.Music.TrackTime<3,"wake-up uses intact soft opening");
            Flow(ExperienceState.Countdown);yield return new WaitForSeconds(3.8f);
            Check(host.Flow==ExperienceState.Gameplay&&host.Music.TrackTime>=15.8f&&host.Music.TrackTime<19,"skipped intro enters strong segment without replaying opening");
            float elapsed=host.Game.Elapsed,musicTime=host.Music.TrackTime;Flow(ExperienceState.Paused);yield return new WaitForSeconds(2);
            Check(Mathf.Abs(host.Game.Elapsed-elapsed)<.02f&&host.Music.TrackTime>musicTime+1.5f,"pause freezes competition while music continues");
            musicTime=host.Music.TrackTime;Flow(ExperienceState.Gameplay);yield return new WaitForSeconds(.2f);
            Check(host.Music.TrackTime>musicTime&&host.Music.TrackTime<musicTime+.7f,"resume preserves track position");
            yield return new WaitForSeconds(1.2f);float before=Sources().Sum(s=>s.volume);host.Sound.Cue("null");yield return new WaitForSeconds(.2f);
            Check(Sources().Sum(s=>s.volume)<before*.8f,"NULL ducks music without stopping it");yield return new WaitForSeconds(1.3f);
            Check(Sources().Sum(s=>s.volume)>before*.9f,"duck recovers");
            var source=Sources().First(s=>s.isPlaying);source.timeSamples=Mathf.RoundToInt(59.2f*source.clip.frequency);yield return new WaitForSeconds(1.5f);
            Check(host.Music.TrackTime>=16&&host.Music.TrackTime<20&&Sources().Length==2,"loop crossfade returns to chosen segment with no new sources");
            host.Music.SetVolume(originalVolume);Debug.Log("LAST_KEY_MUSIC_RUNTIME_QA_COMPLETE");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.Exit(0);
#endif
        }
    }
}
