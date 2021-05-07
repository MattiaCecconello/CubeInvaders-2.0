using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using redd096;

[AddComponentMenu("Cube Invaders/Enemy Graphics/Enemy Graphics")]
public class EnemyGraphics : MonoBehaviour
{
    [Header("Blink")]
    [SerializeField] GameObject objectToFlick = default;
    [SerializeField] Material blinkMaterial = default;
    [SerializeField] float blinkTime = 0.1f;

    [Header("VFX")]
    [SerializeField] ParticleSystem explosionParticlePrefab = default;
    [SerializeField] AudioStruct explosionSound = default;

    [Header("Radar things")]
    [SerializeField] Slider healthSlider = default;

    Enemy enemy;

    //for blink
    Dictionary<Renderer, Material> originalMaterials = new Dictionary<Renderer, Material>();
    Coroutine blink_Coroutine;

    void OnEnable()
    {
        enemy = GetComponent<Enemy>();
        if (enemy)
        {
            enemy.onGetDamage += OnGetDamage;
            enemy.onEnemyDeath += OnEnemyDeath;
            enemy.onShowHealth += OnShowHealth;
            enemy.onHideHealth += OnHideHealth;
        }

        //if there is not object to flick, select this object
        if (objectToFlick == null)
            objectToFlick = gameObject;

        //set original materials
        foreach(Renderer r in objectToFlick.GetComponentsInChildren<Renderer>())
        {
            if (originalMaterials.ContainsKey(r) == false)
                originalMaterials.Add(r, r.material);
        }

        //by default, hide health
        OnHideHealth();
    }

    void OnDisable()
    {
        if (enemy)
        {
            enemy.onGetDamage -= OnGetDamage;
            enemy.onEnemyDeath -= OnEnemyDeath;
            enemy.onShowHealth -= OnShowHealth;
            enemy.onHideHealth -= OnHideHealth;
        }
    }

    #region events

    void OnGetDamage(float currentHealth, float maxHealth)
    {
        //blink on get damage
        if (blink_Coroutine == null && gameObject.activeInHierarchy)
            blink_Coroutine = StartCoroutine(Blink_Coroutine());

        //update health slider
        if (healthSlider)
            healthSlider.value = currentHealth / maxHealth;
    }

    IEnumerator Blink_Coroutine()
    {
        //set blink materials
        foreach (Renderer r in originalMaterials.Keys)
            r.material = blinkMaterial;

        //wait
        yield return new WaitForSeconds(blinkTime);

        //back to original material
        foreach (Renderer r in originalMaterials.Keys)
            r.material = originalMaterials[r];

        blink_Coroutine = null;
    }

    void OnEnemyDeath(Enemy enemy)
    {
        //vfx and sound
        ParticlesManager.instance.Play(explosionParticlePrefab, transform.position, transform.rotation);
        SoundManager.instance.Play(explosionSound.audioClip, transform.position, explosionSound.volume);
    }

    void OnShowHealth()
    {
        //show health
        if (GameManager.instance.levelManager.generalConfig.showEnemiesHealth)
        {
            healthSlider?.gameObject.SetActive(true);
        }
    }

    void OnHideHealth()
    {
        //hide health
        healthSlider?.gameObject.SetActive(false);
    }

    #endregion
}
