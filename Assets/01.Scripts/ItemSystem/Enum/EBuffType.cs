using System;

[Flags]
public enum EBuffType
{
    NONE = 0,
    INVINCIBLE = 1,
    DASHING = 2,
    RANGEUP = 4,
    HEAVY = 8,
    DOUBLEDASH= 16,
}