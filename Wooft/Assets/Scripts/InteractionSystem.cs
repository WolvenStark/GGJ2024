using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionSystem : MonoBehaviour
{
    public static InteractionSystem Instance;

    [Header("Detection Fields")]
    //Detection Point
    public Transform detectionPoint;
    //Detection Radius
    private const float detectionRadius = 0.2f;
    //Detection Layer
    public LayerMask detectionLayer;
    //Cached Trigger Object
    public GameObject detectedObject;
    [Header("Examine Fields")]
    //Examine window object
    public GameObject examineWindow;
    public GameObject examinedObject;
    public GameObject grabbedObject;
    public float grabbedObjectYValue;
    public float grabbedObjectZValue;
    public Transform grabPoint;
    public Image examineImage;
    public TextMeshProUGUI examineText;
    public bool isExamining;
    public bool isGrabbing;
    public bool useIsometricYasZ = true;

    protected string grabbedDesiredLayer = "Interactable";
    protected int grabbedDesiredOrder = 3;
    protected string grabbedOriginalLayer = "Default";
    protected int grabbedOriginalOrder = 1;

    public Color detectedHighlightColor;
    protected Color detectedOriginalColor = Color.white;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Update()
    {
        if (DetectObject())
        {
            if (InteractInput())
            {
                // Cleanup previous cooroutine
                Instance.StopAllCoroutines();

                //If we are grabbing something don't interact with other items, drop the grabbed item first
                if (isGrabbing)
                {
                    GrabDrop(grabbedObject.GetComponentInChildren<Interactable>());
                    return;
                }

                detectedObject.GetComponentInChildren<Interactable>().Interact();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(detectionPoint.position, detectionRadius);
    }

    public bool InteractInput()
    {
        if (PlayerMovement.AllowGameInput)
        {
            return Input.GetKeyDown(KeyCode.E);
        }

        return false;
    }

    public void RestoreDetetcedObjectStats()
    {
        if (detectedObject != null)
        {
            var detectedRenderer = detectedObject.GetComponentInChildren<SpriteRenderer>();
            if (detectedRenderer != null)
            {
                detectedRenderer.color = detectedOriginalColor;
            }
        }
    }

    public void TrackDetetcedObjectStats()
    {
        if (detectedObject != null)
        {
            var detectedRenderer = detectedObject.GetComponentInChildren<SpriteRenderer>();
            if (detectedRenderer != null)
            {
                detectedOriginalColor = detectedRenderer.color;

                if (grabbedObject == detectedObject)
                {
                    RestoreDetetcedObjectStats();
                }
                else
                {
                    // Apply changes
                    detectedRenderer.color = detectedHighlightColor;
                }
            }
        }
    }

    public bool DetectObject()
    {
        Collider2D obj = Physics2D.OverlapCircle(detectionPoint.position, detectionRadius, detectionLayer);

        if (obj == null)
        {
            RestoreDetetcedObjectStats();
            detectedObject = null;
            return false;
        }
        else
        {
            RestoreDetetcedObjectStats();
            detectedObject = obj.gameObject;
            TrackDetetcedObjectStats();

            return true;
        }
    }

    public void ExamineItem(Interactable item)
    {
        if (isExamining)
        {
            examinedObject = null;

            //Hide the Examine Window
            examineWindow.SetActive(false);
            //disable the boolean
            isExamining = false;

            // Return music to normal
            AudioManager.RestoreMainTheme();
        }   
        else
        {
            //Show the item's image in the middle
            examineImage.sprite = item.spriteRenderer.sprite;
            //Write description text underneath the image
            examineText.text = item.descriptionText;
            //Display an Examine Window
            examineWindow.SetActive(true);
            //enable the boolean
            isExamining = true;

            examinedObject = item.gameObject;

            ApplyItemStats(item);

            Instance.StopAllCoroutines();
            Instance.StartCoroutine(WaitForMirrorToEnd());
        }
    }

    public IEnumerator WaitForMirrorToEnd()
    {
        yield return new WaitForSeconds(6.5f);

        // Return item
        Instance.ExamineItem(examinedObject.GetComponentInChildren<Interactable>());
    }

    public void RestoreGrabbedObjectStats(Interactable item)
    {
        //set the y position to its origin
        if (useIsometricYasZ)
        {
            grabbedObject.transform.position =
                new Vector3(grabbedObject.transform.position.x, grabbedObject.transform.position.y, grabbedObjectZValue);
        }
        else
        {
            grabbedObject.transform.position =
                new Vector3(grabbedObject.transform.position.x, grabbedObjectYValue, grabbedObject.transform.position.z);
        }

        // Reestore sorting order
        item.spriteRenderer.sortingLayerName = grabbedOriginalLayer;
        item.spriteRenderer.sortingOrder = grabbedOriginalOrder;

        // Re-enable collider
        item.col.enabled = true;
    }

    public void TrackGrabbedObjectStats(Interactable item)
    {
        if (detectedObject != null)
        {
            grabbedObjectYValue = grabbedObject.transform.position.y;
            grabbedObjectZValue = grabbedObject.transform.position.z;
            grabbedOriginalLayer = item.spriteRenderer.sortingLayerName;
            grabbedOriginalOrder = item.spriteRenderer.sortingOrder;

            // Apply changes
            grabbedObject.transform.localPosition = grabPoint.localPosition;
            item.spriteRenderer.sortingLayerName = grabbedDesiredLayer;
            item.spriteRenderer.sortingOrder = grabbedDesiredOrder;

            // Disable collider
            item.col.enabled = false;

            if (grabbedObject == detectedObject)
            {
                RestoreDetetcedObjectStats();
            }
        }
    }

    public void GrabDrop(Interactable item)
    {
        //Check if we do have a grabbed object => drop it
        if (isGrabbing)
        {
            //make isGrabbing false
            isGrabbing = false;
            //unparent the grabbed object
            grabbedObject.transform.parent = null;

            RestoreGrabbedObjectStats(item);

            //null the grabbed object reference
            grabbedObject = null;

            // Return music to normal
            AudioManager.RestoreMainTheme();

        }
        //Check if we have nothing grabbed grab the detected item
        else
        {
            //Enable the isGrabbing bool
            isGrabbing = true;
            //assign the grabbed object to the object itself
            //Parent the grabbed object to the player
            //Cache the y value of the object
            //Adjust the position of the grabbed object to be closer to hands                        
            grabbedObject = detectedObject;
            grabbedObject.transform.parent = transform;

            TrackGrabbedObjectStats(item);

            ApplyItemStats(item);
        }
    }

    public void ApplyItemStats(Interactable item)
    {
        if (item.musicInteract != string.Empty && item.musicInteract != "" && item.musicInteract != "null")
        {
            AudioManager.ChangeMusicCaller(item.musicInteract);
        }
        if (item.sfxInteract != string.Empty && item.sfxInteract != "" && item.sfxInteract != "null")
        {
            AudioManager.PlaySFX(item.sfxInteract);
        }
        if (item.pointsValue != 0)
        {
            if (item.pointsValue > 0)
            {
                ProgressBar.Instance.IncrementProgress(item.pointsValue);
            }
            else
            {
                ProgressBar.Instance.DecrementProgress(item.pointsValue);
            }
        }
    }

    public void Consume()
    {
        // Todo
    }
}
