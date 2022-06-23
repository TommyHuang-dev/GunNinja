using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrappleIndicator : MonoBehaviour
{
    GameObject gameObj;
    GrappleHook gh;
    public CanvasGroup canvasGroup;

    bool grappleHookFound;

    // Start is called before the first frame update
    void Start()
    {
        gameObj = GameObject.FindGameObjectWithTag("Player");
        if (!gameObj.TryGetComponent<GrappleHook>(out gh))
        {
            grappleHookFound = false;
        }
        grappleHookFound = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (grappleHookFound && gh.grappleReady)
        {
            Show();
        } else
        {
            Hide();
        }
    }

    void Hide()
    {
        canvasGroup.alpha = 0f; //this makes everything transparent
        canvasGroup.blocksRaycasts = false; //this prevents the UI element to receive input events
    }

    void Show()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }
}
