﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Dyvenix.Genit.Models;

public class PropertyModel : INotifyPropertyChanged
{
	private string _name;

	#region Ctors

	[JsonConstructor]
	public PropertyModel()
	{
	}

	public PropertyModel(Guid id)
	{
		Id = id;
	}

	public PropertyModel(Guid id, AssocModel assoc)
	{
		Id = id;
		Name = assoc.RelatedPropertyName;
		PrimitiveType = assoc.PrimaryPKType;
		FKAssoc = assoc;
	}

	#endregion

	public Guid Id { get; init; }
	public string Name
	{
		get {
			if (FKAssoc != null)
				return FKAssoc.RelatedPropertyName;
			return _name;
		}
		set {
			if (FKAssoc != null)
				throw new ApplicationException($"Can't rename property, it is a FK association property.");
			SetProperty(ref _name, value);
		}
	}

	public int PrimitiveTypeId { get; set; }

	private PrimitiveType _primitiveType;
	[JsonIgnore]
	public PrimitiveType PrimitiveType
	{
		get => _primitiveType;
		set {
			PrimitiveTypeId = (value != null) ? value.Id : -1;
			SetProperty(ref _primitiveType, value);
		}
	}

	public Guid EnumTypeId { get; set; }

	private EnumModel _enumType;
	[JsonIgnore]
	public EnumModel EnumType
	{
		get => _enumType;
		set {
			EnumTypeId = (value != null) ? value.Id : Guid.Empty;
			SetProperty(ref _enumType, value);
		}
	}

	[JsonIgnore]
	public string DatatypeName
	{
		get {
			if (this.PrimitiveType != PrimitiveType.None)
				return this.PrimitiveType.CSType;
			else if (this.EnumType != null)
				return this.EnumType.Name;
			else if (this.FKAssoc != null)
				return this.FKAssoc.RelatedEntity.Name;
			else
				return string.Empty;
		}
		set {
		}
	}

	[JsonIgnore]
	public bool IsForeignKey
	{
		get => (this.FKAssoc != null);
	}

	private AssocModel _fkAssoc;
	public AssocModel FKAssoc
	{
		get => _fkAssoc;
		set => SetProperty(ref _fkAssoc, value);
	}

	private bool _isPrimaryKey;
	public bool IsPrimaryKey
	{
		get => _isPrimaryKey;
		set => SetProperty(ref _isPrimaryKey, value);
	}

	private bool _isIdentity;
	public bool IsIdentity
	{
		get => _isIdentity;
		set => SetProperty(ref _isIdentity, value);
	}

	private bool _nullable;
	public bool Nullable
	{
		get => _nullable;
		set => SetProperty(ref _nullable, value);
	}

	private int _maxLength;
	public int MaxLength
	{
		get => _maxLength;
		set => SetProperty(ref _maxLength, value);
	}

	private bool _isIndexed;
	public bool IsIndexed
	{
		get => _isIndexed;
		set => SetProperty(ref _isIndexed, value);
	}

	private bool _isIndexUnique;
	public bool IsIndexUnique
	{
		get => _isIndexUnique;
		set => SetProperty(ref _isIndexUnique, value);
	}

	private bool _isIndexClustered;
	public bool IsIndexClustered
	{
		get => _isIndexClustered;
		set => SetProperty(ref _isIndexClustered, value);
	}

	private int _displayOrder;
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

	private ObservableCollection<string> _addlUsings = new ObservableCollection<string>();
	public ObservableCollection<string> AddlUsings
	{
		get => _addlUsings;
		set => SetProperty(ref _addlUsings, value);
	}

	#region Methods

	public void InitializeOnLoad(ObservableCollection<EnumModel> enums)
	{
		if (this.PrimitiveTypeId > 0) {
			this.PrimitiveType = PrimitiveType.GetAll().First(p => p.Id == this.PrimitiveTypeId);

		} else if (this.EnumTypeId != Guid.Empty) {
			this.EnumType = enums.First(e => e.Id == this.EnumTypeId);
		}
	}

	public bool Validate(string entityName, List<string> errorList)
	{
		var errs = new List<string>();

		if (this.PrimitiveType != PrimitiveType.None) {
			if (this.PrimitiveType == PrimitiveType.String) {
				if (this.MaxLength < 0)
					errs.Add($"Property {entityName}.{this.Name}: String values must have a MaxLength >= 0 (0 == NVARCHAR(MAX))");

			} else if (this.PrimitiveType == PrimitiveType.ByteArray) {
				if (this.MaxLength <= 0)
					errs.Add($"Property {entityName}.{this.Name}: Byte array type must have a MaxLength > 0");
			}

		} else if (this.EnumType != null) {


		} else if (this.FKAssoc != null) {


		} else {
			errs.Add($"Property {entityName}.{this.Name}: No data type defined.");
		}

		errorList.AddRange(errs);
		return (errs.Count == 0);
	}

	#endregion

	public override string ToString()
	{
		return this.Name;
	}

	public event PropertyChangedEventHandler PropertyChanged;

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
