﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Dyvenix.Genit.Models;

public class NavPropertyModel : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler PropertyChanged;

	#region Fields

	private string _name;
	private int _displayOrder;
	private AssocModel _assoc;

	#endregion

	#region Ctors

	[JsonConstructor]
	public NavPropertyModel()
	{
	}

	public NavPropertyModel(Guid id, AssocModel assoc)
	{
		this.Id = id;
		this.Assoc = assoc;
	}

	#endregion

	#region Properties

	public Guid Id { get; init; }

	public string Name
	{
		get => _name;
		set => SetProperty(ref _name, value);
	}

	public int DisplayOrder
	{
		get => _displayOrder;
		set => SetProperty(ref _displayOrder, value);
	}

	private ObservableCollection<string> _attributes = new ObservableCollection<string>();
	public ObservableCollection<string> Attributes
	{
		get => _attributes;
		set => SetProperty(ref _attributes, value);
	}

	public AssocModel Assoc
	{
		get { return _assoc; }
		set {
			SetProperty(ref _assoc, value);
		}
	}

	public EntityModel FKEntity
	{
		get { return Assoc?.FKEntity; }
		set {
			if (Assoc != null)
				Assoc.FKEntity = value;
		}
	}

	#endregion

	#region Non-serialized Properties

	[JsonIgnore]
	public Cardinality Cardinality
	{
		get { return Assoc == null ? Models.Cardinality.None : Assoc.Cardinality; }
		set {
			if (Assoc != null)
				Assoc.Cardinality = value;
		}
	}

	[JsonIgnore]
	public PropertyModel FKProperty
	{
		get { return Assoc?.FKProperty; }
		set {
			if (Assoc != null)
				Assoc.FKProperty = value;
		}
	}

	#endregion

	#region Methods

	public bool Validate(string entityName, List<string> errorList)
	{
		var errs = new List<string>();

		if (string.IsNullOrWhiteSpace(this.Name))
			errs.Add($"NavProperty {entityName}.{this.Name}: Name is required.");

		if (this.Assoc == null)
			errs.Add($"NavProperty {entityName}.{this.Name}: Assoc is required.");

		errorList.AddRange(errs);

		return (errs.Count == 0);
	}

	public override string ToString()
	{
		return this.Name;
	}

	#endregion

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
}
