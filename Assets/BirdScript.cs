using UnityEngine;
using UnityEngine.UI;

public class BirdScript : MonoBehaviour
{
    public float gridSize = 0.5f;  // ÍÌã ÇáÎØæÉ áßá ÍÑßÉ
    public float moveInterval = 0.1f; // ÇáæÞÊ Èíä ßá ÎØæÉ
    private int direction = 1; // 1 = íãíä, -1 = íÓÇÑ

    private float minX, maxX;
    private float timer = 0f;

    // ÚÏÇÏ áÊÞáíá moveInterval
    private float elapsedTime = 0f;
    private float decreaseInterval = 15f; // ßá 15 ËÇäíÉ
    private float decreaseAmount = 0.01f;

    // ÚÏÇÏ ááãÓÊæì / ÇáæÞÊ ÇáÅÌãÇáí
    private float totalTime = 0f;
    private float gameDuration = 120f; // 2 ÏÞíÞÉ = 120 ËÇäíÉ

    public int point;
    public bool blinked;
    public Text points;

    public bool finish;
    public GameObject pointfinishui;
    public Text congratsui;
    public Text pointfinish;
    void Start()
    {
        Time.timeScale = 1;
        // ÍÓÇÈ ÍÏæÏ ÇáÔÇÔÉ
        Vector3 screenBottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 10f));
        Vector3 screenTopRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 10f));

        minX = screenBottomLeft.x;
        maxX = screenTopRight.x;

        // ÇáÈÏÇíÉ Úáì ÇáíÓÇÑ
        transform.position = new Vector3(minX, transform.position.y, transform.position.z);
    }

    void Update()
    {
        points.text = point.ToString();

        timer += Time.deltaTime;
        elapsedTime += Time.deltaTime;
        totalTime += Time.deltaTime;

        // ÊÞáíá moveInterval ßá 15 ËÇäíÉ
        if (elapsedTime >= decreaseInterval)
        {
            moveInterval = Mathf.Max(0.01f, moveInterval - decreaseAmount);
            elapsedTime = 0f;
            Debug.Log("moveInterval decreased: " + moveInterval);
        }

        // ÊÍÞÞ ãä ÇäÊåÇÁ ÇááÚÈÉ ÈÚÏ ÏÞíÞÊíä
        if (totalTime >= gameDuration)
        {
            if(finish == false)
            {
                Finish();
            }
        }

        if (timer >= moveInterval)
        {
            timer = 0f;

            // ÊÍÑíß Bird ÈãÞÏÇÑ gridSize
            transform.position += new Vector3(gridSize * direction, 0, 0);

            // ÇäÚßÇÓ ÚäÏ ÇáæÕæá ááÍÏæÏ
            if (transform.position.x >= maxX)
            {
                direction = -1;
            }
            else if (transform.position.x <= minX)
            {
                direction = 1;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        print("Triggered");
        /*
        if(blinked)
        {
            Destroy(other.gameObject);
            point++;
        }
        */
    }

    private void Finish()
    {
        Debug.Log("Game Finished!");
        finish = true;
        Time.timeScale = 0;
        pointfinishui.SetActive(true);
        if(point > PlayerPrefs.GetInt("HighPoint"))
        {
            PlayerPrefs.SetInt("HighPoint", point);
        }
        congratsui.text = "High Point: " + PlayerPrefs.GetInt("HighPoint").ToString();
    }

    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

}   
