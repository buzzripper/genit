using Microsoft.VisualStudio.Modeling;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Dyvenix.GenIt.DslPackage.Editors.Services.Controls
{
	public partial class ReturnDtoEditControl : UserControlBase
	{
		private ReadMethodModel _selectedMethod;
		private LinkedElementCollection<DtoModel> _dtos;
		private ObservableCollection<ReturnDtoDisplayViewModel> _viewModels = new ObservableCollection<ReturnDtoDisplayViewModel>();

		public Action<DtoModel> DtoSelectionChanged { get; set; }

		public ReturnDtoEditControl()
		{
			InitializeComponent();
			grdReturnDtos.ItemsSource = _viewModels;
		}

		public void SetDtos(LinkedElementCollection<DtoModel> dtos)
		{
			_suspendUpdates = true;
			try
			{
				_dtos = dtos;
				ClearViewModels();

				if (dtos == null)
					return;

				foreach (var dto in dtos)
				{
					var vm = new ReturnDtoDisplayViewModel(dto);
					vm.PropertyChanged += ViewModel_PropertyChanged;
					_viewModels.Add(vm);
				}
			}
			finally
			{
				_suspendUpdates = false;
			}
		}

		public void SetSelectedMethod(ReadMethodModel method)
		{
			_suspendUpdates = true;
			try
			{
				_selectedMethod = method;

				foreach (var vm in _viewModels)
					vm.IsSelected = false;

				if (method == null)
				{
					grdReturnDtos.IsEnabled = false;
					return;
				}

				grdReturnDtos.IsEnabled = true;

				var returnDto = method.ReturnDto;
				if (returnDto != null)
				{
					var matchingVm = _viewModels.FirstOrDefault(vm => vm.DtoModel == returnDto);
					if (matchingVm != null)
						matchingVm.IsSelected = true;
				}
			}
			finally
			{
				_suspendUpdates = false;
			}
		}

		public bool Readonly
		{
			get { return !grdReturnDtos.IsEnabled; }
			set { grdReturnDtos.IsEnabled = !value; }
		}

		private void ClearViewModels()
		{
			foreach (var vm in _viewModels)
				vm.PropertyChanged -= ViewModel_PropertyChanged;
			_viewModels.Clear();
		}

		private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (_suspendUpdates) return;

			var vm = sender as ReturnDtoDisplayViewModel;
			if (vm == null || e.PropertyName != nameof(ReturnDtoDisplayViewModel.IsSelected))
				return;

			if (_selectedMethod == null || _selectedMethod.Store == null)
				return;

			if (vm.IsSelected)
			{
				// Exclusive: uncheck all others
				_suspendUpdates = true;
				try
				{
					foreach (var other in _viewModels)
					{
						if (other != vm)
							other.IsSelected = false;
					}
				}
				finally
				{
					_suspendUpdates = false;
				}

				// Set ReturnDto on the model
				DslTransactionHelper.ExecuteInTransaction(_selectedMethod, "Set Return DTO", () =>
				{
					_selectedMethod.ReturnDto = vm.DtoModel;
				});

				DtoSelectionChanged?.Invoke(vm.DtoModel);
			}
			else
			{
				// Unchecked: clear ReturnDto
				DslTransactionHelper.ExecuteInTransaction(_selectedMethod, "Clear Return DTO", () =>
				{
					_selectedMethod.ReturnDto = null;
				});

				DtoSelectionChanged?.Invoke(null);
			}
		}
	}

	public class ReturnDtoDisplayViewModel : INotifyPropertyChanged
	{
		private bool _isSelected;

		public DtoModel DtoModel { get; }
		public string Name { get { return DtoModel.Name; } }

		public ReturnDtoDisplayViewModel(DtoModel dtoModel)
		{
			DtoModel = dtoModel;
		}

		public bool IsSelected
		{
			get { return _isSelected; }
			set { _isSelected = value; OnPropertyChanged(nameof(IsSelected)); }
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
