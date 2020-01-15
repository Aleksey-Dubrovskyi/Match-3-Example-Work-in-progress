using UnityEngine;

public enum Clip { Select, Swap, Clear, Snap, Click, TileSelect };

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    private AudioSource[] sfx;

    // Use this for initialization

    private void Start()
    {
        Instance = GetComponent<SFXManager>();
        sfx = GetComponents<AudioSource>();
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
            {
                this.gameObject.SetActive(false);
            }
            else
            {
                this.gameObject.SetActive(true);
            }
        }
        else
        {
            this.gameObject.SetActive(false);
        }
        //if (Instance == null)
        //{
        //    DontDestroyOnLoad(this.gameObject);
        //    Instance = this;
        //}
        //else
        //{
        //    Destroy(this.gameObject);
        //}
    }

    public void PlaySFX(Clip audioClip)
    {
        sfx[(int)audioClip].Play();
    }
}
