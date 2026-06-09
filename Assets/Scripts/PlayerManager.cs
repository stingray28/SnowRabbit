using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager instance;
    public static PlayerManager Instance { get { return instance; } }

    private Rigidbody2D _rigid;
    private CapsuleCollider2D _collider2D;

    public bool PlayerPause { get; set; } = false;

    public bool Debug_setInvincibilityMode { get; set; } = false;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        GetComponent<PlayerInput>().enabled = true;
        _rigid = GetComponent<Rigidbody2D>();
        _collider2D = GetComponent<CapsuleCollider2D>();
        transform.position = DataManager.Instance.NowPlayData.lastSpawnPoint;
    }

    public void PlayerPauseOn()
    {
        PlayerPause = true;
        _rigid.bodyType = RigidbodyType2D.Static;
        _collider2D.enabled = false;
    }

    public void PlayerPauseOff()
    {
        PlayerPause = false;
        _rigid.bodyType = RigidbodyType2D.Dynamic;
        _collider2D.enabled = true;
    }

    public void Reload()
    {
        GameManager.Instance.LoadScene(DataManager.Instance.NowPlayData.lastSceneIndex);
    }

    public void Respawn()
    {
        transform.position = DataManager.Instance.NowPlayData.lastSpawnPoint;
    }

    public void OnInvincibilityMode()
    {
        Debug_setInvincibilityMode = !Debug_setInvincibilityMode;
    }

    //public void OnPause()
    //{
    //    GameManager.Instance.Pause();
    //}
}