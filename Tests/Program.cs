using System;
using System.Linq;
using LastKey.Core;

static class Program
{
    static int passed;
    static void Check(bool condition, string name) { if (!condition) throw new Exception(name); passed++; Console.WriteLine("PASS " + name); }
    static Competition Game(Rules r = null) { var g = new Competition(r); g.StartRound(); return g; }
    static void Main()
    {
        var g = Game();
        Check(g.Players.Length == 4 && g.Players.Count(p => p.Bot) == 3, "one human, three bots");
        g.Tick(1); g.Input(0, g.Text[0]);
        Check(g.Players[0].Correct == 1 && g.Players[0].RoundScore == 1 && Math.Abs(g.Players[0].Wpm - 12) < .01, "correct character and WPM");
        g.Input(0, '#'); Check(g.Players[0].Errors == 1 && g.Players[0].Accuracy == 50 && g.Players[0].Combo == 0, "error accuracy combo");
        var a = Game(); a.Input(0, '#'); var b = Game(); b.BeginDirective(Directive.PrecisionProtocol); b.Input(0, '#');
        Check(b.Players[0].Danger == a.Players[0].Danger * 2, "precision protocol doubles danger");
        while (!a.Players[0].Eliminated) a.Input(0, '#');
        int score = a.Players[0].RoundScore; a.Input(0, a.Text[0]); Check(a.Players[0].RoundScore == score, "eliminated player cannot score");
        a.EndRound(); a.StartRound(); Check(!a.Players[0].Eliminated && a.Round == 2, "elimination ends at next round");
        var c = Game(); c.BeginDirective(Directive.CleanStreak);
        for (int i = 0; i < c.Rules.cleanTarget; i++) c.Input(0, c.Text[c.Players[0].Cursor]);
        Check(c.Players[0].DirectiveAwarded && c.Players[0].RoundScore >= c.Rules.directiveBonus, "clean streak award");
        var d = Game(); d.Tick(34); Check(d.EchoOffered, "echo offered to all");
        Check(d.AcceptEcho(0), "echo optional accept");
        foreach (char ch in Competition.EchoText) d.Input(0, ch);
        Check(d.Players[0].EchoResolved && d.Players[0].RoundScore >= d.Rules.echoReward, "echo success reward");
        var e = Game(); e.Tick(34); e.AcceptEcho(0); e.Tick(13);
        Check(e.Players[0].EchoResolved && Math.Abs(e.Players[0].Danger - e.Rules.echoPenalty) < .01, "echo timeout penalty");
        var f = Game(new Rules { rounds = 2, roundSeconds = 10 }); f.Input(0, f.Text[0]); f.EndRound(); int total = f.Players[0].TotalScore;
        f.EndRound(); Check(f.Players[0].TotalScore == total, "results cannot double count score");
        f.StartRound(); Check(f.Players[0].TotalScore == total && f.Players[0].RoundScore == 0, "accumulated score survives rounds");
        f.Tick(10); Check(f.Phase == Phase.Finished && f.Winners().All(p => p.TotalScore == f.Players.Max(q => q.TotalScore)), "winner determined by accumulated score");
        Check(f.Players.Skip(1).All(p => p.Correct > 0), "bots type through same validation");
        var m = new Competition(); m.StartRound(RoundMode.Memory); m.Input(0, m.Text[0]); Check(m.Players[0].Correct == 0, "memory reveal prevents premature input");
        m.Tick(4.1f); m.Input(0, m.Text[0]); Check(m.Players[0].Correct == 1, "memory input after reveal");
        var sd = new Competition(); sd.StartRound(RoundMode.SuddenDeath); sd.Input(0, '#'); Check(sd.Players[0].Danger == 32, "sudden death penalty");
        var h1 = Game(); var h2 = Game(); h1.Tick(15); for (int i=0; i<150; i++) h2.Tick(.1f);
        Check(h1.Players.Skip(1).Select(p=>p.Correct).SequenceEqual(h2.Players.Skip(1).Select(p=>p.Correct)), "bot simulation stable across frame subdivisions");
        var s = Game(new Rules { roundSeconds = 200 }); s.BeginDirective(Directive.SpeedCheck);
        for(int i=0;i<35;i++)s.Input(0,s.Text[s.Players[0].Cursor]);s.Tick(s.Rules.directiveSeconds+.1f);
        Check(s.Players[0].DirectiveAwarded,"speed check uses directive window");
        var ignored = Game(); ignored.Tick(34); ignored.IgnoreEcho(0);float danger=ignored.Players[0].Danger; ignored.Tick(13);
        Check(ignored.Players[0].Danger==danger&&!ignored.AcceptEcho(0),"ignoring echo has no penalty");
        var combo = Game(); for(int i=0;i<22;i++)combo.Input(0,combo.Text[combo.Players[0].Cursor]);int previous=combo.Players[0].RoundScore;
        combo.Input(0,combo.Text[combo.Players[0].Cursor]);Check(combo.Players[0].RoundScore-previous>=2,"clean combo multiplies score");
        Console.WriteLine("TOTAL " + passed + " PASS");
    }
}
