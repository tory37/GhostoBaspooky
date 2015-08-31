using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ButtonPressCycler : MonoBehaviour {


    public Sprite buttonUp;
    public Sprite buttonDown;

    public Image thisButtonImage;

    public float secondsBetweenCycle;

	void Start()
    {
        StartCoroutine(CycleUpAndDown());
    }

    IEnumerator CycleUpAndDown()
    {
        while (true)
        {
            if (thisButtonImage.sprite == buttonDown)
                thisButtonImage.sprite = buttonUp;
            else if (thisButtonImage.sprite == buttonUp)
                thisButtonImage.sprite = buttonDown;

            yield return new WaitForSeconds(secondsBetweenCycle);
        }
    }
}
