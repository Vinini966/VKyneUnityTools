using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace VkyneTools.Core
{
    public abstract class InputManager<T> : Singleton<InputManager<T>> where T : IInputActionCollection2, new()
    {
        float StickDeadZone;

        T ActionMap;

        UnityEngine.Events.UnityEvent OnControlsChanged;

        // Start is called before the first frame update
        protected override void Awake()
        {
            base.Awake();
            ActionMap = new T();
        }

        public abstract void EnableControls();

        public abstract void DisableControls();
    }
}
