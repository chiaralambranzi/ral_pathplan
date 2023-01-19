// GENERATED AUTOMATICALLY FROM 'Assets/PlayerControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerControls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""Gameplay"",
            ""id"": ""bf30b2d0-e368-4289-9715-bd2189432710"",
            ""actions"": [
                {
                    ""name"": ""Move_xy"",
                    ""type"": ""PassThrough"",
                    ""id"": ""562d4c87-407d-4e0d-8c99-5cddff0c5773"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Move_z"",
                    ""type"": ""PassThrough"",
                    ""id"": ""08205ab5-9ee6-41f3-aa3d-d9118a748c42"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Rotate"",
                    ""type"": ""PassThrough"",
                    ""id"": ""69474bf9-c18a-4fca-b2ff-ba03a0aae517"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""a6237b02-a86b-4f87-8339-a09051d62136"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move_xy"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""95978b9c-f087-4722-b3b4-47536f2afbd9"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move_z"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""077c3480-32d9-4fdb-9ab1-e23955a7aa3b"",
                    ""path"": ""<DualShockGamepad>/dpad"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Gameplay
        m_Gameplay = asset.FindActionMap("Gameplay", throwIfNotFound: true);
        m_Gameplay_Move_xy = m_Gameplay.FindAction("Move_xy", throwIfNotFound: true);
        m_Gameplay_Move_z = m_Gameplay.FindAction("Move_z", throwIfNotFound: true);
        m_Gameplay_Rotate = m_Gameplay.FindAction("Rotate", throwIfNotFound: true);
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
    private readonly InputAction m_Gameplay_Move_xy;
    private readonly InputAction m_Gameplay_Move_z;
    private readonly InputAction m_Gameplay_Rotate;
    public struct GameplayActions
    {
        private @PlayerControls m_Wrapper;
        public GameplayActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move_xy => m_Wrapper.m_Gameplay_Move_xy;
        public InputAction @Move_z => m_Wrapper.m_Gameplay_Move_z;
        public InputAction @Rotate => m_Wrapper.m_Gameplay_Rotate;
        public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
        public void SetCallbacks(IGameplayActions instance)
        {
            if (m_Wrapper.m_GameplayActionsCallbackInterface != null)
            {
                @Move_xy.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove_xy;
                @Move_xy.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove_xy;
                @Move_xy.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove_xy;
                @Move_z.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove_z;
                @Move_z.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove_z;
                @Move_z.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove_z;
                @Rotate.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRotate;
                @Rotate.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRotate;
                @Rotate.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRotate;
            }
            m_Wrapper.m_GameplayActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move_xy.started += instance.OnMove_xy;
                @Move_xy.performed += instance.OnMove_xy;
                @Move_xy.canceled += instance.OnMove_xy;
                @Move_z.started += instance.OnMove_z;
                @Move_z.performed += instance.OnMove_z;
                @Move_z.canceled += instance.OnMove_z;
                @Rotate.started += instance.OnRotate;
                @Rotate.performed += instance.OnRotate;
                @Rotate.canceled += instance.OnRotate;
            }
        }
    }
    public GameplayActions @Gameplay => new GameplayActions(this);
    public interface IGameplayActions
    {
        void OnMove_xy(InputAction.CallbackContext context);
        void OnMove_z(InputAction.CallbackContext context);
        void OnRotate(InputAction.CallbackContext context);
    }
}
