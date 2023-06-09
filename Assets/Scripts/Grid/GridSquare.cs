using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GridSquare : MonoBehaviour
{
    [HideInInspector] public RectTransform squareRect;

    public Image hooverImage;
    public Image activeImage;
    public Image normalImage;
    public List<Sprite> normalImages;

    private Vector3 defaultScale;

    public bool Selected { get; set; }
    public int SquareIndex { get; set; }
    public bool SquareOccupied { get; set; }

    private void Awake()
    {
        squareRect = GetComponent<RectTransform>();
    }

    private void Start()
    {
        Selected = false;
        SquareOccupied = false;

        defaultScale = activeImage.rectTransform.localScale;
    }

    public bool CanWeUseThisSquare()
    {
        return hooverImage.gameObject.activeSelf;
    }

    public void PlaceShapeOnBoard()
    {
        ActivateSquare();
    }

    public void ActivateSquare()
    {
        hooverImage.gameObject.SetActive(false);
        activeImage.gameObject.SetActive(true);
        Selected = true;
        SquareOccupied = true;
    }

    public void DeactivateSquare()
    {
        activeImage.rectTransform.DOScale(0.0f, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            activeImage.gameObject.SetActive(false);
            activeImage.rectTransform.localScale = defaultScale;
        });

    }

    public void ClearOccupied()
    {
        Selected = false;
        SquareOccupied = false;
    }

    public void SetImage(bool setFirstImage)
    {
        normalImage.sprite = setFirstImage ? normalImages[1] : normalImages[0];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (SquareOccupied == false)
        {
            Selected = true;
            hooverImage.gameObject.SetActive(true);
        }
        else if (collision.GetComponent<ShapeSquare>() != null)
        {
            collision.GetComponent<ShapeSquare>().SetOccupied();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Selected = true;

        if (SquareOccupied == false)
        {
            hooverImage.gameObject.SetActive(true);
        }
        else if (collision.GetComponent<ShapeSquare>() != null)
        {
            collision.GetComponent<ShapeSquare>().SetOccupied();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (SquareOccupied == false)
        {
            Selected = false;
            hooverImage.gameObject.SetActive(false);
        }
        else if (collision.GetComponent<ShapeSquare>() != null)
        {
            collision.GetComponent<ShapeSquare>().UnSetOccupied();
        }
    }
}
