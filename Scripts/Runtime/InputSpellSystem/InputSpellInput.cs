using System;
using UnityEngine;

public enum InputSpellInput
{
    [InspectorName("A | X | 1")]
    SOUTH = 0,
    
    [InspectorName("B | \u25CB | 2")]
    EAST = 1,
    
    [InspectorName("Y | \u25a1 | 3")]
    NORTH = 2,
    
    [InspectorName("X | \u25B3 | 4")]
    WEST = 3
}
