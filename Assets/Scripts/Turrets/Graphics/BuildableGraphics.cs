using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using redd096;

[AddComponentMenu("Cube Invaders/Turret Graphics/Buildable Graphics")]
public class BuildableGraphics : MonoBehaviour
{
    [Header("Important")]
    [SerializeField] Transform objectToRotate = default;
    [SerializeField] Transform baseToRotate = default;

    [Header("Cupola")]
    [SerializeField] GameObject cupolaObject = default;

    [Header("Height On Preview")]
    [SerializeField] float heightOnPreview = 0.5f;
    [SerializeField] float timeLerpOnBuild = 0.5f;

    [Header("Deactivate Turrets Effect")]
    [SerializeField] Color effectColor = Color.cyan;

    [Header("VFX")]
    [SerializeField] ParticleSystem buildVFX = default;
    [SerializeField] AudioStruct buildAudio = default;

    protected BuildableObject buildableObject;
    Dictionary<Renderer, Color> normalColors = new Dictionary<Renderer, Color>();

    Dictionary<Transform, Quaternion> defaultRotations = new Dictionary<Transform, Quaternion>();

    Coroutine lerpHeightCoroutine;

    protected virtual void Awake()
    {
        //set logic component
        buildableObject = GetComponent<BuildableObject>();

        //set normal colors
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            normalColors.Add(r, r.material.color);
        }

        if (buildableObject)
        {
            buildableObject.onDeactivateStart += OnDeactivateStart;
            buildableObject.onBuildTurret += OnBuildTurret;
            buildableObject.onShowPreview += OnShowPreview;
            buildableObject.onFinishFirstWave += OnFinishFirstWave;
        }

        //save default rotations
        SaveDefaultRotation(objectToRotate);
        SaveDefaultRotation(baseToRotate);
    }

    protected virtual void OnDestroy()
    {
        if (buildableObject)
        {
            buildableObject.onDeactivateStart -= OnDeactivateStart;
            buildableObject.onBuildTurret -= OnBuildTurret;
            buildableObject.onShowPreview -= OnShowPreview;
            buildableObject.onFinishFirstWave -= OnFinishFirstWave;
        }
    }

    protected virtual void Update()
    {
        //hide or show cupola
        HideShowCupola();

        //do nothing when preview mode
        if (buildableObject.IsPreview)
            return;

        //rotate
        if (baseToRotate)
            LookAtEnemy_TwoAxis();
        else
            LookAtEnemy();
    }

    protected virtual Enemy GetEnemy()
    {
        return null;
    }

    #region look at enemy

    void LookAtEnemy()
    {
        //do only if there is something to rotate
        if (objectToRotate == null)
            return;

        //find forward direction (from model to enemy)
        Vector3 forwardDirection;
        if (GetEnemy() && buildableObject.IsActive)
        {
            forwardDirection = (GetEnemy().transform.position - objectToRotate.position).normalized;
        }
        //else normal forward
        else
        {
            forwardDirection = transform.forward;
        }

        //set new rotation
        SetRotation(objectToRotate, forwardDirection);
    }

    void LookAtEnemy_TwoAxis()
    {
        //if active and there is an enemy, rotate towards enemy
        if (buildableObject.IsActive && GetEnemy())
        {
            RotateOnAxis(baseToRotate, transform.forward, baseToRotate.up);
            RotateOnAxis(objectToRotate, baseToRotate.right, objectToRotate.right);
        }
        //else look normal forward
        else
        {
            SetRotationToDefault(baseToRotate);
            SetRotationToDefault(objectToRotate);
        }
    }

    #endregion

    #region rotate transform

    void RotateOnAxis(Transform transformToRotate, Vector3 planeAxis, Vector3 rotateAxis)
    {
        if (transformToRotate == null)
            return;

        //project enemy and object position on same plane, then calculate direction
        Vector3 enemyPosition = Vector3.ProjectOnPlane(GetEnemy().transform.position, planeAxis);
        Vector3 transformPosition = Vector3.ProjectOnPlane(transformToRotate.position, planeAxis);
        Vector3 direction = (enemyPosition - transformPosition).normalized;

        //calculate angle (if angle is 0, stop rotation)
        float angle = Vector3.Angle(direction, transformToRotate.forward);
        if (angle == Mathf.Epsilon)
            return;

        //get rotation on axis 
        Quaternion rotation = Quaternion.AngleAxis(angle, rotateAxis) * transformToRotate.rotation;

        //if angle is greater, then angle must be negative
        if (Vector3.Angle(direction, rotation * Vector3.forward) > angle)
        {
            rotation = Quaternion.AngleAxis(-angle, rotateAxis) * transformToRotate.rotation;
        }

        //set rotation
        transformToRotate.rotation = rotation;
    }

    void SetRotation(Transform transformToRotate, Vector3 forwardDirection)
    {
        if (transformToRotate == null)
            return;

        //set new rotation
        Quaternion forwardRotation = Quaternion.FromToRotation(transformToRotate.forward, forwardDirection) * transformToRotate.rotation;
        transformToRotate.rotation = forwardRotation;
    }

    void SetRotationToDefault(Transform transformToRotate)
    {
        if (transformToRotate == null)
            return;

        //set default rotation
        transformToRotate.localRotation = defaultRotations[transformToRotate];
    }

    void SaveDefaultRotation(Transform transformToRotate)
    {
        //be sure is not already in dictionary
        if (transformToRotate == null || defaultRotations.ContainsKey(transformToRotate))
            return;

        //save default rotation
        defaultRotations.Add(transformToRotate, transformToRotate.localRotation);
    }

    #endregion

    #region deactivated effect

    Coroutine deactivateEffectCoroutine;

    void OnDeactivateStart(float durationEffect)
    {
        if (deactivateEffectCoroutine != null)
            StopCoroutine(deactivateEffectCoroutine);

        //start coroutine
        if(gameObject.activeInHierarchy)
            deactivateEffectCoroutine = StartCoroutine(DeactivateEffectCoroutine(durationEffect));
    }

    IEnumerator DeactivateEffectCoroutine(float durationEffect)
    {
        //while deactivated
        while(Time.time < buildableObject.TimerObjectDeactivated)
        {
            //from 1 to 0
            float delta = (buildableObject.TimerObjectDeactivated - Time.time) / durationEffect;

            //foreach renderer set color
            foreach (Renderer renderer in normalColors.Keys)
            {
                renderer.material.color = Color.Lerp(normalColors[renderer], effectColor, delta);
            }

            yield return null;
        }

        //on end deactivated, reset every color
        foreach (Renderer renderer in normalColors.Keys)
        {
            renderer.material.color = Color.Lerp(normalColors[renderer], effectColor, 0);
        }
    }

    #endregion

    #region on preview and on build turret - and on finish first wave

    void OnShowPreview()
    {
        //set position, rotation and size
        transform.localPosition = Vector3.forward * heightOnPreview;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    void OnBuildTurret()
    {
        //lerp to correct height - only if not must to be on air for this strategic phase
        if (GameManager.instance.levelManager.generalConfig.TurretsOnAirFirstStrategicPhase == false)
        {
            if (lerpHeightCoroutine != null)
                StopCoroutine(lerpHeightCoroutine);

            lerpHeightCoroutine = StartCoroutine(LerpHeightCoroutine());
        }

        //vfx and sound
        ParticlesManager.instance.Play(buildVFX, transform.position, transform.rotation);
        SoundManager.instance.Play(buildAudio.audioClip, transform.position, buildAudio.volume);
    }

    void OnFinishFirstWave()
    {
        //lerp to correct height - only if not already lerpes on build turret
        if (GameManager.instance.levelManager.generalConfig.TurretsOnAirFirstStrategicPhase)
        {
            if (lerpHeightCoroutine != null)
                StopCoroutine(lerpHeightCoroutine);

            lerpHeightCoroutine = StartCoroutine(LerpHeightCoroutine());
        }
    }

    IEnumerator LerpHeightCoroutine()
    {
        Vector3 startPosition = transform.localPosition;
        float delta = 0;

        while(delta < 1)
        {
            delta += Time.deltaTime / timeLerpOnBuild;

            //move to correct height position
            transform.localPosition = Vector3.Lerp(startPosition, Vector3.zero, delta);
            yield return null;
        }
    }

    #endregion

    #region cupola

    void HideShowCupola()
    {
        if (cupolaObject == null)
            return;

        //show cupola when buildableObject is active and hide when not active
        if (cupolaObject.activeInHierarchy != buildableObject.IsActive)
            cupolaObject.SetActive(!cupolaObject.activeInHierarchy);
    }

    #endregion
}
