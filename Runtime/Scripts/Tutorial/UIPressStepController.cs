using ECDA.VRTutorialKit;
using UnityEngine;
using UnityEngine.UIElements;

public class UIPressStepController : MonoBehaviour
{
    private UIFeedbackManager uiFeedbackManager;
    Button successButton;
    Button errorButton;
    Toggle toggle;
    [SerializeField] private TutorialSubStep tutorialSubStep;

    bool successClicked = false;
    bool errorClicked = false;
    bool toggleClicked = false;

    void OnEnable()
    {
        uiFeedbackManager = GetComponent<UIFeedbackManager>();

        uiFeedbackManager.RegisterButton("SuccessButton", AlwaysTrue);
        uiFeedbackManager.RegisterButton("ErrorButton", AlwaysFalse);

        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        successButton = root.Q<Button>("SuccessButton");
        errorButton = root.Q<Button>("ErrorButton");
        toggle = root.Q<Toggle>("ExampleToggle");

        if (toggle != null)
        {
            toggle.RegisterValueChangedCallback(evt => OnToggleChanged(evt.newValue));
        }
    }

    void OnDisable()
    {
        if (toggle != null)
        {
            toggle.UnregisterValueChangedCallback(evt => OnToggleChanged(evt.newValue));
        }
    }

    public void OnToggleChanged(bool value)
    {
        toggleClicked = true;
        CheckCompletion();
    }

    bool AlwaysTrue()
    {
        successClicked = true;
        CheckCompletion();
        return true;
    }

    bool AlwaysFalse()
    {
        errorClicked = true;
        CheckCompletion();
        return false;
    }

    void CheckCompletion()
    {
        if (successClicked && errorClicked && toggleClicked)
        {
            tutorialSubStep.Complete();
        }
    }
}
