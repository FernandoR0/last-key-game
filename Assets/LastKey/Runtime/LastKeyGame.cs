using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using LastKey.Core;
namespace LastKey
{
    public enum ExperienceState {Boot,Loading,MainMenu,ModeSelect,CharacterSelect,MatchSetup,RoundIntro,Countdown,Gameplay,RoundResult,FinalResult,Paused}
    public sealed partial class LastKeyGame:MonoBehaviour
    {
        public Competition Game {get;private set;}
        public ArenaFactory Arena {get;private set;}
        public Canvas Canvas {get;private set;}
        public CameraDirector Cameras {get;private set;}
        public ExperienceState Flow {get;private set;}
        public int HumanIndex {get;private set;}
        public RoundMode SelectedMode {get;private set;}
        public float StateTime {get;private set;}
        public GameAudio Sound {get;private set;}
        public MusicManager Music {get;private set;}
        Rules rules;ContentBank content;TMP_FontAsset font;
        Transform menu;GameObject hud,intro,nullBacking,echoBacking;
        TMP_Text top,metrics,typing,rivals,directive,echo,caption,flash,dangerLabel,announcement; CanvasGroup announceGroup; float announceUntil;
        Image danger,loading;CompetitionEffects effects;
        bool echoChoice=true; bool cinematicPreview,visualPreview,wide,echoAnnounced,warning;Directive priorDirective;int priorCorrect,priorCombo,priorErrors;float flashUntil;
        readonly Color ink=new Color(.145f,.23f,.285f),cyan=new Color(.737f,.91f,.898f),muted=new Color(.39f,.49f,.55f);
        public static readonly string[] Aliases={"CIPHER","UPLINK","RELAY","VECTOR"};
        public static readonly string[] Names={"Valeria Rojas","Ethan Cole","Akira Mori","Noah Bennett"};
        public static readonly string[] Specialties={"Seguridad y análisis de sistemas","Redes y conectividad","Integración y arquitectura","Datos y optimización"};
        void Awake()
        {
            Time.timeScale=1;Application.targetFrameRate=60;AudioListener.volume=Mathf.Clamp01(PlayerPrefs.GetFloat("LAST_KEY_MASTER_VOLUME",1));
            rules=JsonUtility.FromJson<Rules>(Resources.Load<TextAsset>("last_key_rules").text);content=JsonUtility.FromJson<ContentBank>(Resources.Load<TextAsset>("last_key_content").text);
            font=Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");Game=new Competition(rules,content);Arena=new ArenaFactory();Arena.Create(font);Cameras=new CameraDirector(Arena);effects=new CompetitionEffects(Arena);
            Sound=gameObject.AddComponent<GameAudio>();Sound.Initialize();Music=gameObject.AddComponent<MusicManager>();Music.Initialize(this);Sound.Route(Music);CreateCanvas();Switch(ExperienceState.Boot);
            if(Environment.GetCommandLineArgs().Contains("--music-runtime-qa"))gameObject.AddComponent<MusicRuntimeQa>();
            if(Environment.GetCommandLineArgs().Contains("--music-evidence")){gameObject.AddComponent<PolishEvidence>();gameObject.AddComponent<MusicCapture>();}
            if(Environment.GetCommandLineArgs().Contains("--evidence-v2"))gameObject.AddComponent<PolishEvidence>();
            if(Environment.GetCommandLineArgs().Contains("--wardrobe-review"))gameObject.AddComponent<WardrobeShow>();
            if(Environment.GetCommandLineArgs().Contains("--pose-audit"))gameObject.AddComponent<PoseAudit>();
            if(Environment.GetCommandLineArgs().Contains("--minimal-ui-qa"))gameObject.AddComponent<MinimalInterfaceQa>();
            if(Environment.GetCommandLineArgs().Contains("--typing-motion-evidence"))gameObject.AddComponent<TypingMotionEvidence>();
            if(Environment.GetCommandLineArgs().Contains("--cinematic-evidence"))gameObject.AddComponent<CinematicEvidence>();
        }
        void Update()
        {
            StateTime+=Time.deltaTime;
            if(NavigateInterface())return;
            if(Flow==ExperienceState.Boot&&StateTime>.15f)Switch(ExperienceState.Loading);
            else if(Flow==ExperienceState.Loading){loading.fillAmount=Mathf.Clamp01(StateTime/2);if(StateTime>2.1f)Switch(ExperienceState.MainMenu);}
            if(Flow==ExperienceState.RoundIntro)
            {
                caption.text="";
                if(StateTime>=MusicManager.IntroSeconds){if(cinematicPreview)StateTime=0;else Switch(ExperienceState.Countdown);}
            }
            if(Flow==ExperienceState.Countdown){caption.text="RONDA "+Game.Round+"\n"+Mathf.Max(1,3-Mathf.FloorToInt(StateTime));Cameras.Gameplay();if(StateTime>=3){echoAnnounced=warning=false;priorCorrect=priorCombo=priorErrors=0;priorDirective=Directive.None;Switch(ExperienceState.Gameplay);}}
            if(Flow==ExperienceState.Gameplay)
            {
                if(Input.GetKeyDown(KeyCode.Escape)){Switch(ExperienceState.Paused);return;}
                if(Game.EchoOffered&&!Game.Players[HumanIndex].EchoResolved&&Input.GetKeyDown(KeyCode.Tab))echoChoice=!echoChoice; if(Game.EchoOffered&&!Game.Players[HumanIndex].EchoResolved&&Input.GetKeyDown(KeyCode.Return))RespondEcho(echoChoice);
                if(Input.GetKeyDown(KeyCode.F3)){wide=!wide;Cameras.Gameplay(wide);}
                foreach(char ch in Input.inputString)if(ch!='\b'&&ch!='\r'&&ch!='\n'&&ch!='\t'){Arena.TouchKeys[HumanIndex].Press(ch);Game.Input(HumanIndex,ch);}
                Game.Tick(Time.deltaTime);Feedback();effects.Tick(Game,Sound);
                Cameras.Gameplay(wide); // Elimination effects remain visible in the world without interrupting the player camera.
                if(Game.Phase==Phase.Results)Switch(ExperienceState.RoundResult);else if(Game.Phase==Phase.Finished)Switch(ExperienceState.FinalResult);
            }
            else if(Flow==ExperienceState.Paused&&Input.GetKeyDown(KeyCode.Escape))Switch(ExperienceState.Gameplay);
            Arena.Animate(Time.time,Game);if(Flow!=ExperienceState.RoundIntro){Arena.Board.text="";for(int i=0;i<4;i++)Arena.Screens[i].text=Flow==ExperienceState.Gameplay&&i!=HumanIndex?Game.Players[i].Name:"";}if(Flow==ExperienceState.RoundIntro)Arena.ShowOpening(StateTime);Refresh();if(visualPreview)Canvas.enabled=Flow==ExperienceState.RoundResult||Flow==ExperienceState.FinalResult||Flow==ExperienceState.Paused||Flow==ExperienceState.MainMenu;
        }
        void LateUpdate()
        {
            if(Arena==null||Game==null)return;
            // Final animation/contact corrections must run after Unity evaluates Animator streams.
            foreach(var c in Arena.Characters)c.GetComponent<MatchCharacterVisual>().Present(Game,Flow,StateTime);
            if(Flow==ExperienceState.RoundIntro){Cameras.Intro(Mathf.Clamp01(StateTime/MusicManager.IntroSeconds));intro.SetActive(false);}
        }
        public void StopSessionAudio(){Music.StopSessionAudio();Sound.StopSessionAudio();}
        void Feedback()
        {
            var p=Game.Players[HumanIndex];if(p.Correct>priorCorrect){Sound.Cue("typing");flash.text="<color=#42D5CB>+</color>";flashUntil=Time.time+.15f;}
            if(p.Errors>priorErrors){errorPulseUntil=Time.time+.45f;flash.text="<color=#FF5860>!</color>";flashUntil=Time.time+.35f;}priorErrors=p.Errors;if(Time.time>flashUntil)flash.text="";
            if(p.Combo/20>priorCombo/20){comboPulseAt=Time.time;Sound.Cue("combo");flash.text="<color=#E5AE63>Combo</color>";flashUntil=Time.time+.65f;}priorCorrect=p.Correct;priorCombo=p.Combo;
            if(Game.ActiveDirective!=Directive.None&&Game.ActiveDirective!=priorDirective){Sound.Cue("null");Announce("NULL · Nueva directiva",cyan);}priorDirective=Game.ActiveDirective;
            if(Game.EchoOffered&&!echoAnnounced){echoAnnounced=true;Sound.Cue("echo");Announce("ECHO · Reto opcional",gold);}if(p.Danger>=40&&!warning){warning=true;Sound.Cue("warning");}
        }
        void Announce(string text,Color color){announcement.text=text;announcement.color=color;announceUntil=Time.time+3;}
        RectTransform Rect(GameObject o,Transform p,float x,float y,float w,float h){o.transform.SetParent(p,false);var r=o.GetComponent<RectTransform>();r.anchorMin=r.anchorMax=new Vector2(0,1);r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);return r;}
        GameObject Panel(string n,float x,float y,float w,float h,Color c,Transform p=null){var o=new GameObject(n,typeof(RectTransform),typeof(Image));Rect(o,p??Canvas.transform,x,y,w,h);o.GetComponent<Image>().color=c;o.GetComponent<Image>().raycastTarget=false;return o;}
        TMP_Text Label(string n,string s,float x,float y,float w,float h,float size,Color c,Transform p=null){var o=new GameObject(n,typeof(RectTransform),typeof(TextMeshProUGUI));Rect(o,p??Canvas.transform,x,y,w,h);var t=o.GetComponent<TextMeshProUGUI>();t.font=font;t.text=s;t.fontSize=size;t.color=c;t.textWrappingMode=TextWrappingModes.Normal;t.raycastTarget=false;t.overflowMode=TextOverflowModes.Ellipsis;return t;}
        void Button(string s,float x,float y,float w,Action a,Transform p=null,Color? c=null){ActionButton(s,x,y,w,a,p,c);}






        public void BeginVisualPreview(){visualPreview=true;BeginIntro();}
        public void BeginCinematicPreview(){cinematicPreview=true;BeginIntro();for(int i=0;i<4;i++)if(i!=HumanIndex)foreach(var renderer in Arena.Stations[i].GetComponentsInChildren<Renderer>())renderer.enabled=false;Canvas.enabled=false;}
        public void EnableBrowserAudio(){Music.EnableBrowserAudio();}
        public void OpenModeSelect(){Switch(ExperienceState.ModeSelect);}
        public void OpenCharacterSelect(){Switch(ExperienceState.CharacterSelect);}
        public void ChooseCharacter(int i){HumanIndex=i;Arena.Arrange(i);Switch(ExperienceState.CharacterSelect);}
        public void ConfirmCharacter(){Switch(ExperienceState.MatchSetup);}
        public void BeginIntro(){if(Game.Phase==Phase.Ready&&Game.HumanIndex!=HumanIndex)Game=new Competition(rules,content,humanIndex:HumanIndex);Game.StartRound(SelectedMode);Switch(Game.Round==1?ExperienceState.RoundIntro:ExperienceState.Countdown);if(Flow==ExperienceState.RoundIntro){foreach(var c in Arena.Characters)c.GetComponent<MatchCharacterVisual>().Present(Game,Flow,0);Cameras.SnapIntro();}}
        public void Rematch(){Game=new Competition(rules,content,humanIndex:HumanIndex);BeginIntro();}
        public void MainMenu(){Game=new Competition(rules,content,humanIndex:HumanIndex);Switch(ExperienceState.MainMenu);}
        public void StartNext(){if(Game.Phase==Phase.Finished)Rematch();else BeginIntro();}
        public void SelectMode(RoundMode mode){SelectedMode=mode;}
        public void RespondEcho(bool accept){if(accept)Game.AcceptEcho(HumanIndex);else Game.IgnoreEcho(HumanIndex);EventSystem.current.SetSelectedGameObject(null);}

        static string Escape(string s){return s.Replace("&","&amp;").Replace("<","&lt;").Replace(">","&gt;");}
    }
}
