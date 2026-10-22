using System;using System.IO;using System.Reflection;using TMPro;using UnityEditor;using UnityEditor.SceneManagement;using UnityEngine;
namespace LastKey.Editor {
// Disposable scene for UI sketch backgrounds. Never saves a production scene or changes gameplay.
public static class UiMockupCapture {
 static ArenaFactory arena;static RenderTexture target;static string root;static int ticks;
 public static void Run(){
  var args=Environment.GetCommandLineArgs();root=args[Array.IndexOf(args,"--evidence-path")+1];Directory.CreateDirectory(root);
  EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);arena=new ArenaFactory();arena.Create(Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF"));
  arena.Board.text="";foreach(var screen in arena.Screens)screen.text="";
  foreach(var character in arena.Characters){var pilot=character.GetComponent<ImportedMotionPilot>();pilot.Sample(4,2,4);typeof(ImportedMotionPilot).GetField("transitionAt",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(pilot,-100f);pilot.Sample(4,2,4);}
  target=new RenderTexture(1920,1080,24);target.Create();arena.Camera.targetTexture=target;ShaderUtil.allowAsyncCompilation=false;EditorApplication.update+=Tick;
 }
 static void Tick(){try{EditorApplication.QueuePlayerLoopUpdate();if(++ticks<12)return;new CameraDirector(arena).Gameplay();Save("arena-clean.png");new CameraDirector(arena).Portrait(0);Save("character-clean.png");Debug.Log("UI_MOCKUP_BACKGROUNDS_READY");EditorApplication.update-=Tick;EditorApplication.Exit(0);}catch(Exception e){Debug.LogException(e);EditorApplication.update-=Tick;EditorApplication.Exit(1);}}
 static void Save(string name){arena.Camera.Render();var prior=RenderTexture.active;RenderTexture.active=target;var image=new Texture2D(1920,1080,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,1920,1080),0,0);image.Apply();File.WriteAllBytes(Path.Combine(root,name),image.EncodeToPNG());UnityEngine.Object.DestroyImmediate(image);RenderTexture.active=prior;}
}}
