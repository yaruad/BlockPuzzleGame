using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPausePanelAnimation : MonoBehaviour
{
    [SerializeField]
    private GameObject imageBackgroundOverlay;
    [SerializeField]
    private Animator animator;

    public void OnAppear()
    {
        imageBackgroundOverlay.SetActive(true);
        gameObject.SetActive(true);

        animator.SetTrigger("onAppear");
    }

    public void OnDisAppear()
    {
        animator.SetTrigger("onDisappear");
    }

    public void EndOfDisappear()
    {
        imageBackgroundOverlay?.SetActive(false);
        gameObject.SetActive(false);
    }
}
