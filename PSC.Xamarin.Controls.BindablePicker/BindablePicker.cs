using System;
using Xamarin.Forms;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using PSC.Xamarin.Controls.BindablePicker.EventsArgs;

namespace PSC.Xamarin.Controls.BindablePicker {
    /// <summary>
    /// Bindable Picker
    /// </summary>
    /// <seealso cref="Xamarin.Forms.Picker" />
    public class BindablePicker : Picker {
        /// <summary>
        /// Initializes a new instance of the <see cref="BindablePicker"/> class.
        /// </summary>
        public BindablePicker() {
            base.SelectedIndexChanged += OnSelectedIndexChanged;
        }

        public static void Init() { }

        #region Events 
        /// <summary>
        /// Form error handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ParamErrorEventArgs"/> instance containing the event data.</param>
        public delegate void BindableSelectedChangedHandler(object sender, BindableSelectedChangedEventArgs e);

        /// <summary>
        /// Occurs when parameter error
        /// </summary>
        public event BindableSelectedChangedHandler BindableSelectedChanged;

        /// <summary>
        /// Handles the <see cref="E:ParamError" /> event.
        /// </summary>
        /// <param name="e">The <see cref="ParamErrorEventArgs" /> instance containing the event data.</param>
        protected virtual void OnBindableSelectedChanged(BindableSelectedChangedEventArgs e) {
            this.BindableSelectedChanged?.Invoke(this, e);
        }
        #endregion

        /// <summary>
        /// The selected item property
        /// </summary>
        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create("SelectedItem", typeof(object), typeof(BindablePicker), null, BindingMode.TwoWay, null, new BindableProperty.BindingPropertyChangedDelegate(BindablePicker.OnSelectedItemChanged), null, null, null);

        /// <summary>
        /// The items source property
        /// </summary>
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create("ItemsSource", typeof(IEnumerable), typeof(BindablePicker), null, BindingMode.OneWay, null, new BindableProperty.BindingPropertyChangedDelegate(BindablePicker.OnItemsSourceChanged), null, null, null);

        /// <summary>
        /// The display property property
        /// </summary>
        public static readonly BindableProperty DisplayPropertyProperty = BindableProperty.Create("DisplayProperty", typeof(string), typeof(BindablePicker), null, BindingMode.OneWay, null, new BindableProperty.BindingPropertyChangedDelegate(BindablePicker.OnDisplayPropertyChanged), null, null, null);
        private bool disableEvents;

        /// <summary>
        /// Gets or sets the items source.
        /// </summary>
        /// <value>The items source.</value>
        public IList ItemsSource
        {
            get { return (IList)base.GetValue(BindablePicker.ItemsSourceProperty); }
            set { base.SetValue(BindablePicker.ItemsSourceProperty, value); }
        }

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        /// <value>The selected item.</value>
        public object SelectedItem
        {
            get { return base.GetValue(BindablePicker.SelectedItemProperty); }
            set {
                base.SetValue(BindablePicker.SelectedItemProperty, value);
                if (ItemsSource != null && SelectedItem != null) {
                    if (ItemsSource.Contains(SelectedItem)) {
                        SelectedIndex = ItemsSource.IndexOf(SelectedItem);
                    }
                    else {
                        SelectedIndex = SelectedDisplayTextChanged();
                    }
                }
            }
        }

        /// <summary>
        /// Selected the display text changed.
        /// </summary>
        /// <returns>Item Index or -1 if an item doesn't exists</returns>
        private int SelectedDisplayTextChanged() {
            int rtn = -1;

            int index = 0;
            string itm = "";
            var propItem = SelectedItem.GetType().GetRuntimeProperties().FirstOrDefault(p => string.Equals(p.Name, DisplayProperty, StringComparison.OrdinalIgnoreCase));
            if (propItem != null) {
                itm = propItem.GetValue(SelectedItem).ToString();
            }
            else {
                itm = propItem.ToString();
            }

            foreach (object obj in (IEnumerable)ItemsSource) {
                string tmp = string.Empty;
                if (DisplayProperty != null) {
                    var prop = obj.GetType().GetRuntimeProperties().FirstOrDefault(p => string.Equals(p.Name, DisplayProperty, StringComparison.OrdinalIgnoreCase));
                    if (prop != null) {
                        tmp = prop.GetValue(obj).ToString();
                    }
                    else {
                        tmp = obj.ToString();
                    }
                }
                else {
                    tmp = obj.ToString();
                }
                if (tmp == itm) {
                    SelectedIndex = index;
                    rtn = index;
                    break;
                }
                index++;
            }

            return rtn;
        }

        /// <summary>
        /// Gets or sets the display property.
        /// </summary>
        /// <value>The display property.</value>
        public string DisplayProperty
        {
            get { return (string)base.GetValue(BindablePicker.DisplayPropertyProperty); }
            set { base.SetValue(BindablePicker.DisplayPropertyProperty, value); }
        }

        /// <summary>
        /// Handles the <see cref="E:SelectedIndexChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnSelectedIndexChanged(object sender, EventArgs e) {
            if (disableEvents) return;

            if (SelectedIndex == -1) {
                this.SelectedItem = null;
                OnBindableSelectedChanged(new BindableSelectedChangedEventArgs() { Item = null, Index = -1 });
            }
            else {
                this.SelectedItem = ItemsSource[SelectedIndex];
                OnBindableSelectedChanged(new BindableSelectedChangedEventArgs() { Item = this.SelectedItem, Index = SelectedIndex });
            }
        }

        /// <summary>
        /// Called when [selected item changed].
        /// </summary>
        /// <param name="bindable">The bindable.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private static void OnSelectedItemChanged(BindableObject bindable, object oldValue, object newValue) {
            BindablePicker picker = (BindablePicker)bindable;
            picker.SelectedItem = newValue;
            if (picker.ItemsSource != null && picker.SelectedItem != null) {
                int count = 0;
                foreach (object obj in picker.ItemsSource) {
                    if (obj == picker.SelectedItem) {
                        picker.SelectedIndex = count;
                        break;
                    }
                    count++;
                }
            }
        }

        /// <summary>
        /// Called when [display property changed].
        /// </summary>
        /// <param name="bindable">The bindable.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private static void OnDisplayPropertyChanged(BindableObject bindable, object oldValue, object newValue) {
            BindablePicker picker = (BindablePicker)bindable;
            picker.DisplayProperty = (string)newValue;
            loadItemsAndSetSelected(bindable);
        }

        /// <summary>
        /// Called when [items source changed].
        /// </summary>
        /// <param name="bindable">The bindable.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue) {
            BindablePicker picker = (BindablePicker)bindable;

            picker.ItemsSource = (IList)newValue;

            loadItemsAndSetSelected(bindable);
        }

        /// <summary>
        /// Loads the items and set selected.
        /// </summary>
        /// <param name="bindable">The bindable.</param>
        static void loadItemsAndSetSelected(BindableObject bindable) {
            BindablePicker picker = (BindablePicker)bindable;
            if (picker.ItemsSource as IEnumerable != null) {
                picker.disableEvents = true;
                picker.SelectedIndex = -1;
                picker.Items.Clear();
                int count = 0;
                foreach (object obj in (IEnumerable)picker.ItemsSource) {
                    string value = string.Empty;
                    if (picker.DisplayProperty != null) {
                        var prop = obj.GetType().GetRuntimeProperties().FirstOrDefault(p => string.Equals(p.Name, picker.DisplayProperty, StringComparison.OrdinalIgnoreCase));
                        if (prop != null) {
                            value = prop.GetValue(obj).ToString();
                        }
                        else {
                            value = obj.ToString();
                        }
                    }
                    else {
                        value = obj.ToString();
                    }
                    picker.Items.Add(value);
                    if (picker.SelectedItem != null) {
                        if (picker.SelectedItem == obj) {
                            picker.SelectedIndex = count;
                        }
                    }
                    count++;
                }
                picker.disableEvents = false;
            }
        }
    }
}