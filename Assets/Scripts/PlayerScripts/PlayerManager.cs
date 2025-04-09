using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    /* Singletion */
    static PlayerManager _instance;
    public static PlayerManager Instance => _instance;

    /* Player status */
    

    /* Assign on inspector */


    /* Assign in script */


    /* Not organized: temp field */


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

}
