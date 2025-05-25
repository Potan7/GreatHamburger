using UnityEngine;

public class TrashCan : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ingredient"))
        {
            
            Destroy(other.gameObject);
        }
    }
}
