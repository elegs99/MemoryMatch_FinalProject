using UnityEngine;

public class DiagonalMovement : MonoBehaviour
{
    public GameObject spritePrefab;
    public float speed = 5.0f;
    public int numberOfSprites = 20;

    private GameObject[] sprites;
    private Vector2 screenBounds;

    void Start()
    {
        sprites = new GameObject[numberOfSprites];
        Transform canvasTransform = transform;
        screenBounds = new Vector2(canvasTransform.localScale.x, canvasTransform.localScale.y);

        // Initialize sprites at random positions across the entire canvas
        for (int i = 0; i < numberOfSprites; i++)
        {
            Vector3 startPosition = new Vector3(
                Random.Range(-screenBounds.x / 2, screenBounds.x / 2),
                Random.Range(-screenBounds.y / 2, screenBounds.y / 2),
                0);

            sprites[i] = Instantiate(spritePrefab, startPosition, Quaternion.identity, canvasTransform);
        }
    }

    void Update()
    {
        Vector3 moveDirection = new Vector3(1, 1, 0); // Diagonal movement to the upper right
        foreach (GameObject sprite in sprites)
        {
            sprite.transform.Translate(moveDirection * speed * Time.deltaTime);

            // Check if sprite exceeds the bounds of the canvas
            if (sprite.transform.localPosition.x > screenBounds.x / 2 || sprite.transform.localPosition.y > screenBounds.y / 2)
            {
                // Respawn at a random position on the opposite side
                float newX = -screenBounds.x / 2;
                float newY = Random.Range(-screenBounds.y / 2, screenBounds.y / 2);
                sprite.transform.localPosition = new Vector3(newX, newY, 0);
            }
        }
    }
}
