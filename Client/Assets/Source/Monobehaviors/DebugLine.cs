using UnityEngine;
using UnityEngine.UI;

public class DebugLine : MonoBehaviour
{
    private enum FadeState
    {
        Showing,
        Fading
    }

    private const float DISPLAY_TIME = 5f;
    private const float DISPLAY_FADE_TIME = 2f;

    public Text TextField;
    private FadeState fadeState;
    private float timeLeft;

    public void Initialize(string message)
    {
        fadeState = FadeState.Showing;
        timeLeft = DISPLAY_TIME;
        TextField.text = message;
    }

    void Update()
    {
        timeLeft -= Time.deltaTime;
        if (fadeState == FadeState.Fading)
        {
            Color currentColor = TextField.color;
            currentColor.a = timeLeft / DISPLAY_FADE_TIME;
            TextField.color = currentColor;
        }

        if (timeLeft <= 0f)
        {
            if (fadeState == FadeState.Showing)
            {
                fadeState = FadeState.Fading;
                timeLeft = DISPLAY_FADE_TIME;
            }
            else if (fadeState == FadeState.Fading)
            {
                GameObject.Destroy(gameObject);
            }
        }
    }
}
