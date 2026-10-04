using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace LastKey
{
    // Retarget downloaded CC0 motion; contact IK remains in CharacterMotion.
    public sealed class ReusableCharacterMotion
    {
        PlayableGraph graph;
        AnimationMixerPlayable mixer;
        readonly AnimationClipPlayable[] clips=new AnimationClipPlayable[5];
        float gestureWeight,hitWeight,hitClock,typingWeight;
        bool priorEliminated;
        int priorErrors;
        public ReusableCharacterMotion(Animator animator)
        {
            animator.enabled=true;animator.applyRootMotion=false;
            graph=PlayableGraph.Create("LAST KEY / reused Quaternius motion");graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
            mixer=AnimationMixerPlayable.Create(graph,5);
            string[] names={"Sitting_Idle_Loop","Sitting_Talking_Loop","Hit_Chest","Typing_CMU","Typing_CMU"};
            for(int i=0;i<names.Length;i++)
            {
                var clip=Resources.Load<AnimationClip>("VisualMotion/"+names[i]);
                clips[i]=AnimationClipPlayable.Create(graph,clip);clips[i].SetApplyFootIK(false);clips[i].SetApplyPlayableIK(false);
                graph.Connect(clips[i],0,mixer,i);
            }
            var output=AnimationPlayableOutput.Create(graph,"Humanoid body",animator);output.SetSourcePlayable(mixer);graph.Play();
        }
        public void Sample(float time,int player,int errors,bool expressive,bool eliminated,bool typing)
        {
            if(eliminated&&!priorEliminated){hitClock=0;hitWeight=.8f;}priorEliminated=eliminated;
            if(errors>priorErrors){hitClock=0;hitWeight=.45f;}
            priorErrors=errors;
            gestureWeight=Mathf.MoveTowards(gestureWeight,expressive?.65f:.08f,Time.deltaTime*2);
            hitClock+=Time.deltaTime;hitWeight=Mathf.MoveTowards(hitWeight,0,Time.deltaTime*2);
            for(int i=0;i<2;i++)clips[i].SetTime((time+player*.37f)%clips[i].GetAnimationClip().length);
            clips[2].SetTime(Mathf.Min(hitClock,clips[2].GetAnimationClip().length));
            typingWeight=Mathf.MoveTowards(typingWeight,typing?.85f:0,Time.deltaTime*2);
            float phase=Mathf.Repeat(time+player*.77f,7.7f),wrap=Mathf.Clamp01((phase-7.4f)/.3f);
            clips[3].SetTime(4.3f+phase);clips[4].SetTime(4+Mathf.Max(0,phase-7.4f));
            float gesture=Mathf.Min(gestureWeight,1-hitWeight);
            mixer.SetInputWeight(0,(1-gesture-hitWeight)*(1-typingWeight));mixer.SetInputWeight(1,gesture*(1-typingWeight));mixer.SetInputWeight(2,hitWeight*(1-typingWeight));
            mixer.SetInputWeight(3,typingWeight*(1-wrap));mixer.SetInputWeight(4,typingWeight*wrap);
            graph.Evaluate(0);
        }
        public void Dispose(){if(graph.IsValid())graph.Destroy();}
    }
}
