using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using LastKey.Core;
namespace LastKey.Editor
{
    public static class PolishQa
    {
        public static void Run()
        {
            CoreQa.Run();int count=0;
            Action<bool,string> check=(ok,name)=>{if(!ok)throw new Exception(name);count++;Debug.Log("POLISH_PASS "+name);};
            for(int h=0;h<4;h++)
            {
                var g=new Competition(humanIndex:h);g.StartRound();g.Tick(2);
                check(!g.Players[h].Bot&&g.Players.Count(p=>p.Bot)==3,"selection "+h);
                check(g.Players[h].Correct==0&&g.Players.Where(p=>p.Bot).All(p=>p.Correct>0),"bots exclude selected "+h);
                g.Input(h,g.Target(h)[0]);check(g.Players[h].Correct==1,"selected human input "+h);
            }
            var p=new PromptManager(91);var seen=new HashSet<string>();p.BeginRound(new[]{"uno","dos","tres","cuatro"});
            for(int i=0;i<40;i++)check(seen.Add(p.Next()),"no duplicate prompt "+i);
            var echo=new Competition();echo.StartRound();echo.Tick(42);
            check(!echo.EchoOffered&&echo.Players[0].EchoResolved&&!echo.Players[0].EchoAccepted&&echo.Players[0].Danger==0,"echo timeout auto ignore");
            Debug.Log("LAST_KEY_POLISH_CORE_PASS "+count);
        }
    }
}
