using System;
using System.Collections;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using LastKey.Core;
namespace LastKey {
// Opt-in native smoke test: real UI callbacks, real score input and real player screenshots.
// No scores, rules, timers or poses are assigned by this test.
public sealed class MinimalInterfaceQa:MonoBehaviour {
 LastKeyGame host;string root;int checks;
 IEnumerator Start(){
  host=GetComponent<LastKeyGame>();var args=Environment.GetCommandLineArgs();root=args[Array.IndexOf(args,"--evidence-path")+1];Directory.CreateDirectory(root);Time.captureFramerate=30;QualitySettings.vSyncCount=0;
  Application.logMessageReceived+=FailOnException;
  while(host.Flow!=ExperienceState.MainMenu)yield return null;yield return new WaitForSeconds(.6f);
  Check(!host.Canvas.GetComponentsInChildren<Button>().Any(b=>b.name.ToLowerInvariant().Contains("sala")),"no room controls");
  yield return Snap("01_MAIN_MENU");var title=host.Canvas.GetComponentsInChildren<TMP_Text>().First(t=>t.name=="Main title");Check(!title.isTextOverflowing,"main title fits");
  Click("Sound");yield return null;var sliders=host.Canvas.GetComponentsInChildren<Slider>();Check(sliders.Length==2,"master and music sliders");float master=sliders[0].value,music=sliders[1].value;
  sliders[0].value=.35f;sliders[1].value=.45f;yield return null;Check(Mathf.Approximately(AudioListener.volume,.35f),"master volume connected");Check(Mathf.Approximately(host.Music.Volume,.45f),"music volume connected");yield return Snap("02_SOUND_SETTINGS");sliders[0].value=master;sliders[1].value=music;Click("Volver");
  Click("Modos de juego");yield return null;Check(host.Flow==ExperienceState.ModeSelect,"mode menu opens");foreach(string mode in new[]{"Classic","Code","Numbers","Memory","Sudden Death"})Check(Find(mode)!=null,"mode control "+mode);yield return Snap("03_MODE_SELECT");
  Click("Classic");yield return null;Check(host.Flow==ExperienceState.CharacterSelect,"character menu opens");yield return Snap("04_CHARACTER_SELECT");Click("Continuar con CIPHER");yield return null;Check(host.Flow==ExperienceState.MatchSetup,"setup opens");yield return Snap("05_MATCH_SETUP");Click("Comenzar competencia");
  while(host.Flow==ExperienceState.RoundIntro&&host.StateTime<27.5f)yield return null;Check(host.Flow==ExperienceState.RoundIntro,"first round intro plays");yield return Snap("06_ORIGINAL_COLLAR_MOTION");
  while(host.Flow!=ExperienceState.Gameplay)yield return null;while(host.Game.Elapsed<3){TypeOne();yield return new WaitForSeconds(.15f);}Check(host.Game.Players[0].Correct>0,"typing updates competition");Check(host.Cameras.Role=="GameplayPlayerCamera","first person camera");yield return Snap("07_GAMEPLAY");
  host.Game.Input(0,'\ufffd');yield return null;Check(host.Game.Players[0].Errors==1,"error updates competition");yield return Snap("08_ERROR_FEEDBACK");
  float beforePause=host.Game.Elapsed;Click("Pause");yield return new WaitForSeconds(.5f);Check(host.Flow==ExperienceState.Paused,"pause callback");Check(Mathf.Approximately(host.Game.Elapsed,beforePause),"pause holds round timer");yield return Snap("09_PAUSE");Click("Continuar");yield return null;Check(host.Flow==ExperienceState.Gameplay,"resume callback");
  while(host.Game.ActiveDirective==Directive.None&&host.Game.Elapsed<30){TypeOne();yield return new WaitForSeconds(.15f);}Check(host.Game.ActiveDirective!=Directive.None,"NULL directive active");yield return Snap("10_NULL_DIRECTIVE");
  while(!host.Game.EchoOffered&&host.Game.Elapsed<42){TypeOne();yield return new WaitForSeconds(.15f);}Check(host.Game.EchoOffered,"ECHO offered");yield return null;yield return Snap("11_ECHO_OFFER");var promptRect=host.Canvas.GetComponentsInChildren<TMP_Text>().First(t=>t.name=="Typing target").rectTransform;foreach(string choice in new[]{"Aceptar reto","Seguir escribiendo"})Check(!Bounds(Find(choice).GetComponent<RectTransform>()).Overlaps(Bounds(promptRect)),"ECHO choice clear of typing "+choice);Click("Aceptar reto");yield return null;Check(host.Game.Players[0].EchoAccepted,"ECHO button connected");for(int i=0;i<5;i++){TypeOne();yield return new WaitForSeconds(.15f);}yield return Snap("12_ECHO_TYPING");
  Debug.Log("LAST_KEY_MINIMAL_INTERFACE_QA_PASS "+checks);Application.logMessageReceived-=FailOnException;Application.Quit();
 }
 Rect Bounds(RectTransform target){var corners=new Vector3[4];target.GetWorldCorners(corners);return Rect.MinMaxRect(corners[0].x,corners[0].y,corners[2].x,corners[2].y);}
 Button Find(string name)=>host.Canvas.GetComponentsInChildren<Button>().FirstOrDefault(b=>b.name==name&&b.IsInteractable());
 void Click(string name){var button=Find(name);Check(button!=null,"click "+name);button.onClick.Invoke();}
 void TypeOne(){var p=host.Game.Players[0];if(p.Eliminated)return;string target=p.EchoAccepted&&!p.EchoResolved?Competition.EchoText:p.Prompt;int cursor=p.EchoAccepted&&!p.EchoResolved?p.EchoCursor:p.Cursor;if(cursor>=target.Length)return;char key=target[cursor];host.Arena.TouchKeys[0].Press(key);host.Game.Input(0,key);}
 IEnumerator Snap(string name){yield return new WaitForEndOfFrame();foreach(var text in host.Canvas.GetComponentsInChildren<TMP_Text>()){text.ForceMeshUpdate();if(text.isTextOverflowing)Debug.LogWarning("UI_TEXT_OVERFLOW "+text.name+" "+text.text);}var tex=ScreenCapture.CaptureScreenshotAsTexture();File.WriteAllBytes(Path.Combine(root,name+".png"),tex.EncodeToPNG());Destroy(tex);Debug.Log("MINIMAL_UI_CAPTURE "+name+" flow="+host.Flow);}
 void Check(bool ok,string name){if(!ok)throw new InvalidOperationException("MINIMAL_UI_QA_FAIL "+name);checks++;Debug.Log("MINIMAL_UI_PASS "+name);}
 void FailOnException(string condition,string trace,LogType type){if(type==LogType.Exception)Application.Quit(1);}
 void OnDestroy(){Application.logMessageReceived-=FailOnException;}
}}
