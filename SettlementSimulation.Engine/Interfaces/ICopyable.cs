namespace SettlementSimulation.Engine.Interfaces
{
    public interface ICopyable<out T> where T : class
    {
        T Copy();
    }
}