using System;using System.IO;using System.Linq;
using UnityEditor;using UnityEditor.SceneManagement;using UnityEngine;using TMPro;
namespace LastKey.Editor {
// Captures the pilot in the existing arena. Never saves over the production scene.
public static class IntegratedMotionTrial {
 static ArenaFactory arena;static ImportedMotionPilot pilot;static RenderTexture target;static Texture2D image;
 static string root;static int frame;static double next;static Transform station;static bool poseReady;static bool poseReview,collarAudit;
 public static void Run(){
  var args=Environment.GetCommandLineArgs();root=args[Array.IndexOf(args,"--evidence-path")+1];Directory.CreateDirectory(Path.Combine(root,"frames","integrated_motion"));poseReview=args.Contains("--pose-review");collarAudit=args.Contains("--collar-audit");
  AssetDatabase.Refresh();var sources=new[]{"SittingIdleBreathing","WakingSeated","BeingStrangled","Typing","BeingElectrocuted","SittingVictory"};var clips=sources.Select(Source).ToArray();
  string p="Assets/PrivateActorCore/Male/Actor/party-m-0001/party-m-0001.fbx";var imp=(ModelImporter)AssetImporter.GetAtPath(p);imp.isReadable=true;imp.animationType=ModelImporterAnimationType.Human;imp.avatarSetup=ModelImporterAvatarSetup.CreateFromThisModel;imp.importBlendShapes=true;imp.SaveAndReimport();
  EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);arena=new ArenaFactory();arena.Create(TMP_Settings.defaultFontAsset);foreach(var c in arena.Characters)UnityEngine.Object.DestroyImmediate(c.gameObject);for(int i=1;i<4;i++)arena.Stations[i].gameObject.SetActive(false);station=arena.Stations[0];
  var body=UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(p),station);var rig=body.GetComponentInChildren<Animator>();if(!rig.avatar.isValid||!rig.avatar.isHuman)throw new Exception("Pilot avatar invalid");
  var skins=body.GetComponentsInChildren<SkinnedMeshRenderer>();var b=skins[0].bounds;foreach(var s in skins)b.Encapsulate(s.bounds);body.transform.localScale*=1.81f/b.size.y;
  var material=new Material(Shader.Find("Universal Render Pipeline/Lit"));string tex="Assets/PrivateActorCore/Male/Actor/party-m-0001/party-m-0001.fbm/Character_Pbr_";material.SetTexture("_BaseMap",AssetDatabase.LoadAssetAtPath<Texture2D>(tex+"Diffuse.png"));material.SetTexture("_BumpMap",AssetDatabase.LoadAssetAtPath<Texture2D>(tex+"Normal.jpg"));material.EnableKeyword("_NORMALMAP");material.SetFloat("_BumpScale",.3f);material.SetFloat("_Smoothness",.25f);foreach(var s in skins){s.sharedMaterials=Enumerable.Repeat(material,s.sharedMaterials.Length).ToArray();s.updateWhenOffscreen=true;}
  pilot=body.AddComponent<ImportedMotionPilot>();pilot.Initialize(rig,station,clips);
  // Match desk height/reach to this pilot's chair and body before changing production furniture.
  station.Cast<Transform>().First(c=>c.name=="Imported / SM_Desk").localPosition+=new Vector3(0,.14f,-.13f);
  station.Find("Integrated touch terminal").localPosition+=new Vector3(0,.14f,-.13f);
  target=new RenderTexture(1920,1080,24);target.Create();arena.Camera.targetTexture=target;image=new Texture2D(1920,1080,TextureFormat.RGB24,false);ShaderUtil.allowAsyncCompilation=false;
  frame=poseReview?30:0;poseReady=false;next=EditorApplication.timeSinceStartup+1;EditorApplication.update+=Tick;
 }
 static AnimationClip Source(string name){string p="Assets/PrivateMixamo/"+name+".fbx";var imp=(ModelImporter)AssetImporter.GetAtPath(p);if(imp==null)throw new Exception("Missing downloaded motion "+name);imp.animationType=ModelImporterAnimationType.Human;imp.avatarSetup=ModelImporterAvatarSetup.CreateFromThisModel;imp.importAnimation=true;imp.SaveAndReimport();var clip=AssetDatabase.LoadAllAssetsAtPath(p).OfType<AnimationClip>().Where(c=>c.humanMotion&&!c.name.StartsWith("__preview__")&&c.length>.1f).OrderByDescending(c=>c.length).First();Debug.Log("PILOT_SOURCE "+name+" seconds="+clip.length);return clip;}
 static void Tick(){EditorApplication.QueuePlayerLoopUpdate();if(EditorApplication.timeSinceStartup<next)return;next=EditorApplication.timeSinceStartup+1.0/30;
  try{float t=frame/30f;if(!poseReady){if(collarAudit){pilot.CollarSourceTime=frame/60f;t=7;}pilot.Sample(t);bool typing=t>=10&&t<18;arena.Screens[0].text=typing?"ESCRIBE CON PRECISIÓN\nCada tecla decide la ronda.":t<5?"SISTEMA EN ESPERA":t<10?"COLLAR ACTIVO":t<30?pilot.Stage:"THE LAST KEY";arena.Screens[0].color=pilot.Intensity>0?new Color(1,.8f,.75f):new Color(.70f,1,1);arena.Board.text="PRUEBA DE ANIMACIÓN\n"+pilot.Stage;
   arena.Displays[0].material.color=pilot.Intensity>0?new Color(.15f,.02f,.04f):new Color(.012f,.07f,.085f);
   arena.Camera.fieldOfView=43;arena.Camera.transform.position=station.TransformPoint(new Vector3(1.20f,1.57f,.75f));arena.Camera.transform.LookAt(station.TransformPoint(new Vector3(0,1.10f,-.77f)));
   if(t>=5&&t<10){arena.Camera.fieldOfView=35;arena.Camera.transform.position=station.TransformPoint(new Vector3(-.55f,1.51f,-.27f));arena.Camera.transform.LookAt(pilot.CollarCenter);}
   if(t>=14&&t<18){arena.Camera.fieldOfView=78;arena.Camera.transform.position=pilot.Rig.GetBoneTransform(HumanBodyBones.Head).position+station.TransformVector(new Vector3(0,.045f,.18f));arena.Camera.transform.LookAt(station.TransformPoint(new Vector3(0,.985f,-.44f)));}
   // Let the editor player loop update GPU skinning before capturing accessories and the body.
   poseReady=true;return;}
   arena.Camera.Render();var old=RenderTexture.active;RenderTexture.active=target;image.ReadPixels(new Rect(0,0,1920,1080),0,0);image.Apply();RenderTexture.active=old;File.WriteAllBytes(Path.Combine(root,"frames","integrated_motion",frame.ToString("D5")+".jpg"),image.EncodeToJPG(90));
   if(frame%60==30){File.WriteAllBytes(Path.Combine(root,"review_"+frame.ToString("D4")+".png"),image.EncodeToPNG());var tip=pilot.Rig.GetBoneTransform(HumanBodyBones.LeftMiddleDistal);Debug.Log("PILOT_REVIEW t="+t+" stage="+pilot.Stage+" fingertip="+station.InverseTransformPoint(tip.position)+" collar="+pilot.Collar.bounds.size);}
   poseReady=false;frame+=poseReview?60:1;if(frame>=1020){EditorApplication.update-=Tick;if(!poseReview)WriteAudio();Debug.Log("INTEGRATED_MOTION_CAPTURE_COMPLETE");EditorApplication.Exit(0);}
  }catch(Exception e){Debug.LogException(e);EditorApplication.update-=Tick;EditorApplication.Exit(1);}
 }
 static void WriteAudio(){const int rate=44100;var mix=new float[34*rate];AddCue(mix,"elimination",18,.30f);AddCue(mix,"elimination",21,.55f);AddCue(mix,"elimination",24,.8f);AddCue(mix,"scream",24.15f,.65f);AddCue(mix,"victory",30,.65f);for(float t=10;t<18;t+=.18f)AddCue(mix,"typing",t,.38f);using(var w=new BinaryWriter(File.Create(Path.Combine(root,"frames","integrated_motion","audio.f32"))))foreach(float x in mix)w.Write(Mathf.Clamp(x,-.95f,.95f));File.WriteAllText(Path.Combine(root,"audio_format.json"),"{\"rate\":44100,\"channels\":1}");}
 static void AddCue(float[] dest,string name,float time,float gain){var clip=Resources.Load<AudioClip>("Audio/VisualSfx/"+name);if(clip==null)throw new Exception("Missing free audio cue "+name);clip.LoadAudioData();var data=new float[clip.samples*clip.channels];if(!clip.GetData(data,0))throw new Exception("Cannot decode source cue "+name);int offset=(int)(time*44100),count=(int)(clip.length*44100);for(int i=0;i<count&&offset+i<dest.Length;i++){int sample=Mathf.Min(clip.samples-1,(int)(i*(clip.frequency/44100f)));float x=0;for(int c=0;c<clip.channels;c++)x+=data[sample*clip.channels+c];dest[offset+i]+=x/clip.channels*gain;}}
}}
