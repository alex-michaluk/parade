using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParadeRobots : Parade {

    protected override GameObject GetCharacter(int row, int col, int personNumber, out System.Type type)
    {
        type = typeof(Techie);
        return (Util.Robot);
    }
}
