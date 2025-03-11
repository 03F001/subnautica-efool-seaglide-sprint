using Nautilus.Commands;
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
	[Nautilus.Options.Attributes.Slider("Seaglide Sprint multiplier", 1.0f, 10.0f, Step = 0.05f, DefaultValue = 1.0f, Format = "{0:F2}")]
	public float seaglideSprintMultiplier = 2.0f;
}

public static class Commands
{
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

		bool seaglideActive = Player.main.motorMode == Player.MotorMode.Seaglide;
		if ( !seaglideActive ) {
			switch ( Inventory.main.equipment.GetTechTypeInSlot("Foots") ) {
			case TechType.Fins          : speed += 1.9f; break;
			case TechType.UltraGlideFins: speed += 3.2f; break;
			}

			if ( Inventory.main.GetHeldTool() != null )
				--speed;
		}

		if ( (double)__instance.gameObject.transform.position.y > (double)Player.main.GetWaterLevel() )
			speed *= 1.3f;

		if ( GameInput.GetIsRunning() )
			speed *= Plugin.config.seaglideSprintMultiplier;

		___currentPlayerSpeedMultipler = Mathf.MoveTowards(___currentPlayerSpeedMultipler, __instance.playerSpeedModifier, 0.3f * Time.deltaTime);
		__result = speed * ___currentPlayerSpeedMultipler;

		return false;
	}
#endif
}

} // org.efool.subnautica.seaglide_sprint