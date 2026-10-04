using System;using System.Linq;using UnityEngine;

namespace LastKey {

public static class CharacterFactory {

static readonly string[] Models={"rain","snow","snow","snow"};

public static Transform CreateStanding(Transform parent,int index,Color accent,out Animator animator){

var root=new GameObject(new[]{"CIPHER_Valeria_Rojas","UPLINK_Ethan_Cole","RELAY_Akira_Mori","VECTOR_Noah_Bennett"}[index]);root.transform.SetParent(parent,false);

var prefab=Resources.Load<GameObject>("VisualCharacters/"+Models[index]);if(prefab==null)throw new InvalidOperationException("Blender Studio visual asset missing "+Models[index]);var body=UnityEngine.Object.Instantiate(prefab,root.transform);body.name="Blender Studio imported modern outfit";

animator=body.GetComponentInChildren<Animator>();if(animator==null||animator.avatar==null||!animator.avatar.isValid||!animator.avatar.isHuman)throw new InvalidOperationException("Modular Humanoid avatar invalid: "+Models[index]);animator.enabled=false;





var renderers=body.GetComponentsInChildren<SkinnedMeshRenderer>().Where(r=>r.enabled).ToArray();Bounds bounds=renderers[0].bounds;foreach(var r in renderers)bounds.Encapsulate(r.bounds);

float scale=new[]{1.73f,1.81f,1.76f,1.85f}[index]/bounds.size.y;root.transform.localScale=Vector3.one*scale;

foreach(var r in renderers){r.updateWhenOffscreen=true;
 var mats=r.materials;
 foreach(var m in mats){m.SetColor("_ShadowMapColor",new Color(.43f,.48f,.60f));m.SetFloat("_OutlineWidth",.25f);string n=m.name.ToLowerInvariant();if(n.Contains("top")||n.Contains("shirt")||n.Contains("pants")||n.Contains("jeans"))m.SetShaderPassEnabled("Outline",false);
 if(n.Contains("top")||n.Contains("shirt"))m.SetColor("_BaseColor",accent*.7f);
 else if(!n.Contains("pant")&&!n.Contains("jean")&&!n.Contains("shoe")&&!n.Contains("eye")&&(n.Contains("body")||n.Contains("hands")||n.Contains("head")))m.SetColor("_BaseColor",new[]{new Color(.68f,.4f,.27f),new Color(.82f,.58f,.4f),new Color(.72f,.51f,.36f),new Color(.35f,.19f,.13f)}[index]);
 else if(n.Contains("hair"))m.SetColor("_BaseColor",new[]{new Color(.08f,.045f,.025f),new Color(.3f,.12f,.055f),new Color(.025f,.035f,.055f),new Color(.05f,.03f,.025f)}[index]);
 }
}
root.transform.localScale=new Vector3(scale*new[]{1f,1.03f,.94f,1.13f}[index],scale,scale);

var head=animator.GetBoneTransform(HumanBodyBones.Head);
if(index==1||index==2){foreach(var r in renderers)if(r.name.ToLowerInvariant().Contains("hair"))r.enabled=false;
 var hair=VisualProps.Fit(index==1?"Hair_SimpleParted":"Hair_Buzzed",head,head.InverseTransformPoint(head.position+Vector3.up*(index==1?.13f:.11f)),index==1?new Vector3(.285f,.26f,.295f):new Vector3(.275f,.22f,.285f),index==1?new Color(.27f,.12f,.055f):new Color(.025f,.035f,.055f));
 hair.name="Imported Quaternius hairstyle";}
if(index==3){var beard=VisualProps.Fit("Hair_Beard",head,head.InverseTransformPoint(head.position+Vector3.up*.055f+Vector3.forward*.085f),new Vector3(.20f,.12f,.15f),new Color(.035f,.02f,.016f));beard.name="Imported Quaternius beard";}
// Original free garments and hair meshes; no clothing modelled from scratch.

var hips=animator.GetBoneTransform(HumanBodyBones.Hips);root.transform.localPosition=new Vector3(0,-bounds.min.y*scale,0);

return root.transform;

}

public static Transform Create(Transform station,int index,Color accent,out Renderer collar){

var root=CreateStanding(station,index,accent,out var animator);var hips=animator.GetBoneTransform(HumanBodyBones.Hips);root.localPosition+=station.InverseTransformVector(station.TransformPoint(new Vector3(0,.66f,-.97f))-hips.position);

collar=AttachCollar(root,animator,station,index,accent);

var motion=root.gameObject.AddComponent<CharacterMotion>();motion.Initialize(animator,index,station);return root;

}

public static Renderer AttachCollar(Transform root,Animator animator,Transform station,int index,Color accent){
var neckBone=animator.GetBoneTransform(HumanBodyBones.Neck)??animator.GetBoneTransform(HumanBodyBones.Head);
var hardware=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("VisualProps/collar"),station);
hardware.name="Imported tracking collar / Danger";
var renderers=hardware.GetComponentsInChildren<Renderer>();var bounds=renderers[0].bounds;foreach(var r in renderers)bounds.Encapsulate(r.bounds);
// Fit the ring around the actual imported neck surface, not below the neck joint.
var headBone=animator.GetBoneTransform(HumanBodyBones.Head);
var neckCenter=Vector3.Lerp(neckBone.position,headBone.position,index==0?.38f:.50f);
var skinBounds=new Bounds(neckCenter,Vector3.zero);bool foundSkin=false;
foreach(var skin in root.GetComponentsInChildren<SkinnedMeshRenderer>()){
 if(!skin.enabled||skin.sharedMesh==null||!skin.sharedMesh.isReadable)continue;
 var mesh=skin.sharedMesh;var vertices=mesh.vertices;var weights=mesh.boneWeights;var binds=mesh.bindposes;var bones=skin.bones;
 for(int n=0;n<vertices.Length;n++){var w=weights[n];var v=vertices[n];
  var world=(bones[w.boneIndex0].localToWorldMatrix*binds[w.boneIndex0]).MultiplyPoint3x4(v)*w.weight0;
  if(w.weight1>0)world+=(bones[w.boneIndex1].localToWorldMatrix*binds[w.boneIndex1]).MultiplyPoint3x4(v)*w.weight1;
  if(w.weight2>0)world+=(bones[w.boneIndex2].localToWorldMatrix*binds[w.boneIndex2]).MultiplyPoint3x4(v)*w.weight2;
  if(w.weight3>0)world+=(bones[w.boneIndex3].localToWorldMatrix*binds[w.boneIndex3]).MultiplyPoint3x4(v)*w.weight3;
  var offset=world-neckCenter;
  if(Mathf.Abs(offset.y)<.012f&&new Vector2(offset.x,offset.z).magnitude<.105f){if(!foundSkin){skinBounds=new Bounds(world,Vector3.zero);foundSkin=true;}else skinBounds.Encapsulate(world);}
 }
}
var ring=renderers.First(r=>r.name.EndsWith("_0"));
// The source light sat above the antenna housing. Mount it flush on the band.
foreach(var part in renderers)if(part.name.EndsWith("_2")){var local=part.transform.localPosition;local.y=ring.transform.localPosition.y;part.transform.localPosition=local;}
var fitCenter=foundSkin?new Vector3(skinBounds.center.x,neckCenter.y,skinBounds.center.z):neckCenter;
var diameter=foundSkin?new Vector3(Mathf.Clamp(skinBounds.size.x+.010f,.10f,new[]{.115f,.16f,.15f,.175f}[index]),.024f,Mathf.Clamp(skinBounds.size.z+.010f,.10f,new[]{.115f,.145f,.14f,.15f}[index])):new Vector3(index==0?.12f:.135f,.024f,index==0?.12f:.135f);
// Imported torus has an inner diameter ~83% of its outer diameter.
hardware.transform.localScale=Vector3.Scale(hardware.transform.localScale,new Vector3(diameter.x/ .83f/ring.bounds.size.x,diameter.y/ring.bounds.size.y,diameter.z/ .83f/ring.bounds.size.z));
hardware.transform.position+=fitCenter-ring.bounds.center;
// This source part includes a protruding antenna; omit it to keep the jaw clear.
foreach(var part in renderers)if(part.name.EndsWith("_1"))part.enabled=false;
Debug.Log("LAST_KEY_COLLAR_FIT "+index+" neck="+fitCenter+" measured="+foundSkin+" inner="+diameter);
hardware.transform.SetParent(neckBone,true);
Material indicator=null;
foreach(var r in renderers){var mats=r.sharedMaterials;for(int j=0;j<mats.Length;j++){
 var source=mats[j];Color color=source==null?Color.gray:source.color;
 bool light=(source!=null&&source.name.ToLowerInvariant().Contains("glow"))||(color.g>.45f&&color.b>.35f&&color.r<color.g*.75f);
 mats[j]=VisualProps.Surface(light?accent:new Color(.09f,.12f,.16f),light);
 if(light)indicator=mats[j];
}r.sharedMaterials=mats;}
if(indicator==null){indicator=renderers[renderers.Length-1].material;indicator.SetFloat("_UseEmission",1);}
var collarVisual=hardware.AddComponent<CollarVisual>();collarVisual.Initialize(indicator);return ring;


}

static Material Material(Color tint,bool glow=false){var m=new Material(Shader.Find("Universal Render Pipeline/Lit"));m.color=tint;m.SetFloat("_Smoothness",.22f);if(glow){m.EnableKeyword("_EMISSION");m.SetColor("_EmissionColor",tint*1.2f);}return m;}

}}


