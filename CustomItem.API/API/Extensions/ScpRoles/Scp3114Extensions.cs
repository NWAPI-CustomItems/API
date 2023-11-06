using MEC;
using Mirror;
using PlayerRoles.PlayableScps.Scp3114;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWAPI.CustomItems.API.Extensions.ScpRoles
{
    /// <summary>
    /// Provides extension methods for the <see cref="Scp3114Role"/>.
    /// </summary>
    public static class Scp3114Extensions
    {
        /// <summary>
        /// Changes the duration of Scp-3114's disguise ability. Warning this change will affect all SCP-3114.
        /// </summary>
        /// <param name="role">The 'Scp3114Role' instance to change the disguise duration for.</param>
        /// <param name="duration">The new duration in seconds.</param>
        public static void ChangeDisguiseDuration(this Scp3114Role role, float duration)
        {
            if (role == null)
                return;

            if (role.SubroutineModule.TryGetSubroutine<Scp3114Identity>(out var subroutine))
            {
                if (subroutine._disguiseDurationSeconds == duration)
                    return;

                subroutine._disguiseDurationSeconds = duration;
                //subroutine.ServerSendRpc(true);
            }
        }

        /// <summary>
        /// Sets Scp-3114's disguise with the provided name, roletype, and optional duration.
        /// </summary>
        /// <param name="role">The 'Scp3114Role' instance to set the disguise for.</param>
        /// <param name="name">The name of the disguise.</param>
        /// <param name="roletype">The roletype of the disguise.</param>
        /// <param name="duration">The duration of the disguise in seconds (default is -1, meaning infinite).</param>
        public static void SetDisguise(this Scp3114Role role, string name, RoleTypeId roletype, float duration = -1)
        {
            if (role == null)
                return;

            if (role.SubroutineModule.TryGetSubroutine<Scp3114Disguise>(out var subroutine))
            {
                var ragdoll = roletype.GetRagdoll(name, "SCP-3114");

                subroutine._identity.CurIdentity.Ragdoll = ragdoll;
                role.Ragdoll = ragdoll;
                subroutine._identity.RemainingDuration.NextUse = NetworkTime.time - 1;
                subroutine._identity.CurIdentity.Status = Scp3114Identity.DisguiseStatus.Active;
                subroutine._identity.ServerResendIdentity();
                subroutine._identity._wasDisguised = true;
                subroutine._identity.OnIdentityStatusChanged();

                if (duration != -1 && duration > 0)
                {
                    Timing.CallDelayed(0.2f, () =>
                    {
                        subroutine._identity.RemainingDuration.Trigger(duration);
                        subroutine._identity.ServerSendRpc(true);
                    });
                }
            }
        }

        /// <summary>
        /// Tries to set Scp-3114's disguise with the provided name, roletype, and optional duration.
        /// </summary>
        /// <param name="role">The 'Scp3114Role' instance to attempt to set the disguise for.</param>
        /// <param name="name">The name of the disguise.</param>
        /// <param name="roletype">The roletype of the disguise.</param>
        /// <param name="duration">The duration of the disguise in seconds (default is -1, meaning infinite).</param>
        /// <returns>True if the disguise was successfully set, otherwise false.</returns>
        public static bool TrySetDisguise(this Scp3114Role role, string name, RoleTypeId roletype, float duration = -1)
        {
            if (role == null)
                return false;

            if (role.SubroutineModule.TryGetSubroutine<Scp3114Disguise>(out var subroutine))
            {
                var ragdoll = roletype.GetRagdoll(name, "SCP-3114");

                subroutine._identity.CurIdentity.Ragdoll = ragdoll;
                role.Ragdoll = ragdoll;
                subroutine._identity.RemainingDuration.NextUse = NetworkTime.time - 1;
                subroutine._identity.CurIdentity.Status = Scp3114Identity.DisguiseStatus.Active;
                subroutine._identity.ServerResendIdentity();
                subroutine._identity._wasDisguised = true;
                subroutine._identity.OnIdentityStatusChanged();

                if (duration != -1 && duration > 0)
                {
                    Timing.CallDelayed(0.2f, () =>
                    {
                        subroutine._identity.RemainingDuration.Trigger(duration);
                        subroutine._identity.ServerSendRpc(true);
                    });
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the duration of Scp-3114's disguise ability.
        /// </summary>
        /// <param name="role">The 'Scp3114Role' instance to get the disguise duration for.</param>
        /// <returns>The current disguise duration in seconds, or -1 if the role is not found.</returns>
        public static float GetDisguiseDuration(this Scp3114Role role)
        {
            if (role == null)
                return -1;

            if (role.SubroutineModule.TryGetSubroutine<Scp3114Identity>(out var subroutine))
            {
                return subroutine._disguiseDurationSeconds;
            }

            return -1;
        }

        /// <summary>
        /// Tries to get the duration of Scp-3114's disguise ability.
        /// </summary>
        /// <param name="role">The 'Scp3114Role' instance to attempt to get the disguise duration for.</param>
        /// <param name="duration">The current disguise duration in seconds, or -1 if the role is not found.</param>
        /// <returns>True if the duration was successfully retrieved, otherwise false.</returns>
        public static bool TryGetDisguiseDuration(this Scp3114Role role, out float duration)
        {
            duration = GetDisguiseDuration(role);

            if (duration == -1)
                return false;
            else
                return true;
        }
    }
}
