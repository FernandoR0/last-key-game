using System;
using System.Linq;
using UnityEngine;
namespace LastKey {
// Existing CC BY mesh: Infernalight / the space collar. No generated collar geometry.
public sealed class ImportedSpaceCollar:MonoBehaviour {
 Animator rig;Transform station;Renderer padding,statusScreen;Vector3 paddingLocalCenter;
 Transform[] skinBones;Matrix4x4[] anchorBind;BoneWeight ringWeight,statusWeight;Vector3 ringBind,statusBind;
 readonly System.Collections.Generic.List<Material> indicators=new System.Collections.Generic.List<Material>();
 public Vector3 StatusCenter => DeformPoint(statusBind,statusWeight);
 public void SetDanger(Color color){foreach(var material in indicators){material.color=color;material.SetColor("_EmissionColor",color*1.3f);}}
 public Renderer Casing {get;private set;}
 public Vector3 RingCenter => DeformPoint(ringBind,ringWeight);
 public static Renderer Attach(Animator rig,Transform station){
  var source=Resources.Load<GameObject>("VisualProps/SpaceCollar/SpaceCollar");if(source==null)throw new InvalidOperationException("Downloaded SpaceCollar is missing");
  var obj=new GameObject("Infernalight imported metallic control collar");obj.transform.SetParent(station,false);
  // Preserve the FBX coordinate conversion on the child; anatomical pose belongs to a separate fitting root.
  var imported=Instantiate(source,obj.transform);
  var fit=obj.AddComponent<ImportedSpaceCollar>();fit.rig=rig;fit.station=station;
  var renderers=obj.GetComponentsInChildren<MeshRenderer>();
  fit.padding=renderers.First(r=>r.sharedMaterials.Any(m=>m.name.Contains("Inner polymer")));
  fit.Casing=renderers.First(r=>r.sharedMaterials.Any(m=>m.name.Contains("Graphite")||m.name.Contains("Brushed steel")));
  // Imported GLB/FBX may retain Z as the thin axis. Align the source ring plane before measuring its fit.
  var originalBounds=fit.padding.bounds;
  if(originalBounds.size.z<originalBounds.size.y&&originalBounds.size.z<originalBounds.size.x)imported.transform.localRotation=Quaternion.AngleAxis(-90,Vector3.right)*imported.transform.localRotation;
  
  // The internal cushion defines the ring, so asymmetric control modules do not offset the neck fit.
  var bounds=fit.padding.bounds;obj.transform.localScale=Vector3.Scale(obj.transform.localScale,new Vector3(.19f/bounds.size.x,.052f/bounds.size.y,.19f/bounds.size.z));
  fit.paddingLocalCenter=obj.transform.InverseTransformPoint(fit.padding.bounds.center);
  // Omit the source's rear projecting tag plate, which conflicts with this jacket.
  foreach(var renderer in renderers)if(renderer.name=="Material3.004"||renderer.name=="Material2.010")renderer.gameObject.SetActive(false);
  Material status=null;var materialCache=new System.Collections.Generic.Dictionary<string,Material>();
  foreach(var renderer in renderers){var materials=renderer.sharedMaterials;for(int j=0;j<materials.Length;j++){
   string name=materials[j].name;if(materialCache.TryGetValue(name,out var cached)){materials[j]=cached;continue;}var m=new Material(Shader.Find("Universal Render Pipeline/Lit"));m.name=name;
   bool light=name.Contains("Danger status");bool cushion=name.Contains("Inner polymer");
   var color=light?new Color(.025f,.32f,.38f):cushion?new Color(.025f,.035f,.048f):name.Contains("Control")?new Color(.04f,.16f,.19f):name.Contains("Graphite")?new Color(.085f,.11f,.14f):new Color(.30f,.36f,.42f);
   m.color=color;m.SetFloat("_Metallic",cushion?0:light?.1f:.85f);m.SetFloat("_Smoothness",cushion?.18f:.62f);
   if(light){m.EnableKeyword("_EMISSION");m.SetColor("_EmissionColor",new Color(.06f,.65f,.8f));status=m;fit.indicators.Add(m);if(fit.statusScreen==null||renderer.name=="Material3.002")fit.statusScreen=renderer;}
   materialCache[name]=m;materials[j]=m;
  }renderer.sharedMaterials=materials;}
  if(status==null)throw new InvalidOperationException("New collar has no status screen material");
  var neck=rig.GetBoneTransform(HumanBodyBones.Neck);var head=rig.GetBoneTransform(HumanBodyBones.Head);
  obj.transform.rotation=Quaternion.LookRotation(station.forward,station.up);
  // Low neckline fit: the jacket naturally covers the rear hardware, while the
  // front remains below the chin. Do not modify the face or the garment mesh.
  obj.transform.position=Vector3.Lerp(neck.position,head.position,.08f)-obj.transform.TransformVector(fit.paddingLocalCenter);
  fit.BindToNeckSkin(renderers);
  obj.AddComponent<CollarVisual>().Initialize(status);return fit.Casing;
 }
 public Vector3 GripContact(bool right){
  return RingCenter+station.right*(right?.09f:-.09f)+station.forward*.04f;
 }
 public void SamplePose(){} // Animator skinning owns the attachment; no separate pose override.
 void BindToNeckSkin(MeshRenderer[] renderers){
  var body=rig.GetComponentsInChildren<SkinnedMeshRenderer>().First(s=>s.sharedMesh.boneWeights.Length==s.sharedMesh.vertexCount);
  skinBones=body.bones;anchorBind=skinBones.Select(b=>b.worldToLocalMatrix).ToArray();
  var vertices=body.sharedMesh.vertices;var weights=body.sharedMesh.boneWeights;
  var candidates=new System.Collections.Generic.List<int>();
  var neck=rig.GetBoneTransform(HumanBodyBones.Neck);var head=rig.GetBoneTransform(HumanBodyBones.Head);
  var neckSet=new System.Collections.Generic.HashSet<int>();for(int b=0;b<skinBones.Length;b++)if(skinBones[b].name.Contains("Neck")||skinBones[b]==head)neckSet.Add(b);
  var points=vertices.Select(v=>body.transform.TransformPoint(v)).ToArray();
  float bottom=Vector3.Dot(neck.position,station.up)-.012f,top=Vector3.Dot(head.position,station.up)+.012f;
  for(int i=0;i<points.Length;i++){var w=weights[i];float sum=(neckSet.Contains(w.boneIndex0)?w.weight0:0)+(neckSet.Contains(w.boneIndex1)?w.weight1:0)+(neckSet.Contains(w.boneIndex2)?w.weight2:0)+(neckSet.Contains(w.boneIndex3)?w.weight3:0);float h=Vector3.Dot(points[i],station.up);if(sum>.45f&&h>=bottom&&h<=top)candidates.Add(i);}
  if(candidates.Count<8)throw new InvalidOperationException("No valid neck surface for collar skin attachment");
  BoneWeight WeightAt(Vector3 point){
   // Blend the nearest four neck-surface weights rather than changing weights at mesh seams.
   var nearest=candidates.OrderBy(i=>(points[i]-point).sqrMagnitude).Take(4).ToArray();
   var contributions=new System.Collections.Generic.Dictionary<int,float>();
   foreach(int i in nearest){float amount=1/Mathf.Max(.00001f,(points[i]-point).sqrMagnitude);var w=weights[i];
    void Add(int bone,float weight){if(weight<=0)return;contributions.TryGetValue(bone,out float value);contributions[bone]=value+weight*amount;}
    Add(w.boneIndex0,w.weight0);Add(w.boneIndex1,w.weight1);Add(w.boneIndex2,w.weight2);Add(w.boneIndex3,w.weight3);
   }
   var strongest=contributions.OrderByDescending(p=>p.Value).Take(4).ToArray();float norm=strongest.Sum(p=>p.Value);
   var result=new BoneWeight();if(strongest.Length>0){result.boneIndex0=strongest[0].Key;result.weight0=strongest[0].Value/norm;}if(strongest.Length>1){result.boneIndex1=strongest[1].Key;result.weight1=strongest[1].Value/norm;}if(strongest.Length>2){result.boneIndex2=strongest[2].Key;result.weight2=strongest[2].Value/norm;}if(strongest.Length>3){result.boneIndex3=strongest[3].Key;result.weight3=strongest[3].Value/norm;}return result;
  }
  ringBind=transform.TransformPoint(paddingLocalCenter);ringWeight=WeightAt(ringBind);statusBind=statusScreen.bounds.center;statusWeight=WeightAt(statusBind);
  foreach(var source in renderers.Where(r=>r.gameObject.activeSelf)){
   var mesh=Instantiate(source.GetComponent<MeshFilter>().sharedMesh);mesh.name+=" / neck skin attachment";
   // User-approved front-only cuff: discard rear triangles hidden by the jacket.
   // The downloaded source remains intact; this runtime copy cannot protrude behind it.
   var localVertices=mesh.vertices;var front=localVertices.Select(v=>Vector3.Dot(source.transform.TransformPoint(v)-ringBind,station.forward)>=-.018f).ToArray();
   for(int sub=0;sub<mesh.subMeshCount;sub++){var triangles=mesh.GetTriangles(sub);var retained=new System.Collections.Generic.List<int>();for(int i=0;i<triangles.Length;i+=3)if(front[triangles[i]]&&front[triangles[i+1]]&&front[triangles[i+2]]){retained.Add(triangles[i]);retained.Add(triangles[i+1]);retained.Add(triangles[i+2]);}mesh.SetTriangles(retained,sub);}
   // The liner/band can follow the neck's skin. Small solid modules share one
   // attachment weight across their vertices, preserving their shape.
   var centreWeight=WeightAt(source.bounds.center);bool band=source==padding||source==Casing;
   mesh.boneWeights=mesh.vertices.Select(v=>band?WeightAt(source.transform.TransformPoint(v)):centreWeight).ToArray();
   mesh.bindposes=skinBones.Select(b=>b.worldToLocalMatrix*source.transform.localToWorldMatrix).ToArray();
   var materials=source.sharedMaterials;var owner=source.gameObject;bool casing=source==Casing,cushion=source==padding,screen=source==statusScreen;
   // Unity replaces the MeshRenderer when adding a SkinnedMeshRenderer.
   // Cache its materials and role before that replacement invalidates the source.
   var skin=owner.AddComponent<SkinnedMeshRenderer>();skin.bones=skinBones;skin.sharedMesh=mesh;skin.sharedMaterials=materials;skin.rootBone=rig.GetBoneTransform(HumanBodyBones.Hips);skin.updateWhenOffscreen=true;skin.localBounds=mesh.bounds;
   if(casing)Casing=skin;if(cushion)padding=skin;if(screen)statusScreen=skin;
  }
  Debug.Log("COLLAR_NECK_SKIN_BOUND samples="+candidates.Count+" bones="+skinBones.Length);
 }
 Vector3 DeformPoint(Vector3 bindPoint,BoneWeight w){
  return skinBones[w.boneIndex0].TransformPoint(anchorBind[w.boneIndex0].MultiplyPoint3x4(bindPoint))*w.weight0+skinBones[w.boneIndex1].TransformPoint(anchorBind[w.boneIndex1].MultiplyPoint3x4(bindPoint))*w.weight1+skinBones[w.boneIndex2].TransformPoint(anchorBind[w.boneIndex2].MultiplyPoint3x4(bindPoint))*w.weight2+skinBones[w.boneIndex3].TransformPoint(anchorBind[w.boneIndex3].MultiplyPoint3x4(bindPoint))*w.weight3;
 }
}
}
