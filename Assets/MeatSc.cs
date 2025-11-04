using UnityEngine;

public class MeatSc : MonoBehaviour
{

    BirdScript bc;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bc = GameObject.Find("Bird").GetComponent<BirdScript>();
        //Destroy(gameObject, 6);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        print("Triggered Meat");
        bc.point++;
        Destroy(gameObject);
    }
}
