using System.Collections.Generic;
using UnityEngine;

public class EnableRandomObject : MonoBehaviour
{
    public List<GameObject> Objects;

    void Start()
    {
        int randIndex = Random.Range(0, Objects.Count);
        Objects[randIndex].SetActive(true);
    }
}
