using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using UnityEngine;
namespace LastKey
{
    // Captures Unity's actual mixer output, synchronized to fixed-rate rendered game frames.
    public sealed class MusicCapture:MonoBehaviour
    {
        string root;PolishEvidence evidence;bool running;
        readonly Dictionary<string,BinaryWriter> writers=new Dictionary<string,BinaryWriter>();
        IEnumerator Start()
        {
            evidence=GetComponent<PolishEvidence>();var args=Environment.GetCommandLineArgs();root=args[Array.IndexOf(args,"--evidence-path")+1];
            Directory.CreateDirectory(root);running=AudioRenderer.Start();if(!running)throw new Exception("Unity AudioRenderer failed to start.");
            File.WriteAllText(Path.Combine(root,"audio_format.json"),"{\"rate\":"+AudioSettings.outputSampleRate+",\"channels\":2,\"source\":\"Unity AudioRenderer MASTER output\"}");
            while(running)
            {
                yield return new WaitForEndOfFrame();
                if(!running)yield break;
                int count=AudioRenderer.GetSampleCountForCaptureFrame();if(count<=0)continue;
                using(var samples=new NativeArray<float>(count*2,Allocator.Temp))
                {
                    if(!AudioRenderer.Render(samples))throw new Exception("Unity audio frame failed.");
                    string clip=evidence.ActiveClip;if(clip==null)continue;
                    if(!writers.TryGetValue(clip,out var writer)){Directory.CreateDirectory(Path.Combine(root,"frames",clip));writer=new BinaryWriter(File.Create(Path.Combine(root,"frames",clip,"audio.f32")));writers.Add(clip,writer);}
                    foreach(float sample in samples)writer.Write(sample);
                }
            }
        }
        public void Stop(){if(running)AudioRenderer.Stop();running=false;foreach(var w in writers.Values)w.Dispose();writers.Clear();}
        void OnDestroy(){Stop();}
    }
}
