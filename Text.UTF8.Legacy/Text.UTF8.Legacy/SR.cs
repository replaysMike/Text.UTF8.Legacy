namespace Text.UTF8.Legacy
{
    /// <summary>
    /// Replacement for .Net's internally managed resource strings for exceptions
    /// </summary>
    public static class SR
    {
        public static string Argument_InvalidCharSequenceNoIndex = nameof(Argument_InvalidCharSequenceNoIndex);
        public static string Argument_RecursiveFallbackBytes = nameof(Argument_RecursiveFallbackBytes);
        public static string ArgumentNull_Array = nameof(ArgumentNull_Array);
        public static string ArgumentOutOfRange_NeedNonNegNum = nameof(ArgumentOutOfRange_NeedNonNegNum);
        public static string ArgumentOutOfRange_IndexCountBuffer = nameof(ArgumentOutOfRange_IndexCountBuffer);
        public static string ArgumentOutOfRange_Index = nameof(ArgumentOutOfRange_Index);
        public static string ArgumentOutOfRange_Range = nameof(ArgumentOutOfRange_Range);
        public static string Argument_InvalidCodePageConversionIndex = nameof(Argument_InvalidCodePageConversionIndex);
        public static string Arg_ArgumentException = nameof(Arg_ArgumentException);
        public static string Argument_RecursiveFallback = nameof(Argument_RecursiveFallback);
        public static string ArgumentOutOfRange_IndexCount = nameof(ArgumentOutOfRange_IndexCount);
        public static string ArgumentOutOfRange_GetByteCountOverflow = nameof(ArgumentOutOfRange_GetByteCountOverflow);
        public static string ArgumentOutOfRange_GetCharCountOverflow = nameof(ArgumentOutOfRange_GetCharCountOverflow);
        public static string ArgumentOutOfRange_MustBePositive = nameof(ArgumentOutOfRange_MustBePositive);
        public static string ArgumentOutOfRange_MustBeNonNegNum = nameof(ArgumentOutOfRange_MustBeNonNegNum);
        public static string ArgumentOutOfRange_StartIndex = nameof(ArgumentOutOfRange_StartIndex);
        public static string ArgumentOutOfRange_IndexLength = nameof(ArgumentOutOfRange_IndexLength);
        public static string ArgumentOutOfRange_Capacity = nameof(ArgumentOutOfRange_Capacity);
        public static string ArgumentOutOfRange_SmallMaxCapacity = nameof(ArgumentOutOfRange_SmallMaxCapacity);
        public static string Serialization_StringBuilderMaxCapacity = nameof(Serialization_StringBuilderMaxCapacity);
        public static string Serialization_StringBuilderCapacity = nameof(Serialization_StringBuilderCapacity);
        public static string ArgumentOutOfRange_NegativeCapacity = nameof(ArgumentOutOfRange_NegativeCapacity);
        public static string ArgumentOutOfRange_SmallCapacity = nameof(ArgumentOutOfRange_SmallCapacity);
        public static string Argument_EncoderFallbackNotEmpty = nameof(Argument_EncoderFallbackNotEmpty);

        public static string Format(string format, params object[] args)
        {
            return string.Format(format, args);
        }
    }
}
