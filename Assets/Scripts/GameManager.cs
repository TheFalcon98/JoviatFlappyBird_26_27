using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private const string BestScoreKey = "BestScore";
    public GameObject gameOverPanel;
    public GameObject pipeSpawner;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bestScoreText;
    public TextMeshProUGUI resultText;
    public Background background;
    public Player player;
    public AudioSource audioSource;
    [Range(0f, 1f)] public float sfxVolume = 0.65f;

    private int score;
    private int bestScore;
    private bool started;
    private bool ended;
    private AudioClip flapClip, collisionClip, startClip, recordClip, pointClip;
    public bool IsPlaying => started && !ended;

    private void Awake()
    {
        bestScore = PlayerPrefs.GetInt(BestScoreKey, 0);
        if (!audioSource) audioSource = GetComponent<AudioSource>();
        if (!audioSource) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0;
        flapClip = MakeTone("Flap", 700f, 0.07f);
        collisionClip = MakeTone("Collision", 170f, 0.22f);
        startClip = MakeTone("Start", 520f, 0.12f);
        recordClip = MakeTone("New record", 980f, 0.28f);
        pointClip = MakeTone("Point", 820f, 0.1f);
    }

    private void Start()
    {
        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (!resultText && gameOverPanel) resultText = gameOverPanel.GetComponentInChildren<TextMeshProUGUI>(true);
        if (!player) player = FindFirstObjectByType<Player>();
        if (!background) background = FindFirstObjectByType<Background>();
        if (pipeSpawner) pipeSpawner.SetActive(true);
        UpdateScoreInfo();
    }

    private void Update()
    {
        if (ended || started) return;
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) ||
            (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
            StartGame();
    }

    public void StartGame()
    {
        if (started || ended) return;
        started = true;
        Play(startClip);
        var spawner = pipeSpawner ? pipeSpawner.GetComponent<PipeSpawner>() : null;
        if (spawner) spawner.BeginSpawning();
        if (background) background.SetMoving(true);
        if (player) player.BeginPlaying();
    }

    public void PlayFlapSound() => Play(flapClip);
    public void PlayPointSound() => Play(pointClip);

    private void Play(AudioClip clip)
    {
        if (clip && audioSource) audioSource.PlayOneShot(clip, sfxVolume);
    }

    public void AddScore(int amount)
    {
        if (!IsPlaying || amount <= 0) return;
        score += amount;
        UpdateScoreInfo();
        PlayPointSound();
    }

    private void UpdateScoreInfo()
    {
        if (scoreText) scoreText.text = $"Current Score: {score}\nBest Score: {bestScore}";
        if (bestScoreText) bestScoreText.text = $"Best Score: {bestScore}";
    }

    public void StopGame()
    {
        if (ended) return;
        ended = true;
        if (started) Play(collisionClip);
        if (player) player.Die();
        if (background) background.SetMoving(false);
        if (pipeSpawner)
        {
            var spawner = pipeSpawner.GetComponent<PipeSpawner>();
            if (spawner) spawner.StopSpawning();
        }
        foreach (var pipeObject in GameObject.FindGameObjectsWithTag("PIPE"))
        {
            var pipe = pipeObject.GetComponent<Pipe>();
            if (pipe) pipe.enabled = false;
        }

        bool newRecord = score > bestScore;
        if (newRecord)
        {
            bestScore = score;
            PlayerPrefs.SetInt(BestScoreKey, bestScore);
            PlayerPrefs.Save();
            Play(recordClip);
        }
        UpdateScoreInfo();
        if (resultText) resultText.text = newRecord ? $"NEW RECORD!\nScore: {score}" : $"Game Over\nScore: {score}";
        if (gameOverPanel) gameOverPanel.SetActive(true);
    }

    public void RestartGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    private static AudioClip MakeTone(string clipName, float frequency, float duration)
    {
        const int sampleRate = 22050;
        int count = Mathf.CeilToInt(sampleRate * duration);
        var samples = new float[count];
        for (int i = 0; i < count; i++)
        {
            float envelope = Mathf.Clamp01(Mathf.Min(i / 220f, (count - i) / 900f));
            samples[i] = Mathf.Sin(2f * Mathf.PI * frequency * i / sampleRate) * envelope * 0.35f;
        }
        var clip = AudioClip.Create(clipName, count, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }
}
