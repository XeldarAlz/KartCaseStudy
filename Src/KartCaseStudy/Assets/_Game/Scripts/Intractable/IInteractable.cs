namespace Game.Interaction
{
    public interface IInteractable
    {
        InteractableType InteractType { get; set; }
        void ActivateHighlight();
        void DeactivateHighlight();
        void Interact();
    }
}