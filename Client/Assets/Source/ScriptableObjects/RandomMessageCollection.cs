using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/RandomMessageCollection", order = 1)]
public class RandomMessageCollection : ScriptableObject
{
    public List<string> MessagePool;

    public string GetRandomMessage()
    {
        int randomIndex = Random.Range(0, MessagePool.Count);
        return MessagePool[randomIndex];
    }
}
