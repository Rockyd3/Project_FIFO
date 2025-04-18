using UnityEngine;

public class TrashcanCollider : MonoBehaviour
{   
    // Called when another collider enters the trigger hitbox
    private void OnTriggerEnter(Collider other)
    {
        Trashcan trashCan = transform.parent.GetComponent<Trashcan>();
        Level_manager levelManager = Level_manager.instance.GetComponent<Level_manager>();
        if (levelManager.CurrentlySelectedUpgradeIndex >= levelManager.PlayerHeldUpgrades.Count)
        {
            levelManager.CurrentlySelectedUpgradeIndex = levelManager.PlayerHeldUpgrades.Count - 1;
        }
        // If the other collider is the player, and the player has at least one upgrade:
        if (other.gameObject.CompareTag("Player") && levelManager.PlayerHeldUpgrades.Count >= 1)
        {
            Level_manager.instance.RecyclePlayerUpgrade(transform.parent.gameObject);
            trashCan.upgrade = levelManager.PlayerHeldUpgrades[levelManager.CurrentlySelectedUpgradeIndex];
            trashCan.Label.SetActive(true);
            trashCan.Label.GetComponent<UpgradeLabel>().SetNewUpgrade(trashCan.upgrade);
            trashCan.Label.GetComponent<UpgradeLabel>().ChangeLabelTextBasedOnGameState(trashCan.upgrade);
            trashCan.MakeFullFormattedTextString();
            trashCan.IsTrashcanActive = true;
            // Update the TextMeshPro component according to the new active text
            Player_input_manager.instance.Interactable = transform.parent.gameObject;
            trashCan.DrawLinesToLabelCorners();
            
        }
    }

    // Called when another collider exits the trigger hitbox
    private void OnTriggerExit(Collider other)
    {
        Trashcan trashCan = transform.parent.GetComponent<Trashcan>();
        Level_manager levelManager = Level_manager.instance.GetComponent<Level_manager>();
        // If the other collider is the player, and the item has not already been bought:
        // Make the UI label invisible and disallow the player to interact with this instance's gameObject; make the lines invisble
        if (other.gameObject.CompareTag("Player") && levelManager.PlayerHeldUpgrades.Count >= 1)
        {
            trashCan.Label.SetActive(false);
            trashCan.IsTrashcanActive = false;
            trashCan.DrawLinesToLabelCorners();
        }
    }
}
