using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class WeaponShootEffect : MonoBehaviour
{
    private ParticleSystem shootEffectParticleSystem;

    private void Awake()
    {
        shootEffectParticleSystem = GetComponent<ParticleSystem>();
    }

    /// <summary>
    /// Set the Shoot Effect from the passed in WeaponShootEffectSO and aimAngle
    /// </summary>
    /// <param name="shootEffect"></param>
    /// <param name="aimAngle"></param>
    public void SetShootEffect(WeaponShootEffectSO shootEffect, float aimAngle)
    {
        SetShootEffectColorGradient(shootEffect.colorGradient);

        SetShootEffectParticleSystemStartingValues(shootEffect.duration, shootEffect.startParticleSize, shootEffect.startParticleSpeed, shootEffect.startLifetime, shootEffect.effectGravity, shootEffect.maxParticleNumber);

        SetShootEffectParticleSystemEmission(shootEffect.emissionRate, shootEffect.burstParticleNumber);

        SetEmitterRotation(aimAngle);

        SetShootEffectParticleSprite(shootEffect.sprite);

        SetShootEffectVelocityOverLifeTime(shootEffect.velocityOverLifetimeMin, shootEffect.velocityOverLifetimeMax);
    }

    /// <summary>
    /// Set the shoot effect particle system color gradient
    /// </summary>
    /// <param name="gradient"></param>
    private void SetShootEffectColorGradient(Gradient gradient)
    {
        ParticleSystem.ColorOverLifetimeModule colorOverLifetimeModule = shootEffectParticleSystem.colorOverLifetime;
        colorOverLifetimeModule.color = gradient;
    }

    /// <summary>
    /// Set shoot effect particle system starting values
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="startingParticleSize"></param>
    /// <param name="startingParticleSpeed"></param>
    /// <param name="startLifetime"></param>
    /// <param name="effectGravity"></param>
    /// <param name="maxParticles"></param>
    private void SetShootEffectParticleSystemStartingValues(float duration, float startingParticleSize, float startingParticleSpeed, float startLifetime, float effectGravity, int maxParticles)
    {
        ParticleSystem.MainModule mainModule = shootEffectParticleSystem.main;

        mainModule.duration = duration;
        mainModule.startSize = startingParticleSize;
        mainModule.startSpeed = startingParticleSpeed;
        mainModule.startLifetime = startLifetime;
        mainModule.gravityModifier = effectGravity;
        mainModule.maxParticles = maxParticles;
    }

    /// <summary>
    /// Set shoot effect particle system burst particle number
    /// </summary>
    /// <param name="emissionRate"></param>
    /// <param name="burstParticleNumber"></param>
    private void SetShootEffectParticleSystemEmission(int emissionRate, float burstParticleNumber)
    {
        ParticleSystem.EmissionModule emissionModule = shootEffectParticleSystem.emission;

        ParticleSystem.Burst burst = new ParticleSystem.Burst(0f, burstParticleNumber);
        emissionModule.SetBurst(0, burst);

        emissionModule.rateOverTime = emissionRate;
    }

    /// <summary>
    /// Set shoot effect particle system sprite
    /// </summary>
    /// <param name="sprite"></param>
    private void SetShootEffectParticleSprite(Sprite sprite)
    {
        ParticleSystem.TextureSheetAnimationModule textureSheetAnimationModule = shootEffectParticleSystem.textureSheetAnimation;

        textureSheetAnimationModule.SetSprite(0, sprite);
    }

    /// <summary>
    /// Set the emitter rotation to match the aim angle
    /// </summary>
    /// <param name="aimAngle"></param>
    private void SetEmitterRotation(float aimAngle)
    {
        transform.eulerAngles = new Vector3(0f, 0f, aimAngle);
    }

    /// <summary>
    /// Set the shoot effect velocity over lifetime
    /// </summary>
    /// <param name="minVelocity"></param>
    /// <param name="vector3"></param>
    private void SetShootEffectVelocityOverLifeTime(Vector3 minVelocity, Vector3 maxVelocity)
    {
        ParticleSystem.VelocityOverLifetimeModule velocityOverLifetimeModule = shootEffectParticleSystem.velocityOverLifetime;

        ParticleSystem.MinMaxCurve minMaxCurveX = new ParticleSystem.MinMaxCurve();
        minMaxCurveX.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveX.constantMin = minVelocity.x;
        minMaxCurveX.constantMax = maxVelocity.x;
        velocityOverLifetimeModule.x = minMaxCurveX;

        ParticleSystem.MinMaxCurve minMaxCurveY = new ParticleSystem.MinMaxCurve();
        minMaxCurveY.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveY.constantMin = minVelocity.y;
        minMaxCurveY.constantMax = maxVelocity.y;
        velocityOverLifetimeModule.y = minMaxCurveY;

        ParticleSystem.MinMaxCurve minMaxCurveZ = new ParticleSystem.MinMaxCurve();
        minMaxCurveZ.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveZ.constantMin = minVelocity.z;
        minMaxCurveZ.constantMax = maxVelocity.z;
        velocityOverLifetimeModule.z = minMaxCurveZ;
    }


}
