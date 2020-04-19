using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Info : MonoBehaviour {

    [Header("0=Right Text, 1=Left Text")]
    public Text[] text=new Text[2];
    private IEnumerator[] _tween = new IEnumerator[2];
    private static readonly Color _invisible = new Color(1, 1, 1, 0);

    private void Awake() {
        text[0].text = "";
        text[1].text = "";

        
    }
    public void Left(string s,bool fadeIn=false) {UpdateText(1, s, fadeIn);}
    public void Right(string s,bool fadeIn=false) { UpdateText(0, s, fadeIn); }

    private void UpdateText(int i, string s,bool fadeIn)
    {
        text[i].text = s;
        if (fadeIn==true)
        {
            if (_tween[i] != null) StopCoroutine(_tween[i]);
            _tween[i] = DoTween(text[i]);
            StartCoroutine(_tween[i]);
        }
    }

    IEnumerator DoTween(Text text)
    {
        text.color = _invisible;
        for (var i = 0f; i < 1f; i+=.01f)
        {
            text.color = Color.Lerp(_invisible, Color.white, Mathf.SmoothStep(0.0f, 1.0f, i));
            yield return new WaitForSeconds(.001f);
        }
    }
}
