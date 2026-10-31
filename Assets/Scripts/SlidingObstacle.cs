using UnityEngine;

public class SlidingObstacle : MonoBehaviour
{
    public float range = 4.5f;
    float speed;
    int dir = 1;

    public void ResetObstacle()
    {
        speed = Random.Range(1f, 3.5f);
        dir = Random.value < 0.5f ? 1 : -1;
        var p = transform.localPosition;
        transform.localPosition = new Vector3(Random.Range(-range, range), p.y, p.z);
    }

    void FixedUpdate()
    {
        var p = transform.localPosition;
        p.x += dir * speed * Time.fixedDeltaTime;
        if (p.x > range) { p.x = range; dir = -1; }
        if (p.x < -range) { p.x = -range; dir = 1; }
        transform.localPosition = p;
    }
}
