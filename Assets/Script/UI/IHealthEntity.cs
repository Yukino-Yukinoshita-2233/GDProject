using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealthEntity
{
    float MaxHealth { get; }
    float CurrentHealth { get; }
}
