using UnityEngine;

public class LockManager : MonoBehaviour {

    public bool unlockedBronze;
    public bool unlockedSilver;
    public bool unlockedGold;

    public void Unlock(string lockName) {

        if (lockName == "bronce") unlockedBronze = true;
        if (lockName == "plata") unlockedSilver = true;
        if (lockName == "oro") unlockedGold = true;
        Debug.Log($"[LockManager] Unlock {lockName}");
    }
    public bool AllUnlocked() => unlockedBronze && unlockedSilver && unlockedGold;
    public void ResetLocksIfAvailable() {
        unlockedBronze = unlockedSilver = unlockedGold = false;
    }
}