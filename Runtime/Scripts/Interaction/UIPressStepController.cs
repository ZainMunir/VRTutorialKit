using ECDA.VRTutorialKit;
using UnityEngine;
using UnityEngine.UIElements;

public class UIPressStepController : MonoBehaviour
{
    private UIFeedbackManager uiFeedbackManager;
    Button successButton;
    Button errorButton;


    void Start()
    {
        uiFeedbackManager = GetComponent<UIFeedbackManager>();

        uiFeedbackManager.RegisterButton("SuccessButton", AlwaysTrue);
        uiFeedbackManager.RegisterButton("ErrorButton", AlwaysFalse);

        VisualElement root = GetComponent<UIDocument>().rootVisualElement;


        successButton = root.Q<Button>("SuccessButton");
        errorButton = root.Q<Button>("ErrorButton");
    }

    void Update()
    {

    }

    bool AlwaysTrue()
    {
        return true;
    }

    bool AlwaysFalse()
    {
        return false;
    }
}
