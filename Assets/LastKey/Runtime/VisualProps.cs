using System;
using UnityEngine;

namespace LastKey
{
    public static class VisualProps
    {
        public static Material Surface(Color color,bool emission=false)
        {
            var m=new Material(Shader.Find("SimpleURPToonLitExample(With Outline)"));
            m.SetColor("_BaseColor",color);m.SetColor("_OutlineColor",new Color(.025f,.035f,.055f));
            m.SetColor("_ShadowMapColor",new Color(.30f,.40f,.57f));m.SetFloat("_IndirectLightMultiplier",.35f);
            m.SetFloat("_OutlineWidth",.22f);m.SetFloat("_CelShadeMidPoint",-.1f);m.SetFloat("_CelShadeSoftness",.14f);
            if(emission){m.SetFloat("_UseEmission",1);m.SetColor("_EmissionColor",color*1.3f);}
            return m;
        }
        public static Material EnvironmentSurface(Color color,bool emission=false)
        {
            var m=new Material(Shader.Find("Universal Render Pipeline/Lit"));m.color=color;m.SetFloat("_Smoothness",.32f);m.SetFloat("_Metallic",.25f);
            if(emission){m.EnableKeyword("_EMISSION");m.SetColor("_EmissionColor",color*2);}
            return m;
        }
        public static GameObject Fit(string resource,Transform parent,Vector3 center,Vector3 size,Color color)
        {
            var prefab=Resources.Load<GameObject>("VisualProps/"+resource);
            if(prefab==null)throw new InvalidOperationException("Missing free visual prop "+resource);
            var result=UnityEngine.Object.Instantiate(prefab,parent);result.name="Imported / "+resource;result.transform.rotation=Quaternion.identity;
            var renderers=result.GetComponentsInChildren<Renderer>();
            var bounds=renderers[0].bounds;foreach(var renderer in renderers)bounds.Encapsulate(renderer.bounds);
            result.transform.localScale=Vector3.Scale(result.transform.localScale,new Vector3(size.x/bounds.size.x,size.y/bounds.size.y,size.z/bounds.size.z));
            // Fit before station rotation is assigned; the arena arranges whole stations afterwards.
            bounds=renderers[0].bounds;foreach(var renderer in renderers)bounds.Encapsulate(renderer.bounds);
            result.transform.position+=parent.TransformPoint(center)-bounds.center;
            foreach(var renderer in renderers)
            {
                var materials=renderer.sharedMaterials;
                for(int i=0;i<materials.Length;i++)materials[i]=resource.StartsWith("Hair_")?Surface(color):EnvironmentSurface(color*(i==0?1:1.28f));
                renderer.sharedMaterials=materials;
            }
            return result;
        }
        public static void Restraint(Transform station,Vector3 center,Vector3 size)
        {
            var source=Resources.Load<GameObject>("VisualProps/collar").GetComponentsInChildren<MeshRenderer>()[0];
            var band=UnityEngine.Object.Instantiate(source.gameObject,station);band.name="Imported restraint band / chair anchor";
            band.transform.rotation=Quaternion.identity;
            var renderer=band.GetComponent<Renderer>();var bounds=renderer.bounds;
            band.transform.localScale=Vector3.Scale(band.transform.localScale,new Vector3(size.x/bounds.size.x,size.y/bounds.size.y,size.z/bounds.size.z));
            band.transform.position+=station.TransformPoint(center)-renderer.bounds.center;
            renderer.sharedMaterial=EnvironmentSurface(new Color(.24f,.27f,.31f));
        }
    }
    public sealed class CollarVisual:MonoBehaviour
    {
        Material status;
        public void Initialize(Material indicator){status=indicator;}
        public void SetDanger(Color color)
        {
            if(status==null)return;status.SetColor("_BaseColor",color);status.SetColor("_EmissionColor",color*1.8f);
        }
    }
}
