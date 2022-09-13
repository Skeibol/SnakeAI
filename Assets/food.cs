using UnityEngine;

public class food : MonoBehaviour
{
    public BoxCollider2D gridArea;

    private void Start()
    {
        RandomizePosition();


    }

    private void RandomizePosition()
    {
        Bounds bounds = this.gridArea.bounds;

        float x = Mathf.Round(Random.Range(bounds.min.x, bounds.max.x));
        float y = Mathf.Round(Random.Range(bounds.min.y, bounds.max.y));

        this.transform.position = new Vector3 (x, y, 0);


    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "snake" || other.tag == "obstacle")
        {
            RandomizePosition();

        }
    }
}
