using redd096;

public class SelectCellPlayerTutorial : BasePlayerTutorialState
{
    Coordinates coordinatesToReach;
    int numberOfCells;

    bool reachCoordinates;
    bool reachNumberOfCells;

    public SelectCellPlayerTutorial(StateMachine stateMachine, Coordinates coordinatesToReach, int numberOfCells, bool reachCoordinates, bool reachNumberOfCells) : base(stateMachine)
    {
        //set references
        this.coordinatesToReach = coordinatesToReach;
        this.numberOfCells = numberOfCells;
        this.reachCoordinates = reachCoordinates;
        this.reachNumberOfCells = reachNumberOfCells;
    }

    protected override void DoSelectionCell(ERotateDirection direction)
    {
        base.DoSelectionCell(direction);

        //if reached coordinates
        if (reachCoordinates)
        {
            if (player.CurrentCoordinates == coordinatesToReach)
                FinishTutorial();
        }

        //if reached number of cells
        if (reachNumberOfCells)
        {
            //set movement between cells
            numberOfCells--;

            if (numberOfCells <= 0)
                FinishTutorial();
        }
    }
}
