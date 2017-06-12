using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorailController : MonoBehaviour
{
    public static TutorailController Instance = null;
    TutorailController()
    {
        Instance = this;
    }

    [System.NonSerialized]
    public bool InTutorail = true;

    [SerializeField]
    private TutorialCamera Camera_Tutorail;

    [SerializeField]
    private float Tutorail_sec;

    void Start()
    {
        if(Tutorail_sec >= 0)
        {
            StartCoroutine(BeginTutorial());
        }
    }

    public IEnumerator BeginTutorial()
    {
        InTutorail = true;
        Camera_Tutorail.Tutorial(true);
        yield return new WaitForSeconds(Tutorail_sec);
        Camera_Tutorail.Tutorial(false);
        InTutorail = false;
    }
}
