using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieState : BaseUnitState
{
    public DieState(StateMachine sm, BaseUnit unit) : base(sm, unit){}

    public override void Awake()
    {
        base.Awake();
        GameManager.gameManager.StartCoroutine(GameManager.gameManager.RemoveUnit(_me,2));
        //_me.COLL.gameObject.SetActive(false);
        //_me.RB.gameObject.SetActive(false);
        //_me.dead = true;
        //_me.AN.SetTrigger("Die");
    }

    public override void Execute()
    {
        base.Execute();
        _me.transform.Rotate(new Vector3(0, 1, 0), 15 * Time.deltaTime);
        _me.transform.localScale = Vector3.Lerp(_me.transform.localScale, new Vector3(0.1f, 0.1f, 0.1f), Time.deltaTime/2);
    }

    public override void LateExecute()
    {
        base.LateExecute();
    }

    public override void Sleep()
    {
        base.Sleep();
    }
}
