using UnityEngine;

public class Timer : MonoBehaviour
{
    public float MaxTime { get; set; }
    public float CurrentTime { get; set; } = 0;
    public bool Autostart { get; set; }
    public bool Repeatable { get; set; }

    public System.Action onTimeout;

    private float timeScaleAdjust;

    public static Timer CreateTimer(GameObject obj, System.Action onTimeout_, float maxTime_, bool autostart_ = false, bool repeatable_ = true, float timeScaleAdjust_ = 1)
    {
        Timer timer = obj.AddComponent<Timer>();
        timer.onTimeout = onTimeout_;
        timer.MaxTime = maxTime_;
        timer.Autostart = autostart_;
        timer.Repeatable = repeatable_;

        timer.enabled = timer.Autostart;
        timer.timeScaleAdjust = timeScaleAdjust_;
        return timer;
    }

    private void Update()
    {
        Tick();
    }

    private void Tick()
    {
        CurrentTime += Time.deltaTime / timeScaleAdjust;
        if (CurrentTime >= MaxTime)
        {
            onTimeout();

            if (Repeatable)
            {
                Reset();
            }
            else
            {
                Destroy(this);
            }
        }
    }

    public void Reset()
    {
        CurrentTime = 0;
        enabled = Autostart;
    }

    public void Start()
    {
        enabled = true;
    }

    public void Stop()
    {
        enabled = false;
    }
}
