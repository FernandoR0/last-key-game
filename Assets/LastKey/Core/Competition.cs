using System;
using System.Collections.Generic;
using System.Linq;

namespace LastKey.Core
{
    public enum Phase { Ready, Running, Results, Finished }
    public enum RoundMode { Classic, Code, Numbers, Memory, SuddenDeath }
    public enum Directive { None, PrecisionProtocol, SpeedCheck, CleanStreak }

    [Serializable] public class Rules
    {
        public int rounds = 5;
        public float roundSeconds = 60;
        public int characterPoints = 1, wordBonus = 8, streakStep = 20, maxMultiplier = 4;
        public float errorDanger = 8, recoveryPerCharacter = 0.35f;
        public int directiveBonus = 75, cleanTarget = 25;
        public float speedTarget = 30, directiveSeconds = 12, memoryRevealSeconds = 4;
        public int echoReward = 150;
        public float echoRecovery = 15, echoPenalty = 15, echoSeconds = 12;
        public float[] botWpm = { 29, 36, 43 };
        public float[] botAccuracy = { .90f, .94f, .96f };
    }

    [Serializable] public class ContentBank
    {
        public string[] classic = {
            "La pantalla se apagó, pero el contador siguió avanzando.",
            "No confíes en la señal. Comprueba primero los registros.",
            "Cada tecla cuenta. Respira y mantén el ritmo.",
            "La puerta sigue cerrada. Una nueva ronda está por comenzar.",
            "Escribe con calma. La precisión también gana carreras."
        };
        public string[] code = { "if (ready) return true;", "score += bonus;", "count = count + 1;" };
        public string[] numbers = { "4821-73-906", "NODE-07-4821", "906 274 831 560" };
    }

    public sealed class Competitor
    {
        public readonly string Name;
        public readonly bool Bot;
        public int Correct, Errors, Combo, BestCombo, RoundScore, TotalScore, Cursor, Completed;
        public float Danger, ActiveSeconds;
        public bool Eliminated { get { return Danger >= 100; } }
        public float Accuracy { get { return Correct + Errors == 0 ? 100 : 100f * Correct / (Correct + Errors); } }
        public float Wpm { get { return ActiveSeconds <= 0 ? 0 : Correct / 5f * 60f / ActiveSeconds; } }
        public bool DirectiveAwarded, EchoAccepted, EchoResolved;
        public int DirectiveStreak, EchoCursor;
        internal int DirectiveStartCorrect, DirectiveStartErrors;
        public float EchoRemaining;
        internal float BotCredit;
        internal int WordCorrect;
        public Competitor(string name, bool bot) { Name = name; Bot = bot; }
        internal void Reset()
        {
            Correct = Errors = Combo = BestCombo = RoundScore = Cursor = Completed = 0;
            Danger = ActiveSeconds = BotCredit = 0; WordCorrect = 0;
            DirectiveAwarded = EchoAccepted = EchoResolved = false;
            DirectiveStreak = EchoCursor = 0; EchoRemaining = 0;
        }
    }

    public sealed class Competition
    {
        public readonly Rules Rules;
        public readonly ContentBank Content;
        public readonly Competitor[] Players = {
            new Competitor("CIPHER", false), new Competitor("UPLINK", true),
            new Competitor("RELAY", true), new Competitor("VECTOR", true)
        };
        public Phase Phase { get; private set; } = Phase.Ready;
        public int Round { get; private set; }
        public RoundMode Mode { get; private set; }
        public float Remaining { get; private set; }
        public float Elapsed { get { return Rules.roundSeconds - Remaining; } }
        public Directive ActiveDirective { get; private set; }
        public float DirectiveRemaining { get; private set; }
        public bool EchoOffered { get; private set; }
        public const string EchoText = "NODE-07-RECOVERY";
        public string Text { get; private set; } = "";
        public string LastEvent { get; private set; } = "Preparado para la competencia.";
        readonly Random random;
        bool directiveIssued, echoIssued;

        public Competition(Rules rules = null, ContentBank content = null, int seed = 4207)
        {
            Rules = rules ?? new Rules(); Content = content ?? new ContentBank(); random = new Random(seed);
            if (Rules.rounds < 1 || Rules.roundSeconds <= 0 || Rules.directiveSeconds <= 0 ||
                Rules.streakStep < 1 || Rules.maxMultiplier < 1 || Rules.botWpm == null ||
                Rules.botWpm.Length != 3 || Rules.botAccuracy == null || Rules.botAccuracy.Length != 3)
                throw new ArgumentException("Invalid competition rules");
        }

        public void StartRound(RoundMode mode = RoundMode.Classic)
        {
            if (Phase != Phase.Ready && Phase != Phase.Results) return;
            if (Round >= Rules.rounds) return;
            Round++; Mode = mode; Remaining = Rules.roundSeconds;
            foreach (var p in Players) p.Reset();
            ActiveDirective = Directive.None; DirectiveRemaining = 0;
            directiveIssued = echoIssued = EchoOffered = false;
            var bank = mode == RoundMode.Code ? Content.code : mode == RoundMode.Numbers ? Content.numbers : Content.classic;
            if (bank == null || bank.Length == 0 || bank.Any(string.IsNullOrEmpty)) throw new ArgumentException("Empty content bank");
            Text = bank[(Round - 1) % bank.Length];
            Phase = Phase.Running; LastEvent = "RONDA " + Round + " — " + mode;
        }

        public void Tick(float dt)
        {
            if (Phase != Phase.Running || dt <= 0) return;
            // Small steps keep event and bot simulation stable under different frame rates.
            while (dt > 0 && Phase == Phase.Running)
            {
                float step = Math.Min(dt, .05f); dt -= step; Step(step);
            }
        }

        void Step(float dt)
        {
            dt = Math.Min(dt, Remaining); Remaining = Math.Max(0, Remaining - dt);
            if (!directiveIssued && Elapsed >= Rules.roundSeconds * .2f)
            {
                directiveIssued = true; BeginDirective((Directive)(1 + (Round - 1) % 3));
            }
            if (!echoIssued && Elapsed >= Rules.roundSeconds * .55f)
            {
                echoIssued = EchoOffered = true; LastEvent = "ECHO: reto opcional disponible";
            }
            for (int i = 0; i < Players.Length; i++)
            {
                var p = Players[i]; if (p.Eliminated) continue;
                p.ActiveSeconds += dt;
                if (p.EchoAccepted && !p.EchoResolved)
                {
                    p.EchoRemaining = Math.Max(0, p.EchoRemaining - dt);
                    if (p.EchoRemaining <= 0) ResolveEcho(p, false);
                }
                if (!p.Bot) continue;
                if (EchoOffered && !p.EchoAccepted && !p.EchoResolved && p.Danger < 50 && i != 1) AcceptEcho(i);
                if (Mode == RoundMode.Memory && Elapsed < Rules.memoryRevealSeconds) continue;
                p.BotCredit += dt * Rules.botWpm[i - 1] * 5 / 60;
                while (p.BotCredit >= 1 && !p.Eliminated)
                {
                    p.BotCredit--;
                    string target = p.EchoAccepted && !p.EchoResolved ? EchoText : Text;
                    int cursor = p.EchoAccepted && !p.EchoResolved ? p.EchoCursor : p.Cursor;
                    char expected = target[cursor];
                    Input(i, random.NextDouble() < Rules.botAccuracy[i - 1] ? expected : (expected == 'x' ? 'z' : 'x'));
                }
            }
            if (ActiveDirective != Directive.None)
            {
                DirectiveRemaining = Math.Max(0, DirectiveRemaining - dt);
                if (DirectiveRemaining <= 0)
                {
                    foreach (var p in Players)
                    {
                        if (p.Eliminated) continue;
                        if (ActiveDirective == Directive.SpeedCheck && (p.Correct-p.DirectiveStartCorrect)/5f*60/Rules.directiveSeconds >= Rules.speedTarget) AwardDirective(p);
                        if (ActiveDirective == Directive.PrecisionProtocol && p.Errors == p.DirectiveStartErrors) AwardDirective(p);
                    }
                    ActiveDirective = Directive.None;
                }
            }
            if (Remaining <= 0 || Players.All(p => p.Eliminated)) EndRound();
        }

        public void BeginDirective(Directive directive)
        {
            if (Phase != Phase.Running) return;
            ActiveDirective = directive; DirectiveRemaining = Rules.directiveSeconds;
            foreach (var p in Players) { p.DirectiveAwarded = false; p.DirectiveStreak = 0; p.DirectiveStartCorrect=p.Correct; p.DirectiveStartErrors=p.Errors; }
            LastEvent = "NULL: " + directive;
        }

        public bool Input(int index, char typed)
        {
            if (Phase != Phase.Running || index < 0 || index >= Players.Length) return false;
            var p = Players[index]; if (p.Eliminated || (Mode == RoundMode.Memory && Elapsed < Rules.memoryRevealSeconds)) return false;
            bool echo = p.EchoAccepted && !p.EchoResolved;
            string target = echo ? EchoText : Text; int cursor = echo ? p.EchoCursor : p.Cursor;
            if (typed != target[cursor])
            {
                p.Errors++; p.Combo = p.DirectiveStreak = 0; p.WordCorrect = 0;
                float penalty = Rules.errorDanger * (Mode == RoundMode.SuddenDeath ? 4 : 1) * (ActiveDirective == Directive.PrecisionProtocol ? 2 : 1);
                p.Danger = Math.Min(100, p.Danger + penalty);
                if (p.Eliminated) LastEvent = p.Name + " fuera de esta ronda. Vuelve en la siguiente.";
                return false;
            }
            p.Correct++; p.Combo++; p.BestCombo = Math.Max(p.BestCombo, p.Combo); p.DirectiveStreak++;
            p.RoundScore += Rules.characterPoints * Math.Min(Rules.maxMultiplier, 1 + p.Combo / Rules.streakStep);
            p.Danger = Math.Max(0, p.Danger - Rules.recoveryPerCharacter);
            if (ActiveDirective == Directive.CleanStreak && p.DirectiveStreak >= Rules.cleanTarget) AwardDirective(p);
            if (echo)
            {
                p.EchoCursor++; if (p.EchoCursor >= EchoText.Length) ResolveEcho(p, true);
            }
            else
            {
                p.WordCorrect++;
                if (typed == ' ' || cursor == target.Length - 1)
                { if (p.WordCorrect > 0) p.RoundScore += Rules.wordBonus; p.WordCorrect = 0; }
                p.Cursor++; if (p.Cursor >= target.Length) { p.Cursor = 0; p.Completed++; }
            }
            return true;
        }

        void AwardDirective(Competitor p)
        {
            if (p.DirectiveAwarded) return;
            p.DirectiveAwarded = true; p.RoundScore += Rules.directiveBonus;
            LastEvent = p.Name + ": Directiva NULL cumplida +" + Rules.directiveBonus;
        }
        public bool AcceptEcho(int index)
        {
            if (Phase != Phase.Running || !EchoOffered || index < 0 || index >= Players.Length) return false;
            var p = Players[index]; if (p.Eliminated || p.EchoAccepted || p.EchoResolved) return false;
            p.EchoAccepted = true; p.EchoRemaining = Rules.echoSeconds; p.EchoCursor = 0;
            LastEvent = p.Name + " acepta ECHO INTERCEPT"; return true;
        }
        public void IgnoreEcho(int index)
        {
            if (Phase == Phase.Running && EchoOffered && index >= 0 && index < Players.Length && !Players[index].EchoAccepted) Players[index].EchoResolved = true;
        }
        void ResolveEcho(Competitor p, bool success)
        {
            p.EchoResolved = true;
            if (success) { p.RoundScore += Rules.echoReward; p.Danger = Math.Max(0, p.Danger - Rules.echoRecovery); }
            else p.Danger = Math.Min(100, p.Danger + Rules.echoPenalty);
            LastEvent = p.Name + (success ? " ECHO completado +" + Rules.echoReward : " ECHO fallido +" + Rules.echoPenalty + " Danger");
        }
        public void EndRound()
        {
            if (Phase != Phase.Running) return;
            foreach (var p in Players)
            {
                if (p.EchoAccepted && !p.EchoResolved) ResolveEcho(p, false);
                p.TotalScore += p.RoundScore;
            }
            Phase = Round >= Rules.rounds ? Phase.Finished : Phase.Results;
            LastEvent = Phase == Phase.Finished ? "THE LAST KEY — " + string.Join(" / ",Winners().Select(p=>p.Name)) : "Resultados de ronda " + Round;
        }
        // Total score always decides victory. Alphabetical ordering only stabilizes tied display rows.
        public Competitor[] Ranking() { return Players.OrderByDescending(p => p.TotalScore + (Phase == Phase.Running ? p.RoundScore : 0)).ThenBy(p => p.Name).ToArray(); }
        public Competitor[] Winners() { int max = Players.Max(p => p.TotalScore); return Players.Where(p => p.TotalScore == max).ToArray(); }
    }
}
