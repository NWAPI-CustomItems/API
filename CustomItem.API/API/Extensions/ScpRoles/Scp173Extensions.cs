using Hazards;
using Mirror;
using PlayerRoles.PlayableScps.Scp173;
using RelativePositioning;
using UnityEngine;

namespace NWAPI.CustomItems.API.Extensions.ScpRoles
{
    /// <summary>
    /// Provides extension methods for the <see cref="Scp173Role"/>.
    /// </summary>
    public static class Scp173Extensions
    {
        /// <summary>
        /// Resets the cooldown for the Scp-173 Breakneck Speeds ability.
        /// </summary>
        /// <param name="role">The 'Scp173Role' instance to reset the cooldown for.</param>
        public static void ResetBreakneckCooldown(this Scp173Role role)
        {
            if (role == null)
                return;

            if (role.SubroutineModule.TryGetSubroutine<Scp173BreakneckSpeedsAbility>(out var subroutine))
            {
                subroutine.Cooldown.Clear();
                subroutine.ServerSendRpc(true);
            }
        }

        /// <summary>
        /// Sets the cooldown for the Scp-173 Breakneck Speeds ability.
        /// </summary>
        /// <param name="role">The 'Scp173Role' instance to set the cooldown for.</param>
        /// <param name="cooldown">The new cooldown duration in seconds.</param>
        public static void SetBreakneckCooldown(this Scp173Role role, float cooldown)
        {
            if (role == null)
                return;

            if (role.SubroutineModule.TryGetSubroutine<Scp173BreakneckSpeedsAbility>(out var subroutine))
            {
                subroutine.Cooldown.Remaining = cooldown;
                subroutine.ServerSendRpc(true);
            }
        }

        /// <summary>
        /// Resets the cooldown for the Scp-173 Tantrum ability.
        /// </summary>
        /// <param name="role">The 'Scp173Role' instance to reset the cooldown for.</param>
        public static void ResetTantrumCooldown(this Scp173Role role)
        {
            if (role == null)
                return;

            if (role.SubroutineModule.TryGetSubroutine<Scp173TantrumAbility>(out var subroutine))
            {
                if (subroutine.Cooldown.IsReady)
                    return;

                subroutine.Cooldown.Clear();
                subroutine.ServerSendRpc(true);
            }
        }

        /// <summary>
        /// Sets the cooldown for the Scp-173 Tantrum ability.
        /// </summary>
        /// <param name="role">The 'Scp173Role' instance to set the cooldown for.</param>
        /// <param name="cooldown">The new cooldown duration in seconds.</param>
        public static void SetTantrumCooldown(this Scp173Role role, float cooldown)
        {
            if (role == null)
                return;

            if (role.SubroutineModule.TryGetSubroutine<Scp173TantrumAbility>(out var subroutine))
            {
                subroutine.Cooldown.Remaining = cooldown;
                subroutine.ServerSendRpc(true);
            }
        }

        /// <summary>
        /// Resets the cooldown for the Scp-173 Blink ability.
        /// </summary>
        /// <param name="role">The 'Scp173Role' instance to reset the cooldown for.</param>
        public static void ResetBlinkCooldown(this Scp173Role role)
        {
            if (role == null)
                return;

            if (role.SubroutineModule.TryGetSubroutine<Scp173BlinkTimer>(out var subroutine))
            {
                subroutine._initialStopTime = NetworkTime.time;
                subroutine._totalCooldown = 0.1f;
                subroutine.ServerSendRpc(true);
            }
        }

        /// <summary>
        /// Sets the cooldown for the Scp-173 Blink ability.
        /// </summary>
        /// <param name="role">The 'Scp173Role' instance to set the cooldown for.</param>
        /// <param name="cooldown">The new cooldown duration in seconds.</param>
        public static void SetBlinkCooldown(this Scp173Role role, float cooldown)
        {
            if (role == null)
                return;

            if (role.SubroutineModule.TryGetSubroutine<Scp173BlinkTimer>(out var subroutine))
            {
                subroutine._initialStopTime = NetworkTime.time;
                subroutine._totalCooldown = cooldown;
                subroutine.ServerSendRpc(true);
            }
        }

        /// <summary>
        /// Triggers the Scp-173 Tantrum ability with an optional cooldown.
        /// </summary>
        /// <param name="role">The 'Scp173Role' instance to trigger the tantrum ability for.</param>
        /// <param name="cooldown">The cooldown duration in seconds (default is 0).</param>
        /// <param name="failIfIsObserved">Indicates whether to trigger the tantrum if being observed (default is false).</param>
        public static void PlaceTantrum(this Scp173Role role, float cooldown = 0, bool failIfIsObserved = false)
        {
            if (role == null)
                return;

            if (failIfIsObserved && IsObserved(role))
                return;

            if (role.SubroutineModule.TryGetSubroutine<Scp173TantrumAbility>(out var subroutine))
            {
                subroutine.Cooldown.Trigger(cooldown);

                TantrumEnvironmentalHazard tantrumEnvironmentalHazard = UnityEngine.Object.Instantiate(subroutine._tantrumPrefab);
                Vector3 targetPos = role._owner.transform.position + Vector3.up * 1.25f;
                tantrumEnvironmentalHazard.SynchronizedPosition = new RelativePosition(targetPos);
                NetworkServer.Spawn(tantrumEnvironmentalHazard.gameObject);
                foreach (TeslaGate teslaGate in TeslaGateController.Singleton.TeslaGates)
                {
                    if (teslaGate.IsInIdleRange(subroutine.Owner))
                    {
                        teslaGate.TantrumsToBeDestroyed.Add(tantrumEnvironmentalHazard);
                    }
                }

                subroutine.ServerSendRpc(true);

            }
        }

        /// <summary>
        /// Checks if the Scp-173 role is currently being observed.
        /// </summary>
        /// <param name="role">The 'Scp173Role' instance to check.</param>
        /// <returns>True if being observed, otherwise false.</returns>
        public static bool IsObserved(this Scp173Role role)
        {
            if (role == null)
                return true;

            if (role.SubroutineModule.TryGetSubroutine<Scp173ObserversTracker>(out var subroutine))
            {
                return subroutine.IsObserved;
            }

            return false;
        }
    }
}
