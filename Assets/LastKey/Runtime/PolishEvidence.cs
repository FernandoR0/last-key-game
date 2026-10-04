using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using LastKey.Core;
namespace LastKey
{
    // Real QA input replay. Metrics are produced by Competition.Input/Tick, never assigned.
    public sealed class PolishEvidence:MonoBehaviour
    {
        public string ActiveClip=>clip;bool musicEvidence;LastKeyGame host;string root,clip;float credit;bool framePending,ready;
        int frame;readonly HashSet<string> captured=new HashSet<string>();readonly Dictionary<string,int> frameCounts=new Dictionary<string,int>();
        readonly List<State> audit=new List<State>();
        readonly Dictionary<string,List<SoundEvent>> sounds=new Dictionary<string,List<SoundEvent>>();
        class SoundEvent{public int frame;public float[] samples;}
        IEnumerator Start()
        {
            musicEvidence=Environment.GetCommandLineArgs().Contains("--music-evidence");host=GetComponent<LastKeyGame>();var args=Environment.GetCommandLineArgs();root=args[Array.IndexOf(args,"--evidence-path")+1];
            foreach(var folder in new[]{"screenshots","characters","frames","technical"})Directory.CreateDirectory(Path.Combine(root,folder));
            Screen.SetResolution(1920,1080,FullScreenMode.Windowed);Time.captureFramerate=30;QualitySettings.vSyncCount=0;
            host.Sound.Emitted+=RecordSound;yield return null;ready=true;
        }
        void LateUpdate()
        {
            if(!ready)return;var g=host.Game;var p=g.Players[host.HumanIndex];string next=null;
            switch(host.Flow)
            {
                case ExperienceState.Loading:Once("01_LOADING",host.StateTime>.7f);next="01_loading_to_menu";break;
                case ExperienceState.MainMenu:Once("02_MAIN_MENU",host.StateTime>.5f);next=host.StateTime<1?"01_loading_to_menu":null;if(Environment.GetCommandLineArgs().Contains("--loading-only")){if(host.StateTime>30){Finish();return;}}else if(host.StateTime>(musicEvidence?8f:1.5f)){if(argsMenuOnly()){Finish();return;}host.OpenModeSelect();}break;
                case ExperienceState.ModeSelect:Once("03_MODE_SELECT",host.StateTime>.4f);if(host.StateTime>1.5f)host.OpenCharacterSelect();break;
                case ExperienceState.CharacterSelect:Once("04_CHARACTER_SELECT",host.StateTime>.6f);next="02_character_select";if(host.StateTime>3)host.ConfirmCharacter();break;
                case ExperienceState.MatchSetup:if(host.StateTime>1)host.BeginIntro();break;
                case ExperienceState.RoundIntro:credit=0;Once("05_ROUND_WAKEUP_INTRO",g.Round==1&&host.StateTime>.7f);if(g.Round==1)next="03_round_wakeup_intro";break;
                case ExperienceState.Countdown:if(g.Round==1)next="03_round_wakeup_intro";break;
                case ExperienceState.Gameplay:
                    credit+=Time.deltaTime*(musicEvidence&&g.Round==1&&g.Elapsed>=22&&g.Elapsed<30?1.5f:4.5f);
                    while(credit>=1&&!p.Eliminated){credit--;g.Input(host.HumanIndex,g.Round==1&&g.Elapsed>=22&&g.Elapsed<30?'#':p.EchoAccepted&&!p.EchoResolved?Competition.EchoText[p.EchoCursor]:p.Prompt[p.Cursor]);}
                    if(g.Round==2&&g.EchoOffered&&!p.EchoAccepted&&!p.EchoResolved)host.RespondEcho(true);
                    host.Refresh();Once("06_GAMEPLAY_WIDE",g.Round==1&&g.Elapsed>7);Once("07_GAMEPLAY_COMPOSITION_PROOF",g.Round==1&&g.Elapsed>8);Once("08_HUD_DETAIL",g.Round==1&&g.Elapsed>9);Once("09_FOUR_PLAYERS",g.Round==1&&g.Elapsed>10);
                    Once("10_DANGER_WARNING",p.Danger>=40&&p.Danger<75);Once("11_DANGER_CRITICAL",p.Danger>=75&&!p.Eliminated);Once("12_ELIMINATION",p.Eliminated&&host.Cameras.Role=="EliminationCamera");
                    Once("13_NULL_DIRECTIVE",g.ActiveDirective!=Directive.None&&g.Elapsed>14);Once("14_ECHO_INTERCEPT",p.EchoAccepted&&!p.EchoResolved&&p.EchoCursor>2);
                    next=g.Round==1&&g.Elapsed>=7&&g.Elapsed<17?"04_gameplay_typing_with_rivals":g.Round==1&&g.Elapsed>=21&&g.Elapsed<30?"05_danger_elimination":g.Round==1&&g.Elapsed>=13&&g.Elapsed<19?"06_null_directive":g.Round==2&&g.Elapsed>=33&&g.Elapsed<40?"07_echo_intercept":null;
                    // NULL clip overlaps gameplay; separate window after typing clip.
                    if(g.Round==1&&g.Elapsed>=17&&g.Elapsed<21)next="06_null_directive";
                    if(Environment.GetCommandLineArgs().Contains("--polish-smoke")&&g.Elapsed>12){Finish();return;}break;
                case ExperienceState.RoundResult:Once("15_ROUND_RESULTS",g.Round==1&&host.StateTime>.7f);next=g.Round==1&&host.StateTime<6?"08_round_result":null;if(host.StateTime>6)host.BeginIntro();break;
                case ExperienceState.FinalResult:Once("16_FINAL_WINNER",host.StateTime>.7f);next=host.StateTime<6?"09_final_winner":null;if(host.StateTime>6&&!framePending){ready=false;StartCoroutine(Portraits());}break;
            }
            if(musicEvidence)
            {
                next=host.Flow==ExperienceState.MainMenu?"01_music_menu":
                g.Round==1&&(host.Flow==ExperienceState.RoundIntro||host.Flow==ExperienceState.Countdown||host.Flow==ExperienceState.Gameplay&&g.Elapsed<3)?"02_music_wakeup_to_type":
                host.Flow==ExperienceState.Gameplay&&g.Round==1&&g.Elapsed>=7&&g.Elapsed<13?"03_music_gameplay":
                host.Flow==ExperienceState.Gameplay&&g.Round==1&&g.Elapsed>=22&&g.Elapsed<26?"04_music_danger":
                host.Flow==ExperienceState.Gameplay&&g.Round==1&&g.Elapsed>=26&&g.Elapsed<32?"06_music_elimination":
                host.Flow==ExperienceState.Gameplay&&g.Round==2&&g.Elapsed>=12&&g.Elapsed<40?"05_music_null_echo":
                host.Flow==ExperienceState.RoundResult&&g.Round==1?"07_music_round_result":
                host.Flow==ExperienceState.FinalResult?"08_music_final_winner":null;
            }
            SetClip(next);
        }
        IEnumerator Portraits()
        {
            clip=null;host.Canvas.enabled=false;
            for(int i=0;i<4;i++){host.Cameras.Portrait(i);yield return new WaitForEndOfFrame();Save("characters/"+(17+i)+"_"+LastKeyGame.Aliases[i]+".png");}
            host.Canvas.enabled=true;Finish();
        }
        void Once(string name,bool condition){if(condition&&captured.Add(name))StartCoroutine(Snap("screenshots/"+name+".png"));}
        IEnumerator Snap(string path){yield return new WaitForEndOfFrame();Save(path);}
        void SetClip(string name)
        {
            clip=name;if(name==null||framePending)return;if(!frameCounts.ContainsKey(name))frameCounts[name]=0;Directory.CreateDirectory(Path.Combine(root,"frames",name));StartCoroutine(Frame(name));
        }
        IEnumerator Frame(string name){framePending=true;yield return new WaitForEndOfFrame();Save("frames/"+name+"/"+(frameCounts[name]++).ToString("D5")+".jpg");framePending=false;}
        void RecordSound(AudioClip sound)
        {
            if(clip==null||!frameCounts.ContainsKey(clip))return;
            if(!sounds.ContainsKey(clip))sounds[clip]=new List<SoundEvent>();var samples=new float[sound.samples];sound.GetData(samples,0);sounds[clip].Add(new SoundEvent{frame=frameCounts[clip],samples=samples});
        }
        void ExportAudio()
        {
            var ambient=host.Sound.Ambience;var bed=new float[ambient.samples];ambient.GetData(bed,0);
            foreach(var pair in frameCounts)
            {
                var pcm=new float[pair.Value*1470];float volume=pair.Key.StartsWith("01")||pair.Key.StartsWith("02")?.09f:.025f;
                for(int i=0;i<pcm.Length;i++)pcm[i]=bed[i%bed.Length]*volume;
                if(sounds.ContainsKey(pair.Key))foreach(var sound in sounds[pair.Key])for(int i=0;i<sound.samples.Length&&sound.frame*1470+i<pcm.Length;i++)pcm[sound.frame*1470+i]+=sound.samples[i]*.32f;
                using(var stream=new BinaryWriter(File.Create(Path.Combine(root,"frames",pair.Key,"audio.f32"))))foreach(float sample in pcm)stream.Write(Mathf.Clamp(sample,-1,1));
            }
        }
        void Save(string path)
        {
            var tex=ScreenCapture.CaptureScreenshotAsTexture();File.WriteAllBytes(Path.Combine(root,path),path.EndsWith(".jpg")?tex.EncodeToJPG(88):tex.EncodeToPNG());Destroy(tex);
            if(path.EndsWith(".png")){var p=host.Game.Players[host.HumanIndex];audit.Add(new State{file=path,flow=host.Flow.ToString(),camera=host.Cameras.Role,round=host.Game.Round,danger=p.Danger,total=p.TotalScore,score=p.RoundScore,prompt=p.Prompt,motions=host.Arena.Characters.Select(t=>t.GetComponent<CharacterMotion>().State).ToArray()});Debug.Log("V2_CAPTURE "+path+" "+host.Flow);}
        }
        bool argsMenuOnly()=>Environment.GetCommandLineArgs().Contains("--music-menu-only");
        void Finish()
        {
            ready=false;
            if(musicEvidence)GetComponent<MusicCapture>().Stop();else ExportAudio();File.WriteAllText(Path.Combine(root,"capture_states.json"),JsonUtility.ToJson(new Audit{captures=audit.ToArray()},true));Debug.Log("LAST_KEY_V2_EVIDENCE_COMPLETE");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.Exit(0);
#else
            Application.Quit();
#endif
        }
        [Serializable]class Audit{public State[] captures;}
        [Serializable]class State{public string file,flow,camera,prompt;public string[] motions;public int round,total,score;public float danger;}
    }
}
