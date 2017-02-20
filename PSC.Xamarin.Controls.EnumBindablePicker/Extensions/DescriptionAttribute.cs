using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSC.Xamarin.Controls.EnumBindablePicker.Extensions {
    /// <summary>
    /// Description Attribute definition
    /// </summary>
    /// <seealso cref="System.Attribute" />
    public class DescriptionAttribute : Attribute {
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public virtual string Text { get; set; }
    }
}