using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using System;

public class KeyBinder : MonoBehaviour
{
    public InputActionReference actionReference; // La acción a rebindear
    public Text bindingText; // El texto que muestra la tecla actual
    public Button rebindButton;

    private void Start()
    {
        UpdateBindingDisplay();
        rebindButton.onClick.AddListener(StartRebind);
    }

    void UpdateBindingDisplay()
    {
        // Muestra la tecla actual (primera binding)
        bindingText.text = actionReference.action.GetBindingDisplayString();
    }

    public void StartRebind()
    {
        rebindButton.interactable = false;

        actionReference.action.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse") // opcional, excluye ratón
            .OnComplete(operation => {
                operation.Dispose();
                UpdateBindingDisplay();
                rebindButton.interactable = true;
            })
            .Start();
    }
}
