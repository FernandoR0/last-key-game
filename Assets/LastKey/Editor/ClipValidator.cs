using System;

using System.IO;

using UnityEditor;

using UnityEngine;

using UnityEngine.Video;

namespace LastKey.Editor

{

    public static class ClipValidator

    {

        static string root;static string[] clips;static int index,decoded;static VideoPlayer player;static RenderTexture target;static double started;static bool complete;

        public static void Validate()

        {

            var args=Environment.GetCommandLineArgs();root=args[Array.IndexOf(args,"--evidence-path")+1];clips=Directory.GetFiles(Path.Combine(root,"clips"),"*.mp4");Array.Sort(clips);int pick=Array.IndexOf(args,"--clip-name");if(pick>=0)clips=new[]{Path.Combine(root,"clips",args[pick+1]+".mp4")};

            Directory.CreateDirectory(Path.Combine(root,"decoded_review"));EditorApplication.update+=Tick;Next();

        }

        static void Next()

        {

            if(player!=null){player.Stop();UnityEngine.Object.DestroyImmediate(player.gameObject);target.Release();UnityEngine.Object.DestroyImmediate(target);}

            if(index>=clips.Length){Debug.Log("LAST_KEY_ALL_CLIPS_DECODED");EditorApplication.Exit(0);return;}

            decoded=0;complete=false;started=EditorApplication.timeSinceStartup;

            player=new GameObject("Clip QA",typeof(VideoPlayer)).GetComponent<VideoPlayer>();player.playOnAwake=false;player.source=VideoSource.Url;player.url=clips[index];player.audioOutputMode=VideoAudioOutputMode.None;player.skipOnDrop=false;player.sendFrameReadyEvents=true;

            target=new RenderTexture(1920,1080,0,RenderTextureFormat.ARGB32);target.Create();player.renderMode=VideoRenderMode.RenderTexture;player.targetTexture=target;

            player.errorReceived+=(p,error)=>{Debug.LogError("CLIP_DECODE_ERROR "+error);EditorApplication.Exit(1);};

            player.prepareCompleted+=p=>{if(p.width!=1920||p.height!=1080||Math.Abs(p.frameRate-30)>.1)throw new Exception("Unexpected clip format");p.Play();};

            player.frameReady+=(p,frame)=>{decoded++;long count=(long)p.frameCount;if(frame==0||frame==count/4||frame==count/2||frame==count*3/4||frame>=count-2){var old=RenderTexture.active;RenderTexture.active=target;var tex=new Texture2D(1920,1080,TextureFormat.RGB24,false);tex.ReadPixels(new Rect(0,0,1920,1080),0,0);tex.Apply();File.WriteAllBytes(Path.Combine(root,"decoded_review",Path.GetFileNameWithoutExtension(clips[index])+"_"+frame+".png"),tex.EncodeToPNG());UnityEngine.Object.DestroyImmediate(tex);RenderTexture.active=old;}};

            player.loopPointReached+=p=>{Debug.Log("LAST_KEY_CLIP_DECODED "+Path.GetFileName(clips[index])+" frames="+decoded+" expected="+p.frameCount+" fps="+p.frameRate+" length="+p.length);complete=true;};player.Prepare();

        }

        static void Tick(){EditorApplication.QueuePlayerLoopUpdate();if(complete){index++;Next();}else if(EditorApplication.timeSinceStartup-started>60){Debug.LogError("CLIP_DECODE_TIMEOUT");EditorApplication.Exit(1);}}

    }

}
