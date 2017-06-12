using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCamera : MonoBehaviour
{
    public Material material;

    private bool InTutorial = false;

    private float binkDeltaSec = 0.5f;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, material);
    } 

    public void Tutorial(bool open)
    {
        float trigger = open ? 1 : 0;
        material.SetFloat("_Trigger", trigger);

        InTutorial = open;

        if (InTutorial)
        {
            StartCoroutine(Blink());
        }
    }

    private IEnumerator Blink()
    {
        while (InTutorial)
        {
            float blink = material.GetFloat("_Blink");
            blink = blink == 1 ? 0 : 1;
            material.SetFloat("_Blink", blink);
            yield return new WaitForSeconds(binkDeltaSec);
        }
    }
}
