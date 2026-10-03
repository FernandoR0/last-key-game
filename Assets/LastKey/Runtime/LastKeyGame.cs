using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using LastKey.Core;

namespace LastKey
{
    public sealed class LastKeyGame : MonoBehaviour
    {
        public Competition Game { get; private set; }
        public ArenaFactory Arena { get; private set; }
        public Canvas Canvas { get; private set; }
        TMP_FontAsset font;
        TMP_Text title, metrics, typing, eventText, ranking, directive, echo, result, footer;
        Image dangerFill;
        GameObject resultsPanel, echoButtons, startButton;
        GameObject modeControls;
        RoundMode selected = RoundMode.Classic;
        int view;
        Color cyan = new Color(.17f,.83f,.85f), ink = new Color(.025f,.045f,.064f,.97f), muted = new Color(.6f,.7f,.75f);

        void Awake()
        {
            Application.targetFrameRate=60;
            var rules=JsonUtility.FromJson<Rules>(Resources.Load<TextAsset>("last_key_rules").text);
            var bank=JsonUtility.FromJson<ContentBank>(Resources.Load<TextAsset>("last_key_content").text);
            Game=new Competition(rules,bank);
            var baseFont=Resources.Load<Font>("LastKeyFont");
            if(baseFont==null) baseFont=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            font=Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
            if(font==null){font=TMP_FontAsset.CreateFontAsset(baseFont);font.name="LAST KEY Dynamic Font";}
            Arena=new ArenaFactory();Arena.Create(font);CreateHud();
            if(Environment.GetCommandLineArgs().Contains("--evidence")) gameObject.AddComponent<EvidenceRecorder>();
            if(Environment.GetCommandLineArgs().Contains("--keyboard-qa")) gameObject.AddComponent<KeyboardQa>();
        }
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Return)&&Game.Phase!=Phase.Running)StartNext();
            if(Input.GetKeyDown(KeyCode.F2)){view=(view+1)%2;Arena.SetCamera(view);}
            if(Input.GetKeyDown(KeyCode.Escape)) {if(Game.Phase==Phase.Running) Time.timeScale=Time.timeScale==0?1:0;}
            if(Game.Phase==Phase.Running && Time.timeScale>0)
            {
                foreach(char ch in Input.inputString) if(ch!='\b'&&ch!='\r'&&ch!='\n') Game.Input(0,ch);
                Game.Tick(Time.deltaTime);
            }
            Arena.Animate(Time.time,Game);Refresh();
        }
        RectTransform Rect(GameObject obj,Transform parent,float x,float y,float w,float h)
        {
            obj.transform.SetParent(parent,false);var r=obj.GetComponent<RectTransform>();if(r==null)r=obj.AddComponent<RectTransform>();
            r.anchorMin=r.anchorMax=new Vector2(0,1);r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);return r;
        }
        GameObject Panel(string name,float x,float y,float w,float h,Color color,Transform parent=null)
        {
            var obj=new GameObject(name,typeof(RectTransform),typeof(Image));Rect(obj,parent??Canvas.transform,x,y,w,h);obj.GetComponent<Image>().color=color;return obj;
        }
        TMP_Text Label(string name,string text,float x,float y,float w,float h,float size,Color color,Transform parent=null)
        {
            var obj=new GameObject(name,typeof(RectTransform),typeof(TextMeshProUGUI));Rect(obj,parent??Canvas.transform,x,y,w,h);var t=obj.GetComponent<TextMeshProUGUI>();t.font=font;t.text=text;t.fontSize=size;t.color=color;t.textWrappingMode=TextWrappingModes.Normal;t.richText=true;t.raycastTarget=false;return t;
        }
        GameObject Button(string label,float x,float y,float w,float h,Action action,Transform parent=null)
        {
            var obj=Panel(label,x,y,w,h,new Color(.06f,.27f,.31f),parent);var b=obj.AddComponent<Button>();b.onClick.AddListener(()=>action());
            var t=Label("Label",label,12,8,w-24,h-16,24,Color.white,obj.transform);t.alignment=TextAlignmentOptions.Center;return obj;
        }
        void CreateHud()
        {
            var obj=new GameObject("HUD",typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));Canvas=obj.GetComponent<Canvas>();Canvas.renderMode=RenderMode.ScreenSpaceOverlay;
            var scaler=obj.GetComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1920,1080);scaler.matchWidthOrHeight=.5f;
            new GameObject("EventSystem",typeof(EventSystem),typeof(StandaloneInputModule));
            Panel("Header",0,0,1920,105,ink);
            Label("Logo","LAST KEY",40,22,360,55,44,Color.white);
            title=Label("Round","COMPETENCIA DE MECANOGRAFÍA",450,23,1060,50,28,cyan);
            Label("Tagline","ESCRIBE. SOBREVIVE. SÉ EL ÚLTIMO.",40,75,740,30,18,muted);
            Label("Version","MVP  /  1 HUMANO + 3 BOTS",1510,35,380,40,20,muted);
            Panel("Leaderboard",1510,125,380,407,ink);
            Label("Rank heading","CLASIFICACIÓN ACUMULADA",1532,145,340,40,20,cyan);
            ranking=Label("Ranking","",1532,195,340,320,25,Color.white);
            Panel("Directives",40,125,550,118,ink);
            directive=Label("NULL","NULL  /  ÁRBITRO OFICIAL",60,145,510,90,23,cyan);
            Panel("Echo",40,258,550,194,ink);
            echo=Label("ECHO","ECHO  /  SEÑAL EXTERNA\nRetos opcionales de riesgo / recompensa.",60,276,510,95,22,muted);
            echoButtons=Panel("Echo controls",60,380,495,53,Color.clear);
            Button("ACEPTAR",0,0,230,48,()=>Game.AcceptEcho(0),echoButtons.transform);
            Button("IGNORAR",245,0,230,48,()=>Game.IgnoreEcho(0),echoButtons.transform);
            Panel("Metrics",40,600,1840,128,ink);
            metrics=Label("Metrics values","",64,618,1540,95,26,Color.white);
            Label("Danger label","DANGER",1600,620,250,40,23,new Color(1,.36f,.3f));
            Panel("Danger track",1600,667,245,22,new Color(.19f,.12f,.13f));
            dangerFill=Panel("Danger fill",1600,667,1,22,new Color(.9f,.12f,.15f)).GetComponent<Image>();
            Panel("Typing panel",40,749,1840,230,ink);
            Label("Typing heading","CIPHER  /  ESCRIBE EL CARÁCTER SUBRAYADO  ·  LOS ERRORES AUMENTAN DANGER",64,765,1710,35,20,cyan);
            typing=Label("Target text","",64,810,1790,143,37,Color.white);
            eventText=Label("Event","",40,993,1430,40,22,cyan);
            footer=Label("Help","F2 cámara  ·  ESC pausa  ·  Errores: Danger  ·  100%: fuera solo de esta ronda",40,1042,1840,27,18,muted);
            resultsPanel=Panel("Results",605,148,880,418,new Color(.025f,.045f,.064f,.985f));
            result=Label("Results text","LAST KEY\n5 rondas · 60 segundos por ronda\nGana la mayor puntuación total.\nDanger 100%: vuelves en la siguiente ronda.",34,24,810,275,29,Color.white,resultsPanel.transform);
            startButton=Button("INICIAR COMPETENCIA",34,314,810,68,StartNext,resultsPanel.transform);
            // Additional variants share one validated core; Classic is default.
            var modes=Panel("Mode controls",605,580,880,0,Color.clear);
            modeControls=modes;
            for(int i=0;i<5;i++){var mode=(RoundMode)i;Button(new[]{"CLASSIC","CODE","NUMBERS","MEMORY","SUDDEN"}[i],i*176,0,168,43,()=>{if(Game.Phase!=Phase.Running){selected=mode;Refresh();}},modes.transform);}
        }
        public void StartNext()
        {
            if(Game.Phase==Phase.Finished){Game=new Competition(Game.Rules,Game.Content);}
            Game.StartRound(selected);Time.timeScale=1;
        }
        public void SelectMode(RoundMode mode){if(Game.Phase!=Phase.Running)selected=mode;}
        public void Refresh()
        {
            var p=Game.Players[0];
            title.text=Game.Phase==Phase.Ready?"COMPETENCIA POR RONDAS  /  "+selected.ToString().ToUpperInvariant():"RONDA "+Game.Round+" / "+Game.Rules.rounds+"  ·  "+Game.Mode.ToString().ToUpperInvariant()+"  ·  "+Mathf.CeilToInt(Game.Remaining)+" s";
            metrics.text="WPM <b>"+p.Wpm.ToString("0")+"</b>     ACCURACY <b>"+p.Accuracy.ToString("0.0")+"%</b>     ERRORS <b>"+p.Errors+"</b>\nSCORE <b>"+p.RoundScore+"</b>     COMBO <b>"+p.Combo+"</b>     TOTAL <b>"+p.TotalScore+"</b>     DANGER <b>"+p.Danger.ToString("0")+"%</b>";
            dangerFill.rectTransform.sizeDelta=new Vector2(245*p.Danger/100,22);
            ranking.text=string.Join("\n\n",Game.Ranking().Select((q,i)=>(i+1)+"  <b>"+q.Name+"</b>\n     "+(q.TotalScore+(Game.Phase==Phase.Running?q.RoundScore:0))+" pts  ·  "+(q.Eliminated?"FUERA":q.Wpm.ToString("0")+" WPM")));
            directive.text=Game.ActiveDirective==Directive.None?"NULL  /  ÁRBITRO OFICIAL\nSin directiva activa.":"NULL  /  "+Game.ActiveDirective.ToString().ToUpperInvariant()+"  "+Game.DirectiveRemaining.ToString("0")+" s\n"+(Game.ActiveDirective==Directive.PrecisionProtocol?"ERROR = DANGER ×2":Game.ActiveDirective==Directive.SpeedCheck?"OBJETIVO ≥ "+Game.Rules.speedTarget+" WPM  ·  +"+Game.Rules.directiveBonus+" pts":"RACHA "+Game.Rules.cleanTarget+" SIN ERROR  ·  +"+Game.Rules.directiveBonus+" pts");
            echoButtons.SetActive(Game.Phase==Phase.Running&&Game.EchoOffered&&!p.EchoAccepted&&!p.EchoResolved&&!p.Eliminated);
            echo.text=p.EchoAccepted&&!p.EchoResolved?"ECHO INTERCEPT  /  "+p.EchoRemaining.ToString("0.0")+" s\nEscribe NODE-07-RECOVERY":Game.EchoOffered&&!p.EchoResolved?"ECHO INTERCEPT  /  OPCIONAL\nÉXITO +150 pts / −15 Danger\nFALLO +15 Danger  ·  12 s":"ECHO  /  SEÑAL EXTERNA\n"+(p.EchoResolved?"Reto cerrado.":"Esperando señal externa.");
            if(Game.Phase==Phase.Running)
            {
                string target=p.EchoAccepted&&!p.EchoResolved?Competition.EchoText:Game.Text;int cursor=p.EchoAccepted&&!p.EchoResolved?p.EchoCursor:p.Cursor;
                typing.text=p.Eliminated?"<color=#FF5964>ELIMINADO DE ESTA RONDA</color>\nConservas tus puntos y vuelves en la siguiente.":Game.Mode==RoundMode.Memory&&Game.Elapsed>=Game.Rules.memoryRevealSeconds?"TEXTO OCULTO  /  Escribe de memoria\nProgreso: "+cursor+" / "+target.Length:"<color=#39C9CE>"+Escape(target.Substring(0,cursor))+"</color><u>"+Escape(target.Substring(cursor,1))+"</u>"+Escape(target.Substring(cursor+1));
                if(Game.Mode==RoundMode.Memory&&Game.Elapsed<Game.Rules.memoryRevealSeconds)typing.text="MEMORIZA: "+Escape(target);
            }
            else typing.text="Velocidad + precisión + combos.\nLa clasificación final utiliza todos los puntos acumulados.";
            eventText.text=Game.LastEvent;
            footer.text=Time.timeScale==0?"PAUSA  /  ESC para continuar":"F2 cámara  ·  ESC pausa  ·  Errores: Danger  ·  100%: fuera solo de esta ronda";
            resultsPanel.SetActive(Game.Phase!=Phase.Running);
            modeControls.SetActive(Game.Phase!=Phase.Running);
            if(Game.Phase==Phase.Results||Game.Phase==Phase.Finished)
            {
                result.text=(Game.Phase==Phase.Finished?(Game.Winners().Length>1?"EMPATE — THE LAST KEY\n"+string.Join(" / ",Game.Winners().Select(q=>q.Name)):"THE LAST KEY — "+Game.Winners()[0].Name):"RESULTADOS DE RONDA "+Game.Round)+"\n\n"+string.Join("\n",Game.Ranking().Select(q=>q.Name+"     +"+q.RoundScore+" ronda     "+q.TotalScore+" total"));
                startButton.GetComponentInChildren<TMP_Text>().text=Game.Phase==Phase.Finished?"NUEVA COMPETENCIA":"SIGUIENTE RONDA";
            }
        }
        static string Escape(string value){return value.Replace("&","&amp;").Replace("<","&lt;").Replace(">","&gt;");}
    }
}
