﻿using UnityEngine;
using redd096;
using UnityEngine.UI;

#region save class

[System.Serializable]
public class OptionsSave
{
    public float volume;
    public float mouseX;
    public float mouseY;
    public float gamepadX;
    public float gamepadY;
    public bool invertY;

    public OptionsSave(float volume, float mouseX, float mouseY, float gamepadX, float gamepadY, bool invertY)
    {
        this.volume = volume;
        this.mouseX = mouseX;
        this.mouseY = mouseY;
        this.gamepadX = gamepadX;
        this.gamepadY = gamepadY;
        this.invertY = invertY;
    }
}

#endregion

[AddComponentMenu("Cube Invaders/Manager/Options Manager")]
public class OptionsManager : MonoBehaviour
{
    [Header("Default Values")]
    [SerializeField] float volume = 1;
    [SerializeField] float mouseX = 0.1f;
    [SerializeField] float mouseY = 0.002f;
    [SerializeField] float gamepadX = 150;
    [SerializeField] float gamepadY = 1;
    [SerializeField] bool invertY = false;

    [Header("Volume")]
    [SerializeField] Slider volumeSlider = default;
    [SerializeField] float multiplierVolume = 0.1f;

    [Header("Mouse X")]
    [SerializeField] Slider mouseXSlider = default;
    [SerializeField] float multiplierMouseX = 0.025f;

    [Header("Mouse Y")]
    [SerializeField] Slider mouseYSlider = default;
    [SerializeField] float multiplierMouseY = 0.00025f;

    [Header("Gamepad X")]
    [SerializeField] Slider gamepadXSlider = default;
    [SerializeField] float multiplierGamepadX = 0.1f;

    [Header("Gamepad Y")]
    [SerializeField] Slider gamepadYSlider = default;
    [SerializeField] float multiplierGamepadY = 0.1f;

    [Header("Invert Y")]
    [SerializeField] Toggle invertYToggle = default;

    OptionsSave loaded;

    void Start()
    {
        //load class - if no save, create first one
        loaded = SaveLoadJSON.Load<OptionsSave>("Options");
        if (loaded == null)
        {
            loaded = new OptionsSave(volume, mouseX, mouseY, gamepadX, gamepadY, invertY);
            SaveLoadJSON.Save("Options", loaded);
        }

        //and set default sliders
        SetSliders();
    }

    #region private API

    void SetSliders()
    {
        //set default sliders
        if (volumeSlider)
            volumeSlider.value = loaded.volume / multiplierVolume;

        if (mouseXSlider)
            mouseXSlider.value = loaded.mouseX / multiplierMouseX;

        if (mouseYSlider)
            mouseYSlider.value = loaded.mouseY / multiplierMouseY;

        if (gamepadXSlider)
            gamepadXSlider.value = loaded.gamepadX / multiplierGamepadX;

        if (gamepadYSlider)
            gamepadYSlider.value = loaded.gamepadY / multiplierGamepadY;

        if (invertYToggle)
            invertYToggle.isOn = loaded.invertY;
    }

    void SetEverythingInGame()
    {
        //set in game
        AudioListener.volume = loaded.volume;

        if (GameManager.instance && GameManager.instance.player)
            GameManager.instance.player.SetOptionsValue(loaded);
    }

    #endregion

    #region public API

    public void SetVolume(float value)
    {
        //update options
        loaded.volume = value * multiplierVolume;

        //set in game
        AudioListener.volume = loaded.volume;

        //save
        SaveLoadJSON.Save("Options", loaded);
    }

    public void SetMouseX(float value)
    {
        //update options
        loaded.mouseX = value * multiplierMouseX;

        //set in game
        if (GameManager.instance && GameManager.instance.player)
            GameManager.instance.player.SetOptionsValue(loaded);

        //save
        SaveLoadJSON.Save("Options", loaded);
    }

    public void SetMouseY(float value)
    {
        //update options
        loaded.mouseY = value * multiplierMouseY;

        //set in game
        if (GameManager.instance && GameManager.instance.player)
            GameManager.instance.player.SetOptionsValue(loaded);

        //save
        SaveLoadJSON.Save("Options", loaded);
    }

    public void SetGamepadX(float value)
    {
        //update options
        loaded.gamepadX = value * multiplierGamepadX;

        //set in game
        if (GameManager.instance && GameManager.instance.player)
            GameManager.instance.player.SetOptionsValue(loaded);

        //save
        SaveLoadJSON.Save("Options", loaded);
    }

    public void SetGamepadY(float value)
    {
        //update options
        loaded.gamepadY = value * multiplierGamepadY;

        //set in game
        if (GameManager.instance && GameManager.instance.player)
            GameManager.instance.player.SetOptionsValue(loaded);

        //save
        SaveLoadJSON.Save("Options", loaded);
    }

    public void SetInvertY(bool value)
    {
        //update options
        loaded.invertY = value;

        //set in game
        if (GameManager.instance && GameManager.instance.player)
            GameManager.instance.player.SetOptionsValue(loaded);

        //save
        SaveLoadJSON.Save("Options", loaded);
    }

    public void ResetOptions()
    {
        //reset options
        loaded = new OptionsSave(volume, mouseX, mouseY, gamepadX, gamepadY, invertY);

        //set in game
        SetEverythingInGame();

        //set sliders
        SetSliders();
    }

    #endregion
}