using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunManager : MonoBehaviour
{
    public GameObject[] Guns;
    public int ActiveGun = 0;


    void Awake()
    {
        SwapGun(ActiveGun);
    }
    

    public void OnFire(InputAction.CallbackContext context)
    {
        if(context.started)       Guns[ActiveGun].GetComponent<Gun>().FireStarted();
        else if(context.canceled) Guns[ActiveGun].GetComponent<Gun>().FireEnded();
    }

    public void OnAltFire(InputAction.CallbackContext context)
    {
        if(context.started)       Guns[ActiveGun].GetComponent<Gun>().AltFireStarted();
        else if(context.canceled) Guns[ActiveGun].GetComponent<Gun>().AltFireEnded();
    }

    public void OnScroll(InputAction.CallbackContext context)
    {
        float inputScroll = context.ReadValue<float>();
        if(inputScroll != 0)
        {
            ActiveGun -= (int)inputScroll;

            if(ActiveGun < 0)             ActiveGun = Guns.Length-1;
            if(ActiveGun > Guns.Length-1) 
            {
                ActiveGun = 0;
            }

            if(Guns.Length > 1) SwapGun(ActiveGun);
        }
    }

    public void SwapGun(int GunChoice)
    {
        Debug.Log("Swapping to: " + Guns[GunChoice]);
        for(int i = 0; i < Guns.Length; i++)
        {
            Guns[i].SetActive(false);
            if(i == GunChoice) Guns[i].SetActive(true);
        }
    }
}