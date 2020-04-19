using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlKeyboard : MonoBehaviour {

    private Parade Parade { get; set; }
    private string _up, _left, _down, _right, _action, _mode;

    void Start () {Parade = this.GetComponent<Parade>();}

    public void Init(string up,string left,string down,string right,string action,string mode)
    {
        _up = up;
        _left = left;
        _right = right;
        _down = down;
        _action = action;
        _mode = mode;
    }
    void Update () {
        if (Input.GetKeyDown(_up)) Parade.inputUp();
        if (Input.GetKeyDown(_left)) Parade.inputLeft();
        if (Input.GetKeyDown(_down)) Parade.inputDown();
        if (Input.GetKeyDown(_right)) Parade.inputRight();
        if (Input.GetKeyDown(_action)) Parade.inputAction();
        if (Input.GetKeyDown(_mode)) Parade.inputMode();
    }
}
