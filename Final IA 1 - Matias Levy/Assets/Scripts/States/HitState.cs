using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitState : BaseUnitState
{
    public HitState(StateMachine SM, BaseUnit unit):base(SM,unit){}

    public override void Awake()
    {
        base.Awake();
        _me.AN.SetTrigger("Hit");
        _me.AN.SetBool("Walking", false);
        _me.AN.SetBool("Running", false);
        _me.AN.SetBool("Has destination", false);
        _me.stunnedParticle.Play();
    }

    public override void Execute()
    {
        base.Execute();
    }

    public override void LateExecute()
    {
        base.LateExecute();
    }

    public override void Sleep()
    {
        base.Sleep();
        _me.stunnedParticle.Clear();
        _me.stunnedParticle.Stop();
        _me.stunned = false;
    }
}
