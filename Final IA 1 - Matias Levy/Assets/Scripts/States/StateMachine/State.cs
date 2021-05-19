using UnityEngine;
using System.Collections;

public class State {

    protected StateMachine mySM;

    /// <summary>
    /// Crea el estado.
    /// </summary>
    /// <param name="sm">Máquina de estados que va a recibir al estado.</param>
    public State(StateMachine sm)
    {
        mySM = sm;
    }

    /// <summary>
    /// Función que se llama cuando se entra al estado.
    /// </summary>
    public virtual void Awake() { }

    /// <summary>
    /// Función que se llama cuando se sale del estado.
    /// </summary>
    public virtual void Sleep() { }

    /// <summary>
    /// Función que se llama constantemente mientras se encuentre en el estado.
    /// </summary>
    public virtual void Execute() { }

    /// <summary>
    /// Función que se llama constantemente en el Late Udpate mientras se encuentre en el estado
    /// </summary>
    public virtual void LateExecute() { }
}
