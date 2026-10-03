using System;using System.Linq;using UnityEngine;

namespace LastKey {

public static class CharacterFactory {

static readonly string[] Models={"rp_carla_rigged_001_u3d","rp_eric_rigged_001_u3d","rp_manuel_animated_001_dancing","rp_nathan_animated_003_walking"};

public static Transform CreateStanding(Transform parent,int index,Color accent,out Animator animator){

var root=new GameObject(new[]{"CIPHER_Valeria_Rojas","UPLINK_Ethan_Cole","RELAY_Akira_Mori","VECTOR_Noah_Bennett"}[index]);root.transform.SetParent(parent,false);

var prefab=Resources.Load<GameObject>("RealisticCharacters/"+Models[index]);if(prefab==null)throw new InvalidOperationException("Licensed realistic asset missing "+Models[index]);var body=UnityEngine.Object.Instantiate(prefab,root.transform);body.name="Renderpeople imported modern outfit";

animator=body.GetComponentInChildren<Animator>();if(animator==null||animator.avatar==null||!animator.avatar.isValid||!animator.avatar.isHuman)throw new InvalidOperationException("Modular Humanoid avatar invalid: "+Models[index]);animator.enabled=false;





var renderers=body.GetComponentsInChildren<SkinnedMeshRenderer>().Where(r=>r.enabled).ToArray();Bounds bounds=renderers[0].bounds;foreach(var r in renderers)bounds.Encapsulate(r.bounds);

float scale=new[]{1.73f,1.81f,1.76f,1.85f}[index]/bounds.size.y;root.transform.localScale=Vector3.one*scale;

foreach(var r in renderers){r.updateWhenOffscreen=true;}

// Imported garment meshes, no vertex reshaping, generated clothing, cuffs or painted body mesh.

var hips=animator.GetBoneTransform(HumanBodyBones.Hips);root.transform.localPosition=new Vector3(0,-bounds.min.y*scale,0);

return root.transform;

}

public static Transform Create(Transform station,int index,Color accent,out Renderer collar){

var root=CreateStanding(station,index,accent,out var animator);var hips=animator.GetBoneTransform(HumanBodyBones.Hips);root.localPosition+=station.InverseTransformVector(station.TransformPoint(new Vector3(0,.66f,-.97f))-hips.position);

var neck=GameObject.CreatePrimitive(PrimitiveType.Cylinder);neck.name="Danger collar";neck.transform.position=animator.GetBoneTransform(HumanBodyBones.Neck).position;neck.transform.localScale=new Vector3(.13f,.012f,.13f);neck.transform.SetParent(animator.GetBoneTransform(HumanBodyBones.Head),true);collar=neck.GetComponent<Renderer>();collar.sharedMaterial=Material(accent,true);UnityEngine.Object.Destroy(neck.GetComponent<Collider>());

var motion=root.gameObject.AddComponent<CharacterMotion>();motion.Initialize(animator,index,station);return root;

}

static Material Material(Color tint,bool glow=false){var m=new Material(Shader.Find("Universal Render Pipeline/Lit"));m.color=tint;m.SetFloat("_Smoothness",.22f);if(glow){m.EnableKeyword("_EMISSION");m.SetColor("_EmissionColor",tint*1.2f);}return m;}

}}
