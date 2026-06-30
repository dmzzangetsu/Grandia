using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    private float startPos;
    public GameObject camera;
    public float parallaxEffectModifier;
    void Start()
    {
       startPos = transform.position.x;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float distance = camera.transform.position.x * parallaxEffectModifier;
        transform.position =new Vector3(startPos+ distance, transform.position.y,transform.position.z);
    }
}
