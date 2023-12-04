using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPowerUp : MonoBehaviour
{
    //When the power up is collected, gives player speed boost -Ken
    public float speedMultiplier = 1.5f;
    public float duration = 5.0f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("RubyController"))
        {
            RubyController player = other.GetComponent<RubyController>();

            if (player != null)
            {
                player.ApplySpeedBoost(speedMultiplier, duration);
                Destroy(gameObject);
            }
        }
    }
}