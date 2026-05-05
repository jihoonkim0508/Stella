public interface IBoss
{
    string BossName { get; }

    public float MaxHealth => MaxHealth;
    public float CurrentHealth => CurrentHealth;
    public int CurrentPhase => CurrentPhase;
    public bool IsDead => IsDead;

    void TakeDamage(float amount);
    void ChangePhase(int phaseIndex);
}