using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlAI : MonoBehaviour {

    private Parade Parade { get; set; }


    void Start() {
        Parade = this.GetComponent<Parade>();

        StartCoroutine(RandomAI());
    }


    IEnumerator RandomAI()
    {
        //running start
        //Parade.inputMode();Parade.inputAction();
        yield return new WaitForSeconds(3);

        while (true)
        {
            var press = Random.Range(0, 6);
            switch(press)
            {
                case 0: Parade.inputUp(); break;
                case 1: Parade.inputLeft(); break;
                case 2: Parade.inputDown(); break;
                case 3: Parade.inputRight(); break;
                case 4: Parade.inputAction(); break;
                case 5: Parade.inputMode(); break;
            }
            yield return new WaitForSeconds(Random.Range(1, 3));
        }
    }

}
