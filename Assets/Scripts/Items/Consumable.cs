﻿using System;
using UnityEngine;

public class Consumable : Item {

    [SerializeField] private GameObject[] _consumptionStates;

    private int _biteInd;

    public void DoEat() {
        if (gameObject == null) {
            Debug.LogError("GO null");
            Debug.LogError("current action: " + _holder.actionController.currentAction);
        }

        ConsumableProfile profile = _profile as ConsumableProfile;

        _holder.statsController.AddHealth(profile._healthMod);
        _holder.statsController.AddStamina(profile._staminaMod);
        _holder.statsController.AddHunger(profile._hungerMod);
        _holder.statsController.AddHappiness(profile._happinessMod);

        if (++_biteInd >= _consumptionStates.Length) {
            Destroy(gameObject);
            return;
        }

        _consumptionStates[_biteInd - 1].SetActive(false);
        _consumptionStates[_biteInd].SetActive(true);
    }
}

