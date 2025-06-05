using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Hamburger", menuName = "Scriptable Objects/Hamburger")]
public class HamburgerData : ScriptableObject
{
    public List<int> ingredientList = new List<int>();
}