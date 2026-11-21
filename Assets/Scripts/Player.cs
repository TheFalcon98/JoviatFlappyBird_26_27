using UnityEngine;

public class Player : MonoBehaviour
{
    public float flapVelocity = 5.2f;
    public float rotationSpeed = 7f;
    public float maxUpAngle = 25f;
    public float maxDownAngle = -65f;
    public float animationFramesPerSecond = 12f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private GameManager gameManager;
    private Sprite originalSprite;
    private Sprite[] flightFrames;
    private Sprite[] deathFrames;
    private bool playing;
    private bool dead;
    private float animationTimer;
    private int deathFrame;
    private int inputBlockedFrame = -1;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalSprite = spriteRenderer ? spriteRenderer.sprite : null;
        gameManager = FindFirstObjectByType<GameManager>();
        flightFrames = new[] { LoadSprite("flap-1"), LoadSprite("flap-2"), LoadSprite("flap-3"), LoadSprite("flap-4") };
        deathFrames = new[] { LoadSprite("got-hit-1"), LoadSprite("got-hit-2") };
        if (rb)
        {
            rb.gravityScale = 1.8f;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }
    }

    private static Sprite LoadSprite(string name)
    {
        var sprites = Resources.LoadAll<Sprite>("Bird");
        foreach (var sprite in sprites)
            if (sprite.name == name || sprite.name.StartsWith(name + "_")) return sprite;
        return null;
    }

    private void Update()
    {
        if (dead || !playing) return;
        bool flap = Time.frameCount != inputBlockedFrame &&
                    (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) ||
                     (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began));
        if (flap) Flap();

        float targetAngle = rb.linearVelocity.y > 0.15f ? maxUpAngle :
                            rb.linearVelocity.y < -0.15f ? maxDownAngle : 0f;
        float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetAngle, rotationSpeed * 60f * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, 0, angle);

        if (flightFrames.Length > 0)
        {
            animationTimer += Time.deltaTime;
            int frame = Mathf.FloorToInt(animationTimer * animationFramesPerSecond) % flightFrames.Length;
            if (flightFrames[frame]) spriteRenderer.sprite = flightFrames[frame];
        }
    }

    private void Flap()
    {
        if (dead || !playing || !gameManager || !gameManager.IsPlaying) return;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, flapVelocity);
        gameManager.PlayFlapSound();
    }

    public void BeginPlaying()
    {
        if (dead) return;
        playing = true;
        if (rb) rb.simulated = true;
        inputBlockedFrame = Time.frameCount;
        Flap();
    }

    public void Die()
    {
        if (dead) return;
        dead = true;
        playing = false;
        if (rb) rb.linearVelocity = Vector2.zero;
        if (deathFrames.Length > 0)
        {
            int frame = 0;
            while (frame < deathFrames.Length && !deathFrames[frame]) frame++;
            if (frame < deathFrames.Length) spriteRenderer.sprite = deathFrames[frame];
        }
    }

    public void ResetForNewGame()
    {
        dead = false;
        playing = false;
        animationTimer = 0f;
        if (spriteRenderer && originalSprite) spriteRenderer.sprite = originalSprite;
        if (rb)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }
        transform.rotation = Quaternion.identity;
    }
}
