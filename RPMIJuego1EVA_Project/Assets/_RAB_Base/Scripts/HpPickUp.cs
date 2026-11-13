using UnityEngine;

public class HpPickUp : MonoBehaviour
{
    [Header ("Rotate Parameters")]
    public float speedX = 10;
    public float speedY = 20;
    public float speedZ = 0;
    [Header ("Movement Parameters")]
    public float amplitude = 0.5f;
    public float speed = 2;
    private Vector3 startPosition;
    // Update is called once per frame
    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        ObjectMovement();
    }

    void ObjectMovement()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * speed) * amplitude;

        transform.position = new Vector3(
            startPosition.x,
            newY,
            startPosition.z
        );
    }

}
