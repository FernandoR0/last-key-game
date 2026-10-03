using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LastKey.Editor
{
    public static class RealisticImport
    {
        public static readonly string[] Models = { "rp_carla_rigged_001_u3d", "rp_eric_rigged_001_u3d", "rp_manuel_animated_001_dancing", "rp_nathan_animated_003_walking" };
        public static void Inspect()
        {
            AssetDatabase.Refresh();
            foreach (var model in Models)
            {
                string path = "Assets/PrivateRenderpeople/" + model + ".fbx";
                var importer = (ModelImporter)AssetImporter.GetAtPath(path);
                importer.animationType = ModelImporterAnimationType.Human;
                importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
                importer.importAnimation = false;
                importer.isReadable = true;
                importer.materialImportMode = ModelImporterMaterialImportMode.None;
                importer.SaveAndReimport();
                var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                var animator = asset.GetComponentInChildren<Animator>();
                Debug.Log("REALISTIC_IMPORT " + model + " avatar=" + (animator != null && animator.avatar != null ? animator.avatar.isValid + "/" + animator.avatar.isHuman : "none"));
                Debug.Log("REALISTIC_BONES " + model + " " + string.Join(",", asset.GetComponentsInChildren<Transform>().Select(t => t.name)));
                foreach (var mesh in asset.GetComponentsInChildren<SkinnedMeshRenderer>())
                    Debug.Log("REALISTIC_MESH " + model + " " + mesh.name + " verts=" + mesh.sharedMesh.vertexCount + " bounds=" + mesh.bounds);
                string prefix = model.Substring(0, model.IndexOf("_001") + 4);
                if (model.Contains("nathan")) prefix = "rp_nathan_animated_003";
                string textureRoot = "Assets/PrivateRenderpeople/tex/" + prefix;
                foreach (string suffix in new[] { "_lastkey.png", "_norm.jpg" })
                {
                    var ti = (TextureImporter)AssetImporter.GetAtPath(textureRoot + suffix);
                    ti.maxTextureSize = 2048;
                    ti.textureCompression = TextureImporterCompression.Compressed;
                    if (suffix.Contains("norm")) ti.textureType = TextureImporterType.NormalMap;
                    ti.SaveAndReimport();
                }
                string resourceRoot = "Assets/PrivateRenderpeople/Resources/RealisticCharacters";
                Directory.CreateDirectory(resourceRoot);AssetDatabase.Refresh();
                var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                mat.name = prefix;
                mat.SetTexture("_BaseMap",AssetDatabase.LoadAssetAtPath<Texture2D>(textureRoot + "_lastkey.png"));
                mat.SetTexture("_BumpMap",AssetDatabase.LoadAssetAtPath<Texture2D>(textureRoot + "_norm.jpg"));
                mat.EnableKeyword("_NORMALMAP");mat.SetFloat("_Smoothness",.22f);
                string mp=resourceRoot+"/"+prefix+".mat";
                var saved=AssetDatabase.LoadAssetAtPath<Material>(mp);
                if(saved==null){AssetDatabase.CreateAsset(mat,mp);saved=mat;}else{EditorUtility.CopySerialized(mat,saved);UnityEngine.Object.DestroyImmediate(mat);}
                var instance=UnityEngine.Object.Instantiate(asset);
                foreach(var mesh in instance.GetComponentsInChildren<SkinnedMeshRenderer>()){mesh.sharedMaterials=Enumerable.Repeat(saved,mesh.sharedMaterials.Length).ToArray();mesh.updateWhenOffscreen=true;}
                instance.GetComponentInChildren<Animator>().enabled=false;
                PrefabUtility.SaveAsPrefabAsset(instance,resourceRoot+"/"+model+".prefab");
                UnityEngine.Object.DestroyImmediate(instance);
            }
            AssetDatabase.SaveAssets();
        }
    }
}
