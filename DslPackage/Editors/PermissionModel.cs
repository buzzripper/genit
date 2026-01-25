using System.ComponentModel;

namespace Dyvenix.GenIt.DslPackage.Editors
{
	/// <summary>
	/// Represents a permission with Name and Description for UI binding.
	/// </summary>
	public class PermissionModel : INotifyPropertyChanged
	{
		private string _name;
		private string _description;

		public string Name
		{
			get => _name;
			set
			{
				if (_name != value)
				{
					_name = value;
					OnPropertyChanged(nameof(Name));
				}
			}
		}

		public string Description
		{
			get => _description;
			set
			{
				if (_description != value)
				{
					_description = value;
					OnPropertyChanged(nameof(Description));
				}
			}
		}

		public PermissionModel() : this(string.Empty, string.Empty)
		{
		}

		public PermissionModel(string name, string description)
		{
			_name = name ?? string.Empty;
			_description = description ?? string.Empty;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		/// <summary>
		/// Serializes the permission to a string format: "Name|Description"
		/// </summary>
		public string Serialize()
		{
			return $"{Name}|{Description}";
		}

		/// <summary>
		/// Deserializes a permission from a string format: "Name|Description"
		/// </summary>
		public static PermissionModel Deserialize(string serialized)
		{
			if (string.IsNullOrEmpty(serialized))
				return new PermissionModel();

			var parts = serialized.Split(new[] { '|' }, 2);
			var name = parts.Length > 0 ? parts[0] : string.Empty;
			var description = parts.Length > 1 ? parts[1] : string.Empty;
			return new PermissionModel(name, description);
		}
	}
}
