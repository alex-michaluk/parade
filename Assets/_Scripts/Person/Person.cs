using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Text;

public enum PersonAnimTriggers { TurnLeft, TurnRight, Idle, Walk, Select, Throw, Incant, Heal, Poison, Death, Victory1, Victory2, Victory3 }
public enum PersonStates { Ready, Attacking, Dead, GameOver}

public abstract class Person : MonoBehaviour  {

    public string Name { get; protected set; }
    public Animator Anim { get; private set; }
    public PersonStates PersonState { get; private set; }
    public Parade MyParade { get; private set; }
    protected Renderer Render { get; private set; }
    public int Health { get; protected set;}
    public int AttackWait { get; set;}
    public int AttackWaitTimeSec { get; private set;}
    
    //locks
    private bool _pulsingColor = false;
    private bool _highlight = false;

    private UnityEngine.UI.Text _healthText;
    private IEnumerator _healthTween;

    public void Init( Parade p,int row, int col)
    {
       
        PersonState = PersonStates.Ready;
        MyParade = p;
        var x = -((p.Columns - 1) *.5f);
        var y = -((p.Rows - 1) * .5f);
        var xSpacing = 1.2f;
        var ySpacing = 2f;
        transform.localPosition = new Vector3((x+row)*xSpacing,0,(y+col)*ySpacing);
        Anim = gameObject.GetComponent<Animator>();
        Render = gameObject.transform.GetComponentInChildren<Renderer>();
        AttackWait = 0;
        Health = SetHealth();
        AttackWaitTimeSec = SetAttackWaitTimeSec();
        Name = Util.GetName() + " the " + Util.GetAdjective() + " " + this.GetType().Name;
        var hd = Instantiate(Util.HealthDisplay);
        hd.transform.SetParent(this.transform, false); //for placement
        hd.transform.SetParent(MyParade.transform, true);
        _healthText = hd.GetComponentInChildren<UnityEngine.UI.Text>();
        _healthText.text = "-1";
        RenderHighlight();
        ShowHealth();
    }

    protected abstract int SetAttackWaitTimeSec();

    protected abstract int SetHealth();

    public override string ToString()
    {
        var s = Name + "\n";
        if (Health > 0)
        {
            s += "Health: " + Health + "\n";
            if (AttackWait > 0) s += "Action ready in " + AttackWait + " seconds...";
            else s += "READY FOR ACTION!";
        }
        else s += "DEAD!\n";
        return s;
    }



    public void Hurt(int dmg)
    {
        if (PersonState != PersonStates.Dead)
        {
            Health -= dmg;
            if (Health <= 0)
            {
                Health = 0;
                PersonState = PersonStates.Dead;

                //if (MyParade.EnemyParade.CheckForWin()) return; //don't run final health update on last death
                Anim.SetTrigger(PersonAnimTriggers.Death.ToString());
            }
            else PulseColor(Color.red);

            if (_healthTween != null) StopCoroutine(_healthTween);
            _healthTween = TweenHealth(-dmg);
            StartCoroutine(_healthTween);

        }
    }



    public void Heal(int heal)
    {
        if (PersonState != PersonStates.Dead)
        {
            Health += heal;
            if (Health > 100) Health = 100;
            PulseColor(Color.blue);
            if (_healthTween != null) StopCoroutine(_healthTween);
            _healthTween = TweenHealth(heal);
            StartCoroutine(_healthTween);
        }
    }


    IEnumerator TweenHealth(int dmg)
    {

        _healthText.text = dmg.ToString();
        if (dmg < 0) _healthText.color = Color.red;
        else  _healthText.color = Color.blue;

        for (var t = 0f; t < 1f; t += .01f)
        {
            _healthText.fontSize = (int) Mathf.Lerp(0f, 60f, Mathf.SmoothStep(0.0f, 1.0f, t));
            yield return new WaitForSeconds(.001f);
        }
        yield return new WaitForSeconds(.25f);
        ShowHealth();
    }

    public void ShowHealth()
    {
        _healthText.fontSize = 35;
        if (Health == 0)
        {
            _healthText.color = Color.red;
            _healthText.text = "x";
        }
        else
        {
            _healthText.color = Color.white;
            _healthText.text = Health.ToString();
        }

    }


    public void Attack()
    {
        if (CanAttack())
        {
            PersonState = PersonStates.Attacking;
            AttackWait = AttackWaitTimeSec;
            StartCoroutine(TurnAndAttack());
        }
    }

    public bool CanAttack()
    {
        if (PersonState == PersonStates.Ready && AttackWait<=0) return true;
        return false;
    }

    protected abstract void PerformAttack();

    public IEnumerator TurnAndAttack()
    {
        var startRotation = transform.localEulerAngles.y;
        var endRotation = Quaternion.LookRotation(MyParade.EnemyParade.transform.position - transform.position).eulerAngles.y;
        if ((endRotation-startRotation) >180) endRotation -= 360;

        for (var t = 0f; t <= 1f; t += .1f)
        {
            transform.localRotation = Quaternion.Euler(0, Mathf.Lerp(startRotation, endRotation , Mathf.SmoothStep(0.0f, 1.0f, t)), 0);
            yield return new WaitForSeconds(.01f);
        }

        //virtual
        PerformAttack();

        yield return new WaitForSeconds(1); //assume 1 sec attack animation
        for (var t = 0f; t <= 1f; t += .1f)
        {
            transform.localRotation = Quaternion.Euler(0, Mathf.Lerp(endRotation, startRotation, Mathf.SmoothStep(0.0f, 1.0f, t)), 0);
            yield return new WaitForSeconds(.01f);
        }
        PersonState = PersonStates.Ready;
    }


    public void PulseColor(Color c, System.Action callback = null)
    {
        if (!_pulsingColor)
        {
            _pulsingColor = true;
            Util.PulseColor(Render, Render.material.color, c, true, () => { _pulsingColor = false;RenderHighlight(); });
        }
    }

    public void Highlight() { if (!_highlight) { _highlight = true; RenderHighlight(); } }
    public void Unhighlight() { if (_highlight) { _highlight = false; RenderHighlight(); } }
    private void RenderHighlight() {if (_highlight) Util.Tint(Render, Color.white); else Util.Tint(Render, Color.black);}


    public void GameOver()
    {
        PersonState = PersonStates.GameOver;
        if (_healthTween != null) StopCoroutine(_healthTween);
        transform.localRotation = Quaternion.Euler(0, 180, 0);
        _healthText.text = "";
        Highlight();
    }
}