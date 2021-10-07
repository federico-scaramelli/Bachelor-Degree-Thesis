
public interface ISimulationState 
{
    void UpdateState();

    void EnterState();

    void ToNotStartedState();

    void ToWalkingState();

    void ToStreetViewState();
}
