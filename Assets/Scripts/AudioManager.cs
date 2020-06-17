using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    private int songIndex;
    [SerializeField] private AudioClip[] songs;
    [SerializeField] private AudioSource audioSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        SceneManager.sceneLoaded += this.Load;
    }

    private void Load(Scene scene, LoadSceneMode mode)
    {
        this.songIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!this.audioSource.isPlaying)
        {
            this.songIndex = (this.songIndex < this.songs.Length) ? this.songIndex : 0;

            this.audioSource.clip = this.songs[this.songIndex++];
            this.audioSource.Play();
        }
    }
}
