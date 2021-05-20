using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Cube Invaders/Turret Graphics/Generator Graphics")]
public class GeneratorGraphics : MonoBehaviour
{
    [Header("Color Generator")]
    [SerializeField] Color colorWhenActive = Color.red;
    [SerializeField] Renderer[] objectsToColor = default;

    Generator generator;
    bool isActivatingTurrets;

    Dictionary<Renderer, Color> normalColors = new Dictionary<Renderer, Color>();

    void Awake()
    {
        //get references
        generator = GetComponent<Generator>();

        //save normal colors
        foreach(Renderer r in objectsToColor)
        {
            normalColors.Add(r, r.material.color);
        }
    }

    void FixedUpdate()
    {
        //if was activating and now stop, or viceversa - color generator
        if (isActivatingTurrets != IsCurrentlyActivatingTurrets())
        {
            isActivatingTurrets = !isActivatingTurrets;
            ColorGenerator();
        }
    }

    bool IsCurrentlyActivatingTurrets()
    {
        bool isCurrentlyActivatingTurrets = false;

        //if there is at least a turret on this face, is activating turrets
        foreach (Turret turret in GameManager.instance.turretsManager.TurretsOnFace(generator.CellOwner.coordinates.face))
        {
            if (turret != null)
            {
                isCurrentlyActivatingTurrets = true;
                break;
            }
        }

        return isCurrentlyActivatingTurrets;
    }

    void ColorGenerator()
    {
        //color every object - on active color or normal color
        foreach(Renderer r in normalColors.Keys)
        {
            r.material.color = isActivatingTurrets ? colorWhenActive : normalColors[r];
        }
    }
}
