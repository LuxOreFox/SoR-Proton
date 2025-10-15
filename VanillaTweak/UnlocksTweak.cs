using RogueLibsCore;
using Neutron.Helper;

namespace Proton.VanillaTweak;

internal static class UnlocksTweak
{
    [RLSetup]
    private static void SubscribeEvent()
    {
        UnlockHelper.OnAdjustTaitUnlock += AdjustTraitCrooked;
    }

    #region TraitCrooked
    private static int _traitCrooked_Times = 0;

    private static void AdjustTraitCrooked(TraitUnlock unlock)
    {
        if (unlock.Name == VanillaTraits.Crooked || unlock.Name == VanillaTraits.Crooked2)
        {
            unlock.SpecialAbilities.Remove(VanillaAbilities.Handcuffs);
            unlock.LeadingTraits.Add(VanillaTraits.TheLaw);
            unlock.LeadingBigQuests.Add(VanillaAgents.Cop);
            _traitCrooked_Times++;
        }

        if (_traitCrooked_Times == 2)
        {
            UnlockHelper.OnAdjustTaitUnlock -= AdjustTraitCrooked;
        }
    }
    #endregion
}
