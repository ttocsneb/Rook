using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public GameObject Panel;
    public Sprite img1;
    public Sprite img2;
    private List<Sprite> sprites;
    private int currentSprite;
    public void Start() {
        sprites = new List<Sprite>();
        sprites.Add(img1);
        sprites.Add(img2);
        currentSprite = 0;
    }
    public void Next()
    {
        Debug.Log("Next Tutorial thing");
        Panel.GetComponent<Image> ().sprite = sprites[currentSprite];
        currentSprite += 1;
    }
}
