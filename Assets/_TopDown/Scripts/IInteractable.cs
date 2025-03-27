using System.Xml.Linq;
using UnityEngine;

public interface IInteractable
{
    void Interact();
    string GetInteractionPrompt();
    string GetName();
}
