public class CharacterStats
{
    public int strenght { get; private set; }
    public int endurance { get; private set;}
    public int movePoints { get; private set; }
    public int initiative { get; private set; }

    public CharacterStats(int strenght, int endurance, int movePoints, int initiative)
    {
        this.strenght = strenght;
        this.endurance = endurance;
        this.movePoints = movePoints;
        this.initiative = initiative;
    }
}
