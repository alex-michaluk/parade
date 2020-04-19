using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keypad : MonoBehaviour {

    [Header("UP,DOWN,LEFT,RIGHT,ACTION, MODE")]
    public Renderer[] Renderers = new Renderer[6];
    private Color _padColor = new Color(.05f, .05f, .05f);
    private Color _disabledColor = new Color(1, 0, 0 , .75f);
    private Color _pressedColor = Color.green; // new Color(0,1,0);
    private bool _playSFX = true;

    public void Awake()
    {
        foreach (Renderer r in Renderers)
            foreach (Material m in r.materials) m.color = _padColor;    
    }

    public void SilenceKeypad() { _playSFX = false;}

    public void PulseUp() { Util.PulseColor(Renderers[0], _padColor, _pressedColor); SFXClick(); }
    public void PulseDown() { Util.PulseColor(Renderers[1], _padColor, _pressedColor); SFXClick(); }
    public void PulseLeft() { Util.PulseColor(Renderers[2], _padColor, _pressedColor); SFXClick(); }
    public void PulseRight() { Util.PulseColor(Renderers[3], _padColor, _pressedColor); SFXClick(); }
    public void PulseAction() { Util.PulseColor(Renderers[4], _padColor, _pressedColor); }
    public void PulseMode() { Util.PulseColor(Renderers[5], _padColor, _pressedColor); SFXClick(); }
    public void PulseActionDisabled() { Util.PulseColor(Renderers[4], _padColor, _disabledColor); SFXDisabled(); }
    public void PulseModeDisabled() { Util.PulseColor(Renderers[5], _padColor, _disabledColor); SFXDisabled(); }

    private void SFXClick() { if(_playSFX) SFX.Play(CommonSFX.click); }

    private void SFXDisabled() { if (_playSFX) SFX.Play(CommonSFX.disabled);}
}
