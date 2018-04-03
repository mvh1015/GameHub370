using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Health : NetworkBehaviour {

    public const int maxHealth = 100;

    [SyncVar(hook = "OnChangeHealth")]   //SyncVar means it is a variable we want to sync with the server
                                         //hook means that whenever this is changed, the function is invoked on every client

    public int currentHealth = maxHealth;

    public RectTransform healthBar;

    public bool destroyOnDeath;

    public void TakeDamage(int amount)
    {
        if (!isServer)   //Since the server is controlling the hp, only let the server do it.
        {
            return;
        }

        

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            if (destroyOnDeath)
            {
                Destroy(gameObject);
            }
            else
            {
                currentHealth = maxHealth;

                //called on the server, but invoked on the clients
                RpcRespawn();
            }
        }

        //healthBar.sizeDelta = new Vector2(currentHealth, healthBar.sizeDelta.y);
    }

    void OnChangeHealth (int currentHealth)
    {
        healthBar.sizeDelta = new Vector2(currentHealth, healthBar.sizeDelta.y);
    }

    [ClientRpc] //ClientRPC==pc is the opposite of Command. called on the server, executed on the client
    void RpcRespawn()
    {
        if (isLocalPlayer)
        {
            // move back to zero location
            transform.position = Vector3.zero;
        }
    }
}
