namespace OriDeModLoader.CustomSeinAbilities
{
    public abstract class CustomSeinAbility : CharacterState, ISeinReceiver
    {
        public abstract bool AllowAbility(SeinLogicCycle logicCycle);
        public SeinCharacter Sein { get; private set; }
        public void SetReferenceToSein(SeinCharacter sein) => Sein = sein;
    }
}
