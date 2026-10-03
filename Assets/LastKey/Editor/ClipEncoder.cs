using System;
using System.IO;
using UnityEditor;
using UnityEditor.Media;
using UnityEngine;
using Unity.Collections;
namespace LastKey.Editor
{
    public static class ClipEncoder
    {
        public static void Encode()
        {
            var args=Environment.GetCommandLineArgs();int index=Array.IndexOf(args,"--evidence-path");string root=args[index+1];
            Directory.CreateDirectory(Path.Combine(root,"clips"));
            foreach(var folder in Directory.GetDirectories(Path.Combine(root,"frames")))
            {
                string name=Path.GetFileName(folder);var files=Directory.GetFiles(folder,"*.jpg");Array.Sort(files,StringComparer.Ordinal);
                var video=new VideoTrackAttributes{frameRate=new MediaRational(30),width=1920,height=1080,includeAlpha=false};
                string output=Path.Combine(root,"clips",name+".mp4");var tex=new Texture2D(1920,1080,TextureFormat.RGBA32,false);var rgba=new Texture2D(1920,1080,TextureFormat.RGBA32,false);
                string audioFile=Path.Combine(folder,"audio.f32");bool hasAudio=File.Exists(audioFile);
                var audio=new AudioTrackAttributes{sampleRate=new MediaRational(44100),channelCount=1,language="es"};
                using(var encoder=hasAudio?new MediaEncoder(output,video,audio):new MediaEncoder(output,video))
                using(var pcm=hasAudio?new BinaryReader(File.OpenRead(audioFile)):null)
                using(var samples=new NativeArray<float>(1470,Allocator.Temp))
                {
                    var writable=samples;
                    foreach(string file in files){if(!ImageConversion.LoadImage(tex,File.ReadAllBytes(file)))throw new Exception("Invalid frame "+file);rgba.SetPixels32(tex.GetPixels32());rgba.Apply(false);if(!encoder.AddFrame(rgba))throw new Exception("Encoder rejected "+file);if(hasAudio){for(int i=0;i<writable.Length;i++)writable[i]=pcm.ReadSingle();if(!encoder.AddSamples(samples))throw new Exception("Audio encoder rejected samples");}}
                }
                UnityEngine.Object.DestroyImmediate(tex);UnityEngine.Object.DestroyImmediate(rgba);Debug.Log("LAST_KEY_CLIP_SUCCESS "+name+" frames="+files.Length+" fps=30 size="+new FileInfo(output).Length);
            }
        }
    }
}
