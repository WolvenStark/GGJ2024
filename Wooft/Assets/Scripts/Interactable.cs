﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class Interactable : MonoBehaviour
{
    public enum InteractionType
    {
        NONE,
        PickUp,
        Examine,
        GrabDrop,
        Consume,
    }
    public InteractionType interactType;

    public string descriptionText;

    [Header("Custom Events")]
    public UnityEvent customEvent;
    public UnityEvent consumeEvent;

    [HideInInspector]
    public SpriteRenderer spriteRenderer;

    private int targetLayer = 6; // Interactable

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }   

    private void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
        gameObject.layer = targetLayer;
    }

    public void Interact()
    {
        switch (interactType)
        {
            case InteractionType.PickUp:
                // Todo : Impliment
                break;
            case InteractionType.Examine:
                InteractionSystem.Instance.ExamineItem(this);
                break;
            case InteractionType.GrabDrop:
                InteractionSystem.Instance.GrabDrop();
                break;
            case InteractionType.Consume:
                InteractionSystem.Instance.GrabDrop();
                gameObject.SetActive(false);
                break;
            default:
                Debug.Log("Interactable has unknown type assigned");
                break;
        }

        customEvent.Invoke();
    }
}


