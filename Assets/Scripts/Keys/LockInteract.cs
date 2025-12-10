using UnityEngine;

public class LockInteract : MonoBehaviour
{
    public float interactDistance = 3f;
    public Lock targetLock;

    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (Vector3.Distance(player.position, transform.position) <= interactDistance)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                PlayerInventory inv = player.GetComponent<PlayerInventory>();
                targetLock.TryUnlock(inv);
            }
        }
    }
}
