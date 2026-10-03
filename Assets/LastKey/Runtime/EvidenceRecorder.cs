using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using LastKey.Core;

namespace LastKey
{
    // Deterministic QA replay sends characters through the same competition input API.
    // It never overwrites score, danger, accuracy or outcome fields.
    public sealed class EvidenceRecorder : MonoBehaviour
    {
        LastKeyGame host;
        string root;
        HashSet<string> captured=new HashSet<string>();
        float credit, wait;
        bool recording, portraitsDone;
        string clip;
        int clipFrame;
        int variant;
        readonly List<CaptureState> audit=new List<CaptureState>();
        IEnumerator Start()
        {
            host=GetComponent<LastKeyGame>();var args=Environment.GetCommandLineArgs();int i=Array.IndexOf(args,"--evidence-path");
            root=i>=0&&i+1<args.Length?args[i+1]:Path.Combine(Application.persistentDataPath,"handoff");
            foreach(var folder in new[]{"screenshots","characters","modes","frames","technical"})Directory.CreateDirectory(Path.Combine(root,folder));
            Screen.SetResolution(1920,1080,FullScreenMode.Windowed);Time.captureFramerate=30;QualitySettings.vSyncCount=0;
            Shader.WarmupAllShaders();
            yield return new WaitForSecondsRealtime(2);
#if UNITY_EDITOR
            while(UnityEditor.ShaderUtil.anythingCompiling)yield return null;
#endif
            yield return null;yield return null;
            host.Canvas.enabled=false;yield return new WaitForEndOfFrame();Capture("screenshots/01_HERO_ARENA.png");
            var environment=host.Arena.Root.transform.Find("Environment");var stations=host.Arena.Root.transform.Find("Stations");
            environment.gameObject.SetActive(false);stations.gameObject.SetActive(false);
            var backdrop=GameObject.CreatePrimitive(PrimitiveType.Cube);backdrop.name="PortraitBackdrop";backdrop.transform.position=new Vector3(0,1.4f,-2);backdrop.transform.localScale=new Vector3(8,8,.1f);
            var backdropMat=new Material(Shader.Find("Universal Render Pipeline/Lit"));backdropMat.color=new Color(.085f,.105f,.13f);backdrop.GetComponent<Renderer>().sharedMaterial=backdropMat;
            host.Arena.Camera.fieldOfView=30;
            for(int n=0;n<4;n++)
            {
                var portrait=Instantiate(host.Arena.Characters[n].gameObject);portrait.name="Portrait_"+n;portrait.transform.SetParent(null);portrait.transform.position=Vector3.zero;portrait.transform.rotation=Quaternion.Euler(0,180,0);portrait.GetComponent<CharacterMotion>().enabled=false;portrait.SetActive(true);
                var head=portrait.GetComponentInChildren<Animator>().GetBoneTransform(HumanBodyBones.Head).position;
                host.Arena.Camera.transform.position=head+new Vector3(.12f,.13f,1.7f);host.Arena.Camera.transform.LookAt(head-Vector3.up*.12f);
                yield return new WaitForEndOfFrame();
                Capture("characters/"+(13+n)+"_"+new[]{"CIPHER","UPLINK","RELAY","VECTOR"}[n]+".png");Destroy(portrait);yield return null;
            }
            Destroy(backdrop);environment.gameObject.SetActive(true);stations.gameObject.SetActive(true);host.Arena.Camera.fieldOfView=49;
            host.Arena.SetCamera(0);host.Canvas.enabled=true;
            if(Array.IndexOf(args,"--character-smoke")>=0){Finish();yield break;}
            portraitsDone=true;host.StartNext();
        }
        void Update()
        {
            if(!portraitsDone)return;var g=host.Game;var p=g.Players[0];
            if(variant>0)
            {
                if(g.Phase==Phase.Running)
                {
                    credit+=Time.deltaTime*4;while(credit>=1){credit--;g.Input(0,g.Text[p.Cursor]);}
                    Once("modes/"+(17+variant)+"_"+new[]{"","CODE_RACE","NUMBERS_RUSH","MEMORY_TYPE","SUDDEN_DEATH"}[variant]+".png",g.Elapsed>7);
                    if(g.Elapsed>9){g.EndRound();variant++;if(variant>4){Finish();return;}host.SelectMode((RoundMode)variant);host.StartNext();}
                }
                host.Refresh();return;
            }
            if(g.Phase==Phase.Running)
            {
                wait=0;credit+=Time.deltaTime*4.0f;
                while(credit>=1&&!p.Eliminated)
                {
                    credit--;
                    if(g.Round==1&&g.Elapsed>=22&&g.Elapsed<28)g.Input(0,'#');
                    else if(p.EchoAccepted&&!p.EchoResolved)g.Input(0,Competition.EchoText[p.EchoCursor]);
                    else g.Input(0,g.Text[p.Cursor]);
                }
                if(g.Round==2&&g.EchoOffered&&!p.EchoAccepted&&!p.EchoResolved)g.AcceptEcho(0);
                host.Refresh();
                if(g.Round==1)
                {
                    Once("screenshots/02_TYPING_GAMEPLAY.png",g.Elapsed>8);
                    Once("screenshots/03_HUD_DETAIL.png",g.Elapsed>9);
                    Once("screenshots/04_FOUR_PLAYERS.png",g.Elapsed>10);
                    Once("screenshots/05_DANGER_SAFE.png",g.Elapsed>11&&p.Danger<40);
                    Once("screenshots/08_NULL_DIRECTIVE.png",g.ActiveDirective==Directive.PrecisionProtocol&&g.Elapsed>14);
                    Once("screenshots/06_DANGER_WARNING.png",p.Danger>=40&&p.Danger<75);
                    Once("screenshots/07_DANGER_CRITICAL.png",p.Danger>=75&&!p.Eliminated);
                    Once("screenshots/12_ELIMINATED_ROUND.png",p.Eliminated);
                    Once("modes/17_CLASSIC_RACE.png",g.Elapsed>12);
                }
                if(g.Round==2)Once("screenshots/09_ECHO_INTERCEPT.png",p.EchoAccepted&&!p.EchoResolved&&p.EchoCursor>2);
                if(g.Round==2)Once("screenshots/27_RETURN_NEXT_ROUND.png",g.Elapsed>3&&g.Elapsed<7&&!p.Eliminated);
                string nextClip=g.Round==1&&g.Elapsed>=7&&g.Elapsed<17?"gameplay_round":g.Round==1&&g.Elapsed>=21&&g.Elapsed<29?"danger_event":g.Round==2&&g.Elapsed>=12&&g.Elapsed<18?"null_directive":g.Round==2&&g.Elapsed>=34&&g.Elapsed<40?"echo_intercept":null;
                SetClip(nextClip);
            }
            else
            {
                wait+=Time.deltaTime;host.Refresh();
                if(g.Phase==Phase.Results)
                {
                    Once("screenshots/10_ROUND_RESULTS.png",g.Round==1&&wait>.5f);
                    SetClip(g.Round==1&&wait<6?"round_results":null);
                    if(wait>6){host.StartNext();credit=0;}
                }
                else if(g.Phase==Phase.Finished)
                {
                    Once("screenshots/11_FINAL_RESULTS.png",wait>.5f);SetClip(wait<6?"final_winner":null);
                    if(wait>7)
                    {
                        File.WriteAllText(Path.Combine(root,"runtime_results.json"),JsonUtility.ToJson(new RuntimeResults{unity=Application.unityVersion,rounds=g.Round,winner=string.Join(" / ",g.Winners().Select(q=>q.Name)),players=g.Players.Select(q=>new PlayerResult{name=q.Name,total=q.TotalScore}).ToArray()},true));
                        variant=1;host.SelectMode(RoundMode.Code);host.StartNext();credit=0;
                    }
                }
            }
        }
        void Finish()
        {
            File.WriteAllText(Path.Combine(root,"capture_states.json"),JsonUtility.ToJson(new CaptureAudit{captures=audit.ToArray()},true));
            Debug.Log("LAST_KEY_EVIDENCE_COMPLETE "+root);
#if UNITY_EDITOR
            UnityEditor.EditorApplication.Exit(0);
#else
            Application.Quit();
#endif
        }
        void SetClip(string name)
        {
            if(name!=clip){clip=name;clipFrame=0;if(clip!=null)Directory.CreateDirectory(Path.Combine(root,"frames",clip));}
            if(clip!=null&&!recording)StartCoroutine(Frame());
        }
        IEnumerator Frame()
        {
            recording=true;yield return new WaitForEndOfFrame();
            if(clip!=null)Capture("frames/"+clip+"/"+(clipFrame++).ToString("D5")+".jpg");recording=false;
        }
        void Once(string file,bool condition){if(condition&&captured.Add(file))StartCoroutine(Snap(file));}
        IEnumerator Snap(string file){yield return new WaitForEndOfFrame();Capture(file);}
        void Capture(string relative)
        {
            var tex=ScreenCapture.CaptureScreenshotAsTexture();var data=relative.EndsWith(".jpg")?tex.EncodeToJPG(85):tex.EncodeToPNG();
            File.WriteAllBytes(Path.Combine(root,relative),data);Destroy(tex);
            if(relative.EndsWith(".png")){var g=host.Game;var p=g.Players[0];audit.Add(new CaptureState{file=relative,round=g.Round,phase=g.Phase.ToString(),elapsed=g.Elapsed,danger=p.Danger,score=p.RoundScore,total=p.TotalScore,errors=p.Errors,eliminated=p.Eliminated,echoAccepted=p.EchoAccepted,echoResolved=p.EchoResolved,motions=host.Arena.Characters.Select(t=>t.GetComponent<CharacterMotion>().State).ToArray()});Debug.Log("EVIDENCE_CAPTURE "+relative+" round="+g.Round+" phase="+g.Phase+" danger="+p.Danger);}

        }
        [Serializable]class CaptureAudit{public CaptureState[] captures;}
        [Serializable]class CaptureState{public string file,phase;public int round,score,total,errors;public float elapsed,danger;public bool eliminated,echoAccepted,echoResolved;public string[] motions;}
        [Serializable]class RuntimeResults{public string unity,winner;public int rounds;public PlayerResult[] players;}
        [Serializable]class PlayerResult{public string name;public int total;}
    }
}
