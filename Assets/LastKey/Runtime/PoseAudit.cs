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
            var host=GetComponent<LastKeyGame>();host.enabled=false;host.Canvas.enabled=false;var args=Environment.GetCommandLineArgs();var root=args[Array.IndexOf(args,"--evidence-path")+1];Directory.CreateDirectory(root);
            yield return new WaitForSecondsRealtime(2);
            for(int i=0;i<4;i++)
            {
                host.Arena.Arrange(i);var station=host.Arena.Stations[i];host.Arena.Camera.fieldOfView=48;host.Arena.Camera.transform.position=station.TransformPoint(new Vector3(2.6f,1.3f,-.45f));host.Arena.Camera.transform.LookAt(station.TransformPoint(new Vector3(0,.9f,-.8f)));
                for(int state=0;state<3;state++)
                {
                    var g=new Competition(new Rules{rounds=1,roundSeconds=3},humanIndex:i);
                    if(state==2){g.StartRound();for(int n=0;n<120;n++)g.Input(i,g.Target(i)[g.Players[i].Cursor]);g.Tick(3);}
                    host.Arena.Animate(Time.time,g);var motion=host.Arena.Characters[i].GetComponent<CharacterMotion>();motion.Wakeup=state==1;motion.IntroProgress=.2f;
                    yield return null;yield return new WaitForEndOfFrame();var texture=ScreenCapture.CaptureScreenshotAsTexture();File.WriteAllBytes(Path.Combine(root,LastKeyGame.Aliases[i]+"_"+new[]{"seated","wakeup","winner"}[state]+".png"),texture.EncodeToPNG());Destroy(texture);
                }
            }
            Debug.Log("LAST_KEY_POSE_AUDIT_COMPLETE");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.Exit(0);
#endif
        }
    }
}
