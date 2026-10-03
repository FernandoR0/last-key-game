using System;using System.Linq;using UnityEngine;

namespace LastKey {

public static class CharacterFactory {

static readonly string[] Models={"Women_Casual","Men_Casual_Hoodie","Women_Suit","Men_Worker"};

public static Transform CreateStanding(Transform parent,int index,Color accent,out Animator animator){

var root=new GameObject(new[]{"CIPHER_Valeria_Rojas","UPLINK_Ethan_Cole","RELAY_Akira_Mori","VECTOR_Noah_Bennett"}[index]);root.transform.SetParent(parent,false);

var prefab=Resources.Load<GameObject>("ModularCharacters/"+Models[index]+"_Humanoid");if(prefab==null)throw new InvalidOperationException("Clothed Quaternius asset missing "+Models[index]);var body=UnityEngine.Object.Instantiate(prefab,root.transform);body.name="Quaternius imported modern outfit";

animator=body.GetComponentInChildren<Animator>();if(animator==null||animator.avatar==null||!animator.avatar.isValid||!animator.avatar.isHuman)throw new InvalidOperationException("Modular Humanoid avatar invalid: "+Models[index]);animator.enabled=false;

if(index==2||index==3)ReplacePart(body,index==2?"Women_Punk":"Men_Suit","Head");

if(index==1)ReplacePart(body,"Men_Casual_2","Legs");

var renderers=body.GetComponentsInChildren<SkinnedMeshRenderer>().Where(r=>r.enabled).ToArray();Bounds bounds=renderers[0].bounds;foreach(var r in renderers)bounds.Encapsulate(r.bounds);

float scale=new[]{1.73f,1.80f,1.69f,1.84f}[index]/bounds.size.y;root.transform.localScale=Vector3.one*scale;

foreach(var r in renderers){r.sharedMaterials=r.sharedMaterials.Select(m=>Recolor(m,r.name,index,accent)).ToArray();r.updateWhenOffscreen=true;}

// Imported garment meshes, no vertex reshaping, generated clothing, cuffs or painted body mesh.

var hips=animator.GetBoneTransform(HumanBodyBones.Hips);root.transform.localPosition=new Vector3(0,-bounds.min.y*scale,0);

return root.transform;

}

public static Transform Create(Transform station,int index,Color accent,out Renderer collar){

var root=CreateStanding(station,index,accent,out var animator);var hips=animator.GetBoneTransform(HumanBodyBones.Hips);root.localPosition+=station.InverseTransformVector(station.TransformPoint(new Vector3(0,.70f,-.97f))-hips.position);

var neck=GameObject.CreatePrimitive(PrimitiveType.Cylinder);neck.name="Danger collar";neck.transform.position=animator.GetBoneTransform(HumanBodyBones.Head).position-Vector3.up*.065f;neck.transform.localScale=new Vector3(.13f,.012f,.13f);neck.transform.SetParent(animator.GetBoneTransform(HumanBodyBones.Head),true);collar=neck.GetComponent<Renderer>();collar.sharedMaterial=Material(accent,true);UnityEngine.Object.Destroy(neck.GetComponent<Collider>());

var motion=root.gameObject.AddComponent<CharacterMotion>();motion.Initialize(animator,index,station);return root;

}

static void ReplacePart(GameObject body,string name,string part){

var original=body.GetComponentsInChildren<SkinnedMeshRenderer>().First(r=>r.name.Contains(part));original.enabled=false;

var source=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("ModularCharacters/"+name+"_Humanoid"));var head=source.GetComponentsInChildren<SkinnedMeshRenderer>().First(r=>r.name.Contains(part));var map=body.GetComponentsInChildren<Transform>().GroupBy(t=>t.name).ToDictionary(g=>g.Key,g=>g.First());head.bones=head.bones.Select(t=>map[t.name]).ToArray();head.rootBone=map[head.rootBone.name];head.transform.SetParent(body.transform,false);head.transform.localPosition=Vector3.zero;head.transform.localRotation=Quaternion.identity;head.transform.localScale=Vector3.one;UnityEngine.Object.Destroy(source);

}

static Material Material(Color tint,bool glow=false){var m=new Material(Shader.Find("Universal Render Pipeline/Lit"));m.color=tint;m.SetFloat("_Smoothness",.22f);if(glow){m.EnableKeyword("_EMISSION");m.SetColor("_EmissionColor",tint*1.2f);}return m;}

static Material Recolor(Material source,string mesh,int i,Color accent){string n=source==null?"":source.name.ToLowerInvariant();Color c=source!=null?source.color:Color.gray;

if(n.Contains("skin"))c=new[]{new Color(.76f,.52f,.38f),new Color(.89f,.70f,.54f),new Color(.84f,.64f,.49f),new Color(.38f,.23f,.16f)}[i];

else if(n.Contains("eye")&&!n.Contains("brow"))c=new Color(.13f,.09f,.065f);

else if(n.Contains("hair")||n.Contains("brow")||n.Contains("moustache"))c=new[]{new Color(.13f,.075f,.05f),new Color(.36f,.22f,.10f),new Color(.045f,.035f,.04f),new Color(.035f,.025f,.02f)}[i];

else if(mesh.Contains("Body"))c=n.Contains("white")&&i==2?new Color(.68f,.76f,.80f):n.Contains("vest")?new Color(.07f,.10f,.13f):accent*.65f;

else if(mesh.Contains("Legs"))c=new[]{new Color(.045f,.11f,.14f),new Color(.10f,.13f,.19f),new Color(.08f,.10f,.14f),new Color(.19f,.23f,.17f)}[i];

else if(mesh.Contains("Feet"))c=n.Contains("white")?new Color(.66f,.70f,.72f):new Color(.055f,.06f,.065f);

else if(n.Contains("yellow"))c=accent*.55f;

return Material(c);}

}}



