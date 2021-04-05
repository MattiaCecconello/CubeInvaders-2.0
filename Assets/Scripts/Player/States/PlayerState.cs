using UnityEngine;
using redd096;

public class PlayerState : State
{
    protected Player player;
    protected Transform transform;

    public PlayerState(StateMachine stateMachine) : base(stateMachine)
    {
        //get references
        player = stateMachine as Player;
        transform = player.transform;
    }
}