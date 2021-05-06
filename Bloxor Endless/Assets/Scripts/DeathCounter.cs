using UnityEngine;

public class DeathCounter : MonoBehaviour {

    private static DeathCounter instance = null;

    public int ResetCounter { get; internal set; }

    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    internal void IncreaseDeathCounter() {
        ResetCounter += 1;
    }
}
