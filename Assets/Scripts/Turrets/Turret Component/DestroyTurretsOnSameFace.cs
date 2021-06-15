public class DestroyTurretsOnSameFace
{
    Turret turret;

    //on build turret, init this script
    //try to start timer on this face

    //on rotate, try stop timer for this face
    //on end rotation try start timer on new face

    //on remove turret, try stop timer for this face

    //during timer, update feedback
    //on end timer, destroy turret

    public void Init(Turret turret)
    {
        //get references
        this.turret = turret;

        //add events
        AddEvents();

        //try start timer on this face
        GameManager.instance.turretsManager.TryStartTimer(turret, turret.CellOwner.coordinates.face);
    }

    public void Remove()
    {
        //try stop timer on this face
        GameManager.instance.turretsManager.TryStopTimer(turret, turret.CellOwner.coordinates.face);

        //remove events
        RemoveEvents();
    }

    #region events

    void AddEvents()
    {
        if (turret)
        {
            turret.CellOwner.onWorldRotate += OnWorldRotate;
            turret.onEndRotation += OnEndRotation;
        }
    }

    void RemoveEvents()
    {
        if (turret)
        {
            turret.CellOwner.onWorldRotate -= OnWorldRotate;
            turret.onEndRotation -= OnEndRotation;
        }
    }

    void OnWorldRotate(Coordinates coordinates)
    {
        //try stop timer on this face
        GameManager.instance.turretsManager.TryStopTimer(turret, coordinates.face);
    }

    void OnEndRotation()
    {
        //try start timer on this face
        GameManager.instance.turretsManager.TryStartTimer(turret, turret.CellOwner.coordinates.face);
    }

    #endregion
}
