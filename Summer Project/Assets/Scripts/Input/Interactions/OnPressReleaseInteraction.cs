using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif

public class OnPressReleaseInteraction : IInputInteraction
{
    public float pressTime = 0.5f;

    public void Process(ref InputInteractionContext context)
    {
        if (context.isStarted)
        {
            context.Performed();
        }

        if (context.ControlIsActuated())
        {
            context.Started();
            context.SetTimeout(pressTime);
        }
    }

    public void Reset()
    {

    }

    static OnPressReleaseInteraction()
    {
        InputSystem.RegisterInteraction<OnPressReleaseInteraction>();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {

    }
}
