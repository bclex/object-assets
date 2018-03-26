using System.Collections.Generic;
using UltimaXNA.Core.Input;
using UnityEngine;

namespace OA.Ultima.Core.Patterns.MVC
{
    /// <summary>
    /// Abstract Controller - receives input, interacts with state of model.
    /// </summary>
    public abstract class AController
    {
        protected AModel Model;

        public AController(AModel parent_model)
        {
            Model = parent_model;
        }

        public virtual void ReceiveKeyboardInput(List<InputEventKeyboard> events)
        {
        }

        public virtual void ReceiveMouseInput(Vector2Int MousePosition, List<InputEventMouse> events)
        {
        }
    }
}
