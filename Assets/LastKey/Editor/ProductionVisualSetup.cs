using System;using System.IO;using System.Linq;using UnityEditor;using UnityEngine;
namespace LastKey.Editor {
public static class ProductionVisualSetup {
 public static void Prepare(){
  var collarImporter=(ModelImporter)AssetImporter.GetAtPath("Assets/Resources/VisualProps/SpaceCollar/SpaceCollar.fbx");collarImporter.isReadable=true;collarImporter.SaveAndReimport();
  const string source="Assets/PrivateActorCore/Male/Actor/party-m-0001/party-m-0001.fbx";
  var imp=(ModelImporter)AssetImporter.GetAtPath(source);imp.isReadable=true;imp.importBlendShapes=true;imp.animationType=ModelImporterAnimationType.Human;imp.avatarSetup=ModelImporterAvatarSetup.CreateFromThisModel;imp.SaveAndReimport();
  var names=new[]{"SittingIdleBreathing","WakingSeated","BeingStrangled","Typing","BeingElectrocuted","SittingVictory","SittingLookAround","SittingDisbeliefForward","SitToType","TypeToSit","LookingDown"};
  var clips=names.Select(name=>{string path="Assets/PrivateMixamo/"+name+".fbx";var importer=(ModelImporter)AssetImporter.GetAtPath(path);importer.animationType=ModelImporterAnimationType.Human;importer.avatarSetup=ModelImporterAvatarSetup.CreateFromThisModel;importer.SaveAndReimport();return AssetDatabase.LoadAllAssetsAtPath(path).OfType<AnimationClip>().First(c=>c.humanMotion&&!c.name.StartsWith("__preview__")&&c.length>.1f);}).ToArray();
  Directory.CreateDirectory("Assets/Resources/MatchVisuals");var body=UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(source));
  var skins=body.GetComponentsInChildren<SkinnedMeshRenderer>();var bounds=skins[0].bounds;foreach(var skin in skins)bounds.Encapsulate(skin.bounds);body.transform.localScale*=1.81f/bounds.size.y;
  const string pathMat="Assets/Resources/MatchVisuals/ApprovedCharacter.mat";var material=AssetDatabase.LoadAssetAtPath<Material>(pathMat);
  if(material==null){material=new Material(Shader.Find("Universal Render Pipeline/Lit"));AssetDatabase.CreateAsset(material,pathMat);}
  string tex="Assets/PrivateActorCore/Male/Actor/party-m-0001/party-m-0001.fbm/Character_Pbr_";
  material.SetTexture("_BaseMap",AssetDatabase.LoadAssetAtPath<Texture2D>(tex+"Diffuse.png"));material.SetTexture("_BumpMap",AssetDatabase.LoadAssetAtPath<Texture2D>(tex+"Normal.jpg"));material.EnableKeyword("_NORMALMAP");material.SetFloat("_BumpScale",.3f);material.SetFloat("_Smoothness",.25f);
  foreach(var skin in skins){skin.sharedMaterials=Enumerable.Repeat(material,skin.sharedMaterials.Length).ToArray();skin.updateWhenOffscreen=true;}
  body.AddComponent<ImportedMotionLibrary>().Clips=clips;PrefabUtility.SaveAsPrefabAsset(body,"Assets/Resources/MatchVisuals/ApprovedCharacter.prefab");UnityEngine.Object.DestroyImmediate(body);AssetDatabase.SaveAssets();for(int i=0;i<clips.Length;i++)Debug.Log("LAST_KEY_SOURCE "+names[i]+" length="+clips[i].length);Debug.Log("LAST_KEY_APPROVED_VISUALS_PREPARED");
 }
 public static void BuildWindows(){Prepare();ProjectBuilder.BuildWindows();}
 public static void BuildWebGL(){Prepare();UnityEditor.WebGL.UserBuildSettings.codeOptimization=UnityEditor.WebGL.WasmCodeOptimization.RuntimeSpeed;ProjectBuilder.BuildWebGL();}
}}
