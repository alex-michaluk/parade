using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlMouse : MonoBehaviour {

    private Parade Parade { get; set; }

    // Use this for initialization
    void Start () {
        Parade = this.GetComponent<Parade>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Parade.Camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit _rayHit;
            if (Physics.Raycast(ray, out _rayHit))
                Parade.inputClick(_rayHit.transform.gameObject);
        }
    }
}
