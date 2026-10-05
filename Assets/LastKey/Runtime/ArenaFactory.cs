using UnityEngine;
using TMPro;

namespace LastKey
{
    public sealed class ArenaFactory
    {
        public GameObject Root;
        public Transform[] Characters = new Transform[4];
        public Renderer[] Collars = new Renderer[4];
        public Camera Camera;
        public TMP_Text Board;
        public Transform[] Stations=new Transform[4];
        public TMP_Text[] Screens=new TMP_Text[4];
        public Renderer[] Displays=new Renderer[4];
        public TouchKeyboardVisual[] TouchKeys=new TouchKeyboardVisual[4];
        readonly int[] priorErrors=new int[4],priorCombos=new int[4];
        readonly float[] errorUntil=new float[4],comboUntil=new float[4];
        public int HumanIndex;
        readonly Color metal = new Color(.11f,.14f,.17f);
        readonly Color cyan = new Color(.1f,.7f,.72f);
        public readonly Color[] Colors = {new Color(.20f,.84f,.81f),new Color(.30f,.55f,1f),new Color(.71f,.55f,1f),new Color(.96f,.71f,.33f)};
        Material Mat(Color c, bool glow=false)
        {
            var m = glow?VisualProps.Surface(c,true):VisualProps.EnvironmentSurface(c);
            if(glow) {m.EnableKeyword("_EMISSION");m.SetColor("_EmissionColor",c*1.8f);} return m;
        }
        GameObject Shape(string name, PrimitiveType type, Transform parent, Vector3 p, Vector3 scale, Color c, bool glow=false)
        {
            var g=GameObject.CreatePrimitive(type);g.name=name;g.transform.SetParent(parent,false);g.transform.localPosition=p;g.transform.localScale=scale;g.GetComponent<Renderer>().sharedMaterial=Mat(c,glow);return g;
        }
        GameObject Box(string n,Transform t,Vector3 p,Vector3 s,Color c,bool glow=false){return Shape(n,PrimitiveType.Cube,t,p,s,c,glow);}
        Transform Group(string n, Transform t){var g=new GameObject(n);g.transform.SetParent(t,false);return g.transform;}
        void Limb(string n,Transform t,Vector3 a,Vector3 b,float width,Color c)
        {
            var g=Shape(n,PrimitiveType.Cylinder,t,(a+b)*.5f,new Vector3(width,Vector3.Distance(a,b)*.5f,width),c);
            g.transform.localRotation=Quaternion.FromToRotation(Vector3.up,b-a);
        }
        public void Create(TMP_FontAsset font)
        {
            Root=new GameObject("Arena");var env=Group("Environment",Root.transform);
            var floor=Box("Floor",env,new Vector3(0,-.13f,0),new Vector3(13,.25f,10),metal);
            for(int x=-5;x<=5;x+=2)for(int z=-4;z<=4;z+=2)
                VisualProps.Fit("kenney-floor-panel",env,new Vector3(x,.022f,z),new Vector3(1.98f,.045f,1.98f),new Color(.12f,.16f,.22f));
            Box("Rear wall",env,new Vector3(0,2.1f,4.8f),new Vector3(13,4.3f,.2f),metal);
            Box("Left wall",env,new Vector3(-6.5f,2,0),new Vector3(.2f,4.2f,10),metal);
            Box("Right wall",env,new Vector3(6.5f,2,0),new Vector3(.2f,4.2f,10),metal);
            for(int x=-5;x<=5;x+=2)
                VisualProps.Fit(x==-1||x==1?"kenney-wall-detail":"kenney-wall",env,new Vector3(x,2.1f,4.62f),new Vector3(1.98f,4.2f,.22f),new Color(.30f,.36f,.43f));
            var door=VisualProps.Fit("kenney-door-double-closed",env,new Vector3(5.35f,1.45f,4.36f),new Vector3(1.5f,2.9f,.18f),new Color(.42f,.48f,.55f));
            for(int side=-1;side<=1;side+=2)for(int z=-3;z<=3;z+=2)
            {
                var wall=VisualProps.Fit("kenney-wall-detail",env,new Vector3(side*6.36f,2,z),new Vector3(1.98f,4,.22f),new Color(.24f,.31f,.39f));
                wall.transform.RotateAround(new Vector3(side*6.36f,2,z),Vector3.up,side*90);
            }
            VisualProps.Fit("Pipes",env,new Vector3(-4.6f,2.5f,4.48f),new Vector3(2.6f,2.2f,.18f),new Color(.22f,.29f,.35f));
            for(int side=-1;side<=1;side+=2)
            {
                VisualProps.Fit("Props_Shelf_Tall",env,new Vector3(side*5.25f,1.3f,3.9f),new Vector3(.9f,2.6f,.75f),new Color(.065f,.1f,.15f));
                VisualProps.Fit("Column_Slim",env,new Vector3(side*6.2f,2,3.9f),new Vector3(.32f,4,.35f),new Color(.16f,.22f,.29f));
                Box("Emergency light",env,new Vector3(side*5.2f,3,4.48f),new Vector3(.6f,.13f,.07f),new Color(.9f,.32f,.12f),true);
            }
            var stations=Group("Stations",Root.transform);
            for(int i=0;i<4;i++)
            {
                var station=Group("Station_P"+(i+1),stations);Stations[i]=station;
                VisualProps.Fit("SM_Desk",station,new Vector3(0,.56f,-.13f),new Vector3(2.05f,.84f,1.1f),new Color(.45f,.52f,.59f));
                // The player's display and input surface are built into the desktop.
                // No upright monitor, stand, separate keyboard or tower obstructs rivals.
                var terminal=Group("Integrated touch terminal",station);
                terminal.localPosition=new Vector3(0,.980f,-.33f);
                Box("Touchscreen bezel",terminal,Vector3.zero,new Vector3(1.53f,.008f,.72f),new Color(.025f,.035f,.05f));
                var glass=Box("Integrated display glass",terminal,new Vector3(0,.004f,0),new Vector3(1.45f,.002f,.70f),new Color(.012f,.07f,.085f));
                Displays[i]=glass.GetComponent<Renderer>(); var displayMaterial=new Material(Shader.Find("Universal Render Pipeline/Unlit"));displayMaterial.color=new Color(.012f,.07f,.085f);Displays[i].sharedMaterial=displayMaterial;
                var screenText=new GameObject("Live touchscreen text",typeof(TextMeshPro));
                screenText.transform.SetParent(terminal,false);screenText.transform.localPosition=new Vector3(0,.007f,.145f);screenText.transform.localRotation=Quaternion.Euler(90,0,0);
                Screens[i]=screenText.GetComponent<TextMeshPro>();Screens[i].font=font;Screens[i].richText=false;Screens[i].fontSize=.55f;Screens[i].enableAutoSizing=true;Screens[i].fontSizeMin=.22f;Screens[i].fontSizeMax=.55f;Screens[i].textWrappingMode=TextWrappingModes.Normal;Screens[i].overflowMode=TextOverflowModes.Truncate;Screens[i].color=new Color(.8f,1,1);Screens[i].alignment=TextAlignmentOptions.Center;Screens[i].rectTransform.sizeDelta=new Vector2(1.32f,.28f);Screens[i].text="LAST KEY\nESTACIÓN LISTA";
                TouchKeys[i]=terminal.gameObject.AddComponent<TouchKeyboardVisual>();TouchKeys[i].Create(font,Colors[i]);
                Screens[i].fontSize=.60f;Screens[i].fontSizeMin=.35f;Screens[i].fontSizeMax=.60f;
                Screens[i].rectTransform.sizeDelta=new Vector2(1.32f,.39f);
                VisualProps.Fit("SM_Chair",station,new Vector3(0,.60f,-1.08f),new Vector3(.72f,1.20f,.72f),new Color(.17f,.23f,.31f));
                Box("Chair footrest",station,new Vector3(0,.18f,-.72f),new Vector3(.56f,.045f,.30f),metal);
                VisualProps.Restraint(station,new Vector3(0,.74f,-.99f),new Vector3(.46f,.065f,.30f));
                VisualProps.Restraint(station,new Vector3(0,.41f,-.80f),new Vector3(.47f,.045f,.21f));
                Characters[i]=CharacterFactory.Create(station,i,Colors[i],out Collars[i]);
            }
            Box("Scoreboard frame",env,new Vector3(0,2.55f,4.43f),new Vector3(4.5f,1.18f,.12f),new Color(.015f,.027f,.037f));
            var board=new GameObject("Scoreboard",typeof(TextMeshPro));board.transform.SetParent(env,false);board.transform.localPosition=new Vector3(0,2.55f,4.34f);board.transform.localRotation=Quaternion.identity;
            Board=board.GetComponent<TextMeshPro>();Board.font=font;Board.fontSize=2.3f;Board.alignment=TextAlignmentOptions.Center;Board.color=cyan;Board.rectTransform.sizeDelta=new Vector2(4.3f,1.1f);Board.text="LAST KEY\n4 PARTICIPANTES · 5 RONDAS";
            RenderSettings.ambientMode=UnityEngine.Rendering.AmbientMode.Trilight;RenderSettings.ambientSkyColor=new Color(.33f,.40f,.52f);RenderSettings.ambientEquatorColor=new Color(.20f,.26f,.35f);RenderSettings.ambientGroundColor=new Color(.10f,.13f,.18f);RenderSettings.ambientLight=new Color(.22f,.29f,.38f);RenderSettings.fog=true;RenderSettings.fogColor=new Color(.028f,.042f,.06f);RenderSettings.fogDensity=.022f;
            var lighting=Group("Lighting",Root.transform);var lightObj=new GameObject("Main",typeof(Light));lightObj.transform.SetParent(lighting);lightObj.transform.rotation=Quaternion.Euler(48,-25,0);var light=lightObj.GetComponent<Light>();light.type=LightType.Directional;light.intensity=1.2f;light.color=new Color(.93f,.96f,1);light.shadowBias=.035f;light.shadowNormalBias=.35f;light.shadows=LightShadows.Soft;
            var fillObj=new GameObject("Face fill",typeof(Light));fillObj.transform.SetParent(lighting);fillObj.transform.rotation=Quaternion.Euler(30,180,0);var fill=fillObj.GetComponent<Light>();fill.type=LightType.Directional;fill.intensity=.45f;fill.color=new Color(1,.93f,.84f);
            for(int i=0;i<4;i++)
            {
                var spot=new GameObject("Station practical light",typeof(Light));spot.transform.SetParent(Stations[i],false);spot.transform.localPosition=new Vector3(0,3,-.7f);spot.transform.localRotation=Quaternion.Euler(82,0,0);
                var lamp=spot.GetComponent<Light>();lamp.type=LightType.Spot;lamp.range=6;lamp.spotAngle=75;lamp.intensity=2.2f;lamp.color=Color.Lerp(Color.white,Colors[i],.12f);
                Box("Desk accent inset",Stations[i],new Vector3(0,.87f,-.687f),new Vector3(1.65f,.016f,.005f),Colors[i]*.7f,true);
            }
            var cameraObj=new GameObject("Cam_Main",typeof(Camera),typeof(AudioListener));cameraObj.transform.SetParent(Root.transform);Camera=cameraObj.GetComponent<Camera>();Camera.fieldOfView=49;Camera.clearFlags=CameraClearFlags.SolidColor;Camera.backgroundColor=RenderSettings.fogColor;Camera.nearClipPlane=.05f;SetCamera(0);
            Arrange(0);
        }
        public void Arrange(int human)
        {
            HumanIndex=human;var positions=new[]{new Vector3(-1.0f,0,-2.3f),new Vector3(-3.5f,0,1.0f),new Vector3(0,0,3.0f),new Vector3(3.5f,0,1.0f)};
            var angles=new[]{0f,105f,135f,255f};int slot=1;
            for(int i=0;i<4;i++){int s=i==human?0:slot++;Stations[i].localPosition=positions[s];Stations[i].localRotation=Quaternion.Euler(0,angles[s],0);}
        }
        public void SetCamera(int view)
        {
            if(view==0) {Camera.transform.position=new Vector3(0,6.7f,-9.0f);Camera.transform.LookAt(new Vector3(0,.7f,1.4f));}
            else if(view==1) {Camera.transform.position=new Vector3(-4.3f,2.3f,-2.6f);Camera.transform.LookAt(new Vector3(-3.8f,1.2f,.7f));}
            else {int i=view-2;float x=(i-1.5f)*2.55f;Camera.transform.position=new Vector3(x+1.05f,1.75f,.3f);Camera.transform.LookAt(new Vector3(x,1.3f,-.94f));}
        }
        public void ShowOpening(float seconds)
        {
            string text=seconds<16?"":seconds<18?"[ NULL ]\nSISTEMA DE COMPETENCIA":seconds<22?"4 PARTICIPANTES · PUNTOS POR RONDAS\nVELOCIDAD + PRECISIÓN + COMBO":seconds<25?"DANGER 100%: FUERA SOLO DE ESTA RONDA\nGANA LA MAYOR PUNTUACIÓN TOTAL":"[ NULL ]\nINICIANDO COMPETENCIA";
            for(int i=0;i<4;i++){
                TouchKeys[i].Power(seconds>=16);Screens[i].text=text;Screens[i].color=new Color(.72f,.95f,1);
                Displays[i].material.SetColor("_BaseColor",seconds<16?new Color(.006f,.008f,.012f):new Color(.015f,.06f,.09f));
            }
            Board.text=seconds<16?"":"[ NULL ]\nDIRECTIVAS OFICIALES / LAST KEY";
        }
        public void Animate(float time, LastKey.Core.Competition game)
        {
            for(int i=0;i<4;i++)
            {
                var p=game.Players[i];TouchKeys[i].Power(true);
                if(p.Errors>priorErrors[i])errorUntil[i]=time+.32f;
                if(p.Combo/20>priorCombos[i]/20)comboUntil[i]=time+.7f;
                priorErrors[i]=p.Errors;priorCombos[i]=p.Combo;
                Color surface=time<errorUntil[i]?new Color(.35f,.025f,.045f):time<comboUntil[i]?Colors[i]*.12f:new Color(.015f,.04f,.06f);
                Displays[i].material.SetColor("_BaseColor",surface);Displays[i].material.SetFloat("_UseEmission",1);Displays[i].material.SetColor("_EmissionColor",surface*.45f);
                Color color=p.Danger>=80?new Color(.9f,.07f,.08f):p.Danger>=50?new Color(1,.55f,.06f):cyan;
                Collars[i].GetComponentInParent<ImportedSpaceCollar>().SetDanger(color);

                var target=p.EchoAccepted&&!p.EchoResolved?Core.Competition.EchoText:game.Target(i);
                int cursor=p.EchoAccepted&&!p.EchoResolved?p.EchoCursor:p.Cursor;
                Screens[i].text=p.Name+"  "+(p.Eliminated?"FUERA DE RONDA":p.Wpm.ToString("0")+" WPM")+"\n"+(target.Length==0?"LISTO":game.Mode==Core.RoundMode.Memory&&game.Elapsed>=p.MemoryRevealUntil?"MEMORIA / TEXTO OCULTO":target.Substring(Mathf.Min(cursor,target.Length-1),Mathf.Min(72,target.Length-Mathf.Min(cursor,target.Length-1))))+"\n"+cursor+" / "+target.Length+"   "+p.RoundScore+" PTS";
                Screens[i].color=p.Eliminated||time<errorUntil[i]?new Color(1,.65f,.65f):time<comboUntil[i]?Color.white:new Color(.75f,1,1);
            }
            Board.text=game.Round==0?"LAST KEY\n4 PARTICIPANTES · 5 RONDAS":"LAST KEY / RONDA "+game.Round+"\n"+game.Players[0].Name+" "+(game.Players[0].TotalScore+(game.Phase==Core.Phase.Running?game.Players[0].RoundScore:0))+"   "+game.Players[1].Name+" "+(game.Players[1].TotalScore+(game.Phase==Core.Phase.Running?game.Players[1].RoundScore:0))+"\n"+game.Players[2].Name+" "+(game.Players[2].TotalScore+(game.Phase==Core.Phase.Running?game.Players[2].RoundScore:0))+"   "+game.Players[3].Name+" "+(game.Players[3].TotalScore+(game.Phase==Core.Phase.Running?game.Players[3].RoundScore:0));
            if(game.Phase==Core.Phase.Finished){var winner=game.Winners()[0];Board.text="THE LAST KEY\n"+winner.Name+" / "+winner.TotalScore+" PUNTOS";}
        }
    }
}
