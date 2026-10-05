using System;
using System.Collections;
using System.IO;
using UnityEngine;
namespace LastKey {
// Capture of the actual runtime character and imported motions; no concept frames.
public sealed class CinematicEvidence:MonoBehaviour {
 IEnumerator Start(){
  var args=Environment.GetCommandLineArgs();string root=args[Array.IndexOf(args,"--evidence-path")+1];
  Directory.CreateDirectory(Path.Combine(root,"frames","cinematic_character"));Directory.CreateDirectory(Path.Combine(root,"screenshots"));
  yield return null;var host=GetComponent<LastKeyGame>();host.BeginCinematicPreview();host.StopSessionAudio();Time.captureFramerate=30;
  int[] checkpoints={15,195,285,375,465,585,705,825,945,1095,1185,1305,1377};
  for(int frame=0;frame<1380;frame++){
   yield return new WaitForEndOfFrame();var tex=ScreenCapture.CaptureScreenshotAsTexture();
   File.WriteAllBytes(Path.Combine(root,"frames","cinematic_character",frame.ToString("D6")+".jpg"),tex.EncodeToJPG(92));
   if(Array.IndexOf(checkpoints,frame)>=0)File.WriteAllBytes(Path.Combine(root,"screenshots",frame.ToString("D4")+".png"),tex.EncodeToPNG());Destroy(tex);
  }
  Debug.Log("LAST_KEY_CINEMATIC_CAPTURE_COMPLETE frames=1380 fps=30");Application.Quit();
 }
}}
