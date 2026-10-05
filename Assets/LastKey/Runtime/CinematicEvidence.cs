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
   if(frame==825){var pilot=host.Arena.Characters[host.HumanIndex].GetComponent<ImportedMotionPilot>();Debug.Log("AUDIT_HUMAN "+host.HumanIndex);foreach(var bone in new[]{HumanBodyBones.LeftUpperArm,HumanBodyBones.LeftLowerArm,HumanBodyBones.LeftHand,HumanBodyBones.LeftMiddleDistal,HumanBodyBones.RightUpperArm,HumanBodyBones.RightLowerArm,HumanBodyBones.RightHand,HumanBodyBones.RightMiddleDistal}){var b=pilot.Rig.GetBoneTransform(bone);Debug.Log("ARM_AUDIT "+bone+" world="+pilot.Station.InverseTransformPoint(b.position).ToString("F4")+" local="+b.localPosition.ToString("F4"));}foreach(var skin in pilot.GetComponentsInChildren<SkinnedMeshRenderer>())foreach(var b in skin.bones)if(b.name.Contains("Forearm")||b.name.EndsWith("_Hand"))Debug.Log("SKIN_ARM "+b.name+" world="+pilot.Station.InverseTransformPoint(b.position).ToString("F4"));}
  }
  Debug.Log("LAST_KEY_CINEMATIC_CAPTURE_COMPLETE frames=1380 fps=30");Application.Quit();
 }
}}
