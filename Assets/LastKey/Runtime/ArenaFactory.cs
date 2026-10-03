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
        public int HumanIndex;
        readonly Color metal = new Color(.11f,.14f,.17f);
        readonly Color cyan = new Color(.1f,.7f,.72f);
        public readonly Color[] Colors = {new Color(.1f,.65f,.7f),new Color(.7f,.46f,.15f),new Color(.4f,.53f,.72f),new Color(.52f,.65f,.35f)};
        Material Mat(Color c, bool glow=false)
        {
            var m = new Material(Shader.Find("Universal Render Pipeline/Lit")); m.color=c;
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
            var floorTex=Resources.Load<Texture2D>("PolyHaven/metal_plate_diff_1k");
            if(floorTex!=null){floor.GetComponent<Renderer>().material.mainTexture=floorTex;floor.GetComponent<Renderer>().material.mainTextureScale=new Vector2(13,10);floor.GetComponent<Renderer>().material.color=new Color(.45f,.47f,.5f);}
            for(int x=-6;x<=6;x++) Box("Floor seam",env,new Vector3(x,.005f,0),new Vector3(.015f,.01f,10),new Color(.2f,.23f,.25f));
            Box("Rear wall",env,new Vector3(0,2.1f,4.8f),new Vector3(13,4.3f,.2f),metal);
            Box("Left wall",env,new Vector3(-6.5f,2,0),new Vector3(.2f,4.2f,10),metal);
            Box("Right wall",env,new Vector3(6.5f,2,0),new Vector3(.2f,4.2f,10),metal);
            for(int x=-6;x<=6;x+=2) Box("Wall rib",env,new Vector3(x,2.1f,4.62f),new Vector3(.12f,4.2f,.15f),new Color(.23f,.26f,.28f));
            var pipes=Group("Pipes",env);
            for(int i=0;i<3;i++) Limb("Modular pipe",pipes,new Vector3(-6,3.3f+i*.23f,4.4f),new Vector3(6,3.3f+i*.23f,4.4f),.1f,new Color(.35f,.37f,.39f));
            Limb("Down pipe",pipes,new Vector3(-5.8f,.4f,4.4f),new Vector3(-5.8f,3.8f,4.4f),.13f,new Color(.35f,.37f,.39f));
            for(int side=-1;side<=1;side+=2)
            {
                var rack=Group("ServerRack_"+side,env); rack.localPosition=new Vector3(side*5.4f,0,3.5f);
                Box("Cabinet",rack,new Vector3(0,1.2f,0),new Vector3(.8f,2.4f,.7f),new Color(.045f,.06f,.075f));
                for(int row=0;row<8;row++) {Box("Server",rack,new Vector3(0,.35f+row*.25f,-.39f),new Vector3(.66f,.18f,.08f),metal);Box("Status LED",rack,new Vector3(.25f,.35f+row*.25f,-.44f),new Vector3(.04f,.03f,.01f),cyan,true);}
                Box("Emergency light",env,new Vector3(side*5.2f,3,4.48f),new Vector3(.6f,.13f,.07f),new Color(.9f,.32f,.12f),true);
            }
            var stations=Group("Stations",Root.transform);
            for(int i=0;i<4;i++)
            {
                var station=Group("Station_P"+(i+1),stations);Stations[i]=station;
                var deskAsset=Resources.Load<GameObject>("PolyHaven/metal_office_desk");
                if(deskAsset!=null)
                {
                    var desk=Object.Instantiate(deskAsset,station);desk.name="PolyHaven metal_office_desk";
                    var renderers=desk.GetComponentsInChildren<Renderer>();var bounds=renderers[0].bounds;
                    foreach(var r in renderers)bounds.Encapsulate(r.bounds);
                    desk.transform.localScale=new Vector3(2.05f/bounds.size.x,.84f/bounds.size.y,1.1f/bounds.size.z);
                    bounds=renderers[0].bounds;foreach(var r in renderers)bounds.Encapsulate(r.bounds);
                    desk.transform.position+=station.position-new Vector3(bounds.center.x,bounds.min.y,bounds.center.z);
                    var deskMat=Mat(new Color(.65f,.68f,.7f));deskMat.mainTexture=Resources.Load<Texture2D>("PolyHaven/metal_office_desk_diff_1k");foreach(var r in renderers)r.sharedMaterial=deskMat;
                }
                else
                {
                    Box("Desktop",station,new Vector3(0,1.0f,0),new Vector3(2.05f,.13f,1.1f),new Color(.22f,.25f,.27f));
                    for(int side=-1;side<=1;side+=2) Box("Desk leg",station,new Vector3(side*.85f,.48f,0),new Vector3(.1f,.97f,.85f),metal);
                }
                Box("PC tower",station,new Vector3(.68f,.45f,.1f),new Vector3(.3f,.7f,.55f),metal);
                Box("Monitor stand",station,new Vector3(0,1.01f,.37f),new Vector3(.12f,.35f,.09f),metal);
                Box("Monitor",station,new Vector3(0,1.27f,.3f),new Vector3(1.12f,.65f,.09f),new Color(.02f,.035f,.04f));
                Box("Screen",station,new Vector3(0,1.27f,.245f),new Vector3(1.02f,.54f,.01f),new Color(.012f,.07f,.085f));
                var screenText=new GameObject("Live monitor text",typeof(TextMeshPro));screenText.transform.SetParent(station,false);screenText.transform.localPosition=new Vector3(0,1.28f,.23f);
                Screens[i]=screenText.GetComponent<TextMeshPro>();Screens[i].font=font;Screens[i].fontSize=.72f;Screens[i].color=new Color(.8f,1,1);Screens[i].alignment=TextAlignmentOptions.Center;Screens[i].rectTransform.sizeDelta=new Vector2(.96f,.47f);Screens[i].text="LAST KEY\nESTACIÓN LISTA";
                Box("Keyboard",station,new Vector3(0,.86f,-.32f),new Vector3(.65f,.055f,.24f),new Color(.07f,.08f,.1f));
                for(int row=0;row<3;row++)for(int col=0;col<10;col++) Box("Key",station,new Vector3(-.27f+col*.06f,.893f,-.39f+row*.06f),new Vector3(.043f,.012f,.041f),new Color(.35f,.38f,.4f));
                Box("Chair seat",station,new Vector3(0,.53f,-1.05f),new Vector3(.65f,.10f,.42f),metal);
                Box("Chair footrest",station,new Vector3(0,.18f,-.72f),new Vector3(.56f,.045f,.30f),metal);
                Box("Chair back",station,new Vector3(0,.95f,-1.29f),new Vector3(.65f,.65f,.1f),metal);
                Limb("Chair pedestal",station,new Vector3(0,.05f,-1),new Vector3(0,.5f,-1),.12f,metal);
                for(int side=-1;side<=1;side+=2)Box("Competition restraint",station,new Vector3(side*.34f,.71f,-1.0f),new Vector3(.055f,.08f,.29f),new Color(.24f,.28f,.31f));
                Characters[i]=CharacterFactory.Create(station,i,Colors[i],out Collars[i]);
            }
            Box("Scoreboard frame",env,new Vector3(0,2.55f,4.43f),new Vector3(4.5f,1.18f,.12f),new Color(.015f,.027f,.037f));
            var board=new GameObject("Scoreboard",typeof(TextMeshPro));board.transform.SetParent(env,false);board.transform.localPosition=new Vector3(0,2.55f,4.34f);board.transform.localRotation=Quaternion.identity;
            Board=board.GetComponent<TextMeshPro>();Board.font=font;Board.fontSize=2.3f;Board.alignment=TextAlignmentOptions.Center;Board.color=cyan;Board.rectTransform.sizeDelta=new Vector2(4.3f,1.1f);Board.text="LAST KEY\n4 PARTICIPANTES · 5 RONDAS";
            RenderSettings.ambientLight=new Color(.3f,.36f,.43f);RenderSettings.fog=true;RenderSettings.fogColor=new Color(.028f,.042f,.06f);RenderSettings.fogDensity=.022f;
            var lighting=Group("Lighting",Root.transform);var lightObj=new GameObject("Main",typeof(Light));lightObj.transform.SetParent(lighting);lightObj.transform.rotation=Quaternion.Euler(48,-25,0);var light=lightObj.GetComponent<Light>();light.type=LightType.Directional;light.intensity=1.7f;light.color=new Color(.75f,.85f,1);light.shadows=LightShadows.Soft;
            var fillObj=new GameObject("Face fill",typeof(Light));fillObj.transform.SetParent(lighting);fillObj.transform.rotation=Quaternion.Euler(30,180,0);var fill=fillObj.GetComponent<Light>();fill.type=LightType.Directional;fill.intensity=1.0f;fill.color=new Color(1,.93f,.84f);
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
        public void Animate(float time, LastKey.Core.Competition game)
        {
            for(int i=0;i<4;i++)
            {
                var p=game.Players[i];Color color=p.Danger>=75?new Color(.9f,.07f,.08f):p.Danger>=40?new Color(1,.55f,.06f):cyan;
                Collars[i].material.color=color;Collars[i].material.SetColor("_EmissionColor",color*1.5f);
                Characters[i].GetComponent<CharacterMotion>().SetCompetition(game);
                var target=p.EchoAccepted&&!p.EchoResolved?Core.Competition.EchoText:game.Target(i);
                int cursor=p.EchoAccepted&&!p.EchoResolved?p.EchoCursor:p.Cursor;
                Screens[i].text=p.Name+"  "+(p.Eliminated?"FUERA DE RONDA":p.Wpm.ToString("0")+" WPM")+"\n"+(target.Length==0?"LISTO":game.Mode==Core.RoundMode.Memory&&game.Elapsed>=p.MemoryRevealUntil?"MEMORIA / TEXTO OCULTO":target.Substring(Mathf.Min(cursor,target.Length-1),Mathf.Min(42,target.Length-Mathf.Min(cursor,target.Length-1))))+"\n"+cursor+" / "+target.Length+"   "+p.RoundScore+" PTS";
                Screens[i].color=p.Eliminated?new Color(1,.2f,.2f):new Color(.75f,1,1);
            }
            Board.text=game.Round==0?"LAST KEY\n4 PARTICIPANTES · 5 RONDAS":"LAST KEY / RONDA "+game.Round+"\n"+game.Players[0].Name+" "+(game.Players[0].TotalScore+(game.Phase==Core.Phase.Running?game.Players[0].RoundScore:0))+"   "+game.Players[1].Name+" "+(game.Players[1].TotalScore+(game.Phase==Core.Phase.Running?game.Players[1].RoundScore:0))+"\n"+game.Players[2].Name+" "+(game.Players[2].TotalScore+(game.Phase==Core.Phase.Running?game.Players[2].RoundScore:0))+"   "+game.Players[3].Name+" "+(game.Players[3].TotalScore+(game.Phase==Core.Phase.Running?game.Players[3].RoundScore:0));
        }
    }
}
