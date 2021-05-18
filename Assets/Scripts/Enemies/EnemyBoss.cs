using UnityEngine;
using redd096;

[SelectionBase]
[AddComponentMenu("Cube Invaders/Enemy/Enemy Boss")]
[RequireComponent(typeof(EnemyBossGraphics))]
public class EnemyBoss : Enemy
{
    [Header("Important - Is this the Last Phase?")]
    [SerializeField] bool lastPhaseBoss = false;

    [Header("Boss")]
    [CanShow("lastPhaseBoss", NOT = true)] [SerializeField] string sceneToLoad = "Show21";
    [CanShow("lastPhaseBoss", NOT = true)] [SerializeField] float timeBeforeLoadNewScene = 4;

    public bool LastPhaseBoss => lastPhaseBoss;

    public override void Die<T>(T hittedBy)
    {
        if (StillAlive)
        {
            //do only if this is not last phase
            if (lastPhaseBoss == false)
            {
                //save if achievement in this level
                GameManager.instance.levelManager.SaveEndBossLevel();

                //load new scene
                SceneLoader.instance.StartCoroutine(SceneLoader.instance.LoadSceneAfterSeconds(sceneToLoad, timeBeforeLoadNewScene));
            }

            base.Die(hittedBy);
        }
    }
}
