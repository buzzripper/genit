using Microsoft.VisualStudio.Modeling;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Dyvenix.GenIt.DslPackage.Editors.Services.Controls
{
	public partial class ReturnPropertiesEditControl : UserControlBase
	{
		private LinkedElementCollection<PropertyModel> _properties;
		private ObservableCollection<ReturnPropertyDisplayViewModel> _viewModels = new ObservableCollection<ReturnPropertyDisplayViewModel>();

		public ReturnPropertiesEditControl()
		{
			InitializeComponent();
			grdReturnProperties.ItemsSource = _viewModels;
		}

		public void SetProperties(LinkedElementCollection<PropertyModel> properties)
		{
			_properties = properties;
			RebuildViewModels();
		}

		public void HighlightDtoProperties(DtoModel dtoModel)
		{
			RebuildViewModels();

			if (dtoModel == null)
				return;

			var linkedProperties = dtoModel.PropertyModelsOrdered;
			foreach (var vm in _viewModels)
			{
				vm.IsHighlighted = linkedProperties.Contains(vm.Property);
			}
		}

		private void RebuildViewModels()
		{
			_viewModels.Clear();

			if (_properties == null)
				return;

			foreach (var prop in _properties)
			{
				_viewModels.Add(new ReturnPropertyDisplayViewModel(prop));
			}
		}
	}

	public class ReturnPropertyDisplayViewModel : INotifyPropertyChanged
	{
		private bool _isHighlighted;

		public PropertyModel Property { get; }
		public string Name { get { return Property.Name; } }

		public ReturnPropertyDisplayViewModel(PropertyModel property)
		{
			Property = property;
		}

		public bool IsHighlighted
		{
			get { return _isHighlighted; }
			set { _isHighlighted = value; OnPropertyChanged(nameof(IsHighlighted)); }
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
