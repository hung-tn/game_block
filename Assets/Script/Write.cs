using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Write : MonoBehaviour
{
    public List<GameObject> writing;
    // Start is called before the first frame update
    void Start()
    {
        GameEvent.ShowWriting += ShowWriting;
       
    }
    private void OnDisable()
    {
        GameEvent.ShowWriting -= ShowWriting;
    }

    private void ShowWriting()
    {
        var index = UnityEngine.Random.Range(0, writing.Count);
        writing[index].SetActive(true);
    }

}
