using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Cube Invaders/Turret Graphics/Radar Graphics")]
public class RadarGraphics : BuildableGraphics
{
    [Header("Radar")]
    [SerializeField] Renderer[] objectsToFlick = default;

    [Header("Flick")]
    [SerializeField] Gradient flickColor = default;
    [SerializeField] float minFlick = 1;
    [SerializeField] float maxFlick = 10;
    [SerializeField] float timerBeforeChangeObject = 1;

    Radar radar;
    Dictionary<Renderer, Color> normalColors = new Dictionary<Renderer, Color>();

    Coroutine flickColorCoroutine;

    protected override void Awake()
    {
        base.Awake();

        //get logic component as radar
        radar = buildableObject as Radar;

        //set normal colors
        foreach(Renderer r in objectsToFlick)
        {
            normalColors.Add(r, r.material.color);
        }
    }

    void OnDisable()
    {
        //be sure to reset coroutine
        ResetCoroutine();
    }

    protected override void Update()
    {
        base.Update();

        //color radar
        if (buildableObject.IsActive)
        {
            ColorRadar();
        }
    }

    protected override Enemy GetEnemy()
    {
        //get enemy from logic component
        return radar.EnemyToAttack;
    }

    void ColorRadar()
    {
        //need renderer for flick
        if (objectsToFlick == null || objectsToFlick.Length <= 0) 
            return;

        //if enemy is attacking, flick color
        if(GetEnemy())
        {
            if (flickColorCoroutine == null)
                flickColorCoroutine = StartCoroutine(FlickColorCoroutine());
        }
        //else show normal color
        else
        {
            ResetCoroutine();
        }
    }

    IEnumerator FlickColorCoroutine()
    {
        while(true)
        {
            //cycle between objects to flick
            for(int i = 0; i < objectsToFlick.Length; i++)
            {
                //check timer
                float timer = 0;
                while(timer < timerBeforeChangeObject)
                {
                    int currentWave = GameManager.instance.waveManager.CurrentWave;
                    
                    //get flick speed based on enemy distance to its coordinates to attack
                    float distanceFrom1To0 = GetEnemy().DistanceFromCube / GameManager.instance.waveManager.waveConfig.Waves[currentWave].DistanceFromWorld;        //distance from 1 to 0
                    float flickSpeed = Mathf.Lerp(maxFlick, minFlick, distanceFrom1To0);                                                                            //speed from minFlick to maxFlick

                    timer += flickSpeed * Time.deltaTime;

                    //set color based on timer
                    Color color = flickColor.Evaluate(timer / timerBeforeChangeObject);

                    //change color
                    objectsToFlick[i].material.color = color;
                    objectsToFlick[i].material.SetColor("_EmissionColor", color);

                    yield return null;
                }

                //reset color
                objectsToFlick[i].material.color = normalColors[objectsToFlick[i]];
                objectsToFlick[i].material.SetColor("_EmissionColor", normalColors[objectsToFlick[i]]);
            }
        }
    }

    void ResetCoroutine()
    {
        if (flickColorCoroutine != null)
        {
            //stop coroutine
            if(gameObject.activeInHierarchy)
                StopCoroutine(flickColorCoroutine);

            //reset check
            flickColorCoroutine = null;

            //reset color
            foreach (Renderer r in normalColors.Keys)
            {
                r.material.color = normalColors[r];
                r.material.SetColor("_EmissionColor", normalColors[r]);
            }
        }
    }
}
