using CommunityToolkit.Mvvm.ComponentModel;

namespace DslWpf.ViewModels
{
    /// <summary>
    /// A wrapper class for string values that enables two-way data binding in WPF.
    /// Strings are immutable, so this class provides a mutable container with INotifyPropertyChanged support.
    /// </summary>
    public partial class EditableString : ObservableObject
    {
        [ObservableProperty]
        private string _value;

        public EditableString() : this(string.Empty)
        {
        }

        public EditableString(string value)
        {
            _value = value ?? string.Empty;
        }

        /// <summary>
        /// Implicit conversion from string to EditableString.
        /// </summary>
        public static implicit operator EditableString(string value) => new EditableString(value);

        /// <summary>
        /// Implicit conversion from EditableString to string.
        /// </summary>
        public static implicit operator string(EditableString editableString) => editableString?.Value ?? string.Empty;

        public override string ToString() => Value;
    }
}
