using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Text.UTF8.Legacy
{
    public static class MemoryMarshal
    {
        internal static ref readonly T GetNonNullPinnableReference<T>(ReadOnlySpan<T> span) => ref span.GetPinnableReference();

        internal static ref readonly T GetNonNullPinnableReference<T>(Span<T> span) => ref span.GetPinnableReference();

        internal static ref readonly T GetReference<T>(Span<T> span) => ref System.Runtime.InteropServices.MemoryMarshal.GetReference(span);
    }
}
