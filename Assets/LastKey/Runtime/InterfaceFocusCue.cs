using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
namespace LastKey {
// Visible keyboard selection for text-only controls, including Tab/Enter navigation.
public sealed class InterfaceFocusCue:MonoBehaviour,ISelectHandler,IDeselectHandler {
 public TMP_Text Label;
 public void OnSelect(BaseEventData data){if(Label!=null)Label.fontStyle|=FontStyles.Underline;}
 public void OnDeselect(BaseEventData data){if(Label!=null)Label.fontStyle&=~FontStyles.Underline;}
}}
