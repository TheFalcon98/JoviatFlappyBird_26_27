using UnityEngine;

public class Background : MonoBehaviour
{
    [Tooltip("Velocitat de desplaçament del fons; ha de ser inferior a la de les canonades.")]
    public float scrollSpeed = 0.7f;
    private Transform[] tiles;
    private float tileWidth;
    private bool moving;

    private void Start()
    {
        var renderer = GetComponent<SpriteRenderer>();
        if (!renderer || !renderer.sprite) return;
        tileWidth = renderer.bounds.size.x;
        if (tileWidth <= 0f) return;
        tiles = new Transform[3];
        tiles[0] = transform;
        for (int i = 1; i < tiles.Length; i++)
        {
            var tile = Instantiate(gameObject, transform.parent);
            tile.name = gameObject.name + "_Tile" + i;
            tile.transform.position = transform.position + Vector3.right * tileWidth * i;
            tile.GetComponent<SpriteRenderer>().flipX = i % 2 == 1;
            var script = tile.GetComponent<Background>();
            if (script) script.enabled = false;
            tiles[i] = tile.transform;
        }
    }

    public void SetMoving(bool value) => moving = value;

    private void Update()
    {
        if (!moving || tiles == null) return;
        float movement = scrollSpeed * Time.deltaTime;
        foreach (var tile in tiles) tile.position += Vector3.left * movement;
        float leftEdge = Camera.main ? Camera.main.transform.position.x - Camera.main.orthographicSize * Camera.main.aspect : -10f;
        foreach (var tile in tiles)
        {
            if (tile.position.x + tileWidth * 0.5f < leftEdge)
            {
                float rightmost = tiles[0].position.x;
                for (int i = 1; i < tiles.Length; i++) rightmost = Mathf.Max(rightmost, tiles[i].position.x);
                tile.position = new Vector3(rightmost + tileWidth, tile.position.y, tile.position.z);
            }
        }

        // Alternating mirrored tiles make the image meet at identical edge pixels.
        var orderedTiles = (Transform[])tiles.Clone();
        System.Array.Sort(orderedTiles, (a, b) => a.position.x.CompareTo(b.position.x));
        for (int i = 0; i < orderedTiles.Length; i++)
            orderedTiles[i].GetComponent<SpriteRenderer>().flipX = i % 2 == 1;
    }
}
