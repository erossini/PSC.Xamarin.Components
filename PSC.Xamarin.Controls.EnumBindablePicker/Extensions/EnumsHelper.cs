using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PSC.Xamarin.Controls.EnumBindablePicker.Extensions {
    /// <summary>
    /// Enums Helper
    /// </summary>
    public static class EnumsHelper {
        /// <summary>
        /// Gets the type of the attribute of.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumVal">The enum value.</param>
        /// <returns></returns>
        public static T GetAttributeOfType<T>(this Enum enumVal) where T : Attribute {
            var typeInfo = enumVal.GetType().GetTypeInfo();
            var v = typeInfo.DeclaredMembers.First(x => x.Name == enumVal.ToString());
            return v.GetCustomAttribute<T>();
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <param name="enumVal">The enum value.</param>
        /// <returns></returns>
        public static string GetDescription(this Enum enumVal) {
            var attr = GetAttributeOfType<DescriptionAttribute>(enumVal);
            return attr != null ? attr.Text : string.Empty;
        }
    }
}