using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Rnd = UnityEngine.Random;
using KModkit;
using System.Text.RegularExpressions;

public class BallDownFunnel : MonoBehaviour
{

    public KMBombInfo BombInfo;
    public KMAudio Audio;
    public KMBombModule Module;

    private static int _moduleIdCounter = 1;
    private int _moduleId;
    private bool _moduleSolved;
    private float rand;

    private Coroutine moveBall;
    private bool ballMoving = false;

    public KMSelectable funnel;
    public GameObject BallCenter;
    public GameObject BallObject;
    
    void Start()
    {
        _moduleId = _moduleIdCounter++;
        rand = Rnd.Range(0f, 360f);
        BallCenter.transform.localEulerAngles = new Vector3(0f, rand, 0f);
        funnel.OnInteract += delegate ()
        {
            if (!ballMoving)
            {
                moveBall = StartCoroutine(MoveBall());
                ballMoving = true;
                Audio.PlaySoundAtTransform("MarbleSound", transform);
            }
            return false;
        };
    }

    IEnumerator MoveBall()
    {
        var duration = 12.0f;
        var elapsed = 0.0f;
        while (elapsed < duration)
        {
            BallCenter.transform.localEulerAngles = new Vector3(0f, Easing.InQuad(elapsed, rand + 0f, rand + 2880f, duration), 0f);
            BallObject.transform.localEulerAngles = new Vector3(Easing.InQuad(elapsed, 0f, -2880f, duration), 0f, 0f);
            if (elapsed < 10.5f)
            {
                BallObject.transform.localPosition = new Vector3(Easing.InQuad(elapsed, 0.06f, 0f, 10.5f), Easing.InQuad(elapsed, 0f, -0.015f, 10.5f), 0f);
                yield return null;
            }
            else
            {
                BallObject.transform.localPosition = new Vector3(Easing.InQuad(elapsed - 10.5f, 0f, 0f, 1.5f), Easing.OutQuad(elapsed - 10.5f, -0.015f, -0.07f, 1.5f), 0f);
                yield return null;
            }
            elapsed += Time.deltaTime;
        }
        BallObject.transform.localPosition = new Vector3(0f, -0.04f, 0f);
        Module.HandlePass();
    }
}