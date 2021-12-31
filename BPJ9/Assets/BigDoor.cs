using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigDoor : MonoBehaviour
{
    public PressurePlate Plate0;
    public PressurePlate Plate1;
    public bool IsOpen;

    public HashSet<PressurePlate> plates = new HashSet<PressurePlate>();




    // Start is called before the first frame update
    void Start()
    {
        Plate0.OnTrigger.Add(TriggerDown);
        Plate1.OnTrigger.Add(TriggerDown);
        Plate0.OnUntrigger.Add(TriggerUp);
        Plate1.OnUntrigger.Add(TriggerUp);
    }

    public void TriggerDown(PressurePlate p)
    {
        plates.Add(p);
        if (plates.Count == 2)
        {
            SoundController.PlaySFX("Doors Close");
            this.gameObject.SetActive(false);
            IsOpen = true;
        }
    }

    public void TriggerUp(PressurePlate p)
    {
        plates.Remove(p);
        if (IsOpen)
        {
            SoundController.PlaySFX("Doors Close");
            this.gameObject.SetActive(true);
            IsOpen = true;
        }
        else
        {
            this.gameObject.SetActive(true);
            IsOpen = true;
        }
    }

    public void CheckDoor(PressurePlate plate)
    {

    }
}
