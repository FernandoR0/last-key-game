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
    public sealed class LastKeyGame:MonoBehaviour
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
        Transform menu;GameObject hud,intro;
        TMP_Text top,metrics,typing,rivals,directive,echo,caption,flash,dangerLabel;
        Image danger,loading;CompetitionEffects effects;
        bool wide,echoAnnounced,warning;Directive priorDirective;int priorCorrect,priorCombo,priorErrors;float flashUntil;
        readonly Color ink=new Color(.025f,.04f,.055f,.92f),cyan=new Color(.2f,.86f,.86f),muted=new Color(.65f,.73f,.78f);
        public static readonly string[] Aliases={"CIPHER","UPLINK","RELAY","VECTOR"};
        public static readonly string[] Names={"Valeria Rojas","Ethan Cole","Akira Mori","Noah Bennett"};
        public static readonly string[] Specialties={"Seguridad y análisis de sistemas","Redes y conectividad","Integración y arquitectura","Datos y optimización"};
        void Awake()
        {
            Time.timeScale=1;Application.targetFrameRate=60;
            rules=JsonUtility.FromJson<Rules>(Resources.Load<TextAsset>("last_key_rules").text);content=JsonUtility.FromJson<ContentBank>(Resources.Load<TextAsset>("last_key_content").text);
            font=Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");Game=new Competition(rules,content);Arena=new ArenaFactory();Arena.Create(font);Cameras=new CameraDirector(Arena);effects=new CompetitionEffects(Arena);
            Sound=gameObject.AddComponent<GameAudio>();Sound.Initialize();Music=gameObject.AddComponent<MusicManager>();Music.Initialize(this);Sound.Route(Music);CreateCanvas();Switch(ExperienceState.Boot);
            if(Environment.GetCommandLineArgs().Contains("--music-runtime-qa"))gameObject.AddComponent<MusicRuntimeQa>();
            if(Environment.GetCommandLineArgs().Contains("--music-evidence")){gameObject.AddComponent<PolishEvidence>();gameObject.AddComponent<MusicCapture>();}
            if(Environment.GetCommandLineArgs().Contains("--evidence-v2"))gameObject.AddComponent<PolishEvidence>();
            if(Environment.GetCommandLineArgs().Contains("--wardrobe-review"))gameObject.AddComponent<WardrobeShow>();
            if(Environment.GetCommandLineArgs().Contains("--pose-audit"))gameObject.AddComponent<PoseAudit>();
        }
        void Update()
        {
            StateTime+=Time.deltaTime;
            if(Flow==ExperienceState.Boot&&StateTime>.15f)Switch(ExperienceState.Loading);
            else if(Flow==ExperienceState.Loading){loading.fillAmount=Mathf.Clamp01(StateTime/2);if(StateTime>2.1f)Switch(ExperienceState.MainMenu);}
            if(Flow==ExperienceState.RoundIntro)
            {
                float t=Mathf.Clamp01(StateTime/MusicManager.IntroSeconds);Cameras.Intro(t);caption.text=t<.25f?"RONDA "+Game.Round+" / DESPIERTA":t<.5f?"COLLAR ACTIVO / CONTROL DE COMPETENCIA":t<.75f?"TU ESTACIÓN / PRECISIÓN BAJO PRESIÓN":"CUATRO PARTICIPANTES. UNA CLASIFICACIÓN.";
                if(StateTime>=MusicManager.IntroSeconds||Input.GetKeyDown(KeyCode.Space))Switch(ExperienceState.Countdown);
            }
            if(Flow==ExperienceState.Countdown){caption.text="RONDA "+Game.Round+"\n"+Mathf.Max(1,3-Mathf.FloorToInt(StateTime));Cameras.Gameplay();if(StateTime>=3){echoAnnounced=warning=false;priorCorrect=priorCombo=priorErrors=0;priorDirective=Directive.None;Switch(ExperienceState.Gameplay);}}
            if(Flow==ExperienceState.Gameplay)
            {
                if(Input.GetKeyDown(KeyCode.Escape)){Switch(ExperienceState.Paused);return;}
                if(Input.GetKeyDown(KeyCode.F1))RespondEcho(true);if(Input.GetKeyDown(KeyCode.F2))RespondEcho(false);
                if(Input.GetKeyDown(KeyCode.F3)){wide=!wide;Cameras.Gameplay(wide);}
                foreach(char ch in Input.inputString)if(ch!='\b'&&ch!='\r'&&ch!='\n')Game.Input(HumanIndex,ch);
                Game.Tick(Time.deltaTime);Feedback();effects.Tick(Game,Sound);
                if(Time.time-effects.LastEliminationTime<1.7f)Cameras.Event(effects.LastEliminated,effects.LastEliminated==HumanIndex);else Cameras.Gameplay(wide);
                if(Game.Phase==Phase.Results)Switch(ExperienceState.RoundResult);else if(Game.Phase==Phase.Finished)Switch(ExperienceState.FinalResult);
            }
            else if(Flow==ExperienceState.Paused&&Input.GetKeyDown(KeyCode.Escape))Switch(ExperienceState.Gameplay);
            Arena.Animate(Time.time,Game);foreach(var c in Arena.Characters){var m=c.GetComponent<CharacterMotion>();m.Wakeup=Flow==ExperienceState.RoundIntro;m.Preview=Flow==ExperienceState.CharacterSelect||Flow==ExperienceState.Countdown;m.IntroProgress=Mathf.Clamp01(StateTime/MusicManager.IntroSeconds);}Refresh();
        }
        void Feedback()
        {
            var p=Game.Players[HumanIndex];if(p.Correct>priorCorrect){Sound.Cue("typing");flash.text="<color=#42D5CB>+</color>";flashUntil=Time.time+.15f;}
            if(p.Errors>priorErrors){flash.text="<color=#FF5860>!</color>";flashUntil=Time.time+.35f;}priorErrors=p.Errors;if(Time.time>flashUntil)flash.text="";
            if(p.Combo/20>priorCombo/20)Sound.Cue("combo");priorCorrect=p.Correct;priorCombo=p.Combo;
            if(Game.ActiveDirective!=Directive.None&&Game.ActiveDirective!=priorDirective)Sound.Cue("null");priorDirective=Game.ActiveDirective;
            if(Game.EchoOffered&&!echoAnnounced){echoAnnounced=true;Sound.Cue("echo");}if(p.Danger>=40&&!warning){warning=true;Sound.Cue("warning");}
        }
        RectTransform Rect(GameObject o,Transform p,float x,float y,float w,float h){o.transform.SetParent(p,false);var r=o.GetComponent<RectTransform>();r.anchorMin=r.anchorMax=new Vector2(0,1);r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);return r;}
        GameObject Panel(string n,float x,float y,float w,float h,Color c,Transform p=null){var o=new GameObject(n,typeof(RectTransform),typeof(Image));Rect(o,p??Canvas.transform,x,y,w,h);o.GetComponent<Image>().color=c;return o;}
        TMP_Text Label(string n,string s,float x,float y,float w,float h,float size,Color c,Transform p=null){var o=new GameObject(n,typeof(RectTransform),typeof(TextMeshProUGUI));Rect(o,p??Canvas.transform,x,y,w,h);var t=o.GetComponent<TextMeshProUGUI>();t.font=font;t.text=s;t.fontSize=size;t.color=c;t.textWrappingMode=TextWrappingModes.Normal;t.raycastTarget=false;return t;}
        void Button(string s,float x,float y,float w,Action a,Transform p=null,Color? c=null){var o=Panel(s,x,y,w,66,c??new Color(.075f,.28f,.31f),p);o.AddComponent<Button>().onClick.AddListener(()=>{Sound.Cue("ui");a();});Label("Label",s,18,13,w-36,45,24,Color.white,o.transform);}
        void CreateCanvas()
        {
            var o=new GameObject("Presentation and compact HUD",typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));Canvas=o.GetComponent<Canvas>();Canvas.renderMode=RenderMode.ScreenSpaceOverlay;var s=o.GetComponent<CanvasScaler>();s.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;s.referenceResolution=new Vector2(1920,1080);s.matchWidthOrHeight=.5f;new GameObject("EventSystem",typeof(EventSystem),typeof(StandaloneInputModule));
            menu=Panel("Screen",0,0,1920,1080,Color.clear).transform;
            intro=Panel("Intro",0,0,1920,1080,Color.clear);Panel("Caption",440,910,1040,110,ink,intro.transform);caption=Label("Caption","",470,927,980,90,28,Color.white,intro.transform);caption.alignment=TextAlignmentOptions.Center;
            hud=Panel("Gameplay HUD",0,0,1920,1080,Color.clear);Panel("Header",24,20,1872,66,ink,hud.transform);top=Label("Round","",44,34,1840,46,25,Color.white,hud.transform);
            Panel("Rivals",1430,112,458,171,ink,hud.transform);rivals=Label("Rivals","",1447,124,420,151,21,Color.white,hud.transform);
            Panel("NULL",28,112,540,92,ink,hud.transform);directive=Label("Directive","",44,124,510,72,22,cyan,hud.transform);
            Panel("ECHO",28,218,540,109,ink,hud.transform);echo=Label("Echo","",44,230,510,93,21,muted,hud.transform);
            Panel("Metrics",28,861,1860,65,ink,hud.transform);metrics=Label("Metrics","",48,877,1530,47,24,Color.white,hud.transform);dangerLabel=Label("Danger label","DANGER",1600,863,260,26,20,Color.white,hud.transform);Panel("Danger",1600,898,260,12,new Color(.2f,.12f,.14f),hud.transform);danger=Panel("Fill",1600,898,1,12,new Color(.95f,.19f,.23f),hud.transform).GetComponent<Image>();
            Panel("Prompt",28,939,1860,116,ink,hud.transform);typing=Label("Target","",48,951,1778,82,30,Color.white,hud.transform);flash=Label("Feedback","",1820,943,48,75,34,cyan,hud.transform);
        }
        void Header(string s,string sub){Panel("Title backing",0,0,1920,117,ink,menu);Label("Title",s,60,27,1800,55,42,Color.white,menu);Label("Subtitle",sub,62,84,1790,32,20,muted,menu);}
        void Switch(ExperienceState state)
        {
            bool resume=Flow==ExperienceState.Paused&&state==ExperienceState.Gameplay;Flow=state;Music.SetFlow(state,resume);StateTime=0;foreach(Transform child in menu)Destroy(child.gameObject);hud.SetActive(state==ExperienceState.Gameplay);intro.SetActive(state==ExperienceState.RoundIntro||state==ExperienceState.Countdown);menu.gameObject.SetActive(!hud.activeSelf&&!intro.activeSelf);Sound.Menu(state!=ExperienceState.Gameplay);
            if(state==ExperienceState.MainMenu||state==ExperienceState.Paused)MusicSlider();
            switch(state)
            {
                case ExperienceState.Boot:Header("LAST KEY","INICIANDO SISTEMAS");break;
                case ExperienceState.Loading:Cameras.Gameplay(true);Panel("Loading",530,375,860,310,ink,menu);Label("Logo","LAST KEY",630,423,670,75,65,Color.white,menu);Label("Status","CARGANDO LA COMPETENCIA",635,527,650,50,27,cyan,menu);Panel("Track",635,600,650,8,new Color(.12f,.2f,.23f),menu);loading=Panel("Progress",635,600,650,8,cyan,menu).GetComponent<Image>();loading.sprite=Sprite.Create(Texture2D.whiteTexture,new Rect(0,0,Texture2D.whiteTexture.width,Texture2D.whiteTexture.height),new Vector2(.5f,.5f));loading.type=Image.Type.Filled;loading.fillMethod=Image.FillMethod.Horizontal;loading.fillOrigin=0;loading.fillAmount=0;break;
                case ExperienceState.MainMenu:
                    Cameras.Gameplay(true);Header("LAST KEY","ESCRIBE. SOBREVIVE. SÉ EL ÚLTIMO.");Panel("Menu",60,268,660,628,ink,menu);Label("Pitch","CUATRO PARTICIPANTES.\nCADA TECLA CUENTA.",92,306,570,130,39,Color.white,menu);Label("Rules","5 rondas · 60 segundos\nGana la mayor puntuación acumulada.\nDanger 100%: fuera solo de esta ronda.",92,476,570,150,26,muted,menu);Button("JUGAR",92,680,568,OpenModeSelect,menu);Label("Help","F1 aceptar ECHO · F2 ignorar\nF3 cámara · ESC pausa",92,779,570,85,22,muted,menu);break;
                case ExperienceState.ModeSelect:
                    Cameras.Gameplay(true);Header("ELIGE LA PRUEBA","CLASSIC es la experiencia principal. Las variantes están en BETA.");
                    for(int i=0;i<5;i++){int k=i;float x=60+i%3*615,y=228+i/3*259;Panel("Mode",x,y,575,218,ink,menu);Label("Name",new[]{"CLASSIC","CODE / BETA","NUMBERS / BETA","MEMORY / BETA","SUDDEN / BETA"}[i],x+24,y+18,525,45,31,Color.white,menu);Label("Description",new[]{"Frases naturales: velocidad, precisión y combos.","Copia código literal. No es un examen de programación.","Secuencias numéricas y registros.","Observa, recuerda y escribe la frase.","Los errores aumentan Danger cuatro veces más."}[i],x+24,y+73,520,76,24,muted,menu);Button("SELECCIONAR",x+24,y+151,525,()=>{SelectedMode=(RoundMode)k;OpenCharacterSelect();},menu);}
                    Label("Chaos","CHAOS / PRÓXIMAMENTE",1308,534,490,85,27,muted,menu);Button("VOLVER",60,950,350,MainMenu,menu);break;
                case ExperienceState.CharacterSelect:
                    Arena.Arrange(HumanIndex);Cameras.Portrait(HumanIndex,true);Header("ELIGE TU PERSONAJE","Identidad visual, sin ventajas de estadísticas. El elegido será humano.");Panel("Identity",60,205,640,695,ink,menu);
                    for(int i=0;i<4;i++){int k=i;Button(Aliases[i]+" — "+Names[i],86,234+i*91,587,()=>ChooseCharacter(k),menu,i==HumanIndex?Arena.Colors[i]*.65f:(Color?)null);}
                    Label("Specialty",Specialties[HumanIndex],86,650,580,65,27,Arena.Colors[HumanIndex],menu);Label("Preview","MODELO 3D EN VIVO\nLos otros tres participantes serán bots.",86,730,580,85,24,muted,menu);Button("CONFIRMAR "+Aliases[HumanIndex],86,822,587,ConfirmCharacter,menu);Button("VOLVER",60,950,350,OpenModeSelect,menu);break;
                case ExperienceState.MatchSetup:
                    Cameras.Gameplay(true);Header("COMPETENCIA PREPARADA",Aliases[HumanIndex]+" / "+SelectedMode.ToString().ToUpperInvariant());Panel("Setup",60,278,710,545,ink,menu);Label("Details","5 RONDAS × 60 SEGUNDOS\n\nVelocidad + precisión + combo = puntos.\nLos errores aumentan Danger.\n100%: fuera de ronda; vuelves después.\n\nNULL emite directivas automáticamente.\nECHO: F1 acepta · F2 ignora.",92,308,650,375,28,Color.white,menu);Button("ENTRAR A LA ARENA",92,715,646,BeginIntro,menu);Button("VOLVER",60,950,350,OpenCharacterSelect,menu);break;
                case ExperienceState.Countdown:Sound.Cue("round");break;
                case ExperienceState.RoundResult:DrawResults(false);Sound.Cue("round");break;
                case ExperienceState.FinalResult:DrawResults(true);Sound.Cue("victory");break;
                case ExperienceState.Paused:Header("PAUSA","La competencia está detenida.");Button("CONTINUAR",70,345,590,()=>Switch(ExperienceState.Gameplay),menu);Button("MENÚ PRINCIPAL",70,448,590,MainMenu,menu);break;
            }
        }
        void MusicSlider()
        {
            var backing=Panel("Music setting",1320,950,560,98,ink,menu);
            Label("Music volume","MUSIC VOLUME",18,10,250,32,20,muted,backing.transform);
            var track=Panel("Slider",18,54,515,16,new Color(.12f,.24f,.27f),backing.transform);
            var slider=track.AddComponent<Slider>();slider.minValue=0;slider.maxValue=1;
            var fill=Panel("Fill",0,0,515,16,cyan,track.transform);slider.fillRect=fill.GetComponent<RectTransform>();slider.fillRect.sizeDelta=Vector2.zero;
            var handle=Panel("Handle",0,0,24,30,Color.white,track.transform);slider.handleRect=handle.GetComponent<RectTransform>();slider.targetGraphic=handle.GetComponent<Image>();
            slider.SetValueWithoutNotify(Music.Volume);slider.onValueChanged.AddListener(Music.SetVolume);
        }
        void DrawResults(bool final)
        {
            var winner=Game.Winners()[0];Cameras.Results(Array.IndexOf(Game.Players,winner),final);Header(final?(Game.Winners().Length>1?"EMPATE / THE LAST KEY":"THE LAST KEY — "+winner.Name):"RESULTADOS / RONDA "+Game.Round,final?"GANA LA MAYOR PUNTUACIÓN ACUMULADA":"Conservas tus puntos. Danger se restablece en la siguiente ronda.");Panel("Results",50,198,1080,635,ink,menu);
            float[] columns={74,350,480,610,740,880};string[] headings={"POS. / PARTICIPANTE","RONDA","TOTAL","WPM","ACC.","BONUS"};for(int c=0;c<6;c++)Label("Column",headings[c],columns[c],220,c==0?260:120,46,22,muted,menu);
            var ranking=Game.Ranking();for(int i=0;i<4;i++){var p=ranking[i];float y=299+i*119;var values=new[]{(i+1)+"  "+p.Name,p.RoundScore.ToString(),p.TotalScore.ToString(),p.Wpm.ToString("0"),p.Accuracy.ToString("0")+"%","+"+p.Bonus};for(int c=0;c<6;c++)Label("Value",values[c],columns[c],y,c==0?260:120,49,29,p.Bot?Color.white:cyan,menu);Label("State",p.Eliminated?"ELIMINADO SOLO DE ESTA RONDA":"RONDA COMPLETADA",117,y+52,940,34,20,p.Eliminated?new Color(1,.35f,.3f):muted,menu);}
            Button(final?"REVANCHA":"SIGUIENTE RONDA",60,930,620,final?Rematch:(Action)BeginIntro,menu);Button("MENÚ PRINCIPAL",730,930,420,MainMenu,menu);
        }
        public void OpenModeSelect(){Switch(ExperienceState.ModeSelect);}
        public void OpenCharacterSelect(){Switch(ExperienceState.CharacterSelect);}
        public void ChooseCharacter(int i){HumanIndex=i;Arena.Arrange(i);Switch(ExperienceState.CharacterSelect);}
        public void ConfirmCharacter(){Switch(ExperienceState.MatchSetup);}
        public void BeginIntro(){if(Game.Phase==Phase.Ready&&Game.HumanIndex!=HumanIndex)Game=new Competition(rules,content,humanIndex:HumanIndex);Game.StartRound(SelectedMode);Switch(ExperienceState.RoundIntro);}
        public void Rematch(){Game=new Competition(rules,content,humanIndex:HumanIndex);BeginIntro();}
        public void MainMenu(){Game=new Competition(rules,content,humanIndex:HumanIndex);Switch(ExperienceState.MainMenu);}
        public void StartNext(){if(Game.Phase==Phase.Finished)Rematch();else BeginIntro();}
        public void SelectMode(RoundMode mode){SelectedMode=mode;}
        public void RespondEcho(bool accept){if(accept)Game.AcceptEcho(HumanIndex);else Game.IgnoreEcho(HumanIndex);}
        public void Refresh()
        {
            if(Game==null||top==null)return;var p=Game.Players[HumanIndex];int pos=Array.IndexOf(Game.Ranking(),p)+1;
            top.text="LAST KEY / RONDA "+Game.Round+" / "+rules.rounds+"       "+Mathf.CeilToInt(Game.Remaining)+" s       POSICIÓN "+pos+" / 4     ·     "+p.Name+" / "+SelectedMode.ToString().ToUpperInvariant();
            metrics.text="WPM "+p.Wpm.ToString("0")+"   ACCURACY "+p.Accuracy.ToString("0.0")+"%   ERRORS "+p.Errors+"   SCORE "+p.RoundScore+"   TOTAL "+p.TotalScore+"   COMBO "+p.Combo;dangerLabel.text="DANGER "+p.Danger.ToString("0")+"%";danger.rectTransform.sizeDelta=new Vector2(260*p.Danger/100,12);
            rivals.text=string.Join("\n\n",Game.Ranking().Where(q=>q.Bot).Select(q=>q.Name+"  "+(q.TotalScore+(Game.Phase==Phase.Running?q.RoundScore:0))+" pts   "+(q.Eliminated?"FUERA":q.Wpm.ToString("0")+" WPM · "+q.Danger.ToString("0")+"%")));
            directive.text=Game.ActiveDirective==Directive.None?"NULL / ÁRBITRO OFICIAL\nSin directiva activa.":"NULL / "+(Game.ActiveDirective==Directive.PrecisionProtocol?"PRECISION PROTOCOL":Game.ActiveDirective==Directive.SpeedCheck?"SPEED CHECK":"CLEAN STREAK")+" · "+Game.DirectiveRemaining.ToString("0")+" s\n"+(Game.ActiveDirective==Directive.PrecisionProtocol?"ERROR = DANGER ×2":Game.ActiveDirective==Directive.SpeedCheck?"30 WPM en esta ventana · +75 pts":"25 aciertos sin error · +75 pts");
            echo.text=p.EchoAccepted&&!p.EchoResolved?"ECHO ACTIVO / "+p.EchoRemaining.ToString("0.0")+" s\n+150 pts / −15 Danger · Fallo +15":Game.EchoOffered&&!p.EchoResolved?"ECHO / F1 ACEPTAR · F2 IGNORAR\n+150 pts / −15 Danger · Fallo +15\nSin respuesta: ignorar en "+Game.EchoOfferRemaining.ToString("0")+" s":"ECHO / SEÑAL EXTERNA\n"+(p.EchoResolved?"Señal cerrada.":"Esperando señal.");
            string target=p.EchoAccepted&&!p.EchoResolved?Competition.EchoText:p.Prompt;int cursor=p.EchoAccepted&&!p.EchoResolved?p.EchoCursor:p.Cursor;
            typing.text=p.Eliminated?"ELIMINADO SOLO DE ESTA RONDA. Conservas tus puntos y vuelves en la siguiente.":target.Length==0?"Prepárate para escribir.":Game.Mode==RoundMode.Memory&&Game.Elapsed>=p.MemoryRevealUntil?"MEMORIA / TEXTO OCULTO     Progreso "+cursor+" / "+target.Length:"<color=#42D5CB>"+Escape(target.Substring(0,cursor))+"</color><u>"+Escape(target.Substring(cursor,1))+"</u>"+Escape(target.Substring(cursor+1));
        }
        static string Escape(string s){return s.Replace("&","&amp;").Replace("<","&lt;").Replace(">","&gt;");}
    }
}
