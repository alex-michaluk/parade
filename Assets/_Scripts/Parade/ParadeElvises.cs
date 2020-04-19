using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParadeElvises : Parade{

    protected override GameObject GetCharacter (int row, int col, int personNumber,out System.Type type)
    {
        int i = Random.Range(0, 8);
        type = typeof(Kid);
        if (i == 5) type = typeof(Assassin);
        if (i == 6) type = typeof(Healer);
        if (i == 7) type = typeof(Wizard);
        return (Util.Elvis);
    }

}
