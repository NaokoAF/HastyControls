using Landfall.Modding;

namespace HastyControls.Core.Patches;

[LandfallPlugin]
internal static class PlayerEventsPatch
{
	static PlayerEventsPatch()
	{
		On.Player.Awake += (orig, self) =>
		{
			orig(self);
			
			self.takeDamageAction += (damage, source, effect) => Mod.Events.PlayerDamaged?.Invoke(self, damage, source, effect);
			self.pickUpCoinAction += () => Mod.Events.PlayerSparkPickedUp?.Invoke(self);
			self.HealthChangedAction += (health) => Mod.Events.PlayerHealthChanged?.Invoke(self, health);
		};
		
		On.Player.AddResource += (orig, self, amount, source) =>
		{
			orig(self, amount, source);
			Mod.Events.PlayerResourceReceived?.Invoke(self, source, amount);
		};

		On.Player.SetEnergy += (orig, self, amount) =>
		{
			orig(self, amount);
			Mod.Events.PlayerEnergyChanged?.Invoke(self, self.data.energy);
		};
	}
}
