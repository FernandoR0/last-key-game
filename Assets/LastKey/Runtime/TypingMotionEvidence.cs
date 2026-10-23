using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace LastKey {
// Opt-in regression capture: real match flow and imported animation, no pose assignment.
public sealed class TypingMotionEvidence : MonoBehaviour {
 readonly HumanBodyBones[] fingers={HumanBodyBones.LeftThumbProximal,HumanBodyBones.LeftIndexProximal,HumanBodyBones.LeftMiddleProximal,HumanBodyBones.LeftRingProximal,HumanBodyBones.LeftLittleProximal,HumanBodyBones.RightThumbProximal,HumanBodyBones.RightIndexProximal,HumanBodyBones.RightMiddleProximal,HumanBodyBones.RightRingProximal,HumanBodyBones.RightLittleProximal};
 IEnumerator Start(){
  var host=GetComponent<LastKeyGame>();var args=Environment.GetCommandLineArgs();int index=Array.IndexOf(args,"--evidence-path");string root=index>=0?args[index+1]:"TypingMotionEvidence";Directory.CreateDirectory(root);Directory.CreateDirectory(Path.Combine(root,"frames"));Time.captureFramerate=30;
  while(host.Flow!=ExperienceState.MainMenu)yield return null;
  host.OpenModeSelect();host.SelectMode(Core.RoundMode.Classic);host.OpenCharacterSelect();host.ConfirmCharacter();host.BeginIntro();
  while(host.Flow!=ExperienceState.Gameplay)yield return null;
  var motion=host.Arena.Characters[host.HumanIndex].GetComponent<ImportedMotionPilot>();
  var initial=new Quaternion[10];var ranges=new float[10];var low=new Vector3[2];var high=new Vector3[2];
  for(int frame=0;frame<360;frame++){
   yield return new WaitForEndOfFrame();
   if(host.Flow!=ExperienceState.Gameplay)throw new InvalidOperationException("Typing evidence left gameplay");
   for(int f=0;f<10;f++){var bone=motion.Rig.GetBoneTransform(fingers[f]);if(frame==0)initial[f]=bone.localRotation;ranges[f]=Mathf.Max(ranges[f],Quaternion.Angle(initial[f],bone.localRotation));}
   for(int side=0;side<2;side++){var hand=motion.Rig.GetBoneTransform(side==0?HumanBodyBones.LeftHand:HumanBodyBones.RightHand);var pos=motion.Station.InverseTransformPoint(hand.position);if(frame==0)low[side]=high[side]=pos;low[side]=Vector3.Min(low[side],pos);high[side]=Vector3.Max(high[side],pos);}
   var tex=ScreenCapture.CaptureScreenshotAsTexture();File.WriteAllBytes(Path.Combine(root,"frames",frame.ToString("D4")+".png"),tex.EncodeToPNG());if(frame%90==0)File.WriteAllBytes(Path.Combine(root,"typing_"+frame.ToString("D4")+".png"),tex.EncodeToPNG());Destroy(tex);
  }
  for(int f=0;f<10;f++){Debug.Log("TYPING_GAMEPLAY_FINGER "+fingers[f]+" degrees="+ranges[f]);if(ranges[f]<2f)throw new InvalidOperationException("Inactive typing finger "+fingers[f]);}
  Debug.Log("TYPING_GAMEPLAY_HAND_RANGE left="+(high[0]-low[0])+" right="+(high[1]-low[1]));
  Debug.Log("LAST_KEY_TYPING_MOTION_QA_PASS 10 fingers / 360 gameplay frames");Application.Quit();
 }
}}
