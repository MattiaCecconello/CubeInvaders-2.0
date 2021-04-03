﻿using System.Collections;
using UnityEngine;

public class WorldRandomRotator : WorldRotator
{
    #region variables

    bool waitRotation;
    Coroutine randomizeWorld_Coroutine;

    public WorldRandomRotator(World world) : base(world)
    {
    }

    #endregion

    public void StartRandomize()
    {
        //start randomize
        if (randomizeWorld_Coroutine != null)
            world.StopCoroutine(randomizeWorld_Coroutine);

        if(world.gameObject.activeInHierarchy)
            randomizeWorld_Coroutine = world.StartCoroutine(RandomizeWorld());
    }

    IEnumerator RandomizeWorld()
    {
        //wait before randomize
        yield return new WaitForSeconds(world.randomWorldConfig.TimeBeforeRandomize);

        //for n times, rotate row or column
        for (int i = 0; i < world.randomWorldConfig.RandomizeTimes; i++)
        {
            //randomize rotation
            EFace face = (EFace)Random.Range(0, 6);
            int x = Random.Range(0, world.worldConfig.NumberCells);
            int y = Random.Range(0, world.worldConfig.NumberCells);
            ERotateDirection randomDirection = (ERotateDirection)Random.Range(0, 4);

            //effective rotation
            Rotate(new Coordinates(face, x, y), EFace.front, randomDirection, world.randomWorldConfig.RotationTime);

            //wait until the end of the rotation
            OnStartRotation();
            yield return new WaitWhile(() => waitRotation);

            //if not last rotation, wait time between every rotation
            yield return new WaitForSeconds(world.randomWorldConfig.TimeBetweenRotation);

            //repeat
            if (world.randomWorldConfig.Loop)
                i = 0;
        }

        //call start game
        GameManager.instance.levelManager.StartGame();
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

    protected override float GetAnimationCurveValue(float delta)
    {
        //use random rotation animation curve
        return world.randomWorldConfig.RotationAnimationCurve.Evaluate(delta);
    }

    #endregion
}
