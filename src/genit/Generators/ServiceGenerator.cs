﻿using Dyvenix.Genit.Extensions;
using Dyvenix.Genit.Misc;
using Dyvenix.Genit.Models;
using Dyvenix.Genit.Models.Generators;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace Dyvenix.Genit.Generators;

public class ServiceGenerator
{
	#region Constants

	private const string cSvcTemplateFilename = "Services.tmpl";
	private const string cSvcCollTemplateFilename = "ServiceCollExt.tmpl";
	private const string cQueryTemplateFilename = "Query.tmpl";
	private const string cControllersTemplateFilename = "Controllers.tmpl";
	private const string cApiClientsTemplateFilename = "ApiClients.tmpl";

	private const string cToken_CurrTimestamp = "CURR_TIMESTAMP";
	private const string cToken_AddlUsings = "ADDL_USINGS";
	private const string cToken_ServicesNs = "SERVICES_NS";
	private const string cToken_ServiceAttrs = "SERVICE_ATTRS";
	private const string cToken_ServiceName = "SERVICE_NAME";
	private const string cToken_IntfSignatures = "INTERFACE_SIGNATURES";
	private const string cToken_CudMethods = "CUD_METHODS";
	private const string cToken_SingleMethods = "SINGLE_METHODS";
	private const string cToken_ListMethods = "LIST_METHODS";
	private const string cToken_QueryMethods = "QUERY_METHODS";

	#endregion

	#region Properties

	public GeneratorType Type => GeneratorType.Entity;

	#endregion

	public void Run(ServiceGenModel svcGenModel, ObservableCollection<EntityModel> entities, string entitiesNamespace, string templatesFolderpath)
	{
		if (!svcGenModel.Enabled)
			return;

		// Load templates
		
		var templateFilepath = Path.Combine(templatesFolderpath, cSvcTemplateFilename);
		var outputFolder = Utils.ResolveRelativePath(Globals.CurrDocFilepath, svcGenModel.OutputFolder);
		Validate(templateFilepath, outputFolder);
		var serviceTemplate = File.ReadAllText(templateFilepath);

		var queryTemplateFilepath = Path.Combine(templatesFolderpath, cQueryTemplateFilename);
		var queryOutputFolder = Utils.ResolveRelativePath(Globals.CurrDocFilepath, svcGenModel.QueryOutputFolder);
		Validate(queryTemplateFilepath, queryOutputFolder);
		var queryTemplate = File.ReadAllText(queryTemplateFilepath);

		var controllerTemplateFilepath = Path.Combine(templatesFolderpath, cControllersTemplateFilename);
		var controllersOutputFolder = Utils.ResolveRelativePath(Globals.CurrDocFilepath, svcGenModel.ControllerOutputFolder);
		Validate(controllerTemplateFilepath, controllersOutputFolder);
		var controllerTemplate = File.ReadAllText(controllerTemplateFilepath);

		var apClientTemplateFilepath = Path.Combine(templatesFolderpath, cApiClientsTemplateFilename);
		var apiClientOutputFolder = Utils.ResolveRelativePath(Globals.CurrDocFilepath, svcGenModel.ApiClientOutputFolder);
		Validate(apClientTemplateFilepath, apiClientOutputFolder);
		var apiClientTemplate = File.ReadAllText(apClientTemplateFilepath);

		// Generate services

		var serviceEntities = new List<EntityModel>();
		var apiClientEntities = new List<EntityModel>();
		foreach (var entity in entities.Where(e => e.Service.Enabled)) {
			// Generate service class
			GenerateService(entity, svcGenModel, $"{serviceTemplate}", outputFolder);
			serviceEntities.Add(entity);

			// Generate query classes
			foreach (var queryMethod in entity.Service.ReadMethods.Where(m => m.UseQuery))
				new ServiceQueryGenerator().GenerateQueryClass(entity.Service, svcGenModel, $"{queryTemplate}", queryOutputFolder);

			// Generate controller / client
			if (entity.Service.InclController) {
				new ServiceControllerGenerator().GenerateController(entity, svcGenModel, $"{controllerTemplate}", controllersOutputFolder, entitiesNamespace);
				new ApiClientGenerator().GenerateApiClient(entity, svcGenModel, $"{apiClientTemplate}", apiClientOutputFolder, entitiesNamespace);
				apiClientEntities.Add(entity);
			}
		}

		// Register any Services
		if (serviceEntities.Any())
			new ServiceCollExtGenerator().GenerateServiceRegistrations(apiClientEntities, svcGenModel, templatesFolderpath);

		// Register any ApiClient classes
		if (apiClientEntities.Any())
			new ApiClientCollExtGenerator().GenerateApiClientRegistrations(apiClientEntities, svcGenModel, templatesFolderpath);
	}

	private void Validate(string templateFilepath, string outputFolder)
	{
		if (!File.Exists(templateFilepath))
			throw new ApplicationException($"Template file does not exist: {templateFilepath}");

		if (!Directory.Exists(outputFolder))
			throw new ApplicationException($"OutputFolder does not exist: {outputFolder}");
	}

	#region Services

	private void GenerateService(EntityModel entity, ServiceGenModel serviceGen, string template, string outputFolder)
	{
		var serviceName = $"{entity.Name}Service";

		// Addl usings
		var addlUsings = Utils.BuildAddlUsingsList(entity.Service.AddlServiceUsings);
		addlUsings.AddIfNotExists(serviceGen.QueriesNamespace);

		// Interface signatures
		var interfaceOutput = new List<string>();

		// Attributes
		var attrsOutput = new List<string>();
		foreach (var attr in entity.Service.ServiceClassAttributes)
			attrsOutput.Add($"[{attr}]");

		var serviceMethodGenerator = new ServiceMethodGenerator();

		// Create/Update/Delete
		var crudMethodsOutput = new List<string>();
		if (entity.Service.InclCreate || entity.Service.InclUpdate || entity.Service.InclDelete)
			serviceMethodGenerator.GenerateUpdateMethods(entity, crudMethodsOutput, interfaceOutput);

		// GetSingle methods
		var singleMethodsOutput = new List<string>();
		foreach (var singleMethod in entity.Service.ReadMethods.Where(m => !m.UseQuery && !m.IsList)) {
			if (singleMethodsOutput.Count == 0) {
				singleMethodsOutput.AddLine(1, "#region Single Methods");
			}
			serviceMethodGenerator.GenerateReadMethod(entity, singleMethod, singleMethodsOutput, interfaceOutput);
		}
		if (singleMethodsOutput.Count > 0)
			singleMethodsOutput.AddLine(1, "#endregion");

		// Get list methods
		var listMethodsOutput = new List<string>();
		foreach (var listMethod in entity.Service.ReadMethods.Where(m => !m.UseQuery && m.IsList)) {
			if (listMethodsOutput.Count == 0) {
				listMethodsOutput.AddLine(1, "#region List Methods");
			}
			serviceMethodGenerator.GenerateReadMethod(entity, listMethod, listMethodsOutput, interfaceOutput);
		}
		if (listMethodsOutput.Count > 0) {
			listMethodsOutput.AddLine();
			listMethodsOutput.AddLine(1, "#endregion");
		}

		// Query methods
		var queryMethodsOutput = new List<string>();
		if (entity.Service.ReadMethods.Any(m => m.UseQuery)) {
			if (queryMethodsOutput.Count == 0) {
				queryMethodsOutput.AddLine(1, "#region Query Methods");
			}
			foreach (var queryMethod in entity.Service.ReadMethods.Where(m => m.UseQuery))
				serviceMethodGenerator.GenerateQueryMethod(entity, queryMethod, queryMethodsOutput, interfaceOutput);
		}
		if (queryMethodsOutput.Count > 0)
			queryMethodsOutput.AddLine(1, "#endregion");

		// Sorting method
		if (entity.Service.ReadMethods.Where(m => m.UseQuery && m.InclSorting).Any()) {
			serviceMethodGenerator.GenerateSortingMethod(entity, queryMethodsOutput);
			queryMethodsOutput.AddLine();
		}

		// Replace tokens in template
		var fileContents = ReplaceServiceTemplateTokens(template, serviceName, addlUsings, attrsOutput, crudMethodsOutput, singleMethodsOutput, listMethodsOutput, queryMethodsOutput, interfaceOutput, serviceGen.ServicesNamespace);

		var outputFile = Path.Combine(outputFolder, $"{serviceName}.cs");
		if (File.Exists(outputFile))
			File.Delete(outputFile);
		File.WriteAllText(outputFile, fileContents);
	}

	private string ReplaceServiceTemplateTokens(string template, string serviceName, List<string> addlUsings, List<string> attrsOutput, List<string> crudMethodsOutput, List<string> singleMethodsOutput, List<string> listMethodsOutput, List<string> queryMethodsOutput, List<string> interfaceOutput, string servicesNamespace)
	{
		// Header
		template = template.Replace(Utils.FmtToken(cToken_CurrTimestamp), DateTime.Now.ToString("g"));

		// Namespace
		template = template.Replace(Utils.FmtToken(cToken_ServicesNs), servicesNamespace);

		// Usings
		var sb = new StringBuilder();
		addlUsings.ForEach(x => {
			if (sb.Length > 0)
				sb.AppendLine();
			sb.Append($"using {x};");
		});
		template = template.Replace(Utils.FmtToken(cToken_AddlUsings), sb.ToString());

		// Interface
		sb = new StringBuilder();
		interfaceOutput.ForEach(x => {
			if (sb.Length > 0)
				sb.AppendLine();
			sb.Append($"\t{x};");
		});
		template = template.Replace(Utils.FmtToken(cToken_IntfSignatures), sb.ToString());

		// Class Attributes
		sb = new StringBuilder();
		attrsOutput.ForEach(x => {
			if (sb.Length > 0)
				sb.AppendLine();
			sb.Append(x);
		});
		template = template.Replace(Utils.FmtToken(cToken_ServiceAttrs), sb.ToString());

		// Service name
		template = template.Replace(Utils.FmtToken(cToken_ServiceName), serviceName);

		// cToken_IntfSignatures

		// CRUD Methods
		sb = new StringBuilder();
		crudMethodsOutput.ForEach(x => sb.AppendLine(x));
		template = template.Replace(Utils.FmtToken(cToken_CudMethods), sb.ToString());

		// Single Methods
		sb = new StringBuilder();
		singleMethodsOutput.ForEach(x => sb.AppendLine(x));
		template = template.Replace(Utils.FmtToken(cToken_SingleMethods), sb.ToString());

		// List Methods
		sb = new StringBuilder();
		listMethodsOutput.ForEach(x => sb.AppendLine(x));
		template = template.Replace(Utils.FmtToken(cToken_ListMethods), sb.ToString());

		// Query Methods
		sb = new StringBuilder();
		queryMethodsOutput.ForEach(x => sb.AppendLine(x));
		template = template.Replace(Utils.FmtToken(cToken_QueryMethods), sb.ToString());

		return template;
	}

	#endregion
}
