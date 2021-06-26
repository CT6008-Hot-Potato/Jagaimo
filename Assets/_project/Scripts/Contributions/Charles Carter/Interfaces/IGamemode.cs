/////////////////////////////////////////////////////////////
//
//  Script Name: IGamemode.cs
//  Creator: Charles Carter
//  Description: An interface that each gamemode has to use
//  
/////////////////////////////////////////////////////////////

public interface IGamemode
{
    //The contracted functions and variables that every gamemode will have
    //Based Gamemode Identifier
    GAMEMODE_INDEX Return_Mode();

    //Controlling active players
    void SetActivePlayers(CharacterManager[] charactersInGame);
    CharacterManager[] GetActivePlayers();

    void RemoveActivePlayer(CharacterManager charToRemove);
    void AddActivePlayer(CharacterManager charToAdd);

    void LockActivePlayers();
    void UnLockActivePlayers();
    void ForceEliminatePlayer(CharacterManager charEliminated);

    //Controlling events in the game
    void RoundStarted();
    void RoundEnded();
    void CountdownStarted();
    void CountdownEnded();
    void PlayerTagged(CharacterManager charTagged);
    bool WinCondition();
}
