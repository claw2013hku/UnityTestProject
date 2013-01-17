var Player : Transform;
function OnNetworkLoadedLevel () {
 // Instantiating SpaceCraft when Network is loaded
 Network.Instantiate(Player, transform.position, transform.rotation, 0);
}
function OnPlayerDisconnected (player : NetworkPlayer) {
 // Removing player if Network is disconnected
 Debug.Log("Server destroying player");
 Network.RemoveRPCs(player, 0);
 Network.DestroyPlayerObjects(player);
}