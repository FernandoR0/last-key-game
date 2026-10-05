using UnityEngine;
using LastKey.Core;
namespace LastKey {
// Competition owns scores and timers; this adapter only plays the approved imported motions.
public sealed class MatchCharacterVisual:MonoBehaviour {
 public ImportedMotionPilot Motion; public int Identity; int round=-1,level; float eventAt=-100; bool removed; float resultAt=-1;
 public void Present(Competition game,ExperienceState flow,float seconds){
  var p=game.Players[Identity]; if(round!=game.Round){round=game.Round;level=0;removed=false;eventAt=-100;resultAt=-1;}
  int stage=7;float local=seconds;
  if(flow==ExperienceState.RoundIntro){
   float wake=Identity==game.HumanIndex?0:4+Identity*.8f;local=seconds-wake;
   if(local<10){stage=0;local=Mathf.Max(0,local-4)*.55f;}
   else if(local<18){stage=8;local-=10;}
   else if(local<23){stage=9;local-=18;}
   else if(local<32){stage=1;local=(local-23)*.45f;}
   else if(local<36){stage=7;local-=32;}
   else {stage=2;local-=36;}
  }else if(flow==ExperienceState.Gameplay||flow==ExperienceState.Countdown||flow==ExperienceState.Paused){
   int next=p.Eliminated?3:p.Danger>=70?2:p.Danger>=40?1:0;
   if(next>level){level=next;eventAt=Time.time;}if(p.Eliminated&&!removed){removed=true;eventAt=Time.time;}
   stage=2;local=game.Elapsed;
   if(removed){stage=5;local=Mathf.Min(Time.time-eventAt,5.99f);}
   else if(Time.time-eventAt<3&&level>0){stage=level+2;local=Time.time-eventAt;}
  }else if(flow==ExperienceState.FinalResult){
   if(resultAt<0)resultAt=Time.time;bool winner=System.Array.IndexOf(game.Players,game.Winners()[0])==Identity;
   stage=winner?6:7;local=Time.time-resultAt;
  }
  Motion.Sample(Time.time,stage,local);
 }
}}
