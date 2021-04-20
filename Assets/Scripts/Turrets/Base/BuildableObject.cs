using System.Collections;
using UnityEngine;

public class BuildableObject : MonoBehaviour
{
    public Cell CellOwner { get; private set; }
    public bool IsPreview { get; private set; } = true;     //is a preview turret

    bool isActive;                                          //is active (shot and spawn shield)
    public bool IsActive
    {
        get
        {
            //if not preview and is active, return true - else, return false
            return !IsPreview && isActive;
        }
    }

    public System.Action onBuildTurret;

    #region on world rotate

    protected virtual void OnWorldRotate(Coordinates coordinates)
    {
        //use cellOwner.onWorldRotate to know when start to rotate
        GameManager.instance.world.onEndRotation += OnEndRotation;

        //deactivate it
        DeactivateTurret();

        GameManager.instance.turretsManager.RemoveTurretFromDictionary(this);  //remove from dictionary
    }

    protected virtual void OnEndRotation()
    {
        //use World.onEndRotation to know when stop to rotate
        GameManager.instance.world.onEndRotation -= OnEndRotation;

        //try activate it
        TryActivateTurret();

        GameManager.instance.turretsManager.AddTurretToDictionary(this);  //add to dictionary
    }

    #endregion

    #region public API

    public virtual void ActivateTurret()
    {
        isActive = true;
    }

    public virtual void DeactivateTurret()
    {
        isActive = false;
    }

    public virtual void BuildTurret(Cell cellOwner)
    {
        IsPreview = false;

        //get owner and set event
        this.CellOwner = cellOwner;
        cellOwner.onWorldRotate += OnWorldRotate;

        //try activate it
        TryActivateTurret();

        GameManager.instance.turretsManager.AddTurretToDictionary(this);  //add to dictionary

        onBuildTurret?.Invoke();
    }

    public virtual void RemoveTurret()
    {
        IsPreview = true;

        //deactive and remove event
        gameObject.SetActive(false);
        CellOwner.onWorldRotate -= OnWorldRotate;

        //deactive it
        DeactivateTurret();

        GameManager.instance.turretsManager.RemoveTurretFromDictionary(this);  //remove from dictionary
    }

    public virtual void TryActivateTurret()
    {
        ActivateTurret();
    }

    public virtual void TryDeactivateTurret()
    {
        DeactivateTurret();
    }

    #endregion

    #region deactivate effect

    public System.Action<float> onDeactivateStart;
    public float TimerObjectDeactivated { get; private set; }    //timer setted by enemies effect, to deactivate this object
    Coroutine deactiveCoroutine;

    public void Deactivate(float durationEffect)
    {
        //called by enemies effect, deactive until this time
        TimerObjectDeactivated = Time.time + durationEffect;

        //call event
        onDeactivateStart?.Invoke(durationEffect);

        //start coroutine
        if (deactiveCoroutine != null)
            StopCoroutine(deactiveCoroutine);

        deactiveCoroutine = StartCoroutine(DeactiveCoroutine());
    }

    IEnumerator DeactiveCoroutine()
    {
        //deactive turret
        DeactivateTurret();

        //wait
        while (Time.time < TimerObjectDeactivated)
            yield return null;

        //reactive
        ActivateTurret();
    }

    #endregion
}
