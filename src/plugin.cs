using UnityEngine;

namespace org.efool.subnautica.seaglide_sprint {

[BepInEx.BepInPlugin(
	org.efool.subnautica.seaglide_sprint.Info.FQN,
	org.efool.subnautica.seaglide_sprint.Info.title,
	org.efool.subnautica.seaglide_sprint.Info.version)]
[BepInEx.BepInDependency("com.snmodding.nautilus")]
public class Plugin : BepInEx.BaseUnityPlugin
{
	public static ConfigGlobal config { get; private set;}

	private void Awake()
	{
		config = Nautilus.Handlers.OptionsPanelHandler.RegisterModOptions<ConfigGlobal>();
		new HarmonyLib.Harmony(org.efool.subnautica.seaglide_sprint.Info.FQN).PatchAll();
	}
}

[Nautilus.Options.Attributes.Menu(Info.title)]
public class ConfigGlobal : Nautilus.Json.ConfigFile
{
	[Nautilus.Options.Attributes.Slider("Seaglide Sprint Addition", 0.0f, 20.0f, Step = 0.05f, DefaultValue = 0.0f, Format = "{0:F2}", Tooltip="Additive speed boost when sprinting with seaglide. Base swim speed is 6.64. Seaglide speed is 25.")]
	public float seaglideSprintAddition = 0.0f;

	[Nautilus.Options.Attributes.Slider("Seaglide Sprint Multiplier", 1.0f, 10.0f, Step = 0.05f, DefaultValue = 2.0f, Format = "{0:F2}", Tooltip="Multiplicative speed boost on final speed when sprinting.")]
	public float seaglideSprintMultiplier = 2.0f;
}

public static class Commands
{
	[Nautilus.Commands.ConsoleCommand("seaglide_sprint_addition")] public static void seaglide_sprint_addition(float x) { Plugin.config.seaglideSprintAddition = x; }
	[Nautilus.Commands.ConsoleCommand("seaglide_sprint_multiplier")] public static void seaglide_sprint_multiplier(float x) { Plugin.config.seaglideSprintMultiplier = x; }
}

[HarmonyLib.HarmonyPatch]
static class Patch
{
#if original
	[HarmonyLib.HarmonyPrefix]
	[HarmonyLib.HarmonyPatch(typeof(UnderwaterMotor), "AlterMaxSpeed")]
	public static bool UnderwaterMotor_AlterMaxSpeed(UnderwaterMotor __instance, float inMaxSpeed, ref float __result,
		ref float ___currentPlayerSpeedMultipler)
	{
		float num1 = inMaxSpeed;
		Inventory main = Inventory.main;
		Equipment equipment = main.equipment;
		ItemsContainer container = main.container;
		switch (equipment.GetTechTypeInSlot("Tank"))
		{
			case TechType.Tank:
			num1 -= 0.425f;
			break;
			case TechType.DoubleTank:
			num1 -= 0.5f;
			break;
			case TechType.PlasteelTank:
			num1 -= 0.10625f;
			break;
			case TechType.HighCapacityTank:
			num1 -= 0.6375f;
			break;
		}
		int count = container.GetCount(TechType.HighCapacityTank);
		float num2 = num1 - (float) count * 1.275f;
		if ((double) num2 < 2.0)
			num2 = 2f;
		if (equipment.GetTechTypeInSlot("Body") == TechType.ReinforcedDiveSuit)
			num2 = Mathf.Max(2f, num2 - 1f);
		bool flag = Player.main.motorMode == Player.MotorMode.Seaglide;
		if (!flag)
		{
			switch (equipment.GetTechTypeInSlot("Foots"))
			{
			case TechType.Fins:
				num2 += 1.9f;
				break;
			case TechType.UltraGlideFins:
				num2 += 3.2f;
				break;
			}
		}
		if ((flag ? 0 : ((Object) main.GetHeldTool() != (Object) null ? 1 : 0)) != 0)
			--num2;
		if ((double) __instance.gameObject.transform.position.y > (double) Player.main.GetWaterLevel())
			num2 *= 1.3f;
		___currentPlayerSpeedMultipler = Mathf.MoveTowards(___currentPlayerSpeedMultipler, __instance.playerSpeedModifier, 0.3f * Time.deltaTime);
		__result = num2 * ___currentPlayerSpeedMultipler;

		return false;
	}
#else
	[HarmonyLib.HarmonyPrefix]
	[HarmonyLib.HarmonyPatch(typeof(UnderwaterMotor), "AlterMaxSpeed")]
	public static bool UnderwaterMotor_AlterMaxSpeed(UnderwaterMotor __instance, float inMaxSpeed, ref float __result,
		ref float ___currentPlayerSpeedMultipler)
	{
		float speed = inMaxSpeed;

		switch ( Inventory.main.equipment.GetTechTypeInSlot("Tank") ) {
		case TechType.Tank            : speed -= 0.425f  ; break;
		case TechType.DoubleTank      : speed -= 0.5f    ; break;
		case TechType.PlasteelTank    : speed -= 0.10625f; break;
		case TechType.HighCapacityTank: speed -= 0.6375f ; break;
		}

		int count = Inventory.main.container.GetCount(TechType.HighCapacityTank);
		speed = speed - (float)count * 1.275f;
		if ( (double)speed < 2.0 )
			speed = 2f;

		if ( Inventory.main.equipment.GetTechTypeInSlot("Body") == TechType.ReinforcedDiveSuit )
			speed = Mathf.Max(2f, speed - 1f);

		if ( Player.main.motorMode != Player.MotorMode.Seaglide ) {
			switch ( Inventory.main.equipment.GetTechTypeInSlot("Foots") ) {
			case TechType.Fins          : speed += 1.9f; break;
			case TechType.UltraGlideFins: speed += 3.2f; break;
			}

			if ( Inventory.main.GetHeldTool() != null )
				--speed;
		}
		
		if ( GameInput.GetIsRunning() ) {
			// Player.main.motorMode seems to always be Dive instead of Seaglide
			var held = Inventory.main.GetHeld();
			if ( held != null && held.gameObject.GetComponent<Seaglide>() != null ) {
				speed += Plugin.config.seaglideSprintAddition;
				speed *= Plugin.config.seaglideSprintMultiplier;
			}
		}

		if ( (double)__instance.gameObject.transform.position.y > (double)Player.main.GetWaterLevel() )
			speed *= 1.3f;

		___currentPlayerSpeedMultipler = Mathf.MoveTowards(___currentPlayerSpeedMultipler, __instance.playerSpeedModifier, 0.3f * Time.deltaTime);
		__result = speed * ___currentPlayerSpeedMultipler;

		return false;
	}
#endif
}

} // org.efool.subnautica.seaglide_sprint