using UnityEngine;
using UnityEngine.Networking;

public class PlayerConnectionObject : NetworkBehaviour
{

    // Use this for initialization
    void Start()
    {

        //is this my own local PlayerConnectionObject?
        if (!isLocalPlayer)
            return;


        Debug.Log("PlayerObject -- Spawning my own personal unit.");

        CmdSpawnMyUnit();
    }

    void Update()
    {
        //Remember: Update runs on EVERYONE's computer, whether or not they own this
        //particular object
    }

    //GameObject myPlayerUnit;

    public GameObject PlayerUnit;
    
    [Command]
    void CmdSpawnMyUnit()
    {
        // On server right now
        GameObject go = Instantiate(PlayerUnit);

        // Now that the object exists on the server, propagate it
        //to all the clients (and also wire it up with NetworkIdentity)
        NetworkServer.SpawnWithClientAuthority(go, connectionToClient);
    }

    //[Command]
    //void CmdMoveUnitOrWhateverIWantInUpdateOfPlayerController()
    //{
    //    if (myPlayerUnit == null)
    //        return;

    //        //blabla samo sta je on uradio, sto ja trebam razumjeti prvo kako zelim da uradim
    //    myPlayerUnit.transform.Translate(0, 1, 0);
    //}
}
