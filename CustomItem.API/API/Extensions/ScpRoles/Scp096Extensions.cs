using PlayerRoles.PlayableScps.Scp096;
using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWAPI.CustomItems.API.Extensions.ScpRoles
{
    /// <summary>
    /// Provides extension methods for the <see cref="Scp096Role"/>.
    /// </summary>
    public static class Scp096Extensions
    {
        /// <summary>
        /// Ends SCP-096's enrage cycle.
        /// </summary>
        /// <param name="role"></param>
        /// <param name="clearTime"></param>
        public static void EndRage(this Scp096Role role, bool clearTime = true)
        {
            if (role == null)
                return;

            if (role.SubroutineModule.TryGetSubroutine<Scp096RageManager>(out var manager))
            {
                manager.ServerEndEnrage(clearTime);
            }
        }

        /// <summary>
        /// Adds the specified <paramref name="player"/> as an SCP-096 player.
        /// </summary>
        /// <param name="role"></param>
        /// <param name="player"></param>
        public static void AddTarget(this Scp096Role role, Player player)
        {
            if (role == null) return;

            if (role.SubroutineModule.TryGetSubroutine<Scp096TargetsTracker>(out var tracker))
            {
                tracker.AddTarget(player.ReferenceHub, true);
            }
        }

        /// <summary>
        /// Removes the specified <paramref name="target"/> from SCP-096's targets.
        /// </summary>
        /// <param name="role"></param>
        /// <param name="target"></param>
        public static void RemoveTarget(this Scp096Role role, Player target)
        {
            if (role == null) return;

            if (role.SubroutineModule.TryGetSubroutine<Scp096TargetsTracker>(out var tracker))
            {
                tracker.RemoveTarget(target.ReferenceHub);
            }
        }

        /// <summary>
        /// Returns whether or not the provided <paramref name="player"/> is a target of SCP-096.
        /// </summary>
        /// <param name="role"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public static bool HasTarget(this Scp096Role role, Player player)
        {
            if (role.SubroutineModule.TryGetSubroutine<Scp096TargetsTracker>(out var tracker))
            {
                return tracker.Targets.Contains(player.ReferenceHub);
            }
            return false;
        }

        /// <summary>
        /// Sets the Charge Ability Cooldown.
        /// </summary>
        /// <param name="role"></param>
        /// <param name="cooldown"></param>
        public static void SetChargeCooldown(this Scp096Role role, float cooldown)
        {
            if (role == null)
                return;

            if (role.SubroutineModule.TryGetSubroutine<Scp096ChargeAbility>(out var subroutine))
            {
                subroutine.Cooldown.Remaining = cooldown;
                subroutine.ServerSendRpc(true);
            }
        }

        /// <summary>
        /// Sets the Charge Ability duration.
        /// </summary>
        /// <param name="role"></param>
        /// <param name="duration"></param>
        public static void SetChargeDuration(this Scp096Role role, float duration)
        {
            if (role == null)
                return;

            if (role.SubroutineModule.TryGetSubroutine<Scp096ChargeAbility>(out var subroutine))
            {
                subroutine.Duration.Remaining = duration;
                subroutine.ServerSendRpc(true);
            }
        }

        /// <summary>
        /// Sets the amount of time before SCP-096 can be enraged again.
        /// </summary>
        /// <param name="role"></param>
        /// <param name="cooldown"></param>
        public static void SetEnrageCooldown(this Scp096Role role, float cooldown)
        {
            if (role == null)
                return;

            if (role.SubroutineModule.TryGetSubroutine<Scp096RageCycleAbility>(out var subroutine))
            {
                subroutine._activationTime.Remaining = cooldown;
                subroutine.ServerSendRpc(true);
            }
        }

        /// <summary>
        /// Sets enraged time left.
        /// </summary>
        /// <param name="role"></param>
        /// <param name="timeLeft"></param>
        public static void SetEnrageTime(this Scp096Role role, float timeLeft)
        {
            if (role == null)
                return;

            if (role.SubroutineModule.TryGetSubroutine<Scp096RageManager>(out var subroutine))
            {
                subroutine.EnragedTimeLeft = timeLeft;
                subroutine.ServerSendRpc(true);
            }
        }

        /// <summary>
        /// Sets enraged time left.
        /// </summary>
        /// <param name="role"></param>
        /// <param name="totalTime"></param>
        public static void SetTotalEnrageTime(this Scp096Role role, float totalTime)
        {
            if (role == null)
                return;

            if (role.SubroutineModule.TryGetSubroutine<Scp096RageManager>(out var subroutine))
            {
                subroutine.TotalRageTime = totalTime;
                subroutine.ServerSendRpc(true);
            }
        }
    }
}
