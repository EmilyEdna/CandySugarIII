using System;

namespace CandySugar.Com.Library.KeepOn
{
    [Flags]
    public enum ExecutionState : uint
    {
        /// <summary>
        /// Forces the system to be in the working state by resetting the system idle timer.
        /// </summary>
        SystemRequired = 0x01,

        /// <summary>
        /// Forces the display to be on by resetting the display idle timer.
        /// </summary>
        DisplayRequired = 0x02,

        /// <summary>
        /// Enables away mode. This value must be specified with <see cref="Continuous"/>.
        /// <para />
        /// Away mode should be used only by media-recording and media-distribution applications that must perform critical background processing on desktop computers while the computer appears to be sleeping.
        /// </summary>
        AwaymodeRequired = 0x40,

        /// <summary>
        /// Informs the system that the state being set should remain in effect until the next call that uses <see cref="Continuous"/> and one of the other state flags is cleared.
        /// </summary>
        Continuous = 0x80000000,
    }
}
