using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Turret : BuildableObject
{
    [Header("Turret Modifier")]
    [Tooltip("Number of generators necessary to activate (0 = no generator)")] [Min(0)] [SerializeField] int needGenerator = 1;
    [Tooltip("Timer to destroy if player doesn't move it (0 = no destroy)")] [Min(0)] [SerializeField] float timeBeforeDestroy = 5;

    public override void TryActivateTurret()
    {
        //if doesn't need generator or there are enough generators around, activate it
        if (NeedGenerator() == false || CheckGeneratorsAround() >= needGenerator)
            base.TryActivateTurret();
    }

    public override void TryDeactivateTurret()
    {
        //if need generator and there is no enough generators around, deactive it
        if(NeedGenerator() && CheckGeneratorsAround() < needGenerator)
            base.TryDeactivateTurret();
    }

    public override void BuildTurret(Cell cellOwner)
    {
        base.BuildTurret(cellOwner);

        //if destroy turret when no move and time greater than 0, init timer to destroy turret
        if (GameManager.instance.levelManager.levelConfig.DestroyTurretWhenNoMove && timeBeforeDestroy > 0)
            InitTimer();

        //if there is a limit of turrets on same face, check if there are other turrets on same face
        if (GameManager.instance.levelManager.levelConfig.LimitOfTurretsOnSameFace > 0)
            InitCheck();
    }

    public override void RemoveTurret()
    {
        base.RemoveTurret();

        //be sure to remove timer to destroy turret if no move
        if(destroyTurretWhenNoMove != null)
            RemoveTimer();

        //be sure to remove destroy turrets on same face
        if(destroyTurretsOnSameFace != null)
            RemoveCheck();
    }

    #region generator

    bool NeedGenerator()
    {
        return GameManager.instance.levelManager.levelConfig.TurretsNeedGenerator && needGenerator > 0;
    }

    int CheckGeneratorsAround()
    {
        int generatorsOnThisFace = 0;

        //foreach cell on this face
        if (GameManager.instance.levelManager.levelConfig.GeneratorActiveAllFace)
        {
            foreach (Cell cell in GameManager.instance.world.Cells.Values.Where(x => x.coordinates.face == CellOwner.coordinates.face))
            {
                //if there is a turret, is a generator and is active
                if (cell.turret != null && cell.turret is Generator && cell.turret.IsActive)
                    generatorsOnThisFace++;
            }
        }
        //else foreach cell around
        else
        {
            foreach (Cell cell in GameManager.instance.world.GetCellsAround(CellOwner.coordinates))
            {
                //if there is a turret, is a generator and is active
                if (cell.turret != null && cell.turret is Generator && cell.turret.IsActive)
                    generatorsOnThisFace++;
            }
        }

        return generatorsOnThisFace;
    }

    #endregion

    #region timer before destroy

    DestroyTurretWhenNoMove destroyTurretWhenNoMove = null;

    public System.Action<float> updateTimeBeforeDestroy;

    void InitTimer()
    {
        destroyTurretWhenNoMove = new DestroyTurretWhenNoMove();
        destroyTurretWhenNoMove.InitTimer(this, timeBeforeDestroy);
    }

    void RemoveTimer()
    {
        destroyTurretWhenNoMove.RemoveTimer();
    }

    #endregion

    #region no turrets on same face

    DestroyTurretsOnSameFace destroyTurretsOnSameFace = null;

    public System.Action<float> updateFeedbackTurretsOnSameFace;

    void InitCheck()
    {
        destroyTurretsOnSameFace = new DestroyTurretsOnSameFace();
        destroyTurretsOnSameFace.Init(this);
    }

    void RemoveCheck()
    {
        destroyTurretsOnSameFace.Remove();
    }

    #endregion
}
