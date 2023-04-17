public class PlayEmoteEvent
{
    public string[] SpawnObjects;
    public string SpawnMessage;

    public PlayEmoteEvent(string[] spawnObjects, string spawnMessage)
    {
        SpawnObjects = spawnObjects;
        SpawnMessage = spawnMessage;
    }
}
