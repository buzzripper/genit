﻿using Dyvenix.Genit.Models.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Dyvenix.Genit.Models;

public class EntityModel : INotifyPropertyChanged, IDeserializationCallback
{
	public static EntityModel CreateNew()
	{
		var entityModel = new EntityModel(Guid.NewGuid()) {
			Enabled = true,
			InclRowVersion = true
		};

		var idProperty = new PropertyModel(Guid.NewGuid()) {
			Name = "Id",
			PrimitiveType = PrimitiveType.Guid,
			IsPrimaryKey = true,
			IsIndexed = true,
			IsIndexUnique = true
		};
		entityModel.Properties.Add(idProperty);

		entityModel.Service = ServiceModel.CreateNew(Guid.NewGuid(), entityModel);

		return entityModel;
	}
	
	#region Events

	public event PropertyChangedEventHandler PropertyChanged;
	public event EventHandler<NavPropertyAddedEventArgs> NavPropertyAdded;
	public event EventHandler<NavPropertyRemovedEventArgs> NavPropertyRemoved;

	#endregion

	#region Fields

	private Guid _id;
	private string _name;
	private string _schema;
	private string _tableName;
	private bool _enabled;
	private string _namespace;
	private bool _inclRowVersion;

	#endregion

	#region Ctors / Initialization

	[JsonConstructor]
	public EntityModel()
	{
	}

	public EntityModel(Guid id)
	{
		Id = id;
	}

	public void OnDeserialization(object sender)
	{
		NavProperties.CollectionChanged += NavProperties_CollectionChanged;
	}

	#endregion

	#region Properties

	public Guid Id
	{
		get => _id;
		set => SetProperty(ref _id, value);
	}

	public bool InclRowVersion
	{
		get => _inclRowVersion;
		set => SetProperty(ref _inclRowVersion, value);
	}

	public string Name
	{
		get => _name;
		set => SetProperty(ref _name, value);
	}

	public string Schema
	{
		get => _schema;
		set => SetProperty(ref _schema, value);
	}

	public string TableName
	{
		get => _tableName;
		set => SetProperty(ref _tableName, value);
	}

	public bool Enabled
	{
		get => _enabled;
		set => SetProperty(ref _enabled, value);
	}

	public string Namespace
	{
		get => _namespace;
		set => SetProperty(ref _namespace, value);
	}

	public ObservableCollection<PropertyModel> Properties { get; set; } = new ObservableCollection<PropertyModel>();

	public ObservableCollection<NavPropertyModel> NavProperties { get; set; } = new ObservableCollection<NavPropertyModel>();

	public ObservableCollection<string> Attributes { get; set; } = new ObservableCollection<string>();

	public ObservableCollection<string> AddlUsings { get; set; } = new ObservableCollection<string>();

	public ServiceModel Service { get; set; }

	private void NavProperties_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		if (e.Action == NotifyCollectionChangedAction.Add) {
			var navProp = e.NewItems?[0] as NavPropertyModel;
			if (navProp != null)
				NavPropertyAdded?.Invoke(this, new NavPropertyAddedEventArgs(this, navProp));

		} else if (e.Action == NotifyCollectionChangedAction.Remove) {
			var navProp = e.NewItems?[0] as NavPropertyModel;
			if (navProp != null)
				NavPropertyRemoved?.Invoke(this, new NavPropertyRemovedEventArgs(this, navProp));
		}
	}

	#endregion

	#region Methods

	public PropertyModel AddForeignKey(string fkPropName, EntityModel pkEntity, AssocModel assoc)
	{
		var property = new PropertyModel(Guid.NewGuid(), fkPropName, assoc, pkEntity);
		property.PrimitiveType = pkEntity.GetPKProperty().PrimitiveType;
		property.IsIndexed = true;
		property.DisplayOrder = Properties.Count;
		Properties.Add(property);
		return property;
	}

	public PropertyModel GetPKProperty()
	{
		return Properties.FirstOrDefault(p => p.IsPrimaryKey);
	}

	public void Validate(List<string> errorList)
	{
		foreach (var property in Properties) {
			property.Validate(Name, errorList);
		}
	}

	public override string ToString()
	{
		return Name;
	}

	#endregion

	#region IPropertyNotifyEvent

	protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
	{
		if (EqualityComparer<T>.Default.Equals(field, value)) return false;
		field = value;
		OnPropertyChanged(propertyName);
		return true;
	}

	#endregion
}

public class NavPropertyAddedEventArgs : EventArgs
{
	public EntityModel EntityModel { get; private set; }
	public NavPropertyModel NavPropertyModel { get; private set; }

	public NavPropertyAddedEventArgs(EntityModel entityModel, NavPropertyModel navPropertyMdl)
	{
		EntityModel = entityModel;
		NavPropertyModel = navPropertyMdl;
	}
}

public class NavPropertyRemovedEventArgs : EventArgs
{
	public EntityModel EntityModel { get; private set; }
	public NavPropertyModel NavPropertyModel { get; private set; }

	public NavPropertyRemovedEventArgs(EntityModel entityModel, NavPropertyModel navPropertyMdl)
	{
		EntityModel = entityModel;
		NavPropertyModel = navPropertyMdl;
	}
}
