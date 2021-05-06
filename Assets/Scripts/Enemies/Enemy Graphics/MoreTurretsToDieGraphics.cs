using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redd096;

[AddComponentMenu("Cube Invaders/Enemy Graphics/More Turrets To Die Graphics")]
public class MoreTurretsToDieGraphics : EnemyGraphics
{
    [Header("Line Renderer")]
    [SerializeField] bool useLine = false;
    [CanShow("useLine")] [SerializeField] LineRenderer linePrefab = default;

    [Header("Outline")]
    [SerializeField] bool useOutline = true;
    [CanShow("useOutline")] [SerializeField] Outline[] outlineObjects = default;

    EnemyMoreTurretsToDie logic;
    LineRenderer lineFeedback;

    void Start()
    {
        //get logic reference
        logic = GetComponent<EnemyMoreTurretsToDie>();

        //instantiate line feedback
        lineFeedback = Instantiate(linePrefab, transform);
    }

    void Update()
    {
        //set line feedback
        if (useLine)
            SetPositions();

        //set outline feedback
        if (useOutline)
            SetOutlines();
    }

    void SetPositions()
    {
        if (lineFeedback && logic.turretsAiming != null)
        {
            //get position of every turret
            List<Vector3> positions = new List<Vector3>();
            foreach (Turret t in logic.turretsAiming)
            {
                positions.Add(t.GetComponent<TurretGraphics>().LinePosition.position);
                positions.Add(transform.position);  //add also this position, so every line go from a turret to this enemy
            }

            //set positions
            lineFeedback.positionCount = positions.Count;
            lineFeedback.SetPositions(positions.ToArray());
        }
    }

    void SetOutlines()
    {
        //deactive outline when a turrets is aiming this enemy
        for (int i = 0; i < outlineObjects.Length; i++)
        {
            //hide if aimed
            bool hide = logic.turretsAiming != null && logic.turretsAiming.Length > i;

            if(outlineObjects[i].enabled != !hide)
                outlineObjects[i].enabled = !hide;
        }
    }
}
