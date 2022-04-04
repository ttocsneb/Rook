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
    public Sprite img3;
    public Sprite img4;
    public Sprite img5;
    public Sprite img6;
    public Sprite img7;
    public Sprite img8;
    public Sprite img9;
    public Sprite img10;
    public Sprite img11;
    public Sprite img12;
    //public Sprite img13;
    //public Sprite img14;

    private List<Sprite> sprites;
    private int currentSprite;
    public void Start() {
        sprites = new List<Sprite>();
        sprites.Add(img1);
        sprites.Add(img2);
        sprites.Add(img3);
        sprites.Add(img4);
        sprites.Add(img5);
        sprites.Add(img6);
        sprites.Add(img7);
        sprites.Add(img8);
        sprites.Add(img9);
        sprites.Add(img10);
        sprites.Add(img11);
        sprites.Add(img12);


        currentSprite = 0;
    }
    public void Next()
    {
        Debug.Log("Next Tutorial thing");
        Panel.GetComponent<Image> ().sprite = sprites[currentSprite];
        currentSprite += 1;
    }
}
