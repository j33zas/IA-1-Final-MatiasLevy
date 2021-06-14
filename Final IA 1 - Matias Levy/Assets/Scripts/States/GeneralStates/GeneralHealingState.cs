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

        if(Vector3.Angle(_me.transform.forward, toHeal.transform.position - _me.transform.position) <= 5)
            if (toHeal.currentHealth<= 80)//curo hasta 80 de vida
                toHeal.currentHealth += Mathf.RoundToInt(_meG.healSpeed);
        else
            _me.transform.forward = Vector3.Lerp(_me.transform.forward, dir.normalized, Time.deltaTime * _me.rotSpeed);
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
