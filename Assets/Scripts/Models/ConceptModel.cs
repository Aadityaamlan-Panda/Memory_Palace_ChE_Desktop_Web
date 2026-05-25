using System;

[Serializable]
public class ConceptModel
{
    public long id;
    public string title;
    public string description;
    public string mediaUrl;
    public string memoryObject;
    public string location;
    public string visualCue;

    public int strength;
    public int repetitions;
    public string lastReviewed;
    public string createdAt;
    public string updatedAt;
}