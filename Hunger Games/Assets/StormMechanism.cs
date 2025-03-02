using System.Collections;
using UnityEngine;

public class StormMechanism : MonoBehaviour
{
    public GameObject outerSphere; // De originele Sphere (buiten zichtbaar)
    public GameObject innerSphere; // De duplicaat met omgekeerde normals (binnen zichtbaar)

    public float initialRadius = 100f; // Beginradius van de storm
    public float finalRadius = 5f; // Minimale grootte van de storm
    public int shrinkPhases = 12; // Aantal krimpfases
    public float shrinkDuration = 10f; // Tijd dat de cirkel daadwerkelijk krimpt (in seconden)
    public float waitTime = 60f; // Tijd tussen elke krimp (in seconden)

    private float currentRadius;
    private int currentPhase = 0;

    void Start()
    {
        currentRadius = initialRadius;
        StartCoroutine(ShrinkStorm());
    }

    IEnumerator ShrinkStorm()
    {
        while (currentPhase < shrinkPhases)
        {
            yield return new WaitForSeconds(waitTime); // Wacht 1 minuut
            StartCoroutine(ShrinkOverTime(shrinkDuration));
            currentPhase++;
        }
    }

    IEnumerator ShrinkOverTime(float duration)
    {
        float elapsedTime = 0f;
        float startRadius = currentRadius;
        float targetRadius = Mathf.Lerp(initialRadius, finalRadius, (float)(currentPhase + 1) / shrinkPhases);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            currentRadius = Mathf.Lerp(startRadius, targetRadius, elapsedTime / duration);

            // Pas schaal aan van BEIDE spheres
            Vector3 newScale = new Vector3(currentRadius * 2, transform.localScale.y, currentRadius * 2);
            if (outerSphere) outerSphere.transform.localScale = newScale;
            if (innerSphere) innerSphere.transform.localScale = newScale;

            yield return null;
        }
    }
}
