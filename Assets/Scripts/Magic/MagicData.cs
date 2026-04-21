using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum magic_Type 
{ 
    None,
    Element,
    Word
}

[Serializable]
public class MagicData
{
    public string magicName;
    public string Icon_Path;   
    public magic_Type type;
    public float magicCost;
}

[Serializable]
public class MagicListWrapper
{
    public System.Collections.Generic.List<MagicData> magic;
}
