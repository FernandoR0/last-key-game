using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace LastKey
{
    // Physical desktop graphics only. Physical keyboard input stays in LastKeyGame.
    public sealed class TouchKeyboardVisual:MonoBehaviour
    {
        readonly Dictionary<char,Renderer> keys=new Dictionary<char,Renderer>();
        readonly Dictionary<Renderer,float> litUntil=new Dictionary<Renderer,float>();
        Material idle,active;
        readonly List<Mesh> combinedMeshes=new List<Mesh>();
        GameObject surface;
        public void Create(TMP_FontAsset font,Color accent)
        {
            idle=new Material(Shader.Find("Universal Render Pipeline/Unlit"));idle.color=new Color(.035f,.09f,.13f);
            active=new Material(idle);active.color=accent*.75f;
            surface=new GameObject("Complete Spanish touch keyboard");surface.transform.SetParent(transform,false);
            string[][] rows={
                new[]{"º ª","1 ! |","2 \" @","3 · #","4 $ ~","5 % €","6 & ¬","7 / {","8 ( [","9 ) ]","0 = }","' ?","¡ ¿","BORRAR"},
                new[]{"TAB","Q","W","E","R","T","Y","U","I","O","P","´ ¨ ^","+ * `"},
                new[]{"BLOQ","A","S","D","F","G","H","J","K","L","Ñ","{ [","} ]","ENTER"},
                new[]{"MAYÚS","< >","Z","X","C","V","B","N","M",", ;",". :","- _","MAYÚS"},
                new[]{"CTRL","ALT","Á É Í Ó Ú Ü","ESPACIO","ALT GR","\\ |","← ↓ ↑ →"}
            };
            for(int row=0;row<rows.Length;row++)
            {
                float z=-.045f-row*.064f;float x=-.68f;
                for(int col=0;col<rows[row].Length;col++)
                {
                    string label=rows[row][col];float width=row==4?(col==3?.40f:col==2?.26f:col==6?.19f:.12f):1.36f/rows[row].Length;
                    var key=GameObject.CreatePrimitive(PrimitiveType.Cube);key.name="Touch key "+label;key.transform.SetParent(surface.transform,false);
                    key.transform.localPosition=new Vector3(x+width*.5f,.007f,z);key.transform.localScale=new Vector3(width-.006f,.001f,.055f);
                    Destroy(key.GetComponent<Collider>());var renderer=key.GetComponent<Renderer>();renderer.sharedMaterial=idle;
                    var text=new GameObject("Printed key",typeof(TextMeshPro));text.transform.SetParent(surface.transform,false);text.transform.localPosition=new Vector3(x+width*.5f,.008f,z);text.transform.localRotation=Quaternion.Euler(90,0,0);
                    var tmp=text.GetComponent<TextMeshPro>();tmp.font=font;tmp.fontSize=.21f;tmp.enableAutoSizing=true;tmp.fontSizeMin=.09f;tmp.fontSizeMax=.21f;tmp.alignment=TextAlignmentOptions.Center;tmp.rectTransform.sizeDelta=new Vector2(width-.008f,.05f);tmp.text=label;tmp.color=new Color(.74f,.9f,.95f);
                    if(row==0&&col<13||label.Length==1||label.Length==3||row==1&&col>=11||row==4&&col==2||row==4&&col==5)foreach(char c in label)if(!char.IsWhiteSpace(c)&&!keys.ContainsKey(char.ToUpperInvariant(c)))keys[char.ToUpperInvariant(c)]=renderer;
                    if(label=="ESPACIO")keys[' ']=renderer;
                    x+=width;
                }
            }
            CombineStaticSurface();
            // Standard laptop-sized touch layout, leaving the upper display for the prompt.
            surface.transform.localScale=new Vector3(.38f,1,.55f);
            surface.transform.localPosition=new Vector3(0,0,-.12f);
            foreach(char c in "ABCDEFGHIJKLMNOPQRSTUVWXYZÑÁÉÍÓÚÜ0123456789.,;:?!¿¡´¨@#^~`%&$€¬[]{}\\|+-*/=_()<> ")Debug.Assert(keys.ContainsKey(c),"Touch key missing: "+c);
            Debug.Assert(KeyFor('A')=="Touch key A"&&KeyFor('B')=="Touch key B"&&KeyFor('R')=="Touch key R"&&KeyFor(' ')=="Touch key ESPACIO","Touch key label mapping");
        }
        void CombineStaticSurface()
        {
            var backgrounds=new List<CombineInstance>();var labels=new Dictionary<Material,List<CombineInstance>>();
            foreach(var tmp in surface.GetComponentsInChildren<TextMeshPro>())tmp.ForceMeshUpdate();
            foreach(var renderer in surface.GetComponentsInChildren<MeshRenderer>())
            {
                var owner=renderer.GetComponentInParent<TextMeshPro>();if(owner!=null&&owner.text.Contains("←"))continue;
                var filter=renderer.GetComponent<MeshFilter>();if(filter==null||filter.sharedMesh==null)continue;
                var item=new CombineInstance{mesh=filter.sharedMesh,transform=surface.transform.worldToLocalMatrix*renderer.transform.localToWorldMatrix};
                if(renderer.name.StartsWith("Touch key "))backgrounds.Add(item);
                else{var material=renderer.sharedMaterial;if(!labels.ContainsKey(material))labels[material]=new List<CombineInstance>();labels[material].Add(item);}
                renderer.enabled=false;
            }
            foreach(var tmp in surface.GetComponentsInChildren<TextMeshPro>())if(!tmp.text.Contains("←"))tmp.enabled=false;
            Merge("Keyboard base",backgrounds,idle);foreach(var pair in labels)Merge("Printed keyboard labels",pair.Value,pair.Key);
        }
        void Merge(string name,List<CombineInstance> items,Material material)
        {
            var mesh=new Mesh{name=name};mesh.CombineMeshes(items.ToArray(),true,true);combinedMeshes.Add(mesh);
            var obj=new GameObject(name,typeof(MeshFilter),typeof(MeshRenderer));obj.transform.SetParent(surface.transform,false);obj.GetComponent<MeshFilter>().sharedMesh=mesh;obj.GetComponent<MeshRenderer>().sharedMaterial=material;
        }
        public string KeyFor(char value)=>keys.TryGetValue(char.ToUpperInvariant(value),out var key)?key.name:"missing";
        public void Power(bool value){surface.SetActive(value);}
        public void Press(char value)
        {
            if(keys.TryGetValue(char.ToUpperInvariant(value),out var key)){key.sharedMaterial=active;key.enabled=true;var p=key.transform.localPosition;p.y=.0071f;key.transform.localPosition=p;litUntil[key]=Time.time+.12f;}
        }
        void Update(){foreach(var pair in litUntil)if(Time.time>=pair.Value)pair.Key.enabled=false;}
        void OnDestroy(){Destroy(idle);Destroy(active);foreach(var mesh in combinedMeshes)Destroy(mesh);}
    }
}
