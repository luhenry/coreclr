// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Text;
using Internal.Runtime.CompilerServices;

namespace System
{
    // This is an experimental type and not referenced from CoreFx but needs to exists and be public so we can prototype in CoreFxLab.
    public sealed partial class Utf8String : IEquatable<Utf8String>
    {
        /*
         * STATIC FIELDS
         */

        public static readonly Utf8String Empty = FastAllocate(0);

        /*
         * INSTANCE FIELDS
         * Do not reorder these fields. They must match the layout of Utf8StringObject in object.h.
         */

        private readonly int _length;
        private readonly byte _firstByte;

        /*
         * OPERATORS
         */

        /// <summary>
        /// Compares two <see cref="Utf8String"/> instances for equality using a <see cref="StringComparison.Ordinal"/> comparer.
        /// </summary>
        public static bool operator ==(Utf8String a, Utf8String b) => Equals(a, b);

        /// <summary>
        /// Compares two <see cref="Utf8String"/> instances for inequality using a <see cref="StringComparison.Ordinal"/> comparer.
        /// </summary>
        public static bool operator !=(Utf8String a, Utf8String b) => !Equals(a, b);

        /*
         * INSTANCE PROPERTIES
         */

        /// <summary>
        /// Returns the length (in UTF-8 code units) of this instance.
        /// </summary>
        public int Length => _length;

        /*
         * METHODS
         */

        /// <summary>
        /// Returns a <em>mutable</em> reference to the first byte of this <see cref="Utf8String"/>
        /// (or the null terminator if the string is empty).
        /// </summary>
        /// <returns></returns>
        internal ref byte DangerousGetMutableReference() => ref Unsafe.AsRef(in _firstByte);

        /// <summary>
        /// Performs an equality comparison using a <see cref="StringComparison.Ordinal"/> comparer.
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is Utf8String other && this.Equals(other);
        }

        /// <summary>
        /// Performs an equality comparison using a <see cref="StringComparison.Ordinal"/> comparer.
        /// </summary>
        public bool Equals(Utf8String value)
        {
            // First, a very quick check for referential equality.

            if (ReferenceEquals(this, value))
            {
                return true;
            }

            // Otherwise, perform a simple bitwise equality check.

            return !(value is null)
                && this.Length == value.Length
                && SpanHelpers.SequenceEqual(ref this.DangerousGetMutableReference(), ref value.DangerousGetMutableReference(), (uint)Length);
        }

        /// <summary>
        /// Compares two <see cref="Utf8String"/> instances using a <see cref="StringComparison.Ordinal"/> comparer.
        /// </summary>
        public static bool Equals(Utf8String a, Utf8String b)
        {
            // First, a very quick check for referential equality.

            if (ReferenceEquals(a, b))
            {
                return true;
            }

            // Otherwise, perform a simple bitwise equality check.

            return !(a is null)
                && !(b is null)
                && a.Length == b.Length
                && SpanHelpers.SequenceEqual(ref a.DangerousGetMutableReference(), ref b.DangerousGetMutableReference(), (uint)a.Length);
        }

        /// <summary>
        /// Returns a hash code using a <see cref="StringComparison.Ordinal"/> comparison.
        /// </summary>
        public override int GetHashCode()
        {
            // TODO: Should this be using a different seed than String.GetHashCode?

            ulong seed = Marvin.DefaultSeed;
            return Marvin.ComputeHash32(ref DangerousGetMutableReference(), _length /* in bytes */, (uint)seed, (uint)(seed >> 32));
        }

        /// <summary>
        /// Gets an immutable reference that can be used in a <see langword="fixed"/> statement.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)] // for compiler use only
        public ref readonly byte GetPinnableReference() => ref _firstByte;

        /// <summary>
        /// Converts this <see cref="Utf8String"/> instance to a <see cref="string"/>.
        /// </summary>
        /// <remarks>
        /// Invalid subsequences are replaced with U+FFFD during conversion.
        /// </remarks>
        public override string ToString()
        {
            // TODO: Replace me with a better implementation.

            return Encoding.UTF8.GetString(new ReadOnlySpan<byte>(ref DangerousGetMutableReference(), Length));
        }
    }
}
