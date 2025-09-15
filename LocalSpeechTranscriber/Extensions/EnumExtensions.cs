using System.Reflection;
using System.Runtime.Serialization;

namespace ToolBuddy.LocalSpeechTranscriber.Extensions
{
    public static class EnumExtensions
    {
        public static string GetEnumMemberValue(
            this Enum enumValue)
        {
            MemberInfo? member = enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault();
            EnumMemberAttribute? attribute = member?.GetCustomAttribute<EnumMemberAttribute>();
            return attribute?.Value ?? throw new NotSupportedException($"Enum value '{enumValue}' does not have an EnumMember attribute.");
        }
    }
}