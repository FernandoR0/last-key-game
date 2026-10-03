using System;using System.Linq;using System.IO;using System.Collections;using UnityEngine;using UnityEngine.UI;using TMPro;

namespace LastKey {public sealed class WardrobeShow:MonoBehaviour {

IEnumerator Start(){var host=GetComponent<LastKeyGame>();host.enabled=false;host.Canvas.enabled=false;host.Sound.enabled=false;host.Arena.Root.SetActive(false);foreach(var cam in FindObjectsByType<Camera>(FindObjectsSortMode.None))cam.enabled=false;

foreach(var oldLight in FindObjectsByType<Light>(FindObjectsSortMode.None))oldLight.enabled=false;var camera=new GameObject("Wardrobe review camera").AddComponent<Camera>();camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color(.045f,.055f,.07f);camera.orthographic=true;camera.orthographicSize=1.45f;camera.transform.position=new Vector3(0,1,7);camera.transform.LookAt(new Vector3(0,1,0));camera.fieldOfView=40;

var light=new GameObject("Wardrobe key light").AddComponent<Light>();light.type=LightType.Directional;light.intensity=1.0f;light.transform.rotation=Quaternion.Euler(25,160,0);var fill=new GameObject("Review fill").AddComponent<Light>();fill.type=LightType.Point;fill.transform.position=new Vector3(-2,2,3);fill.range=12;fill.intensity=2;RenderSettings.ambientLight=new Color(.5f,.5f,.5f);

var group=new GameObject("Imported outfit lineup").transform;Transform[] models=new Transform[4];Animator[] rigs=new Animator[4];for(int i=0;i<4;i++){models[i]=CharacterFactory.CreateStanding(group,i,host.Arena.Colors[i],out rigs[i]);models[i].localPosition+=new Vector3((1.5f-i)*1.35f,0,0);Relax(rigs[i]);}

var canvas=new GameObject("Review labels").AddComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;var scaler=canvas.gameObject.AddComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1920,1080);

var label=new GameObject("Names").AddComponent<TextMeshProUGUI>();label.transform.SetParent(canvas.transform,false);var rect=label.rectTransform;rect.anchorMin=new Vector2(0,0);rect.anchorMax=new Vector2(1,0);rect.pivot=new Vector2(.5f,0);rect.sizeDelta=new Vector2(0,130);label.alignment=TextAlignmentOptions.Center;label.fontSize=28;label.text="CIPHER                         UPLINK                         RELAY                         VECTOR\nValeria Rojas                 Ethan Cole                    Akira Mori                  Noah Bennett\nROPA IMPORTADA / REVISIÃ“N DE PROPORCIONES / S/0";

label.text="ROPA IMPORTADA / REVISIÓN DE PROPORCIONES / S/0";rect.sizeDelta=new Vector2(0,45);label.fontSize=23;

var names=new TextMeshProUGUI[4];for(int i=0;i<4;i++){var n=new GameObject("Identity "+i).AddComponent<TextMeshProUGUI>();n.transform.SetParent(canvas.transform,false);n.rectTransform.anchorMin=n.rectTransform.anchorMax=new Vector2(.107f+i*.262f,0);n.rectTransform.pivot=new Vector2(.5f,0);n.rectTransform.sizeDelta=new Vector2(420,90);n.rectTransform.anchoredPosition=new Vector2(0,45);n.alignment=TextAlignmentOptions.Center;n.fontSize=27;n.text=LastKeyGame.Aliases[i]+"\n"+LastKeyGame.Names[i];names[i]=n;}

var args=Environment.GetCommandLineArgs();var path=args[Array.IndexOf(args,"--evidence-path")+1];Directory.CreateDirectory(path);yield return new WaitForSecondsRealtime(3);yield return new WaitForEndOfFrame();Save(path,"00_FOUR_FULL_BODY.png");

foreach(var n in names)n.gameObject.SetActive(false);rect.sizeDelta=new Vector2(0,100);label.fontSize=28;for(int i=0;i<4;i++){for(int j=0;j<4;j++)models[j].gameObject.SetActive(j==i);models[i].position=new Vector3(0,models[i].position.y,0);camera.orthographicSize=1.25f;camera.transform.position=new Vector3(.8f,1,5.5f);camera.transform.LookAt(new Vector3(0,.95f,0));label.text=LastKeyGame.Aliases[i]+" / "+LastKeyGame.Names[i]+"\nCUERPO COMPLETO / PRENDA IMPORTADA";yield return null;yield return new WaitForEndOfFrame();Save(path,(i+1).ToString("00")+"_"+LastKeyGame.Aliases[i]+"_FULL_BODY.png");camera.transform.position=new Vector3(5.5f,1,0);camera.transform.LookAt(new Vector3(0,.95f,0));label.text=LastKeyGame.Aliases[i]+" / "+LastKeyGame.Names[i]+"\nVISTA LATERAL / PRENDA IMPORTADA";yield return null;yield return new WaitForEndOfFrame();Save(path,LastKeyGame.Aliases[i]+"_LATERAL.png");}

Debug.Log("LAST_KEY_WARDROBE_REVIEW_COMPLETE");

#if UNITY_EDITOR

UnityEditor.EditorApplication.Exit(0);

#endif

}

static void Relax(Animator a){foreach(var side in new[]{0,1}){var u=a.GetBoneTransform(side==0?HumanBodyBones.LeftUpperArm:HumanBodyBones.RightUpperArm);var l=a.GetBoneTransform(side==0?HumanBodyBones.LeftLowerArm:HumanBodyBones.RightLowerArm);var h=a.GetBoneTransform(side==0?HumanBodyBones.LeftHand:HumanBodyBones.RightHand);var handRotation=h.rotation;u.rotation=Quaternion.FromToRotation(l.position-u.position,new Vector3(Mathf.Sign(u.position.x-a.GetBoneTransform(HumanBodyBones.Hips).position.x)*.32f,-1,.08f))*u.rotation;l.rotation=Quaternion.FromToRotation(h.position-l.position,new Vector3(Mathf.Sign(u.position.x-a.GetBoneTransform(HumanBodyBones.Hips).position.x)*.12f,-1,.02f))*l.rotation;h.rotation=Quaternion.FromToRotation(h.GetComponentsInChildren<Transform>().First(t=>t.name.Contains("middle_01")).position-h.position,Vector3.down)*h.rotation;

var t=a.GetBoneTransform(side==0?HumanBodyBones.LeftUpperLeg:HumanBodyBones.RightUpperLeg);var c=a.GetBoneTransform(side==0?HumanBodyBones.LeftLowerLeg:HumanBodyBones.RightLowerLeg);var f=a.GetBoneTransform(side==0?HumanBodyBones.LeftFoot:HumanBodyBones.RightFoot);var footRotation=f.rotation;t.rotation=Quaternion.FromToRotation(c.position-t.position,Vector3.down)*t.rotation;c.rotation=Quaternion.FromToRotation(f.position-c.position,Vector3.down)*c.rotation;f.rotation=footRotation;}

}

static void Save(string p,string n){var tex=ScreenCapture.CaptureScreenshotAsTexture();File.WriteAllBytes(Path.Combine(p,n),tex.EncodeToPNG());Destroy(tex);}

}}
