using System;
using System.Linq;
using Xamarin.Forms;
using PSC.Xamarin.Controls.EnumBindablePicker.Extensions;

namespace PSC.Xamarin.Controls.EnumBindablePicker {
    /// <summary>
    /// EnumBindablePicker
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Xamarin.Forms.Picker" />
    public class EnumBindablePicker<T> : Picker where T : struct {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumBindablePicker{T}"/> class.
        /// </summary>
        public EnumBindablePicker() {
            this.BindingContextChanged += EnumBindablePicker_BindingContextChanged;
            SelectedIndexChanged += OnSelectedIndexChanged;

            // Fill the Items from the enum
            foreach (Enum value in Enum.GetValues(typeof(T))) {
                Items.Add(value.GetDescription());
            }
        }

        /// <summary>
        /// Handles the BindingContextChanged event of the EnumBindablePicker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void EnumBindablePicker_BindingContextChanged(object sender, EventArgs e) {
            // if the current value is the same as the default,
            // it wouldn't recognize the change. Force OnSelectedItemChanged to handle this case.
            OnSelectedItemChanged(this, SelectedItem, SelectedItem);
        }

        /// <summary>
        /// The selected item property
        /// </summary>
        public static BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(T), typeof(EnumBindablePicker<T>), default(T), propertyChanged: OnSelectedItemChanged, defaultBindingMode: BindingMode.TwoWay);

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        /// <value>
        /// The selected item.
        /// </value>
        public T SelectedItem
        {
            get {
                return (T)GetValue(SelectedItemProperty);
            }
            set {
                SetValue(SelectedItemProperty, value);
            }
        }

        /// <summary>
        /// Called when [selected index changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnSelectedIndexChanged(object sender, EventArgs eventArgs) {
            if (SelectedIndex < 0 || SelectedIndex > Items.Count - 1) {
                SelectedItem = default(T);
            }
            else {
                // try parsing, if using description this will fail
                T match;
                if (!Enum.TryParse<T>(Items[SelectedIndex], out match)) {
                    // find enum by Description
                    match = GetEnumByDescription(Items[SelectedIndex]);
                }
                SelectedItem = (T)Enum.Parse(typeof(T), match.ToString());
            }
        }

        /// <summary>
        /// Called when [selected item changed].
        /// </summary>
        /// <param name="bindable">The bindable.</param>
        /// <param name="oldvalue">The oldvalue.</param>
        /// <param name="newvalue">The newvalue.</param>
        private static void OnSelectedItemChanged(BindableObject bindable, object oldvalue, object newvalue) {
            var picker = bindable as EnumBindablePicker<T>;
            if (newvalue != null) {
                picker.SelectedIndex = picker.Items.IndexOf(newvalue.ToString());
            }
        }

        /// <summary>
        /// Gets the enum by description.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <returns></returns>
        private T GetEnumByDescription(string description) {
            return Enum.GetValues(typeof(T)).Cast<T>().FirstOrDefault(x => string.Equals(Enum.Parse(typeof(T), x.ToString()), description));
        }
    }
}