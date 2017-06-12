using UnityEngine;

/*
 * questa classe contiene tutte le costanti del gioco
 * e i metodi statici accesibili dagli altri script 
 */

public static class Consts
{
    public const float ACTION_DISTANCE = 1.5f;
	public const float MIN_ZOMBIE_SPAWN_TIME = 2f;
	public const float MAX_ZOMBIE_SPAWN_TIME = 6f;
    public const float ZOMBIE_RUNNING_SPEED = 1.8f;
    public const float ZOMBIE_ANIMATION_SPEED_SCALE = 0.4f;
    public const float ZOMBIE_ATTACK_SPEED_SCALE = 1.35f;
    public const float MIN_ZOMBIE_SOUND_TIME = 5f;
    public const float MAX_ZOMBIE_SOUND_TIME = 8f;
    public const int MAX_ZOMBIES_IN_SCENE = 20;
	public const int MAX_ARMOR = 100;
	private const int ARMOR_SCALING = 60;
    private const float CRIT_MULTIPLIER = 3f;

    public static float applyPercent(float number, float percent)
	{
		return (number * percent) / 100;
	}

	public static int CalculateDamage(double damageIn, double protection)
	{
		double percent = System.Math.Atan(protection/ARMOR_SCALING)*(100/System.Math.PI*2);
		double reduction = damageIn * percent / 100;
		return (int)(System.Math.Round((damageIn - reduction)));
	}

	public static int CalculateDamage(double damageIn, double protection, double critChance){
		if (Random.value * 100 < critChance) {
			damageIn *= CRIT_MULTIPLIER;
		}

		return CalculateDamage(damageIn,protection);
	}
}
