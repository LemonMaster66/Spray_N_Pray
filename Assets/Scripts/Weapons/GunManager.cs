using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunManager : MonoBehaviour
{
    public int ActiveGun = 0;

    public GameObject[] Guns;
    public bool[] HasGun;
    private int GunCount;


    void Awake()
    {
        if(HasGun[ActiveGun] != false) SwapGun(ActiveGun);
        else Guns[ActiveGun].SetActive(false);
    }
    

    public void OnFire(InputAction.CallbackContext context)
    {
        if(context.started)       Guns[ActiveGun].GetComponent<Gun>().ShootStart();
        else if(context.canceled) Guns[ActiveGun].GetComponent<Gun>().ShootEnd();
    }

    public void OnAltFire(InputAction.CallbackContext context)
    {
        if(context.started)       Guns[ActiveGun].GetComponent<Gun>().AltShootStart();
        else if(context.canceled) Guns[ActiveGun].GetComponent<Gun>().AltShootEnd();
    }

    public void OnScroll(InputAction.CallbackContext context)
    {
        float inputScroll = context.ReadValue<float>();
        if(inputScroll != 0)
        {
            ActiveGun -= (int)inputScroll;

            if(ActiveGun < 0)             ActiveGun = Guns.Length-1;
            if(ActiveGun > Guns.Length-1) ActiveGun = 0;


            if(Guns.Length > 1) SwapGun(ActiveGun);
        }
    }

    public void SwapGun(int GunChoice)
    {
        for(int i = 0; i < Guns.Length; i++)
        {
            Guns[i].SetActive(false);
            if(i == GunChoice) Guns[i].SetActive(true);
        }
    }
}