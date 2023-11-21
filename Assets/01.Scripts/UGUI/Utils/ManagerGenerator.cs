using Photon.Pun;
using UnityEngine;

public class ManagerGenerator : MonoBehaviour
{
    [SerializeField] private GameObject _manager;
    
    private void Awake()
    {
        if (GameManager.Instance == null)
        {
            var manager = Instantiate(_manager);
            var photonView = manager.GetComponent<PhotonView>();
            photonView.ViewID = 1;
        }
    }
}
