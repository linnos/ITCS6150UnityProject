using System;
using UnityEngine;
public class EndZone : MonoBehaviour
{
    public Action OnReachedEnd;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) OnReachedEnd?.Invoke();
    }
}
