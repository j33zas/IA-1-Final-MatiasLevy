using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralHealingState : BaseUnitState
{
    Vector3 dir;

    BaseUnit toHeal;

    General _meG;

    public GeneralHealingState(StateMachine SM, General G) : base(SM, G) { }

    public override void Awake()
    {
        base.Awake();
        _me.AN.SetBool("Healing", true);
        _meG = _me.GetComponent<General>();
        _meG.HealParticles.Play();
        _meG.healing = true;
        toHeal = _me.objective.GetComponent<BaseUnit>();
    }

    public override void Execute()
    {
        base.Execute();
        if(toHeal)
            dir = (toHeal.transform.position - _me.transform.position).normalized;
        _me.transform.forward = Vector3.Lerp(_me.transform.forward, dir.normalized, Time.deltaTime * _me.rotSpeed);

        if (toHeal.currentHealth <= toHeal.maxHealth * 9/(10))
            toHeal.currentHealth = Mathf.RoundToInt(Mathf.Lerp(toHeal.currentHealth, toHeal.maxHealth, Time.deltaTime * _meG.healSpeed));
        else
            _meG.healing = false;

    }

    public override void LateExecute()
    {
        base.LateExecute();
    }

    public override void Sleep()
    {
        _me.AN.SetBool("Healing", false);
        base.Sleep();
    }
}
