using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Rnd = UnityEngine.Random;
using KModkit;
using System.Text.RegularExpressions;

public class DissipatingCubeScript : MonoBehaviour
{

    public KMBombInfo BombInfo;
    public KMAudio Audio;
    public KMBombModule Module;
    public Material[] cubeMats;
    public MeshRenderer[] cubes;
    public GameObject cubeParent;
    public KMSelectable cubeSelectable;

    private static int _moduleIdCounter = 1;
    private int _moduleId;
    private bool _moduleSolved;
    private readonly List<Quaternion> points = new List<Quaternion>();
    private readonly List<float> weights = new List<float>();

    private void Start()
    {
        _moduleId = _moduleIdCounter++;
        for (int i = 0; i < cubes.Length; i++)
        {
            int rand = Rnd.Range(0, cubeMats.Length);
            cubes[i].material = cubeMats[rand];
        }
        cubeSelectable.OnInteract += () =>
        {
            if (!_moduleSolved)
            {
                StartCoroutine(RemoveCubes());
                Module.HandlePass();
                _moduleSolved = true;
            }
            return false;
        };
    }

    private IEnumerator RemoveCubes()
    {
        var cubeShuffle = Enumerable.Range(0, cubes.Length).ToArray().Shuffle();
        for (int i = 0; i < cubeShuffle.Length; i++)
        {
            cubes[cubeShuffle[i]].gameObject.SetActive(false);
            yield return new WaitForSeconds(.01f);
        }
    }

    private void FixedUpdate()
    {
        Quaternion calc = cubeParent.transform.localRotation;
        List<int> toRemove = new List<int>();
        for (int i = 0; i < points.Count; i++)
        {
            calc = Quaternion.Lerp(calc, points[i], weights[i]);
            weights[i] += (1) * Time.fixedDeltaTime;
            if (weights[i] > 1f)
                toRemove.Add(i);
        }
        foreach (int i in toRemove.OrderByDescending(c => c))
        {
            weights.RemoveAt(i);
            points.RemoveAt(i);
        }

        cubeParent.transform.localRotation = calc;

        if (weights.Count < 2)
        {
            points.Add(Rnd.rotation);
            weights.Add(0f);
        }

        cubeParent.transform.localEulerAngles = new Vector3(cubeParent.transform.localEulerAngles.x, cubeParent.transform.localEulerAngles.y, cubeParent.transform.localEulerAngles.z);
    }
}