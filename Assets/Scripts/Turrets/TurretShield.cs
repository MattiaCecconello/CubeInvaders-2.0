using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
[AddComponentMenu("Cube Invaders/Turret/Turret Shield")]
[RequireComponent(typeof(TurretGraphics))]
public class TurretShield : Turret
{
    [Header("Important")]
    [SerializeField] Shield shield = default;

    static System.Action<EFace> onTurretExitQueue;
    static Dictionary<EFace, List<TurretShield>> shieldsQueue = new Dictionary<EFace, List<TurretShield>>();

    bool shieldIsBroken { get { return shield.CurrentHealth <= 0; } }

    bool isRotating;

    /*
    V quando viene istanziata si crea lo scudo e si resetta (size a zero e vita massima dello scudo), perché ancora la torretta è una preview, non è attiva
    V e ci si registra agli eventi

    V quando si attiva, si aggiunge alla coda
    V prova ad attivare lo scudo (controlla se è la prossima in coda e non ha lo scudo rotto)

    V quando si disattiva, resetta lo scudo nel caso si riattivasse la torretta
    V e si toglie dalla coda

    V a fine ondata, ricarica la vita dello scudo
    V prova ad attivarlo (nel caso si fosse rotto)

    V quando ruota, si setta che sta ruotando, per non fargli checkare l'evento OnTurretExitQueue
    V disattiva lo scudo
    V ed esce dalla coda
    
    V quando finisce di ruotare, si mette nella nuova coda
    V check se il primo della coda può attivare lo scudo, altrimenti lo rimpiazziamo
    V check se attivare lo scudo
    V i check scritti sopra vanno fatti solo se la torretta è ancora attiva (non è stata disattivata da una skill o dall'assenza di un generatore)
    V e si resetta che la torretta non sta più ruotando, quindi controlla se qualcuno esce dalla coda

    V quando una torretta esce dalla coda, check se attivare lo scudo
    V il check va fatto solo se non stiamo ruotando (altrimenti questa torretta è uscita dalla coda)
    V il check va fatto solo se siamo attivi (altrimenti questa torretta è uscita quando è stata venduta, o è disattivata da una skill o dall'assenza di un generatore)

    V quando viene distrutto lo scudo, viene spostato in fondo alla coda
    V disattiva lo scudo
    V lo scudo non potrà più essere utilizzato per questa wave - by default cause when try to activate, check if is broken (current health <= 0) -
    */

    void Start()
    {
        //create shield
        InstantiateShield();

        //set events
        AddEvents();
    }

    private void OnDestroy()
    {
        //remove events
        RemoveEvents();
    }

    #region private API

    #region override

    public override void ActivateTurret()
    {
        base.ActivateTurret();

        //do only if not rotating
        if (isRotating == false)
        {
            //add to queue
            AddToQueue();

            //try activate shield
            TryActivateShield();
        }
    }

    public override void DeactivateTurret()
    {
        base.DeactivateTurret();

        //do only if not rotating
        if (isRotating == false)
        {
            //reset shield for when this turret will be reactivate (immediatly because the turret will be deactivated too)
            shield.ResetShieldSize();

            //remove from queue
            RemoveFromQueue(CellOwner.coordinates);
        }
    }

    protected override void OnWorldRotate(Coordinates coordinates)
    {
        //set isRotating
        isRotating = true;

        base.OnWorldRotate(coordinates);

        //deactivate the shield and remove from queue
        shield.DeactivateShield();
        RemoveFromQueue(coordinates);
    }

    protected override void OnEndRotation()
    {
        base.OnEndRotation();

        if (IsActive)
        {
            //add to queue and try to replace the first in the queue
            AddToQueue();
            TryReplaceFirstInQueue();

            //try activate the shield
            TryActivateShield();
        }

        //set isRotating
        isRotating = false;
    }

    #endregion

    #region events

    void AddEvents()
    {
        GameManager.instance.levelManager.onEndAssaultPhase += OnEndAssaultPhase;
        TurretShield.onTurretExitQueue += OnTurretExitQueue;
        shield.onShieldDestroyed += OnShieldDestroyed;
    }

    void RemoveEvents()
    {
        GameManager.instance.levelManager.onEndAssaultPhase -= OnEndAssaultPhase;
        TurretShield.onTurretExitQueue -= OnTurretExitQueue;
        shield.onShieldDestroyed -= OnShieldDestroyed;
    }

    void OnEndAssaultPhase()
    {
        //regen health
        shield.RegenHealth();

        //try activate shield (if shield was broken)
        TryActivateShield();
    }

    void OnTurretExitQueue(EFace face)
    {
        //don't check if is not active (is only a preview) or is rotating
        if (IsActive == false || isRotating)
            return;

        //if a turret quit from this face queue
        if (face == CellOwner.coordinates.face)
        {
            //try activate the shield
            TryActivateShield();
        }
    }

    void OnShieldDestroyed()
    {
        //move to the end of the queue
        RemoveFromQueue(CellOwner.coordinates);
        AddToQueue();

        //deactivate shield
        shield.DeactivateShield();
    }

    #endregion

    #region queue

    void AddToQueue()
    {
        //create queue if necessary
        if (shieldsQueue.ContainsKey(CellOwner.coordinates.face) == false)
        {
            shieldsQueue.Add(CellOwner.coordinates.face, new List<TurretShield>());
        }

        //add to queue
        shieldsQueue[CellOwner.coordinates.face].Add(this);
    }

    void RemoveFromQueue(Coordinates coordinates)
    {
        if (shieldsQueue.ContainsKey(coordinates.face))
        {
            //remove from queue
            shieldsQueue[coordinates.face].Remove(this);

            //call event
            onTurretExitQueue?.Invoke(coordinates.face);
        }
    }

    void TryReplaceFirstInQueue()
    {
        //can replace only if shield is not broken
        if (shieldIsBroken)
            return;

        //if the first in the queue has a broken shield
        if (shieldsQueue[CellOwner.coordinates.face][0].shieldIsBroken)
        {
            //replace first place in queue
            shieldsQueue[CellOwner.coordinates.face].Insert(0, this);
        }
    }

    #endregion

    #region general

    void InstantiateShield()
    {
        //reset shield
        shield.ResetShieldSize();
        shield.RegenHealth();
    }

    void TryActivateShield()
    {
        //check if can activate, and active it
        if (CheckActivateShield())
            shield.ActivateShield(CellOwner.coordinates);
    }

    bool CheckActivateShield()
    {
        //if the shield is not broken and is the first in the queue
        return shieldIsBroken == false && shieldsQueue[CellOwner.coordinates.face][0] == this;
    }

    #endregion

    #endregion
}
