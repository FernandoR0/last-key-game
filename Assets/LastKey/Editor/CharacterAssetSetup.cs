using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LastKey.Editor
{
    public sealed class CharacterModelImporter : AssetPostprocessor
    {
        void OnPreprocessModel()
        {
            if(!assetPath.Contains("/Quaternius/"))return;
            var importer=(ModelImporter)assetImporter;importer.isReadable=true;
            importer.materialImportMode=ModelImporterMaterialImportMode.ImportStandard;
            if(assetPath.Contains("FullBody")){importer.animationType=ModelImporterAnimationType.Human;importer.avatarSetup=ModelImporterAvatarSetup.CreateFromThisModel;}
            else importer.animationType=ModelImporterAnimationType.None;
        }
    }
    public static class CharacterAssetSetup
    {
        public static void ImportAndAudit()
        {
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            foreach(string name in new[]{"Superhero_Female_FullBody","Superhero_Male_FullBody","Hair_Long","Hair_Buns","Hair_Buzzed","Hair_SimpleParted"})
            {
                string path="Assets/Resources/Quaternius/"+name+".fbx";
                var importer=(ModelImporter)AssetImporter.GetAtPath(path);importer.isReadable=true;
                if(name.Contains("FullBody")){importer.animationType=ModelImporterAnimationType.Human;importer.avatarSetup=ModelImporterAvatarSetup.CreateFromThisModel;}else importer.animationType=ModelImporterAnimationType.None;
                importer.SaveAndReimport();
                var obj=UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(path));
                var animator=obj.GetComponentInChildren<Animator>();
                Debug.Log("MODEL_AUDIT "+name+" scale="+obj.transform.localScale+" humanoid="+(animator!=null&&animator.avatar!=null&&animator.avatar.isHuman)+" valid="+(animator!=null&&animator.avatar!=null&&animator.avatar.isValid));
                foreach(var r in obj.GetComponentsInChildren<Renderer>())
                {
                    Debug.Log("MODEL_RENDERER "+r.name+" bounds="+r.bounds+" materials="+string.Join(",",r.sharedMaterials.Select(m=>m==null?"null":m.name)));
                    if(r is SkinnedMeshRenderer skin)Debug.Log("MODEL_BONES "+string.Join(",",skin.bones.Select(t=>t.name)));
                }
                if(animator!=null&&animator.isHuman)foreach(var bone in new[]{HumanBodyBones.Hips,HumanBodyBones.Head,HumanBodyBones.LeftHand,HumanBodyBones.LeftUpperLeg}){var t=animator.GetBoneTransform(bone);Debug.Log("HUMAN_BONE "+bone+" "+t.name+" position="+t.position);}
                UnityEngine.Object.DestroyImmediate(obj);
            }
        }
    }
}
