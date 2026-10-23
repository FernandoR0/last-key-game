using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
namespace LastKey.Editor {
[InitializeOnLoad]
public static class TypingMotionAudit {
 const string Request="Library/TypingAudit.request";
 static readonly HumanBodyBones[] Fingers={HumanBodyBones.LeftThumbProximal,HumanBodyBones.LeftIndexProximal,HumanBodyBones.LeftMiddleProximal,HumanBodyBones.LeftRingProximal,HumanBodyBones.LeftLittleProximal,HumanBodyBones.RightThumbProximal,HumanBodyBones.RightIndexProximal,HumanBodyBones.RightMiddleProximal,HumanBodyBones.RightRingProximal,HumanBodyBones.RightLittleProximal};
 static TypingMotionAudit(){EditorApplication.update+=Poll;}
 static void Poll(){if(EditorApplication.isCompiling||EditorApplication.isUpdating||!File.Exists(Request))return;if(EditorApplication.isPlaying){EditorApplication.isPlaying=false;return;}string label=File.ReadAllText(Request).Trim();File.Delete(Request);try{Run(label);}catch(Exception e){Debug.LogException(e);}}
 static void ScanSource(ImportedMotionPilot motion){
  for(int window=0;window<8;window++){
   var first=new Quaternion[10];var max=new float[10];var low=new Vector3[2];var high=new Vector3[2];
   for(int i=0;i<=40;i++){
    float source=.35f+window*2+i*.05f;motion.Sample(source,2,15);
    var action=Field<AnimationClipPlayable>(motion,"action");var gesture=Field<AnimationClipPlayable>(motion,"gesture");action.SetTime(source);gesture.SetTime(source);Field<PlayableGraph>(motion,"graph").Evaluate(.000001f);
    for(int f=0;f<10;f++){var bone=motion.Rig.GetBoneTransform(Fingers[f]);if(i==0)first[f]=bone.localRotation;max[f]=Mathf.Max(max[f],Quaternion.Angle(first[f],bone.localRotation));}
    for(int side=0;side<2;side++){var p=motion.Station.InverseTransformPoint(motion.Rig.GetBoneTransform(side==0?HumanBodyBones.LeftHand:HumanBodyBones.RightHand).position);if(i==0)low[side]=high[side]=p;low[side]=Vector3.Min(low[side],p);high[side]=Vector3.Max(high[side],p);}
   }
   string report="TYPING_SOURCE_WINDOW "+(.35f+window*2)+" left="+(high[0]-low[0]).ToString("F4")+" right="+(high[1]-low[1]).ToString("F4")+" fingers="+string.Join(",",Array.ConvertAll(max,x=>x.ToString("F2")));Debug.Log(report);File.AppendAllText("../../work/game/typing-comparison.txt",report+Environment.NewLine);
  }
 }
 public static void Run(){Run("audit");}
 static T Field<T>(object o,string name)=>(T)o.GetType().GetField(name,BindingFlags.Instance|BindingFlags.NonPublic).GetValue(o);
 static void Run(string label){
  var station=new GameObject("Temporary typing audit");station.transform.position=new Vector3(1000,0,0);
  var terminal=new GameObject("Integrated touch terminal");terminal.transform.SetParent(station.transform,false);terminal.transform.localPosition=new Vector3(0,.98f,-.33f);
  Renderer collar;var body=CharacterFactory.Create(station.transform,0,Color.cyan,out collar);var motion=body.GetComponent<ImportedMotionPilot>();
  try{
   var clip=body.GetComponent<ImportedMotionLibrary>().Clips[3];motion.Sample(15,2,15);
   typeof(ImportedMotionPilot).GetField("transitionAt",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(motion,-100f);
   Debug.Log("TYPING_SOURCE seconds="+clip.length);
   foreach(bool adapted in new[]{false,true}){
    var first=new Quaternion[10];var max=new float[10];var low=new Vector3[2];var high=new Vector3[2];
    for(int i=0;i<=208;i++){
     float seconds=14+i*.05f;motion.Sample(seconds,2,seconds);
     if(!adapted){var action=Field<AnimationClipPlayable>(motion,"action");var gesture=Field<AnimationClipPlayable>(motion,"gesture");float source=2.6f+2.6f*(1-Mathf.Cos(seconds*Mathf.PI/5.2f));action.SetTime(source);gesture.SetTime(source);Field<PlayableGraph>(motion,"graph").Evaluate(.000001f);}
     for(int f=0;f<10;f++){var bone=motion.Rig.GetBoneTransform(Fingers[f]);if(i==0)first[f]=bone.localRotation;max[f]=Mathf.Max(max[f],Quaternion.Angle(first[f],bone.localRotation));}
     for(int side=0;side<2;side++){var hand=motion.Rig.GetBoneTransform(side==0?HumanBodyBones.LeftHand:HumanBodyBones.RightHand);var p=station.transform.InverseTransformPoint(hand.position);if(i==0)low[side]=high[side]=p;low[side]=Vector3.Min(low[side],p);high[side]=Vector3.Max(high[side],p);}
    }
    string report="TYPING_RANGE "+label+" adapted="+adapted+" left="+(high[0]-low[0]).ToString("F4")+" right="+(high[1]-low[1]).ToString("F4")+" fingers="+string.Join(",",Array.ConvertAll(max,x=>x.ToString("F2")));
    if(adapted){for(int f=0;f<10;f++)if(max[f]<2f)throw new Exception("Typing finger remains inactive: "+Fingers[f]+" range="+max[f]);}Debug.Log(report);Directory.CreateDirectory("../../work/game");File.AppendAllText("../../work/game/typing-comparison.txt",report+Environment.NewLine);
   }
   ScanSource(motion);Debug.Log("TYPING_COMPARISON_COMPLETE "+label);
  }finally{UnityEngine.Object.DestroyImmediate(station);}
 }
}}


