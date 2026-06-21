namespace HastyControls.Core.Patches;

internal class PlayerEventsPatch : IHastyPatch
{
	public void Patch(HastyControlsMod mod)
	{
		On.Player.Awake += (orig, self) =>
		{
			orig(self);
			
			self.takeDamageAction += (damage, source, effect) => mod.Events.PlayerDamaged?.Invoke(self, damage, source, effect);
			self.pickUpCoinAction += () => mod.Events.PlayerSparkPickedUp?.Invoke(self);
			self.HealthChangedAction += (health) => mod.Events.PlayerHealthChanged?.Invoke(self, health);
		};
		
		On.Player.AddResource += (orig, self, amount, source) =>
		{
			orig(self, amount, source);
			mod.Events.PlayerResourceReceived?.Invoke(self, source, amount);
		};

		On.Player.SetEnergy += (orig, self, amount) =>
		{
			orig(self, amount);
			mod.Events.PlayerEnergyChanged?.Invoke(self, self.data.energy);
		};
	}
}
