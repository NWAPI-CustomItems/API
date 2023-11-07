using PlayerRoles.PlayableScps.Scp106;

namespace NWAPI.CustomItems.API.Extensions.ScpRoles
{
    /// <summary>
    /// Provides extension methods for the <see cref="Scp106Role"/>.
    /// </summary>
    public static class Scp106Extensions
    {
        /// <summary>
        /// Sets a value indicating whether or not SCP-106 is currently submerged.
        /// </summary>
        public static void SetSubmergedStatus(this Scp106Role role, bool isSubmerged)
        {
            if (role == null)
                return;

            if (role.SubroutineModule.TryGetSubroutine<Scp106HuntersAtlasAbility>(out var subroutine))
            {
                subroutine.SetSubmerged(isSubmerged);
            }
        }

        /// <summary>
        /// Sets the amount of time in between player captures.
        /// </summary>
        /// <param name="role"></param>
        /// <param name="cooldown"></param>
        public static void SetCapturePoint(this Scp106Role role, float cooldown)
        {
            if (role == null)
                return;

            if (role.SubroutineModule.TryGetSubroutine<Scp106Attack>(out var subroutine))
            {
                subroutine._hitCooldown = cooldown;
            }
        }

        /// <summary>
        /// Resets the Sinkhole cooldown.
        /// </summary>
        /// <param name="role"></param>
        public static void ResetSinkholeCooldown(this Scp106Role role)
        {
            if (role == null)
                return;

            if (role.SubroutineModule.TryGetSubroutine<Scp106SinkholeController>(out var subroutine))
            {
                subroutine.Cooldown.Clear();
                subroutine.ServerSendRpc(true);
            }
        }

        /// <summary>
        /// Sets the Sinkhole cooldown.
        /// </summary>
        /// <param name="role"></param>
        /// <param name="cooldown"></param>
        public static void SetSinkholeCooldown(this Scp106Role role, float cooldown)
        {
            if (role == null)
                return;

            if (role.SubroutineModule.TryGetSubroutine<Scp106SinkholeController>(out var subroutine))
            {
                subroutine.Cooldown.Remaining = cooldown;
                subroutine.ServerSendRpc(true);
            }
        }

        /// <summary>
        /// Sets a value indicating whether or not SCP-106 will enter his stalking mode.
        /// </summary>
        /// <param name="role"></param>
        /// <param name="isStalking"></param>
        public static void SetStalking(this Scp106Role role, bool isStalking)
        {
            if (role == null)
                return;

            if (role.SubroutineModule.TryGetSubroutine<Scp106StalkAbility>(out var subroutine))
            {
                subroutine.IsActive = isStalking;
            }
        }

        /// <summary>
        /// Sets SCP-106's Vigor.
        /// </summary>
        /// <param name="role"></param>
        /// <param name="value"></param>
        public static void SetVigor(this Scp106Role role, float value)
        {
            if (role == null)
                return;

            if (role.SubroutineModule.TryGetSubroutine<Scp106VigorAbilityBase>(out var subroutine))
            {
                subroutine.VigorAmount = value;
            }
        }

        /// <summary>
        /// Gets SCP-106's Vigor.
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static float GetVigor(this Scp106Role role)
        {
            if (role == null)
                return 0;

            if (role.SubroutineModule.TryGetSubroutine<Scp106VigorAbilityBase>(out var subroutine))
            {
                return subroutine.VigorAmount;
            }

            return 0;
        }
    }
}
