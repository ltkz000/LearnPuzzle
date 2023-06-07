using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShapeSquare : MonoBehaviour
{
    public Image occupiedImage;

    private BoxCollider2D boxCollider2D;

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        occupiedImage.gameObject.SetActive(false);
    }

    public void ActiveShapeSquare()
    {
        boxCollider2D.enabled = true;
        gameObject.SetActive(true);
    }

    public void DeactiveShapeSquare()
    {
        boxCollider2D.enabled = false;
        gameObject.SetActive(false);
    }

    public void SetOccupied()
    {
        occupiedImage.gameObject.SetActive(true);
    }

    public void UnSetOccupied()
    {
        occupiedImage.gameObject.SetActive(false);
    }
}
