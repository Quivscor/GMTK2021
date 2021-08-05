using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    public List<Building> shopBuildings;

    public List<Transform> spawnPoints;

    public List<GameObject> shopInventoryRef;

    private void Awake()
    {
        shopInventoryRef = new List<GameObject>();
        for(int i = 0; i < shopBuildings.Count; i++)
        {
            shopInventoryRef.Add(GameObject.Instantiate<GameObject>(shopBuildings[i].gameObject, spawnPoints[i].position, Quaternion.identity, null));
        }
    }

    private void Start()
    {
        GridController.Instance.OnProcessBuildPlacement += RefillShop;
    }

    public void RefillShop()
    {
        for(int i = 0; i < shopBuildings.Count; i++)
        {
            //check if moved away from shop
            if(shopInventoryRef[i].transform.position != spawnPoints[i].position)
            {
                shopInventoryRef[i] = GameObject.Instantiate(GameObject.Instantiate<GameObject>(shopBuildings[i].gameObject, spawnPoints[i].position, Quaternion.identity, null));
            }
        }    
    }
}
