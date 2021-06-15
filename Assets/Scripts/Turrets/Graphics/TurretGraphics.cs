using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Cube Invaders/Turret Graphics/Turret Graphics")]
public class TurretGraphics : BuildableGraphics
{
    Turret turret;

    [Header("Time Before Destroy - Turret Modifier")]
    [SerializeField] Transform objectToColor = default;
    [SerializeField] Color colorTimeBeforeDestroy = Color.red;

    Dictionary<Renderer, Color> normalColors = new Dictionary<Renderer, Color>();

    [Header("No Turrets On Same Face - Turret Modifier")]
    [SerializeField] Transform linePosition = default;

    public Transform LinePosition => linePosition;

    protected override void Awake()
    {
        base.Awake();

        //get logic component as turret
        turret = buildableObject as Turret;
    }

    void Start()
    {
        AddEvents();

        //set normalColors dictionary
        SetNormalColor();
    }

    protected override void OnDestroy()
    {
        RemoveEvents();
    }

    #region events

    protected virtual void AddEvents()
    {
        turret.updateTimeBeforeDestroy += SetColor;
    }

    protected virtual void RemoveEvents()
    {
        turret.updateTimeBeforeDestroy -= SetColor;
    }

    #endregion

    #region timer before destroy

    void SetNormalColor()
    {
        Renderer[] renderers = objectToColor.GetComponentsInChildren<Renderer>();

        //foreach renderer save normal color in dictionary
        foreach (Renderer renderer in renderers)
        {
            normalColors.Add(renderer, renderer.material.color);
        }
    }

    void SetColor(float delta)
    {
        //foreach renderer in dictionary, update color
        foreach (Renderer renderer in normalColors.Keys)
        {
            renderer.material.color = Color.Lerp(normalColors[renderer], colorTimeBeforeDestroy, delta);
        }
    }

    #endregion
}
