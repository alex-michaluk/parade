using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public enum ParadeStates { Idling, Walking, Turning, GameOver }
public enum ParadeDirections { Up=0, Right=1, Down=2, Left=3}
public enum ParadeEvents { Won };

public abstract class Parade : MonoBehaviour {
    static int NumberOfParades = 0;

    public Person[] People { get; set; }
    public Camera Camera { get; set; }
    public int ParadeNumber { get; private set; }
    public int Rows { get; private set; }
    public int Columns { get; private set; }
    public Parade EnemyParade { get; set; }
    public ParadeStates ParadeState { get; private set; }
    public int ParadeDirection { get; private set; }
    public GameObject Keypad { get; private set; }
    
    public System.Action inputUp, inputDown, inputLeft, inputRight, inputAction, inputMode;
    public System.Action<GameObject> inputClick;

    private static readonly float MOVE_SPEED = .004f;
    private static readonly float BOOST_MULT = 4f;
    private static readonly int BOOST_TIME_SEC = 5; 
    private static readonly int BOOST_RECHARGE_TIME_SEC = 3;

    private Info TextInfo { get; set; }
    private Keypad _keypadScript { get; set; }
    private int _activePerson = 0;
    private int _boost, _boostRecharge = 0;
    private float _boostMult=1;
    private string _boostStatus;
    private string ControllerHelpText = "";
    private ParticleSystem ParticlesHealth { get; set; }

    //publisher
    public delegate void EventHandler(ParadeEvents e, int paradeNumber);
    public static event EventHandler publish;
    private void PublishEvent(ParadeEvents e) { if (publish != null) publish(e, ParadeNumber); }

    public void Awake()
    { 
        ParadeState = ParadeStates.Idling;
        ParadeDirection = (int)ParadeDirections.Down;
        inputMode = CheckToggleMode;
        inputClick = HandleClick;
        SetModeInputActions();
    }

    public void Start() {RefreshLeftText(true);}

    public void Init(int rows, int columns, int xOffset = 0, int zOffset = 0, Camera camera = null, bool silenceKeypad = false)
    {
        Rows = rows;
        Columns = columns;
        Camera = camera;
        if(Camera) TextInfo = camera.GetComponentInChildren<Info>();

        ParadeNumber = NumberOfParades;
        NumberOfParades++;
        People = new Person[Rows * Columns];
        gameObject.name = gameObject.name + " [" + ParadeNumber.ToString() + "]";
        gameObject.transform.position = new Vector3(xOffset, 0, zOffset);
        gameObject.layer = 2; //ignore raycast layer

        Keypad = Instantiate(Util.Keypad);
        Keypad.transform.SetParent(this.transform, false);
        _keypadScript = Keypad.GetComponent<Keypad>();
        if (silenceKeypad || camera == null) {
            _keypadScript.SilenceKeypad();
            Keypad.transform.localScale = Vector3.zero;
        }

        var pObj = Instantiate(Util.ParticlesHealth);
        pObj.transform.SetParent(this.transform, false);
        ParticlesHealth = pObj.GetComponent<ParticleSystem>();
        ParticlesHealth.Stop();

        int personNumber = 0;
        for (var col = 0; col < Columns; col++)
        {
            for (var row = 0; row < Rows; row++)
            {
                System.Type type;
                var go = Instantiate(GetCharacter(row,col,personNumber,out type));
                go.name = personNumber.ToString();
                go.transform.SetParent(this.transform, false);
                var person = go.AddComponent(type) as Person;
                person.Init(this, col, row);
                People[personNumber] = person;
                personNumber++;
            }
        }
        HighlightActivePlayer();
        StartCoroutine(TimedUpdate());
    }

    protected abstract GameObject GetCharacter(int row, int col, int personNumber, out System.Type type);

    private void SetModeInputActions()
    {
        inputUp = () => { _keypadScript.PulseUp(); };
        inputDown = () => { _keypadScript.PulseDown(); };
        inputLeft = () => { _keypadScript.PulseLeft(); };
        inputRight = () => { _keypadScript.PulseRight(); };
        
        if (ParadeState == ParadeStates.Idling)
        {
            ControllerHelpText = "Directional: Select Player \nCenter Button: Attack\nMode Button: Switch to Walk";
            inputUp += () => { _activePerson++; HighlightActivePlayer(); };
            inputDown += () => { _activePerson--; HighlightActivePlayer(); };
            inputLeft += () =>
            {
                if (_activePerson == Rows - 1) _activePerson = (Rows * Columns - Rows); else _activePerson -= Rows;
                if (_activePerson < 0) _activePerson = (_activePerson + (Rows * Columns + 1));
                HighlightActivePlayer();
            };
            inputRight += () =>
            {
                if (_activePerson == (Rows * Columns - 1)) _activePerson = 0; else _activePerson += Rows;
                if (_activePerson >= Rows * Columns) _activePerson = (_activePerson - (Rows * Columns - 1));
                HighlightActivePlayer();
            };
            inputAction = AttemptAction;
        }
        else if (ParadeState==ParadeStates.Walking)
        {
            ControllerHelpText = "Directional: Navigate\nCenter Button: Speed Boost\nMode Button: Switch to Attack";
            inputUp += () => { ParadeTurn(GetTurnAmount((int)ParadeDirections.Up)); };
            inputDown += () => { ParadeTurn(GetTurnAmount((int)ParadeDirections.Down)); };
            inputLeft += () => { ParadeTurn(GetTurnAmount((int)ParadeDirections.Left)); };
            inputRight += () => { ParadeTurn(GetTurnAmount((int)ParadeDirections.Right)); };
            inputAction = AttemptBoost;
        }
        else if (ParadeState == ParadeStates.GameOver)
        {
            inputUp = () => { };
            inputDown = () => { };
            inputLeft = () => { };
            inputRight = () => { };
            inputMode = () => { };
            inputAction = () => { };
            inputClick = (GameObject g) => { };
        }
    }


    public void Update()
    {
        if (ParadeState == ParadeStates.Walking)
        {
            bool turnUD = false, turnLR = false; //auto turn on boundaries

            switch (ParadeDirection)
            { 
                case (int)ParadeDirections.Up: 
                    this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z + MOVE_SPEED * _boostMult);
                    if (this.transform.position.z >= 8.5f) turnLR = true;
                    break;
                case (int)ParadeDirections.Down:
                    this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z - MOVE_SPEED * _boostMult);
                    if (this.transform.position.z <= -8.5f) turnLR = true;   
                    break;
                case (int)ParadeDirections.Left:
                    this.transform.position = new Vector3(this.transform.position.x - MOVE_SPEED * _boostMult, this.transform.position.y, this.transform.position.z);
                    if (this.transform.position.x <=-7.5f) turnUD = true;
                    break;
                case (int)ParadeDirections.Right: 
                    this.transform.position = new Vector3(this.transform.position.x + MOVE_SPEED * _boostMult, this.transform.position.y, this.transform.position.z);
                    if (this.transform.position.x >= 7.5f) turnUD = true;
                    break;
            }

            if(turnLR)
            {
                if(transform.position.x>0) ParadeTurn(GetTurnAmount((int)ParadeDirections.Left));
                 else ParadeTurn(GetTurnAmount((int)ParadeDirections.Right));
            }
            else if (turnUD)
            {
                if (transform.position.z > 0) ParadeTurn(GetTurnAmount((int)ParadeDirections.Down));
                else ParadeTurn(GetTurnAmount((int)ParadeDirections.Up));
            }
        }
        
    }

    IEnumerator TimedUpdate()
    {
        while(true)
        {
            if (_boost > 0)
            {
                _boost--;
                if (_boost == 0)
                {
                    _boostMult = 1;
                    _boostRecharge = BOOST_RECHARGE_TIME_SEC; 
                }
                RefreshRightText();
            }          
            else if (_boostRecharge > 0)
            {
                _boostRecharge--;
                RefreshRightText();
            }

            for(var i=0;i<People.Length;i++)
            {
                if (People[i].AttackWait > 0)
                {
                    People[i].AttackWait--;
                    if (i==_activePerson) RefreshRightText();
                }
            }
            yield return new WaitForSeconds(1);
        }
    }

    private void AttemptBoost()
    {
        if (_boost==0 && _boostRecharge == 0)
        {
            _keypadScript.PulseAction();
            _boostMult = BOOST_MULT;
            _boost = BOOST_TIME_SEC;
            RefreshRightText();
            SFX.Play(CommonSFX.flyby);
        }
        else _keypadScript.PulseActionDisabled();

    }


    public void HandleClick(GameObject go)
    {
        if (go.tag == "PERSON")
        { 
            _activePerson = int.Parse(go.name);
            HighlightActivePlayer();
            if(ParadeState == ParadeStates.Idling) AttemptAction(); //click shortcut for quick mouse attacks
        }
        if (go.tag == "CONTROLPAD")
        {
            switch(go.name)
            {
                case "UP":  inputUp(); break;
                case "DOWN": inputDown(); break;
                case "LEFT": inputLeft(); break;
                case "RIGHT": inputRight(); break;
                case "ACTION": inputAction(); break;
                case "MODE": inputMode(); break;
            }
        }

    }

    private int GetTurnAmount(int i)
    {
        var t = i - ParadeDirection;
        if (t == 3) t = -1;
        if (t == -3) t = 1;
        return t;
    }

    private void ParadeTurn(int turn,ParadeStates returnState=ParadeStates.Walking)
    {
        if (turn == 0 || ParadeState == ParadeStates.Turning) return;
        ParadeState = ParadeStates.Turning;
        if (turn < 0) SetPeopleAnimTriggers(PersonAnimTriggers.TurnLeft);
        else SetPeopleAnimTriggers(PersonAnimTriggers.TurnRight);
        StartCoroutine(TweenTurn((int)ParadeDirection*90, (int)ParadeDirection*90+(turn*90),returnState));
        ParadeDirection+=turn;
        if (ParadeDirection < 0) ParadeDirection += 4;
        ParadeDirection %= 4;
    }


    IEnumerator TweenTurn(float startRotation, float endRotation,ParadeStates returnState=ParadeStates.Walking)
    {
        for (var t = 0f; t <= 1f; t += .04f)
        {
            for (var i = 0; i < People.Length; i++)
                People[i].transform.localRotation = Quaternion.Euler(0, Mathf.Lerp(startRotation, endRotation, Mathf.SmoothStep(0.0f, 1.0f, t)), 0);
            yield return new WaitForSeconds(.01f);
        }
        ParadeState = returnState;
        SetModeInputActions();
    }



    public void HitParade(Transform hitTransform,int damage)
    {
        if (ParadeState != ParadeStates.GameOver)
        {
            var firstHit = true;
            foreach (Person p in People)
            {
                int dmg = damage - (int)(hitTransform.position - p.transform.position).sqrMagnitude;
                if (dmg > 0)
                {
                    if (firstHit) SFX.Play(CommonSFX.damage);
                    firstHit = false;
                    p.Hurt(dmg);
                }
            }
            if (!EnemyParade.CheckForWin()) RefreshRightText();
        }
    }

    private void CheckToggleMode()
    {
        //no people can be in middle of an attack, and we cannot be in the middle of a turning to toggle modes
        var ok = true;
        if (ParadeState == ParadeStates.Turning)
            ok = false;
        if(ParadeState == ParadeStates.Idling)
            foreach (Person p in People) if (p.PersonState == PersonStates.Attacking) ok = false;
        if (ok)
        {
            _keypadScript.PulseMode();
            ToggleMode();
            RefreshLeftText(true);
        }
        else _keypadScript.PulseModeDisabled();         
    }

    private void AttemptAction()
    {
        var ok = false;
        if(ParadeState==ParadeStates.Idling)
            if (People[_activePerson].CanAttack()) ok = true;
        if(ok)
        {
            _keypadScript.PulseAction();
            People[_activePerson].Attack();
        }
        else _keypadScript.PulseActionDisabled();
    }



    private void ToggleMode()
    {
        if (ParadeState == ParadeStates.Walking)
        {
            ParadeState = ParadeStates.Idling;
            SetPeopleAnimTriggers(PersonAnimTriggers.Idle);
            SetModeInputActions();
        }
        else if(ParadeState == ParadeStates.Idling)
        {
            ParadeState = ParadeStates.Walking;
            SetPeopleAnimTriggers(PersonAnimTriggers.Walk);
            SetModeInputActions();
        }
    }

    private void HighlightActivePlayer()
    {
        if (_activePerson < 0) _activePerson += (Rows * Columns);
        _activePerson %= (Rows * Columns);
        foreach (Person p in People) p.Unhighlight();
        People[_activePerson].Highlight();
        RefreshRightText(true);
    }

    private void SetPeopleAnimTriggers(PersonAnimTriggers trigger)
    {
        for (var i = 0; i < People.Length; i++) People[i].Anim.SetTrigger(trigger.ToString());
    }

    public void RefreshRightText(bool fadeIn = false)
    {
        if (TextInfo)
        {
            _boostStatus = "\nBoost ready!";
            if (_boost > 0) _boostStatus = "\nBoosting for " + _boost + " seconds!";
            else if (_boostRecharge > 0) _boostStatus = "\nBoost ready in " + _boostRecharge + " seconds...";
            TextInfo.Right(People[_activePerson].ToString() + _boostStatus, fadeIn);
        }
    }
    public void RefreshLeftText(bool fadeIn = false){if(TextInfo) TextInfo.Left(ControllerHelpText, fadeIn);}

    public void PlayParticlesHealth() {ParticlesHealth.Play();Invoke("StopParticlesHealth", 2); }
    private void StopParticlesHealth() { ParticlesHealth.Stop(); }

    public bool CheckForWin()
    {
        var ok = true;
        foreach (Person p in EnemyParade.People)
            if (p.PersonState != PersonStates.Dead) ok = false;
        if (ok) PublishEvent(ParadeEvents.Won);
        return (ok);
    }

    public void GameOver(bool won)
    {
        StopAllCoroutines();
        ParadeState = ParadeStates.GameOver;
        SetModeInputActions();

        var endAnim = "";
        if (won==true)
        {
            if (TextInfo) TextInfo.Left("GAME OVER\nYOU WON!",true);
            endAnim = "Victory";
        }
        else
        {
            if (TextInfo) TextInfo.Left("GAME OVER\nYOU LOST...",true);
            endAnim = "Lose";
        }
        if(TextInfo) TextInfo.Right("You played a very " + Util.GetAdjective() + " game!", true);
        for (var i = 0; i < People.Length; i++)
        {
            People[i].GameOver();
            People[i].Anim.SetTrigger(endAnim + ((i % 3) + 1));
        }
    }
}






//var paradeRB = GO.AddComponent<Rigidbody>();
//paradeRB.useGravity = false;
//paradeRB.isKinematic = true;
//var paradeCollider = GO.AddComponent<BoxCollider>();
//paradeCollider.size = new Vector3(Columns + 1, 1.5f, Rows + 1);
//paradeCollider.center = new Vector3(0, .75f, 0);
//paradeCollider.isTrigger = true;

//private void OnTriggerEnter(Collider other)
//{
//    Debug.Log("COLLISION: " + other);
//    PublishEvent(ParadeEvents.Collision);
//}


