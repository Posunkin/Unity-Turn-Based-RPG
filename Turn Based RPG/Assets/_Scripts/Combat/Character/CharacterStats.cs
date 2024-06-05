public class CharacterStats
{
    public int movePoints { get; private set; }
    public int initiative { get; private set; }

    public CharacterStats(int movePoints, int initiative)
    {
        this.movePoints = movePoints;
        this.initiative = initiative;
    }
}
