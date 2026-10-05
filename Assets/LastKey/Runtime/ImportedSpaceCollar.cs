using System;
using System.Linq;
using UnityEngine;
namespace LastKey {
// Existing CC BY mesh: Infernalight / the space collar. No generated collar geometry.
public sealed class ImportedSpaceCollar:MonoBehaviour {
 bool bound;Animator rig;Transform station;Renderer padding,statusScreen;MeshCollider[] surfaces;Vector3 paddingLocalCenter;
 readonly System.Collections.Generic.List<Material> indicators=new System.Collections.Generic.List<Material>();
 public Vector3 StatusCenter => statusScreen.bounds.center;
 public void SetDanger(Color color){foreach(var material in indicators){material.color=color;material.SetColor("_EmissionColor",color*1.3f);}}
 public Renderer Casing {get;private set;}
 public Vector3 RingCenter => transform.TransformPoint(paddingLocalCenter);
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
  var bounds=fit.padding.bounds;obj.transform.localScale=Vector3.Scale(obj.transform.localScale,new Vector3(.15f/bounds.size.x,.042f/bounds.size.y,.15f/bounds.size.z));
  fit.paddingLocalCenter=obj.transform.InverseTransformPoint(fit.padding.bounds.center);
  // Omit the source's rear projecting tag plate, which conflicts with this jacket.
  foreach(var renderer in renderers)if(renderer.name=="Material3.004"||renderer.name=="Material2.010")renderer.gameObject.SetActive(false);
  fit.surfaces=renderers.Where(r=>r.gameObject.activeSelf).Select(r=>{var collider=r.gameObject.AddComponent<MeshCollider>();collider.sharedMesh=r.GetComponent<MeshFilter>().sharedMesh;return collider;}).ToArray();
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
  obj.AddComponent<CollarVisual>().Initialize(status);fit.SamplePose();obj.transform.SetParent(rig.GetBoneTransform(HumanBodyBones.Neck),true);fit.bound=true;return fit.Casing;
 }
 public Vector3 GripContact(bool right){
  Physics.SyncTransforms();var lateral=transform.right*(right?1:-1);var destination=RingCenter+transform.forward*.04f;
  var ray=new Ray(destination+lateral*.3f,-lateral);float nearest=float.PositiveInfinity;Vector3 contact=destination+lateral*.085f;
  foreach(var surface in surfaces)if(surface.Raycast(ray,out var hit,.3f)&&hit.distance<nearest){nearest=hit.distance;contact=hit.point+lateral*.003f;}
  return contact;
 }
 public void SamplePose(){
  var neck=rig.GetBoneTransform(HumanBodyBones.Neck);var head=rig.GetBoneTransform(HumanBodyBones.Head);
  // Follow the anatomical neck axis after the final blended pose, not its initial bone rotation.
  var up=(head.position-neck.position).normalized;var forward=Vector3.ProjectOnPlane(station.forward,up).normalized;
  transform.rotation=Quaternion.LookRotation(forward,up);
  transform.position=Vector3.Lerp(neck.position,head.position,.32f)-transform.TransformVector(paddingLocalCenter);
 }
}
}
