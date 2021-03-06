// GENERATED AUTOMATICALLY FROM 'Assets/Resources/Commands/New Controls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @NewControls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @NewControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""New Controls"",
    ""maps"": [
        {
            ""name"": ""Gameplay"",
            ""id"": ""059141be-8b2e-48e5-8c5b-53400a67e353"",
            ""actions"": [
                {
                    ""name"": ""Move Camera"",
                    ""type"": ""Value"",
                    ""id"": ""e8108198-7494-4f84-93e8-38212912acb7"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Select Cell"",
                    ""type"": ""Value"",
                    ""id"": ""fdf70380-251b-489e-aa36-36735ca5c329"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Rotate Cube"",
                    ""type"": ""Value"",
                    ""id"": ""2fba47cb-7338-4f85-9879-e92a0a11d236"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Build Turret"",
                    ""type"": ""Button"",
                    ""id"": ""d49b9540-00cb-4687-9971-3f2dbc167029"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Confirm Turret"",
                    ""type"": ""Button"",
                    ""id"": ""bc501957-de23-41a7-b901-c3553d72c3fe"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Deny Turret"",
                    ""type"": ""Button"",
                    ""id"": ""7e677dfa-8a2f-4f25-8b8b-445559629174"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Keep Pressed To Rotate"",
                    ""type"": ""Button"",
                    ""id"": ""38f67a80-e7cc-4852-b560-acc8283e22ff"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Finish Strategic Phase"",
                    ""type"": ""Button"",
                    ""id"": ""d49597fb-621a-4064-b6dd-c471812b228b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Skip Animation"",
                    ""type"": ""Button"",
                    ""id"": ""4deb37c9-5419-4ca2-9d3c-3ff50dbc186f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Pause Button"",
                    ""type"": ""Button"",
                    ""id"": ""1bbcb3ed-dfe3-4f7a-88df-91e62a18cee8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Resume Button"",
                    ""type"": ""Button"",
                    ""id"": ""499b133a-f75b-478d-a8f3-54fd7f2cdcde"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""0673e61d-5393-49c0-9ce9-c8b584969c5a"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Move Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""990c21a6-b6db-4242-a6b4-fca200590972"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Move Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""44d17102-d8a3-400a-9d43-e1ce5e503014"",
                    ""path"": ""<Gamepad>/dpad"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Select Cell"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a0d94fd8-bd5e-4181-bc3f-e9415967568d"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Select Cell"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""WASD [Keyboard]"",
                    ""id"": ""d1a7e581-73c1-4194-95ae-6cdf17ea03c3"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Select Cell"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""6390ef09-49f9-43f6-90ba-70bef0b14b9f"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Select Cell"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""11ae181e-1b55-4128-813a-7d9a01f80c43"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Select Cell"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""b51a735e-5e3e-4b01-90fa-2cd542c08291"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Select Cell"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""8335e0c0-347f-4d5b-854d-becee47be883"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Select Cell"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Arrows [Keyboard]"",
                    ""id"": ""4ebf0725-2589-4fc1-a9c2-f0410d638760"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Select Cell"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""70985ab5-0211-4bf1-a4fe-1a4b97da92b8"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Select Cell"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""2de66b15-b338-4ec2-80e7-5e4d351e989c"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Select Cell"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""807bfdd2-729d-4a92-949f-f45a81082c93"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Select Cell"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""7453abf9-7bfb-4e98-8bec-10d73ceccb0d"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Select Cell"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""6c9faaa3-a48a-473c-8ab0-b752a2d07dd5"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Build Turret"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a39400f0-246f-4175-93f6-f63d1b566407"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Build Turret"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cbb173ed-3fda-4f1a-bf5c-b5383bc135fb"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Confirm Turret"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""79edeb7e-6ce5-4022-aa58-d8f5076ce74a"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Confirm Turret"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""40feca89-9048-4729-92c3-1cf39195ff7b"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Deny Turret"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""88fa2f3f-5b40-43dd-bd08-0ea2f460abad"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Deny Turret"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a0fade67-9ee2-4776-9c38-871c0e0d6452"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Keep Pressed To Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""be0cf2bb-2771-4c8f-8349-4fd85261f1ec"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Keep Pressed To Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7ba40110-ac4f-4253-aebe-b6cf5cf6cd49"",
                    ""path"": ""<Gamepad>/dpad"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Rotate Cube"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""317ecbed-ae54-4e88-a73b-031769a12db4"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Rotate Cube"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""WASD [Keyboard]"",
                    ""id"": ""f6d4ab7d-ad2d-4a2e-bcb1-56987c5a8892"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate Cube"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""d8ab3525-ad6e-44d2-86f9-3c89338ed199"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Rotate Cube"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""474bedbb-7009-4b27-a9e6-a92365d6c38b"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Rotate Cube"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""e8fb15e7-c8ea-4977-829b-b687c01633c2"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Rotate Cube"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""c9816aad-78fb-4cda-9015-62a812799de9"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Rotate Cube"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Arrows [Keyboard]"",
                    ""id"": ""e305915c-a063-4c3e-9075-7abc2eb79b91"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate Cube"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""ef278a45-e54b-4eb2-8f9d-3fe96842c706"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Rotate Cube"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""2a09ed7b-a394-498e-9f1d-dbbc80f9b7f7"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Rotate Cube"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""7c6985da-cbae-405d-aed1-96c3707e65ac"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Rotate Cube"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""4b47ab10-673d-4041-aa82-a140c87e38e1"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Rotate Cube"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""24f9b5df-5a38-4463-9037-8c080c8cf83a"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Finish Strategic Phase"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""aaa6ebcd-9684-4262-a242-842640367ed9"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Finish Strategic Phase"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""909ceaba-8d38-448b-835f-7b4155191f1e"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Finish Strategic Phase"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5ff6d71a-f38e-4363-b88f-a2afc3923684"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Pause Button"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a715840d-dad9-4560-857e-97b48c592b5d"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Pause Button"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""171b1cc3-5f15-4455-8b35-d7ad8b79ad15"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Resume Button"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1f5acfda-f502-4565-bff6-35f0ac741ef6"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Resume Button"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""931f51b9-3c4b-4156-9152-654926a415ae"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Resume Button"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ca089a15-07b9-4071-9df6-5d1de8f9d534"",
                    ""path"": ""<Keyboard>/anyKey"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Skip Animation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""KeyboardAndMouse"",
            ""bindingGroup"": ""KeyboardAndMouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Gameplay
        m_Gameplay = asset.FindActionMap("Gameplay", throwIfNotFound: true);
        m_Gameplay_MoveCamera = m_Gameplay.FindAction("Move Camera", throwIfNotFound: true);
        m_Gameplay_SelectCell = m_Gameplay.FindAction("Select Cell", throwIfNotFound: true);
        m_Gameplay_RotateCube = m_Gameplay.FindAction("Rotate Cube", throwIfNotFound: true);
        m_Gameplay_BuildTurret = m_Gameplay.FindAction("Build Turret", throwIfNotFound: true);
        m_Gameplay_ConfirmTurret = m_Gameplay.FindAction("Confirm Turret", throwIfNotFound: true);
        m_Gameplay_DenyTurret = m_Gameplay.FindAction("Deny Turret", throwIfNotFound: true);
        m_Gameplay_KeepPressedToRotate = m_Gameplay.FindAction("Keep Pressed To Rotate", throwIfNotFound: true);
        m_Gameplay_FinishStrategicPhase = m_Gameplay.FindAction("Finish Strategic Phase", throwIfNotFound: true);
        m_Gameplay_SkipAnimation = m_Gameplay.FindAction("Skip Animation", throwIfNotFound: true);
        m_Gameplay_PauseButton = m_Gameplay.FindAction("Pause Button", throwIfNotFound: true);
        m_Gameplay_ResumeButton = m_Gameplay.FindAction("Resume Button", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Gameplay
    private readonly InputActionMap m_Gameplay;
    private IGameplayActions m_GameplayActionsCallbackInterface;
    private readonly InputAction m_Gameplay_MoveCamera;
    private readonly InputAction m_Gameplay_SelectCell;
    private readonly InputAction m_Gameplay_RotateCube;
    private readonly InputAction m_Gameplay_BuildTurret;
    private readonly InputAction m_Gameplay_ConfirmTurret;
    private readonly InputAction m_Gameplay_DenyTurret;
    private readonly InputAction m_Gameplay_KeepPressedToRotate;
    private readonly InputAction m_Gameplay_FinishStrategicPhase;
    private readonly InputAction m_Gameplay_SkipAnimation;
    private readonly InputAction m_Gameplay_PauseButton;
    private readonly InputAction m_Gameplay_ResumeButton;
    public struct GameplayActions
    {
        private @NewControls m_Wrapper;
        public GameplayActions(@NewControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @MoveCamera => m_Wrapper.m_Gameplay_MoveCamera;
        public InputAction @SelectCell => m_Wrapper.m_Gameplay_SelectCell;
        public InputAction @RotateCube => m_Wrapper.m_Gameplay_RotateCube;
        public InputAction @BuildTurret => m_Wrapper.m_Gameplay_BuildTurret;
        public InputAction @ConfirmTurret => m_Wrapper.m_Gameplay_ConfirmTurret;
        public InputAction @DenyTurret => m_Wrapper.m_Gameplay_DenyTurret;
        public InputAction @KeepPressedToRotate => m_Wrapper.m_Gameplay_KeepPressedToRotate;
        public InputAction @FinishStrategicPhase => m_Wrapper.m_Gameplay_FinishStrategicPhase;
        public InputAction @SkipAnimation => m_Wrapper.m_Gameplay_SkipAnimation;
        public InputAction @PauseButton => m_Wrapper.m_Gameplay_PauseButton;
        public InputAction @ResumeButton => m_Wrapper.m_Gameplay_ResumeButton;
        public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
        public void SetCallbacks(IGameplayActions instance)
        {
            if (m_Wrapper.m_GameplayActionsCallbackInterface != null)
            {
                @MoveCamera.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMoveCamera;
                @MoveCamera.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMoveCamera;
                @MoveCamera.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMoveCamera;
                @SelectCell.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSelectCell;
                @SelectCell.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSelectCell;
                @SelectCell.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSelectCell;
                @RotateCube.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRotateCube;
                @RotateCube.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRotateCube;
                @RotateCube.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRotateCube;
                @BuildTurret.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnBuildTurret;
                @BuildTurret.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnBuildTurret;
                @BuildTurret.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnBuildTurret;
                @ConfirmTurret.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnConfirmTurret;
                @ConfirmTurret.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnConfirmTurret;
                @ConfirmTurret.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnConfirmTurret;
                @DenyTurret.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnDenyTurret;
                @DenyTurret.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnDenyTurret;
                @DenyTurret.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnDenyTurret;
                @KeepPressedToRotate.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnKeepPressedToRotate;
                @KeepPressedToRotate.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnKeepPressedToRotate;
                @KeepPressedToRotate.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnKeepPressedToRotate;
                @FinishStrategicPhase.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnFinishStrategicPhase;
                @FinishStrategicPhase.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnFinishStrategicPhase;
                @FinishStrategicPhase.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnFinishStrategicPhase;
                @SkipAnimation.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSkipAnimation;
                @SkipAnimation.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSkipAnimation;
                @SkipAnimation.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSkipAnimation;
                @PauseButton.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPauseButton;
                @PauseButton.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPauseButton;
                @PauseButton.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPauseButton;
                @ResumeButton.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnResumeButton;
                @ResumeButton.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnResumeButton;
                @ResumeButton.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnResumeButton;
            }
            m_Wrapper.m_GameplayActionsCallbackInterface = instance;
            if (instance != null)
            {
                @MoveCamera.started += instance.OnMoveCamera;
                @MoveCamera.performed += instance.OnMoveCamera;
                @MoveCamera.canceled += instance.OnMoveCamera;
                @SelectCell.started += instance.OnSelectCell;
                @SelectCell.performed += instance.OnSelectCell;
                @SelectCell.canceled += instance.OnSelectCell;
                @RotateCube.started += instance.OnRotateCube;
                @RotateCube.performed += instance.OnRotateCube;
                @RotateCube.canceled += instance.OnRotateCube;
                @BuildTurret.started += instance.OnBuildTurret;
                @BuildTurret.performed += instance.OnBuildTurret;
                @BuildTurret.canceled += instance.OnBuildTurret;
                @ConfirmTurret.started += instance.OnConfirmTurret;
                @ConfirmTurret.performed += instance.OnConfirmTurret;
                @ConfirmTurret.canceled += instance.OnConfirmTurret;
                @DenyTurret.started += instance.OnDenyTurret;
                @DenyTurret.performed += instance.OnDenyTurret;
                @DenyTurret.canceled += instance.OnDenyTurret;
                @KeepPressedToRotate.started += instance.OnKeepPressedToRotate;
                @KeepPressedToRotate.performed += instance.OnKeepPressedToRotate;
                @KeepPressedToRotate.canceled += instance.OnKeepPressedToRotate;
                @FinishStrategicPhase.started += instance.OnFinishStrategicPhase;
                @FinishStrategicPhase.performed += instance.OnFinishStrategicPhase;
                @FinishStrategicPhase.canceled += instance.OnFinishStrategicPhase;
                @SkipAnimation.started += instance.OnSkipAnimation;
                @SkipAnimation.performed += instance.OnSkipAnimation;
                @SkipAnimation.canceled += instance.OnSkipAnimation;
                @PauseButton.started += instance.OnPauseButton;
                @PauseButton.performed += instance.OnPauseButton;
                @PauseButton.canceled += instance.OnPauseButton;
                @ResumeButton.started += instance.OnResumeButton;
                @ResumeButton.performed += instance.OnResumeButton;
                @ResumeButton.canceled += instance.OnResumeButton;
            }
        }
    }
    public GameplayActions @Gameplay => new GameplayActions(this);
    private int m_GamepadSchemeIndex = -1;
    public InputControlScheme GamepadScheme
    {
        get
        {
            if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
            return asset.controlSchemes[m_GamepadSchemeIndex];
        }
    }
    private int m_KeyboardAndMouseSchemeIndex = -1;
    public InputControlScheme KeyboardAndMouseScheme
    {
        get
        {
            if (m_KeyboardAndMouseSchemeIndex == -1) m_KeyboardAndMouseSchemeIndex = asset.FindControlSchemeIndex("KeyboardAndMouse");
            return asset.controlSchemes[m_KeyboardAndMouseSchemeIndex];
        }
    }
    public interface IGameplayActions
    {
        void OnMoveCamera(InputAction.CallbackContext context);
        void OnSelectCell(InputAction.CallbackContext context);
        void OnRotateCube(InputAction.CallbackContext context);
        void OnBuildTurret(InputAction.CallbackContext context);
        void OnConfirmTurret(InputAction.CallbackContext context);
        void OnDenyTurret(InputAction.CallbackContext context);
        void OnKeepPressedToRotate(InputAction.CallbackContext context);
        void OnFinishStrategicPhase(InputAction.CallbackContext context);
        void OnSkipAnimation(InputAction.CallbackContext context);
        void OnPauseButton(InputAction.CallbackContext context);
        void OnResumeButton(InputAction.CallbackContext context);
    }
}
