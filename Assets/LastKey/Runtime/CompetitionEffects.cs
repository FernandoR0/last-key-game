using UnityEngine;
using LastKey.Core;
namespace LastKey
{
    public sealed class CompetitionEffects
    {
        readonly ArenaFactory arena;
        readonly LineRenderer[] arcs=new LineRenderer[4];
        readonly int[] priorErrors=new int[4];
        readonly bool[] eliminated=new bool[4];
        readonly float[] eventAt=new float[4];
        public int LastEliminated {get;private set;}=-1;
        public float LastEliminationTime {get;private set;}=-10;
        public CompetitionEffects(ArenaFactory value)
        {
            arena=value;
            for(int i=0;i<4;i++)
            {
                var obj=new GameObject("Stylized collar electricity "+i,typeof(LineRenderer));obj.transform.SetParent(arena.Root.transform);
                var line=obj.GetComponent<LineRenderer>();line.positionCount=9;line.startWidth=.018f;line.endWidth=.008f;var electric=Resources.Load<Material>("LastKeyElectric");line.material=electric!=null?new Material(electric):new Material(Shader.Find("Universal Render Pipeline/Lit"));line.material.color=new Color(.6f,.9f,1);line.enabled=false;arcs[i]=line;
            }
        }
        public void Tick(Competition g,GameAudio sound)
        {
            for(int i=0;i<4;i++)
            {
                var p=g.Players[i];
                if(!p.Eliminated){eliminated[i]=false;}
                if(p.Eliminated&&!eliminated[i]){eliminated[i]=true;eventAt[i]=Time.time;LastEliminated=i;LastEliminationTime=Time.time;sound.Cue("elimination");}
                if(p.Errors>priorErrors[i]){eventAt[i]=Time.time;if(i==g.HumanIndex)sound.Cue("error");}priorErrors[i]=p.Errors;
                arcs[i].enabled=false; // Approved character motion owns the collar-bound discharge effect.
                if(arcs[i].enabled){var center=arena.Collars[i].bounds.center;for(int n=0;n<9;n++)arcs[i].SetPosition(n,center+new Vector3(Mathf.Cos(n*.9f)*.2f,Mathf.Sin(n*2.5f+Time.time*30)*.085f,Mathf.Sin(n*.9f)*.2f));}
            }
        }
    }
}
