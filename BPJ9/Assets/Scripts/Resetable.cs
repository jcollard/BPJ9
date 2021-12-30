using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resetable : MonoBehaviour
{
    public GameObject StartState;
    public GameObject CurrentState;
    void Start()
    {
        StartState.SetActive(false);
    }

    public void Reset()
    {
        if (CurrentState != null)
        {
            UnityEngine.Object.Destroy(CurrentState);
            CurrentState = null;
        }

        CurrentState = UnityEngine.Object.Instantiate(StartState);
        CurrentState.transform.SetParent(StartState.transform.parent);
        CurrentState.transform.position = StartState.transform.position;
        CurrentState.SetActive(true);
    }
}
