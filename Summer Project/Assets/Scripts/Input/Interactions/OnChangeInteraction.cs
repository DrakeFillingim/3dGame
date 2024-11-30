using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif

public class OnChangeInteraction : IInputInteraction
{
    public void Process(ref InputInteractionContext context)
    {
        context.Performed();
    }

    public void Reset()
    {

    }

    static OnChangeInteraction()
    {
        InputSystem.RegisterInteraction<OnChangeInteraction>();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {

    }
}
