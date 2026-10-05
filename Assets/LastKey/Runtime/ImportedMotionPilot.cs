using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace LastKey {
// Reuses imported mocap. Furniture adaptation moves wrists, preserving source finger curves.
// Pilot is sampled by the visual review scene; it does not alter competition rules/input.
public sealed class ImportedMotionPilot : MonoBehaviour {
 public Animator Rig {get;private set;} public Renderer Collar {get;private set;}
 public Transform Station {get;private set;}
 public Vector3 CollarCenter => Collar.GetComponentInParent<ImportedSpaceCollar>().RingCenter;
 PlayableGraph graph; AnimationMixerPlayable bodyMixer; AnimationLayerMixerPlayable layers;
 AnimationClipPlayable idle,action,gesture,electric; AnimationClip[] clips; int prior=-1;
 public float CollarSourceTime=-1;
 public Vector3 GazeTarget {get;private set;}
 readonly Quaternion[] gripRotation=new Quaternion[2];readonly bool[] gripReady=new bool[2];
 Transform[] eyeBones,neckBones;Quaternion[] eyeRest,neckRest;Vector3[] eyeForward;Quaternion headRest;Vector3 headForward,headUp;
 Transform[] poseBones;Quaternion[] transitionPose;int previousStage=-1;float transitionAt;
 sealed class HandSurface {public SkinnedMeshRenderer skin;public Vector3[] vertices; public BoneWeight[] weights; public Matrix4x4[] bind,matrices; public int[][] indices;}
 readonly List<HandSurface> handSurfaces=new List<HandSurface>();
 Quaternion jawRest; Transform jaw; LineRenderer arc; Material arcMaterial;
 public string Stage {get;private set;} public int Intensity {get;private set;}
 public void Initialize(Animator rig,Transform station,AnimationClip[] motions,int identity=1){
  Rig=rig;Station=station;clips=motions;rig.enabled=true;rig.applyRootMotion=false;rig.cullingMode=AnimatorCullingMode.AlwaysAnimate;
  jaw=rig.GetBoneTransform(HumanBodyBones.Jaw);if(jaw!=null)jawRest=jaw.localRotation;
  var headBone=rig.GetBoneTransform(HumanBodyBones.Head);headRest=headBone.localRotation;headForward=headBone.InverseTransformDirection(station.forward);headUp=headBone.InverseTransformDirection(station.up);
  eyeBones=new[]{rig.GetBoneTransform(HumanBodyBones.LeftEye),rig.GetBoneTransform(HumanBodyBones.RightEye)};eyeRest=new Quaternion[2];eyeForward=new Vector3[2];for(int i=0;i<2;i++)if(eyeBones[i]!=null){eyeRest[i]=eyeBones[i].localRotation;eyeForward[i]=eyeBones[i].InverseTransformDirection(station.forward);}
  var necks=new List<Transform>();foreach(var bone in rig.GetComponentsInChildren<Transform>())if(bone.name.Contains("NeckTwist"))necks.Add(bone);neckBones=necks.ToArray();neckRest=new Quaternion[neckBones.Length];for(int i=0;i<neckBones.Length;i++)neckRest[i]=neckBones[i].localRotation;
  var boneList=new List<Transform>();for(int i=0;i<(int)HumanBodyBones.LastBone;i++){var bone=rig.GetBoneTransform((HumanBodyBones)i);if(bone!=null)boneList.Add(bone);}poseBones=boneList.ToArray();transitionPose=new Quaternion[poseBones.Length];
  CacheHandSurfaces();
  Collar=ImportedSpaceCollar.Attach(rig,station);
  graph=PlayableGraph.Create("LAST KEY downloaded motion pilot");graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
  idle=AnimationClipPlayable.Create(graph,clips[0]);bodyMixer=AnimationMixerPlayable.Create(graph,2);graph.Connect(idle,0,bodyMixer,0);
  electric=AnimationClipPlayable.Create(graph,clips[4]);layers=AnimationLayerMixerPlayable.Create(graph,3);graph.Connect(bodyMixer,0,layers,0);graph.Connect(electric,0,layers,1);layers.SetInputWeight(0,1);
  var mask=new AvatarMask();for(int i=0;i<(int)AvatarMaskBodyPart.LastBodyPart;i++)mask.SetHumanoidBodyPartActive((AvatarMaskBodyPart)i,i!=(int)AvatarMaskBodyPart.Root&&i!=(int)AvatarMaskBodyPart.LeftLeg&&i!=(int)AvatarMaskBodyPart.RightLeg&&i!=(int)AvatarMaskBodyPart.LeftFootIK&&i!=(int)AvatarMaskBodyPart.RightFootIK&&i!=(int)AvatarMaskBodyPart.LeftFingers&&i!=(int)AvatarMaskBodyPart.RightFingers);
  layers.SetLayerMaskFromAvatarMask(1,mask);layers.SetLayerMaskFromAvatarMask(2,mask);AnimationPlayableOutput.Create(graph,"Imported Humanoid",rig).SetSourcePlayable(layers);graph.Play();
  var obj=new GameObject("Collar discharge preview",typeof(LineRenderer));obj.transform.SetParent(station,false);arc=obj.GetComponent<LineRenderer>();arc.positionCount=12;arc.loop=false;arc.useWorldSpace=true;arc.numCornerVertices=2;arc.numCapVertices=2;arc.widthCurve=AnimationCurve.Linear(0,1,1,1);
  arcMaterial=new Material(Shader.Find("Universal Render Pipeline/Unlit"));arcMaterial.color=new Color(.55f,.9f,1);arc.sharedMaterial=arcMaterial;arc.enabled=false;
 }
 public void Sample(float t,int overrideStage=-1,float overrideSeconds=0){
  // Source order: idle, waking seated, disbelief, typing, electrocuted, victory.
  int stage=overrideStage>=0?overrideStage:t<5?0:t<10?1:t<18?2:t<21?3:t<24?4:t<30?5:6;
  float start=new[]{0f,5,10,18,21,24,30,0,0,0}[stage],s=overrideStage>=0?overrideSeconds:t-start;
  if(previousStage!=stage){for(int i=0;i<poseBones.Length;i++)transitionPose[i]=poseBones[i].localRotation;transitionAt=Time.time;previousStage=stage;}
  Stage=new[]{"DESPERTAR / MOCAP SENTADO","COLLAR / ADAPTACIÓN DE CONTACTO","TECLEO / PANTALLA INTEGRADA","DESCARGA DÉBIL / DANGER 40%","DESCARGA MEDIA / DANGER 70%","ELIMINACIÓN / SOLO ESTA RONDA","VICTORIA / MOCAP SENTADO","ESPERA / MOCAP SENTADO","INSPECCIÓN / MOCAP SENTADO","DESCUBRIR COLLAR"}[stage];
  Intensity=stage>=3&&stage<=5?stage-2:0;
  int clip=stage==0?1:stage==1?2:stage==2||stage==3||stage==4?3:stage==6?5:stage==8?6:0;
  if(prior!=clip){if(action.IsValid()){graph.Disconnect(bodyMixer,1);graph.Disconnect(layers,2);action.Destroy();gesture.Destroy();}action=AnimationClipPlayable.Create(graph,clips[clip]);gesture=AnimationClipPlayable.Create(graph,clips[clip]);if(!graph.Connect(action,0,bodyMixer,1)||!graph.Connect(gesture,0,layers,2))throw new InvalidOperationException("Downloaded motion graph connection failed");prior=clip;}
  idle.SetTime(Mathf.Repeat(t,clips[0].length));
  // Keep the centered typing passage only; ping-pong with zero speed at both ends.
  float typingTime=1.05f+.65f*(1-Mathf.Cos(s*Mathf.PI/1.4f));
  action.SetTime(stage==2||stage==3||stage==4?typingTime:Mathf.Min(s*(stage==0?.65f:stage==8?.6f:stage==9?.45f:1),clips[clip].length-.033f));
  if(stage==1)action.SetTime(CollarSourceTime>=0?CollarSourceTime:6.6f+Mathf.Min(s,1.9f));
  gesture.SetTime(action.GetTime());
  bool upperOnly=stage==0||stage==1||stage==6||stage==8||stage==9;
  float blend=stage==5||upperOnly?0:Mathf.SmoothStep(0,1,Mathf.Clamp01(s/1.8f));bodyMixer.SetInputWeight(0,1-blend);bodyMixer.SetInputWeight(1,blend);layers.SetInputWeight(2,upperOnly?1:0);
  float envelope=Intensity==0?0:Mathf.SmoothStep(0,1,Mathf.Clamp01(s/.18f))*Mathf.SmoothStep(0,1,Mathf.Clamp01(((stage==5?6:3)-s)/.35f));
  electric.SetTime(Mathf.Min(Intensity==3?2+s:Mathf.Repeat(s*1.25f,2.5f),clips[4].length-.033f));layers.SetInputWeight(1,envelope*new[]{0f,.22f,.52f,1}[Intensity]);graph.Evaluate(.000001f);
  float transition=Mathf.SmoothStep(0,1,Mathf.Clamp01((Time.time-transitionAt)/(Intensity>0?.25f:1.8f)));
  if(stage!=0&&transition<1)for(int i=0;i<poseBones.Length;i++)poseBones[i].localRotation=Quaternion.Slerp(transitionPose[i],poseBones[i].localRotation,transition);
  var hips=Rig.GetBoneTransform(HumanBodyBones.Hips);transform.position+=Station.TransformPoint(new Vector3(0,.82f,-.97f))-hips.position;
  if(jaw!=null)jaw.localRotation=jawRest;
  var neck=Rig.GetBoneTransform(HumanBodyBones.Neck);var head=Rig.GetBoneTransform(HumanBodyBones.Head);var hardware=Collar.GetComponentInParent<CollarVisual>().transform;
  // A gentle gaze adaptation over seated idle, rather than the source disbelief's upward gesture.
  for(int i=0;i<neckBones.Length;i++){float angle=Quaternion.Angle(neckRest[i],neckBones[i].localRotation);neckBones[i].localRotation=Quaternion.Slerp(neckRest[i],neckBones[i].localRotation,angle>10?10/angle:1);}
  float headAngle=Quaternion.Angle(headRest,head.localRotation);head.localRotation=Quaternion.Slerp(headRest,head.localRotation,headAngle>28?28/headAngle:1);
  if(stage==9||stage==2){float look=Mathf.SmoothStep(0,1,Mathf.Clamp01(s/1.8f));head.rotation=Quaternion.AngleAxis((stage==9?24:18)*look,Station.right)*head.rotation;}
  hardware.GetComponent<ImportedSpaceCollar>().SamplePose();
  if(stage==2||stage==3||stage==4){ContactHand(false);ContactHand(true);if(stage==2&&s<2.4f){ApproachKeyboard(false,s);ApproachKeyboard(true,s);}}
  if(stage==0||stage==7||stage==8||stage==9){RestHand(false);RestHand(true);}
  // Preserve the downloaded two-hand grasp, adapting reach to the collar hardware.
  if(stage==1&&CollarSourceTime<0){float w=Mathf.SmoothStep(0,1,Mathf.Clamp01((s-.5f)/.8f));CollarGrip(false,w);CollarGrip(true,w);}
  else {gripReady[0]=gripReady[1]=false;}
  float gazeX=stage==8?Mathf.Sin(s*Mathf.PI/4)*2.5f:0;
  GazeTarget=stage==2||stage==3||stage==4?Station.Find("Integrated touch terminal").position:stage==9||stage==1?CollarCenter-Station.up*.10f:Station.TransformPoint(new Vector3(gazeX,1.45f,2.7f));
  for(int i=0;i<eyeBones.Length;i++)if(eyeBones[i]!=null){var eye=eyeBones[i];eye.localRotation=eyeRest[i];var forward=eye.TransformDirection(eyeForward[i]);var direction=(GazeTarget-eye.position).normalized;eye.rotation=Quaternion.RotateTowards(Quaternion.identity,Quaternion.FromToRotation(forward,direction),30)*eye.rotation;}
  var gazeLocal=Quaternion.Inverse(Quaternion.LookRotation(head.TransformDirection(headForward),head.TransformDirection(headUp)))*(GazeTarget-head.position).normalized;
  foreach(var skin in GetComponentsInChildren<SkinnedMeshRenderer>())for(int i=0;i<skin.sharedMesh.blendShapeCount;i++){
   string n=skin.sharedMesh.GetBlendShapeName(i).ToLowerInvariant();float blink=stage==0?1-Mathf.SmoothStep(0,1,Mathf.Clamp01((s-.7f)/2.2f)):Mathf.Clamp01(1-Mathf.Abs(Mathf.Repeat(t+1.8f,3.7f)-.13f)/.13f);
   float weight=n.EndsWith("a14_eye_blink_left")||n.EndsWith("a15_eye_blink_right")?blink*100:n.EndsWith("mouth_open")?Intensity*envelope*12:0;
   if(n.Contains("eye")&&n.Contains("look")){
    float horizontal=Mathf.Clamp(gazeLocal.x,-.28f,.28f)*180,vertical=Mathf.Clamp(gazeLocal.y,-.24f,.24f)*150;
    if(n.Contains("up"))weight=Mathf.Max(0,vertical);else if(n.Contains("down"))weight=Mathf.Max(0,-vertical);
    else if(n.Contains("out_left")||n.Contains("in_right"))weight=Mathf.Max(0,-horizontal);
    else if(n.Contains("in_left")||n.Contains("out_right"))weight=Mathf.Max(0,horizontal);
   }skin.SetBlendShapeWeight(i,weight);
  }
  var color=Intensity==3?new Color(1,.08f,.10f):Intensity==2?new Color(1,.5f,.08f):new Color(.3f,.8f,1);
  var fitting=Collar.GetComponentInParent<ImportedSpaceCollar>();fitting.SetDanger(color);arc.enabled=Intensity>0&&envelope>.05f;arc.widthMultiplier=.0015f*Intensity;
  var c=fitting.StatusCenter;for(int i=0;i<12;i++){float x=-.027f+i*.054f/11;arc.SetPosition(i,c+fitting.transform.TransformDirection(new Vector3(x,Mathf.Sin(i*2.5f+t*43)*.002f*Intensity,.004f)));}
 }
 void RestHand(bool right){
  // Furniture clearance: relaxed hands remain behind the desk's front edge, on the lap.
  var hand=Rig.GetBoneTransform(right?HumanBodyBones.RightHand:HumanBodyBones.LeftHand);
  var local=Station.InverseTransformPoint(hand.position);
  if(local.z>-.78f&&local.y<1.28f){float weight=Mathf.SmoothStep(0,1,1-Mathf.InverseLerp(.98f,1.28f,local.y));SolveArm(right,Vector3.Lerp(hand.position,Station.TransformPoint(new Vector3(right?.20f:-.20f,.84f,-.86f)),weight));}
 }
 void ApproachKeyboard(bool right,float seconds){
  var hand=Rig.GetBoneTransform(right?HumanBodyBones.RightHand:HumanBodyBones.LeftHand);
  var lap=Station.TransformPoint(new Vector3(right?.20f:-.20f,.84f,-.86f));
  var clearance=Station.TransformPoint(new Vector3(right?.20f:-.20f,1.15f,-.80f));
  var target=seconds<1.2f?Vector3.Lerp(lap,clearance,Mathf.SmoothStep(0,1,seconds/1.2f)):Vector3.Lerp(clearance,hand.position,Mathf.SmoothStep(0,1,(seconds-1.2f)/1.2f));
  SolveArm(right,target);
 }
 void CacheHandSurfaces(){
  foreach(var skin in GetComponentsInChildren<SkinnedMeshRenderer>()){
   var weights=skin.sharedMesh.boneWeights;var indices=new List<int>[] {new List<int>(),new List<int>()};
   for(int side=0;side<2;side++){
    var hand=Rig.GetBoneTransform(side==1?HumanBodyBones.RightHand:HumanBodyBones.LeftHand);
    var allowed=new HashSet<int>();for(int b=0;b<skin.bones.Length;b++)if(skin.bones[b]!=null&&(skin.bones[b]==hand||skin.bones[b].IsChildOf(hand)))allowed.Add(b);
    for(int i=0;i<weights.Length;i++){var v=weights[i];float sum=(allowed.Contains(v.boneIndex0)?v.weight0:0)+(allowed.Contains(v.boneIndex1)?v.weight1:0)+(allowed.Contains(v.boneIndex2)?v.weight2:0)+(allowed.Contains(v.boneIndex3)?v.weight3:0);if(sum>.5f)indices[side].Add(i);}
   }
   if(indices[0].Count+indices[1].Count>0)handSurfaces.Add(new HandSurface{skin=skin,vertices=skin.sharedMesh.vertices,weights=weights,bind=skin.sharedMesh.bindposes,matrices=new Matrix4x4[skin.bones.Length],indices=new[]{indices[0].ToArray(),indices[1].ToArray()}});
  }
 }
 void UpdateMatrices(HandSurface surface){for(int i=0;i<surface.matrices.Length;i++)surface.matrices[i]=surface.skin.bones[i].localToWorldMatrix*surface.bind[i];}
 Vector3 WorldVertex(HandSurface surface,int index){var v=surface.vertices[index];var w=surface.weights[index];return surface.matrices[w.boneIndex0].MultiplyPoint3x4(v)*w.weight0+surface.matrices[w.boneIndex1].MultiplyPoint3x4(v)*w.weight1+surface.matrices[w.boneIndex2].MultiplyPoint3x4(v)*w.weight2+surface.matrices[w.boneIndex3].MultiplyPoint3x4(v)*w.weight3;}
 void ContactHand(bool right){
  var hand=Rig.GetBoneTransform(right?HumanBodyBones.RightHand:HumanBodyBones.LeftHand);var finger=Rig.GetBoneTransform(right?HumanBodyBones.RightMiddleDistal:HumanBodyBones.LeftMiddleDistal);
  if(finger==null)return;
  var knuckle=Rig.GetBoneTransform(right?HumanBodyBones.RightMiddleProximal:HumanBodyBones.LeftMiddleProximal);
  if(knuckle!=null){var palm=knuckle.position-hand.position;var flat=Vector3.ProjectOnPlane(palm,Station.up);if(flat.sqrMagnitude>.00001f){var desired=flat-Station.up*(flat.magnitude*.12f);hand.rotation=Quaternion.FromToRotation(palm,desired)*hand.rotation;}}
  var point=Station.InverseTransformPoint(finger.position);var terminal=Station.Find("Integrated touch terminal");
  // Adapt reach; use the actual lowest skinned hand vertex, not a fingertip bone inside the mesh.
  var correction=Station.TransformVector(new Vector3(0,0,Mathf.Clamp(point.z,terminal.localPosition.z-.27f,terminal.localPosition.z-.17f)-point.z));SolveArm(right,hand.position+correction);
  for(int pass=0;pass<2;pass++){
   float lowest=float.PositiveInfinity;
   foreach(var surface in handSurfaces){UpdateMatrices(surface);foreach(int i in surface.indices[right?1:0])lowest=Mathf.Min(lowest,Station.InverseTransformPoint(WorldVertex(surface,i)).y);}
   if(float.IsInfinity(lowest))return;float delta=terminal.localPosition.y+.012f-lowest;
   if(Mathf.Abs(delta)<.001f)return;SolveArm(right,hand.position+Station.up*delta);
  }
 }
 void CollarGrip(bool right,float weight){
  var hand=Rig.GetBoneTransform(right?HumanBodyBones.RightHand:HumanBodyBones.LeftHand);
  var pad=Rig.GetBoneTransform(right?HumanBodyBones.RightMiddleDistal:HumanBodyBones.LeftMiddleDistal);if(pad==null)return;
  var fitting=Collar.GetComponentInParent<ImportedSpaceCollar>();int side=right?1:0;
  if(!gripReady[side]){gripRotation[side]=Quaternion.Inverse(fitting.transform.rotation)*hand.rotation;gripReady[side]=true;}
  hand.rotation=Quaternion.Slerp(hand.rotation,fitting.transform.rotation*gripRotation[side],weight);
  var anchor=fitting.RingCenter+fitting.transform.right*(right?.078f:-.078f)+fitting.transform.forward*.035f+fitting.transform.up*.027f;
  // One stable finger pad supports the upper edge. No per-frame nearest-vertex switching.
  SolveArm(right,Vector3.Lerp(hand.position,hand.position+anchor-pad.position,weight));
 }
 void SolveArm(bool right,Vector3 target){
  var upper=Rig.GetBoneTransform(right?HumanBodyBones.RightUpperArm:HumanBodyBones.LeftUpperArm);var lower=Rig.GetBoneTransform(right?HumanBodyBones.RightLowerArm:HumanBodyBones.LeftLowerArm);var end=Rig.GetBoneTransform(right?HumanBodyBones.RightHand:HumanBodyBones.LeftHand);var wrist=end.rotation;
  Vector3 a=upper.position;float l1=Vector3.Distance(a,lower.position),l2=Vector3.Distance(lower.position,end.position);Vector3 direction=target-a;float d=Mathf.Clamp(direction.magnitude,Mathf.Abs(l1-l2)+.001f,l1+l2-.001f);direction.Normalize();float along=(l1*l1-l2*l2+d*d)/(2*d);float height=Mathf.Sqrt(Mathf.Max(0,l1*l1-along*along));
  Vector3 pole=lower.position-(a+end.position)*.5f;Vector3 bend=Vector3.ProjectOnPlane(pole,direction).normalized;if(bend.sqrMagnitude<.01f)bend=Station.right*(right?1:-1);Vector3 elbow=a+direction*along+bend*height;
  upper.rotation=Quaternion.FromToRotation(lower.position-upper.position,elbow-upper.position)*upper.rotation;lower.rotation=Quaternion.FromToRotation(end.position-lower.position,target-lower.position)*lower.rotation;end.rotation=wrist;
 }
 void OnDestroy(){if(graph.IsValid())graph.Destroy();if(arcMaterial!=null)Destroy(arcMaterial);if(arc!=null)Destroy(arc.gameObject);}
}
}
