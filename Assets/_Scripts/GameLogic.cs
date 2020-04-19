using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour {

    public Camera[] Cameras = new Camera[3];
    public CameraControl CamControl;

    private Parade[] Parades { get; set; }
    private bool _gameWon = false;

	void Start ()
    {
        Random.InitState(System.DateTime.Now.Millisecond);

        Parade.publish += ParadeEvent;
        Parades = new Parade[3]; //note that these are just references of the abstract class

        var paradeGO = new GameObject("PARADE AI");
        Parades[0] = paradeGO.AddComponent<ParadeElvises>(); //this is the "new" instantiation
        Parades[0].Init(2, 4, 0, 7, Cameras[0], true);
        Cameras[0].transform.SetParent(paradeGO.transform, false);
        paradeGO.AddComponent<ControlAI>();
        //var control1 = paradeGO.AddComponent<ControlKeyboard>();
        //control1.Init("w", "a", "x", "d", "s", "c");

        paradeGO = new GameObject("PARADE PLAYER");
        Parades[1] = paradeGO.AddComponent<ParadeKids>();
        Parades[1].Init(2, 4, 0, -7, Cameras[1]);
        Cameras[1].transform.SetParent(paradeGO.transform, false);
        paradeGO.AddComponent<ControlMouse>();

        //paradeGO = new GameObject("PARADE SIDEKICKS");
        //Parades[2] = paradeGO.AddComponent<ParadeRobots>();
        //Parades[2].Init(1, 2, -4, -7);
        //paradeGO.AddComponent<ControlAI>();

        //set parade enemies
        Parades[0].EnemyParade = Parades[1];
        Parades[1].EnemyParade = Parades[0];
/*        Parades[2].EnemyParade = Parades[0]*/;


        CamControl.AddTarget(Parades[0].transform);
        CamControl.AddTarget(Parades[1].transform);

    }



    private void ParadeEvent(ParadeEvents e, int paradeNumber)
    {
        //Debug.Log("PARADE EVENT->" + e + ":" + paradeNumber);
        if(e==ParadeEvents.Won && !_gameWon)
        {
            Parades[paradeNumber].GameOver(true);
            Parades[(paradeNumber + 1) % 2].GameOver(false);
            SFX.Play(CommonSFX.dance,null,false,true);
            _gameWon = true;
        }
    }

}
