using System.Collections;
using System.Collections.Generic;
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
    public GameObject grabbedObject;
    public float grabbedObjectYValue;
    public float grabbedObjectZValue;
    public Transform grabPoint;
    public Image examineImage;
    public Text examineText;
    public bool isExamining;
    public bool isGrabbing;
    public bool useIsometricYasZ = true;

    protected string grabbedDesiredLayer = "Interactable";
    protected int grabbedDesiredOrder = 3;
    protected string grabbedOriginalLayer = "Default";
    protected int grabbedOriginalOrder = 1;

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
        return Input.GetKeyDown(KeyCode.E);
    }

    public bool DetectObject()
    {

        Collider2D obj = Physics2D.OverlapCircle(detectionPoint.position, detectionRadius, detectionLayer);

        if (obj == null)
        {
            detectedObject = null;
            return false;
        }
        else
        {
            detectedObject = obj.gameObject;
            return true;
        }
    }

    public void ExamineItem(Interactable item)
    {
        if (isExamining)
        {
            //Hide the Examine Window
            examineWindow.SetActive(false);
            //disable the boolean
            isExamining = false;
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

            //null the grabbed object reference
            grabbedObject = null;

            // Return music to normal
            AudioManager.ChangeMusicCaller("MainTheme");

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

            grabbedObjectYValue = grabbedObject.transform.position.y;
            grabbedObjectZValue = grabbedObject.transform.position.z;
            grabbedOriginalLayer = item.spriteRenderer.sortingLayerName;
            grabbedOriginalOrder = item.spriteRenderer.sortingOrder;

            item.spriteRenderer.sortingLayerName = grabbedDesiredLayer;
            item.spriteRenderer.sortingOrder = grabbedDesiredOrder;

            grabbedObject.transform.localPosition = grabPoint.localPosition;

            // Disable collider
            item.col.enabled = false;

            // Change music to sock
            AudioManager.ChangeMusicCaller("SockTheme");
        }
    }

    public void Consume()
    {
        // Todo
    }
}
