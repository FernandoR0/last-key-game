using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LastKey.Editor
{
    public static class VisualAssetSetup
    {
        public static void Inspect()
        {
            const string path="Assets/LastKey/Visual/Animations/UAL1_Standard.fbx";
            var importer=(ModelImporter)AssetImporter.GetAtPath(path);
            importer.animationType=ModelImporterAnimationType.Human;
            importer.avatarSetup=ModelImporterAvatarSetup.CreateFromThisModel;
            importer.importAnimation=true;
            importer.SaveAndReimport();
            var clips=AssetDatabase.LoadAllAssetsAtPath(path).OfType<AnimationClip>().Where(c=>!c.name.StartsWith("__preview__")).ToArray();
            Directory.CreateDirectory("../../work/game/visual-assets");
            File.WriteAllLines("../../work/game/visual-assets/unity-animation-inventory.txt",clips.Select(c=>c.name+" | "+c.length+"s | humanoid="+c.humanMotion));
            var avatar=AssetDatabase.LoadAllAssetsAtPath(path).OfType<Avatar>().FirstOrDefault();
            Debug.Log("VISUAL_ANIMATION_IMPORT clips="+clips.Length+" valid="+(avatar!=null&&avatar.isValid&&avatar.isHuman));
            foreach(var shader in AssetDatabase.FindAssets("t:Shader",new[]{"Assets/LastKey/Visual/Shaders"}))
            {
                var s=AssetDatabase.LoadAssetAtPath<Shader>(AssetDatabase.GUIDToAssetPath(shader));
                var errors=ShaderUtil.GetShaderMessages(s).Where(m=>m.severity==UnityEditor.Rendering.ShaderCompilerMessageSeverity.Error).ToArray();
                foreach(var error in errors)Debug.LogError(error.message);
                Debug.Log("VISUAL_SHADER_IMPORT "+s.name+" errors="+errors.Length);
            }
            SetupModels();
        }

        public static void ImportTyping()
        {
            AssetDatabase.Refresh();const string path="Assets/LastKey/Visual/Animations/Typing_CMU.fbx";
            var importer=(ModelImporter)AssetImporter.GetAtPath(path);importer.animationType=ModelImporterAnimationType.Human;importer.avatarSetup=ModelImporterAvatarSetup.CreateFromThisModel;
            string[] human={"Hips","Spine","Chest","UpperChest","Neck","Head","LeftShoulder","LeftUpperArm","LeftLowerArm","LeftHand","RightShoulder","RightUpperArm","RightLowerArm","RightHand","LeftUpperLeg","LeftLowerLeg","LeftFoot","LeftToes","RightUpperLeg","RightLowerLeg","RightFoot","RightToes"};
            string[] bones={"root","lowerback","upperback","thorax","lowerneck","head","lclavicle","lhumerus","lradius","lwrist","rclavicle","rhumerus","rradius","rwrist","lfemur","ltibia","lfoot","ltoes","rfemur","rtibia","rfoot","rtoes"};
            var desc=importer.humanDescription;desc.human=human.Select((n,i)=>new HumanBone{humanName=n,boneName=bones[i],limit=new HumanLimit{useDefaultValues=true}}).ToArray();importer.humanDescription=desc;importer.importAnimation=true;importer.SaveAndReimport();
            var avatar=AssetDatabase.LoadAllAssetsAtPath(path).OfType<Avatar>().First();Debug.Log("CMU_TYPING_AVATAR valid="+avatar.isValid+" human="+avatar.isHuman);
            if(!avatar.isValid||!avatar.isHuman)throw new Exception("Typing mocap avatar invalid");
            var clip=AssetDatabase.LoadAllAssetsAtPath(path).OfType<AnimationClip>().First(c=>!c.name.StartsWith("__preview__"));
            var copy=UnityEngine.Object.Instantiate(clip);copy.name="Typing_CMU";const string asset="Assets/Resources/VisualMotion/Typing_CMU.anim";
            var existing=AssetDatabase.LoadAssetAtPath<AnimationClip>(asset);if(existing==null)AssetDatabase.CreateAsset(copy,asset);else{EditorUtility.CopySerialized(copy,existing);UnityEngine.Object.DestroyImmediate(copy);}
            AssetDatabase.SaveAssets();Debug.Log("CMU_TYPING_IMPORTED "+clip.length+" seconds humanoid="+clip.humanMotion);
        }

        public static void SetupModels()
        {
            Directory.CreateDirectory("Assets/Resources/VisualCharacters");
            foreach(string model in new[]{"rain","snow"})
            {
                string path="Assets/LastKey/Visual/Characters/BlenderStudio/"+model+"_unity.fbx";
                var importer=(ModelImporter)AssetImporter.GetAtPath(path);
                importer.animationType=ModelImporterAnimationType.Human;
                importer.avatarSetup=ModelImporterAvatarSetup.CreateFromThisModel;
                // Collar fitting reads skinned vertices in Windows/WebGL players.
                importer.isReadable=true;
                var raw=AssetDatabase.LoadAssetAtPath<GameObject>(path);
 var names=raw.GetComponentsInChildren<Transform>().Select(t=>t.name).ToHashSet();
 var description=importer.humanDescription;
 description.human=HumanTrait.BoneName.Where(n=>names.Contains(n.Replace(" ",""))).Select(n=>new HumanBone{humanName=n,boneName=n.Replace(" ",""),limit=new HumanLimit{useDefaultValues=true}}).ToArray();
 importer.humanDescription=description;
 importer.importBlendShapes=true;
                importer.materialImportMode=ModelImporterMaterialImportMode.ImportStandard;
                importer.SaveAndReimport();
                var original=AssetDatabase.LoadAssetAtPath<GameObject>(path);
                var instance=UnityEngine.Object.Instantiate(original);
                var avatar=instance.GetComponent<Animator>().avatar;
                Debug.Log("VISUAL_CHARACTER_IMPORT "+model+" human="+avatar.isHuman+" valid="+avatar.isValid);
                if(!avatar.isHuman||!avatar.isValid)throw new Exception(model+" avatar invalid");
                foreach(var renderer in instance.GetComponentsInChildren<SkinnedMeshRenderer>())
                {
                    renderer.updateWhenOffscreen=true;
                    renderer.sharedMaterials=renderer.sharedMaterials.Select(source=>
                    {
                        var material=new Material(Shader.Find("SimpleURPToonLitExample(With Outline)"));
                        string name=renderer.name.ToLowerInvariant()+"_"+(source==null?"body":source.name.ToLowerInvariant());
                        material.name=model+" "+name;
                        Color color=name.Contains("hair")?new Color(.08f,.045f,.035f):name.Contains("eyebrow")?new Color(.08f,.045f,.035f):name.Contains("eye")?new Color(.93f,.96f,1):name.Contains("teeth")?new Color(.95f,.93f,.84f):name.Contains("jeans")||name.Contains("pants")?new Color(.09f,.13f,.19f):name.Contains("top")||name.Contains("shirt")?new Color(.12f,.32f,.37f):name.Contains("shoe")||name.Contains("lace")?new Color(.07f,.08f,.1f):name.Contains("gum")||name.Contains("tongue")?new Color(.45f,.15f,.17f):new Color(.72f,.42f,.27f);
                        material.SetColor("_BaseColor",color);material.SetColor("_OutlineColor",new Color(.035f,.045f,.07f));
                        if(name.Contains("eyes")){
 string texturePath=model=="rain"?"Assets/LastKey/Visual/Characters/BlenderStudio/TEX-rain_eyes.png":"Assets/LastKey/Visual/Characters/BlenderStudio/snow_unity.fbm/eyes_diffuse.png";
 var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);if(texture!=null){material.SetTexture("_BaseMap",texture);material.SetColor("_BaseColor",Color.white);}}
 material.SetFloat("_OutlineWidth",.45f);material.SetFloat("_CelShadeMidPoint",0);material.SetFloat("_CelShadeSoftness",.13f);
                        string materialPath="Assets/Resources/VisualCharacters/"+model+"_"+name.Replace(".","_").Replace(" ","_")+".mat";
                        var old=AssetDatabase.LoadAssetAtPath<Material>(materialPath);
                        if(old!=null){EditorUtility.CopySerialized(material,old);UnityEngine.Object.DestroyImmediate(material);return old;}
                        AssetDatabase.CreateAsset(material,materialPath);return material;
                    }).ToArray();
                    if(renderer.sharedMesh.blendShapeCount>0)Debug.Log("VISUAL_FACE_MORPHS "+renderer.name+" "+renderer.sharedMesh.blendShapeCount);
                }
                PrefabUtility.SaveAsPrefabAsset(instance,"Assets/Resources/VisualCharacters/"+model+".prefab");
                UnityEngine.Object.DestroyImmediate(instance);
            }
            foreach(var clip in AssetDatabase.LoadAllAssetsAtPath("Assets/LastKey/Visual/Animations/UAL1_Standard.fbx").OfType<AnimationClip>().Where(c=>!c.name.StartsWith("__preview__")))
            {
                string name=clip.name.Split('|').Last();
                if(!new[]{"Sitting_Idle_Loop","Sitting_Talking_Loop","Hit_Head","Hit_Chest","Interact","PickUp_Table"}.Contains(name))continue;
                Directory.CreateDirectory("Assets/Resources/VisualMotion");
                string path="Assets/Resources/VisualMotion/"+name+".anim";
                if(AssetDatabase.LoadAssetAtPath<AnimationClip>(path)==null)AssetDatabase.CreateAsset(UnityEngine.Object.Instantiate(clip),path);
            }
            AssetDatabase.SaveAssets();
        }
    }
}

