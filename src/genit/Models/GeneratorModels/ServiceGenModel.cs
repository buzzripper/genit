﻿using Dyvenix.Genit.Generators;
using System;

namespace Dyvenix.Genit.Models.Generators;

public class ServiceGenModel : GenModelBase
{
	#region Static

	public static ServiceGenModel CreateNew()
	{
		return new ServiceGenModel(Guid.NewGuid()) {
			Enabled = true,
			InclHeader = true
		};
	}

	#endregion

	#region Ctors / Init

	public ServiceGenModel(Guid id) : base(id, GeneratorType.Service)
	{
	}

	#endregion

	#region Properties

	public string OutputFolder { get; set; }
	public string QueryOutputFolder { get; set; }
	public string ControllerOutputFolder { get; set; }
	public string ApiClientOutputFolder { get; set; }
	public string ApiClientExtOutputFilepath { get; set; }
	public string ServicesNamespace { get; set; }
	public string ServicesExtOutputFilepath { get; set; }
	public string QueriesNamespace { get; set; }
	public string ControllersNamespace { get; set; }
	public string ApiClientsNamespace { get; set; }
	public string DtoOutputFolder { get; set; }
	public string DtoNamespace { get; set; }

	protected override string GetName() => "Service Generator";

	#endregion
}