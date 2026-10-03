using System;
using System.Linq;
using System.Reflection;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
namespace LastKey.Editor
{
    public static class MusicSetup
    {
        static object Call(object o,string name,params object[] args)=>o.GetType().GetMethod(name,BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance).Invoke(o,args);
        public static void Prepare()
        {
            AssetDatabase.Refresh();Directory.CreateDirectory("Assets/Resources/Audio");AssetDatabase.Refresh();
            string path="Assets/Resources/Audio/LastKey.mixer";
            var mixer=AssetDatabase.LoadAssetAtPath<AudioMixer>(path);
            if(mixer==null)
            {
                var type=AppDomain.CurrentDomain.GetAssemblies().Select(a=>a.GetType("UnityEditor.Audio.AudioMixerController")).First(t=>t!=null);
                mixer=(AudioMixer)type.GetMethod("CreateMixerControllerAtPath",BindingFlags.Public|BindingFlags.Static).Invoke(null,new object[]{path});
                var master=type.GetProperty("masterGroup").GetValue(mixer);((UnityEngine.Object)master).name="MASTER";
                var snapshot=type.GetProperty("startSnapshot").GetValue(mixer);
                string[] names={"MUSIC","SFX","UI","AMBIENCE"};float[] levels={-14,0,-3,-14};
                for(int i=0;i<names.Length;i++)
                {
                    var group=Call(mixer,"CreateNewGroup",names[i],false);Call(mixer,"AddChildToParent",group,master);
                    Call(group,"SetValueForVolume",mixer,snapshot,levels[i]);
                }
                EditorUtility.SetDirty(mixer);
            }
            var library=AssetDatabase.LoadAssetAtPath<MusicLibrary>("Assets/Resources/Audio/MusicLibrary.asset");
            if(library==null){library=ScriptableObject.CreateInstance<MusicLibrary>();AssetDatabase.CreateAsset(library,"Assets/Resources/Audio/MusicLibrary.asset");}
            library.Mixer=mixer;
            var names2=new[]{"Iron Dominion","Dead Current","Mainframe"};var clips=new AudioClip[3];
            for(int i=0;i<3;i++)
            {
                string ap="Assets/Audio/Music/Masters/"+names2[i]+".wav";var importer=(AudioImporter)AssetImporter.GetAtPath(ap);
                var settings=importer.defaultSampleSettings;settings.loadType=AudioClipLoadType.Streaming;settings.compressionFormat=AudioCompressionFormat.Vorbis;settings.quality=.7f;
                settings.preloadAudioData=false;importer.defaultSampleSettings=settings;importer.loadInBackground=true;importer.forceToMono=false;importer.SaveAndReimport();
                clips[i]=AssetDatabase.LoadAssetAtPath<AudioClip>(ap);Debug.Log("MUSIC_IMPORTED "+names2[i]+" seconds="+clips[i].length+" samples="+clips[i].samples);
            }
            library.IronDominion=clips[0];library.DeadCurrent=clips[1];library.Mainframe=clips[2];EditorUtility.SetDirty(library);
            AssetDatabase.SaveAssets();Debug.Log("MUSIC_MIXER_READY "+string.Join(",",mixer.FindMatchingGroups("").Select(g=>g.name)));
        }
        public static void ArchiveFallback()
        {
            if(!AssetDatabase.IsValidFolder("Assets/CharacterFallback"))AssetDatabase.CreateFolder("Assets","CharacterFallback");
            foreach(string name in new[]{"ModularCharacters","Quaternius"})
            {
                string from="Assets/Resources/"+name,to="Assets/CharacterFallback/"+name;
                if(AssetDatabase.IsValidFolder(from)){string error=AssetDatabase.MoveAsset(from,to);if(error!="")throw new Exception(error);}
            }
            AssetDatabase.SaveAssets();Debug.Log("FALLBACK_PRESERVED_OUTSIDE_RESOURCES");
        }
    }
}
