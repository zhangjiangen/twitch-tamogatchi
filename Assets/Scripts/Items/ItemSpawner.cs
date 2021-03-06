﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public static ItemSpawner singleton;

    [SerializeField] private GameObject[] _items;
    [SerializeField] private GameObject _dust;

    private void Awake() {
        singleton = this;
    }

    void Start() {

        MessengerServer.singleton.SetHandler(NetMsgInds.SpawnMessage, OnSpawnMessage);
        MessengerServer.singleton.SetHandler(NetMsgInds.ClickMessage, OnClickMessage);
    }

    private void OnClickMessage(NetMsg msg) {
        ClickMessage click = (ClickMessage)msg;
        Vector3 screenPos = new Vector3(click.x * Screen.width, click.y * Screen.height, 0);
        SpawnRandomItem(CoordsUtils.ScreenToWorldPos(screenPos));

    }

    private void OnSpawnMessage(NetMsg msg) {
        SpawnMessage spawnMsg = (SpawnMessage)msg;
        SpawnItem(spawnMsg.itemInd, CoordsUtils.ScreenToWorldPos(new Vector3(spawnMsg.x, spawnMsg.y, 0)));
    }

    void Update() {
        if(Input.GetButtonDown("Spawn")) {
            SpawnItemAtRandomPos();
        }
    }

    private void SpawnItemAtRandomPos() {
        SpawnRandomItem(CoordsUtils.RandomWorldPointOnScreen());
    }

    public void SpawnRandomItem(Vector3 worldPos) {
        SpawnItem(UnityEngine.Random.Range(0, _items.Length), worldPos);
    }

    public void SpawnItem(int spawnInd, Vector3 worldPos) {
        if(spawnInd < 0 || spawnInd >= _items.Length) {
            throw new IndexOutOfRangeException("Item ind out of range");
        }

        Item itemPrefab = _items[spawnInd].GetComponent<Item>();

        Vector3 itemPos = new Vector3(worldPos.x, worldPos.y, worldPos.z);
        if(itemPrefab.dropsIn) {
            itemPos.y = itemPos.y + 30;
        } else {
            itemPos.y = itemPrefab.transform.position.y;
        }

        Debug.Log("Itempos: " + itemPos + ", " + itemPrefab.transform.position);

        Quaternion itemRot = Quaternion.AngleAxis(360 * UnityEngine.Random.value, Vector3.up) *
            itemPrefab.transform.rotation;

        GameObject clone = Instantiate(itemPrefab.gameObject, itemPos, itemRot);

        Vector3 originalScale = clone.transform.localScale;
        clone.transform.localScale = (1 / 2.5f) * originalScale;

        iTween.ScaleTo(clone.gameObject, iTween.Hash("scale", originalScale,
            "time", 1.0f, "easetype", iTween.EaseType.easeOutElastic));

        iTween.RotateBy(clone.gameObject, iTween.Hash("amount", _items[spawnInd].transform.up,
            "time", 1f, "easetype", iTween.EaseType.easeOutSine));
    }

    public GameObject MakeDust() { 

        GameObject clone = Instantiate(_dust, new Vector3(transform.position.x, 
            transform.position.y - 1.5f, transform.position.z),
            Quaternion.Euler(new Vector3(90, 0, 0)), null) as GameObject;

        clone.transform.localScale = 2.0f * Vector3.one;

        return clone;
    }
}
