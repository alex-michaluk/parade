using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParadeKids : Parade {

    private int[] _randomKids = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };

    private new void Awake()
    {
        base.Awake();
        for (int t = 0; t < _randomKids.Length; t++)
        {
            int tmp = _randomKids[t];
            int r = Random.Range(t, _randomKids.Length);
            _randomKids[t] = _randomKids[r];
            _randomKids[r] = tmp;
        }
    }

    protected override GameObject GetCharacter(int row, int col, int personNumber, out System.Type type)
    {
        type = typeof(Kid); //default
        //space out specials
        int i = (Rows * Columns);
        if (personNumber == (int)i/4) type = typeof(Healer);
        if (personNumber == (int)i/2) type = typeof(Assassin);
        if (personNumber == (int)(i*.75)) type = typeof(Wizard);
        if (personNumber == (int)(i*.95f)) type = typeof(Techie);
        return (Util.Kids[_randomKids[personNumber % 8]]);
    }
}



