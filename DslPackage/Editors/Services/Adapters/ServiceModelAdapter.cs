using Microsoft.VisualStudio.Modeling;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Dyvenix.GenIt.DslPackage.Editors.Services.Adapters
{
	/// <summary>
	/// Adapter class that wraps a DSL ServiceModel and provides WPF-friendly observable collection properties.
	/// This bridges the DSL domain model with WPF data binding requirements.
	/// </summary>
	public class ServiceModelAdapter : INotifyPropertyChanged
	{
		private readonly ServiceModel _serviceModel;
		private ObservableCollection<EditableString> _serviceUsingsList;
		private ObservableCollection<EditableString> _serviceAttributesList;
		private ObservableCollection<EditableString> _controllerUsingsList;
		private ObservableCollection<EditableString> _controllerAttributesList;

		public ServiceModelAdapter(ServiceModel serviceModel)
		{
			_serviceModel = serviceModel ?? throw new ArgumentNullException(nameof(serviceModel));
		}

		/// <summary>
		/// Gets the underlying DSL ServiceModel.
		/// </summary>
		public ServiceModel Model => _serviceModel;

		/// <summary>
		/// Gets the DSL Store for transaction management.
		/// </summary>
		public Store Store => _serviceModel.Store;

		#region DSL Property Accessors

		public bool Enabled
		{
			get => _serviceModel.Enabled;
			set => SetDslProperty(nameof(ServiceModel.Enabled), _serviceModel.Enabled, value, () => _serviceModel.Enabled = value);
		}

		public bool InclCreate
		{
			get => _serviceModel.InclCreate;
			set => SetDslProperty(nameof(ServiceModel.InclCreate), _serviceModel.InclCreate, value, () => _serviceModel.InclCreate = value);
		}

		public bool InclUpdate
		{
			get => _serviceModel.InclUpdate;
			set => SetDslProperty(nameof(ServiceModel.InclUpdate), _serviceModel.InclUpdate, value, () => _serviceModel.InclUpdate = value);
		}

		public bool InclDelete
		{
			get => _serviceModel.InclDelete;
			set => SetDslProperty(nameof(ServiceModel.InclDelete), _serviceModel.InclDelete, value, () => _serviceModel.InclDelete = value);
		}

		public bool InclEndpoints
		{
			get => _serviceModel.InclEndpoints;
			set => SetDslProperty(nameof(ServiceModel.InclEndpoints), _serviceModel.InclEndpoints, value, () => _serviceModel.InclEndpoints = value);
		}

		public bool InclAngService
		{
			get => _serviceModel.InclAngService;
			set => SetDslProperty(nameof(ServiceModel.InclAngService), _serviceModel.InclAngService, value, () => _serviceModel.InclAngService = value);
		}

		public string Version
		{
			get => _serviceModel.Version;
			set => SetDslProperty(nameof(ServiceModel.Version), _serviceModel.Version, value, () => _serviceModel.Version = value);
		}

		public LinkedElementCollection<ReadMethodModel> ReadMethods => _serviceModel.ReadMethods;
		public LinkedElementCollection<UpdateMethodModel> UpdateMethods => _serviceModel.UpdateMethods;

		#endregion

		#region Observable Collection Properties

		/// <summary>
		/// Gets an ObservableCollection wrapper for ServiceUsings.
		/// Changes to this collection will be synced back to the underlying DSL string property.
		/// </summary>
		public ObservableCollection<EditableString> ServiceUsingsList
		{
			get
			{
				if (_serviceUsingsList == null)
				{
					_serviceUsingsList = CreateObservableFromString(_serviceModel.ServiceUsings);
					_serviceUsingsList.CollectionChanged += (s, e) => SyncServiceUsings();
					SubscribeToItemChanges(_serviceUsingsList, () => SyncServiceUsings());
				}
				return _serviceUsingsList;
			}
		}

		/// <summary>
		/// Gets an ObservableCollection wrapper for ServiceAttributes.
		/// </summary>
		public ObservableCollection<EditableString> ServiceAttributesList
		{
			get
			{
				if (_serviceAttributesList == null)
				{
					_serviceAttributesList = CreateObservableFromString(_serviceModel.ServiceAttributes);
					_serviceAttributesList.CollectionChanged += (s, e) => SyncServiceAttributes();
					SubscribeToItemChanges(_serviceAttributesList, () => SyncServiceAttributes());
				}
				return _serviceAttributesList;
			}
		}

		/// <summary>
		/// Gets an ObservableCollection wrapper for EndpointsUsings.
		/// </summary>
		public ObservableCollection<EditableString> EndpointsUsingsList
		{
			get
			{
				if (_controllerUsingsList == null)
				{
					_controllerUsingsList = CreateObservableFromString(_serviceModel.EndpointsUsings);
					_controllerUsingsList.CollectionChanged += (s, e) => SyncEndpointsUsings();
					SubscribeToItemChanges(_controllerUsingsList, () => SyncEndpointsUsings());
				}
				return _controllerUsingsList;
			}
		}

		/// <summary>
		/// Gets an ObservableCollection wrapper for EndpointsAttributes.
		/// </summary>
		public ObservableCollection<EditableString> EndpointsAttributesList
		{
			get
			{
				if (_controllerAttributesList == null)
				{
					_controllerAttributesList = CreateObservableFromString(_serviceModel.EndpointsAttributes);
					_controllerAttributesList.CollectionChanged += (s, e) => SyncEndpointsAttributes();
					SubscribeToItemChanges(_controllerAttributesList, () => SyncEndpointsAttributes());
				}
				return _controllerAttributesList;
			}
		}

		#endregion

		#region Sync Methods

		private void SyncServiceUsings()
		{
			var newValue = string.Join(Environment.NewLine, _serviceUsingsList.Select(es => es.Value));
			if (_serviceModel.ServiceUsings != newValue)
			{
				DslTransactionHelper.SetProperty<string>(_serviceModel, nameof(ServiceModel.ServiceUsings), () => _serviceModel.ServiceUsings = newValue);
			}
		}

		private void SyncServiceAttributes()
		{
			var newValue = string.Join(Environment.NewLine, _serviceAttributesList.Select(es => es.Value));
			if (_serviceModel.ServiceAttributes != newValue)
			{
				DslTransactionHelper.SetProperty<string>(_serviceModel, nameof(ServiceModel.ServiceAttributes), () => _serviceModel.ServiceAttributes = newValue);
			}
		}

		private void SyncEndpointsUsings()
		{
			var newValue = string.Join(Environment.NewLine, _controllerUsingsList.Select(es => es.Value));
			if (_serviceModel.EndpointsUsings != newValue)
			{
				DslTransactionHelper.SetProperty<string>(_serviceModel, nameof(ServiceModel.EndpointsUsings), () => _serviceModel.EndpointsUsings = newValue);
			}
		}

		private void SyncEndpointsAttributes()
		{
			var newValue = string.Join(Environment.NewLine, _controllerAttributesList.Select(es => es.Value));
			if (_serviceModel.EndpointsAttributes != newValue)
			{
				DslTransactionHelper.SetProperty<string>(_serviceModel, nameof(ServiceModel.EndpointsAttributes), () => _serviceModel.EndpointsAttributes = newValue);
			}
		}

		#endregion

		#region Helper Methods

		private void SetDslProperty<T>(string propertyName, T currentValue, T newValue, Action setter)
		{
			if (!Equals(currentValue, newValue))
			{
				DslTransactionHelper.SetProperty<T>(_serviceModel, propertyName, setter);
				OnPropertyChanged(propertyName);
			}
		}

		private static ObservableCollection<EditableString> CreateObservableFromString(string value)
		{
			var collection = new ObservableCollection<EditableString>();
			if (!string.IsNullOrWhiteSpace(value))
			{
				var lines = value.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
				foreach (var line in lines)
				{
					var trimmed = line.Trim();
					if (!string.IsNullOrWhiteSpace(trimmed))
					{
						collection.Add(new EditableString(trimmed));
					}
				}
			}
			return collection;
		}

		private static void SubscribeToItemChanges(ObservableCollection<EditableString> collection, Action onChanged)
		{
			foreach (var item in collection)
			{
				item.PropertyChanged += (s, e) => onChanged();
			}

			collection.CollectionChanged += (s, e) =>
			{
				if (e.NewItems != null)
				{
					foreach (EditableString item in e.NewItems)
					{
						item.PropertyChanged += (s2, e2) => onChanged();
					}
				}
			};
		}

		#endregion

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion
	}
}
