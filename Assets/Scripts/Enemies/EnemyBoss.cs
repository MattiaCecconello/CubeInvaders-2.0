using UnityEngine;
using redd096;

[SelectionBase]
[AddComponentMenu("Cube Invaders/Enemy/Enemy Boss")]
[RequireComponent(typeof(EnemyBossGraphics))]
public class EnemyBoss : Enemy
{
    [Header("Boss")]
    [SerializeField] string sceneToLoad = "Show21";
    [SerializeField] float timeBeforeLoadNewScene = 4;

    public override void Die<T>(T hittedBy)
    {
        if (StillAlive)
        {
            //load new scene
            SceneLoader.instance.StartCoroutine(SceneLoader.instance.LoadSceneAfterSeconds(sceneToLoad, timeBeforeLoadNewScene));

            base.Die(hittedBy);
        }
    }
}
