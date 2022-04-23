namespace SeinAbilities
{
    public abstract class CustomSeinAbility : CharacterState
    {
        public abstract bool AllowAbility(SeinLogicCycle logicCycle);
    }
}
