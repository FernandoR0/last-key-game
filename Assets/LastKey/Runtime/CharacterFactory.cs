using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LastKey
{
    public static class CharacterFactory
    {
        public static Transform Create(Transform station,int index,Color accent,out Renderer collar)
        {
            bool female=index==0||index==2;
            string baseName=female?"Superhero_Female_FullBody":"Superhero_Male_FullBody";
            var prefab=Resources.Load<GameObject>("Quaternius/"+baseName);
            if(prefab==null)throw new InvalidOperationException("Required Quaternius model missing: "+baseName);
            var root=new GameObject(new[]{"CIPHER_Valeria_Rojas","UPLINK_Ethan_Cole","RELAY_Akira_Mori","VECTOR_Noah_Bennett"}[index]);root.transform.SetParent(station,false);
            var body=UnityEngine.Object.Instantiate(prefab,root.transform);body.name="Quaternius humanoid";
            var animator=body.GetComponentInChildren<Animator>();
            if(animator==null||animator.avatar==null||!animator.avatar.isValid||!animator.avatar.isHuman)throw new InvalidOperationException("Quaternius Humanoid avatar invalid");
            animator.enabled=false;
            var allRenderers=body.GetComponentsInChildren<SkinnedMeshRenderer>();
            var main=allRenderers.OrderByDescending(r=>r.sharedMesh.vertexCount).First();
            var bounds=main.bounds;float scale=new[]{1.73f,1.80f,1.69f,1.84f}[index]/bounds.size.y;
            root.transform.localScale=new Vector3(scale*(index==2?.94f:1),scale,scale);
            root.transform.localRotation=Quaternion.Euler(0,180,0);
            root.transform.localPosition=new Vector3(0,.67f-animator.GetBoneTransform(HumanBodyBones.Hips).position.y+station.position.y,-1);
            string skinName=female?(index==0?"T_Superhero_Female_Light_BaseColor":"T_Superhero_Female_Dark_BaseColor"):(index==1?"T_Superhero_Male_Ligh":"T_Superhero_Male_Dark");
            Color skinTint=new[]{new Color(.87f,.73f,.60f),Color.white,new Color(.96f,.89f,.78f),new Color(.56f,.40f,.29f)}[index];
            foreach(var renderer in allRenderers)
            {
                var old=renderer.sharedMaterials;var mats=new Material[old.Length];
                for(int m=0;m<mats.Length;m++)
                {
                    string n=old[m]==null?"":old[m].name.ToLowerInvariant();
                    mats[m]=Material(n.Contains("eye")&&!n.Contains("brow")?"T_Eye_Brown":n.Contains("hair")?"T_Hair_1_BaseColor":skinName,n.Contains("hair")?new Color(.2f,.14f,.1f):n.Contains("eye")?Color.white:skinTint);
                }
                renderer.sharedMaterials=mats;renderer.updateWhenOffscreen=true;
            }
            AddHair(body.transform,animator.GetBoneTransform(HumanBodyBones.Head),new[]{"Hair_Long","Hair_SimpleParted","Hair_Buns","Hair_Buzzed"}[index],new[]{new Color(.21f,.13f,.08f),new Color(.4f,.27f,.14f),new Color(.11f,.08f,.065f),new Color(.12f,.085f,.06f)}[index]);
            if(index==3)AddHair(body.transform,animator.GetBoneTransform(HumanBodyBones.Head),"Hair_Beard",new Color(.12f,.08f,.055f));
            var headBone=animator.GetBoneTransform(HumanBodyBones.Head);
            headBone.localScale=Vector3.Scale(headBone.localScale,new[]{new Vector3(1,.98f,1),new Vector3(.97f,1.02f,1),new Vector3(.92f,1.03f,.96f),new Vector3(1.04f,.98f,1.02f)}[index]);
            DressBody(main,accent*.55f);
            foreach(var side in new[]{HumanBodyBones.LeftHand,HumanBodyBones.RightHand})
            {
                var hand=animator.GetBoneTransform(side);var forearm=hand.parent;
                var cuff=GameObject.CreatePrimitive(PrimitiveType.Cylinder);cuff.name="Jacket cuff";
                cuff.transform.position=Vector3.Lerp(forearm.position,hand.position,.92f);
                cuff.transform.rotation=Quaternion.FromToRotation(Vector3.up,hand.position-forearm.position);
                cuff.transform.localScale=new Vector3(.095f,.028f,.095f);cuff.transform.SetParent(hand,true);
                cuff.GetComponent<Renderer>().sharedMaterial=Material(null,accent*.55f);UnityEngine.Object.Destroy(cuff.GetComponent<Collider>());
            }

            // A narrow neck band follows the actual head bone. No placeholder body is retained.
            var neck=GameObject.CreatePrimitive(PrimitiveType.Cylinder);neck.name="Danger collar";
            neck.transform.position=animator.GetBoneTransform(HumanBodyBones.Head).position-Vector3.up*.06f;
            neck.transform.localScale=new Vector3(.13f,.012f,.13f);neck.transform.SetParent(animator.GetBoneTransform(HumanBodyBones.Head),true);
            collar=neck.GetComponent<Renderer>();collar.sharedMaterial=Material(null,accent,true);
            UnityEngine.Object.Destroy(neck.GetComponent<Collider>());
            var motion=root.AddComponent<CharacterMotion>();motion.Initialize(animator,index,station);
            return root.transform;
        }
        static Material Material(string texture,Color tint,bool emission=false)
        {
            var m=new Material(Shader.Find("Universal Render Pipeline/Lit"));m.color=tint;m.SetFloat("_Smoothness",.25f);
            if(texture!=null)m.mainTexture=Resources.Load<Texture2D>("Quaternius/"+texture);
            if(emission){m.EnableKeyword("_EMISSION");m.SetColor("_EmissionColor",tint*1.2f);}return m;
        }
        static void AddHair(Transform body,Transform head,string name,Color tint)
        {
            var hair=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Quaternius/"+name),body);hair.name=name;
            hair.transform.localPosition=Vector3.zero;hair.transform.localRotation=Quaternion.Euler(0,180,0)*hair.transform.localRotation;
            foreach(var r in hair.GetComponentsInChildren<Renderer>())r.sharedMaterials=r.sharedMaterials.Select(m=>Material(m!=null&&m.name.Contains("2")?"T_Hair_2_BaseColor":"T_Hair_1_BaseColor",tint)).ToArray();
            // Hair FBXs use the base character's rest coordinate system. Preserve this alignment.
            hair.transform.SetParent(head,true);
        }
        static void DressBody(SkinnedMeshRenderer source,Color shirt)
        {
            var original=source.sharedMesh;var mesh=UnityEngine.Object.Instantiate(original);mesh.name="LAST KEY fitted work clothing";
            var vertices=mesh.vertices;var weights=mesh.boneWeights;var types=new int[vertices.Length];
            float hip=source.bones.First(t=>t.name=="pelvis").position.y;
            for(int i=0;i<vertices.Length;i++)
            {
                var w=weights[i];int bone=w.boneIndex0;float largest=w.weight0;
                if(w.weight1>largest){bone=w.boneIndex1;largest=w.weight1;}if(w.weight2>largest){bone=w.boneIndex2;largest=w.weight2;}if(w.weight3>largest)bone=w.boneIndex3;
                string b=source.bones[bone].name.ToLowerInvariant();
                types[i]=b.Contains("foot")||b.Contains("ball")?3:b.Contains("thigh")||b.Contains("calf")?2:b.Contains("pelvis")?2:b.Contains("neck")||b.Contains("spine")||b.Contains("clavicle")||b.Contains("upperarm")||b.Contains("lowerarm")?1:0;
            }
            var groups=new[]{new List<int>(),new List<int>(),new List<int>(),new List<int>()};var tri=mesh.triangles;
            for(int i=0;i<tri.Length;i+=3){int a=types[tri[i]],b=types[tri[i+1]],c=types[tri[i+2]];int kind=a==b||a==c?a:b==c?b:a;groups[kind].Add(tri[i]);groups[kind].Add(tri[i+1]);groups[kind].Add(tri[i+2]);}
            mesh.subMeshCount=4;for(int i=0;i<4;i++)mesh.SetTriangles(groups[i],i);
            source.sharedMesh=mesh;source.sharedMaterials=new[]{source.sharedMaterial,Material(null,shirt),Material(null,new Color(.045f,.063f,.084f)),Material(null,new Color(.04f,.045f,.05f))};
        }
    }
}
