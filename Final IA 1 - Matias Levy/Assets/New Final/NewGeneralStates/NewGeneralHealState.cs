using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGeneralHealState : BaseUnitState
{
    NewGeneral _meG;

    float currentHealWarmUp = 0;
    float HealWarmUp = 2;

    bool healed = false;

    public NewGeneralHealState(StateMachine SM, BaseUnit unit) : base(SM, unit) { }

    public override void Awake()
    {
        base.Awake();
        currentHealWarmUp = HealWarmUp;
        _meG = _me.GetComponent<NewGeneral>();
        _meG.healingPart.Play();
    }

    public override void Execute()
    {
        currentHealWarmUp -= Time.deltaTime;
        if(currentHealWarmUp <= 0 && !healed)
        {
            _meG.Heal(10);
            healed = true;
        }
    }

    public override void LateExecute()
    {
        base.LateExecute();
    }

    public override void Sleep()
    {
        base.Sleep();
        if (_meG.healingPart.isEmitting)
            _meG.healingPart.Stop();
        healed = false;
        currentHealWarmUp = HealWarmUp;
    }
}
