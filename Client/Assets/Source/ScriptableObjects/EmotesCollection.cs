using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Emote
{
    [SerializeField]
    private List<GameObject> emoteResources;
    public string[] EmoteResources
    {
        get
        {
            int numEmoteResources = emoteResources.Count;
            string[] retList = new string[numEmoteResources];
            for (int i = 0; i < emoteResources.Count; ++i)
            {
                retList[i] = emoteResources[i].name;
            }
            return retList;
        }
    }
    public RandomMessageCollection Messages;
    public string ButtonText;
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/EmotesCollection", order = 1)]
public class EmotesCollection : ScriptableObject
{
    public List<Emote> Emotes;
}
