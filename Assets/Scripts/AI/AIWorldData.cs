﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIWorldData {

    private Skin _pet;

    private Item _closestItem;
    public Item closestItem { get { return _closestItem; } }

    private List<Item> _itemsInRange;
    public List<Item> itemsInRange { get { return _itemsInRange; } }

    private Collider[] _castColliders = new Collider[100];

    public AIWorldData(Skin pet) {
        _pet = pet;
        _itemsInRange = new List<Item>();
    }

    public void UpdateData() {
        UpdateItems();
    }

    private void UpdateItems() {
        _itemsInRange.Clear();
        _closestItem = null;

        int numHits = Physics.OverlapSphereNonAlloc(_pet.feetTransform.position,
            _pet.itemController.pickupRange, _castColliders, VBLayerMask.Item);

        if(numHits == _castColliders.Length) {
            Debug.LogWarning("Ran out of space in cast colliders array! " +
            	"May have missed some items. Consider lengthening the array...");
        }

        float minD = float.MaxValue;
        for (int i = 0; i < numHits; i++) {
            Item item = _castColliders[i].GetComponentInParent<Item>();

            if (item != null && item.CanBePickedUp()) {
                _itemsInRange.Add(item);

                Vector3 d = item.transform.position - _pet.feetTransform.position;
                float dMag = d.sqrMagnitude;
                if (dMag < minD) {
                    minD = dMag;
                    _closestItem = item;
                }
            }
        }
    }
}
