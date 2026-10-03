using System;
using System.Collections;
using System.IO;
using UnityEngine;
namespace LastKey
{
    public sealed class KeyboardQa:MonoBehaviour
    {
        void Update(){if(Input.anyKeyDown||Input.inputString.Length>0)Debug.Log("KEYBOARD_RAW chars="+Input.inputString+" mouse="+Input.mousePosition+" click="+Input.GetMouseButtonDown(0)+" return="+Input.GetKeyDown(KeyCode.Return));}
        IEnumerator Start()
        {
            var host=GetComponent<LastKeyGame>();var args=Environment.GetCommandLineArgs();string root=args[Array.IndexOf(args,"--evidence-path")+1];
            while(host.Game.Players[0].Correct<10||host.Game.Players[0].Errors<1)yield return null;
            host.Refresh();yield return new WaitForEndOfFrame();Directory.CreateDirectory(Path.Combine(root,"technical"));
            var tex=ScreenCapture.CaptureScreenshotAsTexture();File.WriteAllBytes(Path.Combine(root,"technical","23_KEYBOARD_INPUT.png"),tex.EncodeToPNG());Destroy(tex);
            var p=host.Game.Players[0];Debug.Log("LAST_KEY_KEYBOARD_QA_SUCCESS correct="+p.Correct+" errors="+p.Errors+" score="+p.RoundScore+" danger="+p.Danger);
            yield return new WaitForSecondsRealtime(2);
#if UNITY_EDITOR
            UnityEditor.EditorApplication.Exit(0);
#endif
        }
    }
}
