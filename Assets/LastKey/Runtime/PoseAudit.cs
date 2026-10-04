using System;
using System.Collections;
using System.IO;
using UnityEngine;
using LastKey.Core;
namespace LastKey
{
    // Diagnostic pose checks only; these accelerated test matches are never final gameplay evidence.
    public sealed class PoseAudit:MonoBehaviour
    {
        IEnumerator Start()
        {
            var host=GetComponent<LastKeyGame>();host.enabled=false;host.Canvas.enabled=false;host.StopSessionAudio();host.Music.enabled=false;var args=Environment.GetCommandLineArgs();var root=args[Array.IndexOf(args,"--evidence-path")+1];Directory.CreateDirectory(root);
            yield return new WaitForSecondsRealtime(2);
            for(int i=0;i<4;i++)
            {
                host.Arena.Arrange(i);var station=host.Arena.Stations[i];host.Arena.Camera.fieldOfView=48;host.Arena.Camera.transform.position=station.TransformPoint(new Vector3(2.6f,1.3f,-.45f));host.Arena.Camera.transform.LookAt(station.TransformPoint(new Vector3(0,.9f,-.8f)));
                for(int state=0;state<4;state++)
                {
                    var g=new Competition(new Rules{rounds=1,roundSeconds=3},humanIndex:i);
                    if(state==3)g.StartRound();
                    if(state==2){g.StartRound();for(int n=0;n<120;n++)g.Input(i,g.Target(i)[g.Players[i].Cursor]);g.Tick(3);}
                    host.Arena.Animate(Time.time,g);var motion=host.Arena.Characters[i].GetComponent<CharacterMotion>();motion.Wakeup=state==1;motion.IntroProgress=.2f;motion.OpeningSeconds=state==1?6:0;yield return new WaitForSecondsRealtime(1.1f);
                    yield return null;yield return new WaitForEndOfFrame();var texture=ScreenCapture.CaptureScreenshotAsTexture();File.WriteAllBytes(Path.Combine(root,LastKeyGame.Aliases[i]+"_"+new[]{"seated","wakeup","winner","typing"}[state]+".png"),texture.EncodeToPNG());Destroy(texture);
                }
            }
            host.Arena.Arrange(0);var match=new Competition(new Rules{rounds=1,roundSeconds=30},humanIndex:0);match.StartRound();
            host.Arena.Animate(Time.time,match);foreach(var character in host.Arena.Characters)character.GetComponent<CharacterMotion>().Wakeup=false;
            for(int n=0;n<90;n++){host.Cameras.Gameplay();yield return null;}
            yield return new WaitForEndOfFrame();var fps=ScreenCapture.CaptureScreenshotAsTexture();File.WriteAllBytes(Path.Combine(root,"INTEGRATED_TOUCHSCREEN_FIRST_PERSON.png"),fps.EncodeToPNG());Destroy(fps);
            Debug.Log("LAST_KEY_POSE_AUDIT_COMPLETE");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.Exit(0);
#endif
        }
    }
}
