using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class DoorLightingControl : MonoBehaviour
{
    private bool isLit = false;
    private Door door;

    private void Awake()
    {
        door = GetComponentInParent<Door>();
    }

    /// <summary>
    /// Fade in door
    /// </summary>
    /// <param name="door"></param>
    public void FadeInDoor(Door door)
    {
        // create new material to fade in
        Material material = new Material(GameResources.Instance.variableLitShader);

        if (!isLit)
        {
            SpriteRenderer[] spriteRendererArray = GetComponentsInParent<SpriteRenderer>();
            foreach (SpriteRenderer spriteRenderer in spriteRendererArray)
            {
                StartCoroutine(FadeInDoorRoutine(spriteRenderer, material));
            }

            isLit = true;
        }
    }

    /// <summary>
    /// fade in door coroutine
    /// </summary>
    /// <param name="spriteRenderer"></param>
    /// <param name="material"></param>
    /// <returns></returns>
    private IEnumerator FadeInDoorRoutine(SpriteRenderer spriteRenderer, Material material)
    {
        spriteRenderer.material = material;

        for (float i = 0.5f; i <= 1f; i += Time.deltaTime / Settings.fadeInTime)
        {
            material.SetFloat("Alpha_Slider", i);
            yield return null;
        }

        spriteRenderer.material = GameResources.Instance.litMaterial;
    }

    // fade in door if triggered
    private void OnTriggerEnter2D(Collider2D collision)
    {
        FadeInDoor(door);
    }
}
