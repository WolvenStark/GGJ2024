using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightChoreography : MonoBehaviour
{
    protected Light2D GoldenAura;
    protected List<Light2D> AmbientWindowLighting;

    // Start is called before the first frame update
    public void Start()
    {
        for (var childId = 0; childId < transform.childCount; childId++)
        {
            var child = transform.GetChild(childId);
            var childName = child.name;

            if (name == "Golden Aura")
            {
                GoldenAura = child.GetComponent<Light2D>();
            }
            else if (name.Contains("GodRays"))
            {
                AmbientWindowLighting.Add(child.GetComponent<Light2D>());
            }

        }
    }

}
