using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusManager : MonoBehaviour
{
    public List<GameObject> bonusList;

    void Start()
    {
        GameEvent.ShowBonusScreen += ShowBonusScreen;
    }

    private void OnDisable()
    {
        GameEvent.ShowBonusScreen -= ShowBonusScreen;
    }

    private void ShowBonusScreen(Config.SquareColor color)
    {
        GameObject obj = null;
        foreach (var bonus in bonusList)
        {
            var bonusComp = bonus.GetComponent<Bonus>();
            if (bonusComp.color == color)
            {
                obj = bonus; // Corrected: Store the found bonus
                bonus.SetActive(true);
            }
        }

        if (obj != null) // Ensure obj is not null before starting coroutine
        {
            StartCoroutine(DeactivateBonus(obj));
        }
        else
        {
            Debug.LogWarning("No matching bonus found, DeactivateBonus not started.");
        }
    }

    private IEnumerator DeactivateBonus(GameObject obj)
    {
        yield return new WaitForSeconds(2);
        obj.SetActive(false);
    }
}
