using UnityEngine;

public class InteractingArea : MonoBehaviour
{
    
    private float rotSpeed = 100f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateInteractionAreaPosition(Vector2 pos)
    {


        if (pos.x > 0)
        {
            transform.rotation = Quaternion.Euler(0,0,270);
        }
        else if (pos.x < 0)
        {
            transform.rotation = Quaternion.Euler(0,0,90);
        }
        else if(pos.y < 0)
        {
            transform.rotation = Quaternion.Euler(0,0,180);
        }
        else if(pos.y > 0)
        {
            transform.rotation = Quaternion.Euler(Vector3.zero);
        }
        
    }
}
