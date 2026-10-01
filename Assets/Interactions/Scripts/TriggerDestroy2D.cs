using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDestroy2D : MonoBehaviour
{
    public GameObject ObjetADetruire;
    public string TagDuPlayer;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == TagDuPlayer)
        {
            Destroy(ObjetADetruire);
        }
    }
}
