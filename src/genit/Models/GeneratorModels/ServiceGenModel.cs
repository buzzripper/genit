﻿using Dyvenix.Genit.Generators;
using System;

namespace Dyvenix.Genit.Models.Generators;

public class ServiceGenModel : GenModelBase
{
	public ServiceGenModel(Guid id) : base(id, GeneratorType.Service)
	{
	}

	public string OutputFolder { get; set; }
	public string QueryTemplateFilepath { get; set; }
	public string QueryOutputFolder { get; set; }
	public string ControllerTemplateFilepath { get; set; }
	public string ControllerOutputFolder { get; set; }
	public string ApiClientTemplateFilepath { get; set; }
	public string ApiClientOutputFolder { get; set; }
	public string ApiClientServicesExtTemplateFilepath { get; set; }
	public string ApiClientServicesExtOutputFilepath { get; set; }

	protected override string GetName() => "Service Generator";
}