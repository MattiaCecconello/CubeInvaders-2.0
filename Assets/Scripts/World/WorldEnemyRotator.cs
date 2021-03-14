using System.Collections;
using UnityEngine;

public class WorldEnemyRotator : WorldRotator
{
    #region variables

    int numberRotations;
    float rotationTime;

    bool waitRotation;
    Coroutine randomizeWorld_Coroutine;

    public WorldEnemyRotator(World world) : base(world)
    {
    }

    #endregion

    public void StartRandomize(int numberRotations, float rotationTime)
    {
        //set references
        this.numberRotations = numberRotations;
        this.rotationTime = rotationTime;

        //start randomize
        if (randomizeWorld_Coroutine != null)
            world.StopCoroutine(randomizeWorld_Coroutine);

        if (world.gameObject.activeInHierarchy)
            randomizeWorld_Coroutine = world.StartCoroutine(RandomizeWorld());
    }

    IEnumerator RandomizeWorld()
    {
        //for n times, rotate row or column
        for (int i = 0; i < numberRotations; i++)
        {
            //randomize rotation
            EFace face = (EFace)Random.Range(0, 6);
            int x = Random.Range(0, world.worldConfig.NumberCells);
            int y = Random.Range(0, world.worldConfig.NumberCells);
            ERotateDirection randomDirection = (ERotateDirection)Random.Range(0, 4);

            //effective rotation
            Rotate(new Coordinates(face, x, y), EFace.front, randomDirection);

            //wait until the end of the rotation
            OnStartRotation();
            yield return new WaitWhile(() => waitRotation);

            //wait next frame
            yield return null;
        }
    }

    void OnStartRotation()
    {
        //start wait rotation
        waitRotation = true;
        world.onEndRotation += OnEndRotation;
    }

    void OnEndRotation()
    {
        //end wait rotation
        waitRotation = false;
        world.onEndRotation -= OnEndRotation;
    }

    #region override world rotator

    protected override float GetRotationTime()
    {
        //use setted rotation time
        return rotationTime;
    }

    protected override bool SkipAnimation(float delta)
    {
        //can't skip random rotation
        return false;
    }

    #endregion
}
