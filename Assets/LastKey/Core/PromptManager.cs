using System;
using System.Collections.Generic;
using System.Linq;

namespace LastKey.Core
{
    // A round consumes every bank entry at most once. Recent entries survive round changes.
    public sealed class PromptManager
    {
        readonly Random random;
        readonly Queue<string> recent=new Queue<string>();
        readonly Queue<string> bag=new Queue<string>();
        readonly HashSet<string> used=new HashSet<string>();
        int fallback;
        string last;
        public PromptManager(int seed){random=new Random(seed);}
        public void BeginRound(string[] bank)
        {
            if(bank==null||bank.Length==0||bank.Any(string.IsNullOrWhiteSpace))throw new ArgumentException("Empty content bank");
            used.Clear();bag.Clear();
            // Difficulty progresses in length bands; each band is shuffled, never cycled.
            var sorted=bank.Distinct().OrderBy(s=>s.Length).ToArray();
            int band=Math.Max(1,(sorted.Length+2)/3);
            for(int offset=0;offset<sorted.Length;offset+=band)
            {
                var chunk=sorted.Skip(offset).Take(band).OrderBy(_=>random.Next()).ToArray();
                foreach(var text in chunk.Where(s=>!recent.Contains(s)).Concat(chunk.Where(s=>recent.Contains(s))))bag.Enqueue(text);
            }
        }
        public string Next()
        {
            string text;
            do{text=bag.Count>0?bag.Dequeue():"Registro "+(++fallback).ToString("D4")+": conserva el ritmo y verifica cada tecla.";}while(used.Contains(text)||text==last);
            used.Add(text);last=text;recent.Enqueue(text);while(recent.Count>5)recent.Dequeue();return text;
        }
    }
}
