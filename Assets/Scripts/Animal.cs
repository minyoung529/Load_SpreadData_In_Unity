using UnityEngine;

// --------------------- TEST SCRIPT -----------------------------

[System.Serializable]
public class Animal
{
    public string name;
    public int attack;
    public float shield;
    public ColorType colorType;

    public virtual void Hello()
    {
        Debug.Log("Animal!");
    }
}

public class Rabbit : Animal
{
    public override void Hello()
    {
        Debug.Log("±øÃÑ±øÃÑ!");
    }
}

public class Tiger : Animal
{
    public override void Hello()
    {
        Debug.Log("¾îÈï!");
    }
}

public enum ColorType
{
    Pink,
    Orange,
    Blue
}