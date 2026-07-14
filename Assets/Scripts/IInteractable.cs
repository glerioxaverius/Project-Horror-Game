using UnityEngine;
            
public interface IInteractable
{
    /// <param name="interactor">The game object that is interacting with the object.</param>
    void Interact(GameObject interactor);
}