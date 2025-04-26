using System;

[Serializable]
public class CardInstance
{
    public CardData data;
    public int instanceId;
    private static int nextId = 1;

    public CardInstance(CardData data)
    {
        this.data = data;
        this.instanceId = nextId++;
    }
}
