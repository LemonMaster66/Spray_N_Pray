using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunManager : MonoBehaviour
{
    public int ActiveGun = 0;

    public Gun[] Guns;
    public bool[] HasGun;

    private PlayerSFX playerSFX;


    void Awake()
    {
        playerSFX = FindAnyObjectByType<PlayerSFX>();

        if(HasGun[ActiveGun] == true) SwapGun(ActiveGun);
        else Guns[ActiveGun].gameObject.SetActive(false);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1)) GunState(0, true);
        if(Input.GetKeyDown(KeyCode.F2)) GunState(1, true);
        if(Input.GetKeyDown(KeyCode.F3)) GunState(2, true);
        if(Input.GetKeyDown(KeyCode.F4)) GunState(3, true);
    }
    

    public void OnFire(InputAction.CallbackContext context)
    {
        if(CountGuns() < 1) return;
        if(context.started)       Guns[ActiveGun].ShootStart();
        else if(context.canceled) Guns[ActiveGun].ShootEnd();
    }

    public void OnAltFire(InputAction.CallbackContext context)
    {
        if(CountGuns() < 1) return;
        if(context.started)       Guns[ActiveGun].AltShootStart();
        else if(context.canceled) Guns[ActiveGun].AltShootEnd();
    }

    public void OnScroll(InputAction.CallbackContext context)
    {
        if(CountGuns() < 1) return;
        float inputScroll = context.ReadValue<float>();
        if(inputScroll != 0)
        {
            while(true)
            {
                ActiveGun -= (int)inputScroll;

                if(ActiveGun < 0)             ActiveGun = Guns.Length-1;
                if(ActiveGun > Guns.Length-1) ActiveGun = 0;

                if(HasGun[ActiveGun]) break;
            }

            SwapGun(ActiveGun);
        }
    }

    public void SwapGun(int GunChoice)
    {
        ActiveGun = GunChoice;
        if(Guns[ActiveGun].gameObject.activeSelf) return;

        for(int i = 0; i < Guns.Length; i++)
        {
            Guns[i].gameObject.SetActive(false);
            if(i == GunChoice) Guns[i].gameObject.SetActive(true);
        }

        playerSFX.PlaySound(playerSFX.WeaponSwap, 1, 0.25f, 0.1f);
    }

    public void GunState(int SelectedGun, bool State)
    {
        HasGun[SelectedGun] = State;
        if(State) SwapGun(SelectedGun);
    }

    public int CountGuns()
    {
        int result = 0;
        foreach(bool gunCheck in HasGun) if(gunCheck == true) result++;
        return result;
    }
}