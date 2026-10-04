using UnityEngine;

namespace LastKey
{
    // Expressions are baked from the original free Blender Studio rigs.
    public sealed class CharacterFace:MonoBehaviour
    {
        SkinnedMeshRenderer[] meshes;
        int player;
        public void Initialize(int index){player=index;meshes=GetComponentsInChildren<SkinnedMeshRenderer>();}
        public void Sample(float time,string state,float opening)
        {
            float blinkPhase=(time+player*.71f)%4.3f;
            float blink=blinkPhase<.18f?Mathf.Sin(blinkPhase/.18f*Mathf.PI):0;
            if(state=="wakeup")blink=Mathf.Max(blink,1-Mathf.SmoothStep(0,1,Mathf.Clamp01(opening*4)));
            Set("Blink",blink*100);
            Set("Alert",state=="danger_reaction"||state=="wakeup"?35:8);
            Set("Pain",state=="mistake_reaction"?55:state=="eliminated"?25:0);
            Set("Smile",state=="final_win"||state=="round_win"?65:0);
            float gaze=Mathf.Sin(time*.45f+player)*12;
            Set("GazeLeft",Mathf.Max(0,gaze));Set("GazeRight",Mathf.Max(0,-gaze));
        }
        void Set(string name,float weight)
        {
            foreach(var mesh in meshes)
            {
                int index=mesh.sharedMesh.GetBlendShapeIndex(name);
                if(index>=0)mesh.SetBlendShapeWeight(index,Mathf.MoveTowards(mesh.GetBlendShapeWeight(index),weight,Time.deltaTime*650));
            }
        }
    }
}
