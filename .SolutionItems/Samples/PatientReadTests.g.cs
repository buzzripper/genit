//------------------------------------------------------------------------------------------------------------
// This file was auto-generated on 2/15/2026 10:23 AM. Any changes made to it will be lost.
//------------------------------------------------------------------------------------------------------------
using Dyvenix.App1.App.Shared.ApiClients.v1;
using Dyvenix.App1.App.Shared.Contracts.v1;
using Dyvenix.App1.App.Shared.Requests.v1;
using Dyvenix.App1.Common.Data.Shared.Entities;
using Dyvenix.App1.Tests.Integration.Data;
using Dyvenix.App1.Tests.Integration.DataSets;
using Dyvenix.App1.Tests.Integration.Fixtures;
using System;

namespace Dyvenix.App1.App.Shared.Requests.v1;

public class PatientReadTestFixture(GlobalTestFixture globalFixture) : IAsyncLifetime
{
	public GlobalTestFixture GlobalFixture { get; } = globalFixture;
	public TestDataSet DataSet { get; private set; } = default!;

	public async ValueTask InitializeAsync()
	{
		var dataManager = GlobalFixture.Services.GetRequiredService<IDataManager>();
		DataSet = await dataManager.Reset(DataSetType.Main.ToString());
	}

	public ValueTask DisposeAsync() => default;
}

[Collection(nameof(GlobalTestCollection))]
public class PatientReadTests : TestBase, IClassFixture<PatientReadTestFixture>
{
	private readonly PatientReadTestFixture _fixture;
	private IPatientService _patientService = default!;

	public PatientReadTests(GlobalTestFixture globalFixture, PatientReadTestFixture fixture)
		: base(globalFixture)
	{
		_fixture = fixture;
	}

	public override async ValueTask InitializeAsync()
	{
		await base.InitializeAsync();
		_patientService = _scope.ServiceProvider.GetRequiredService<IPatientService>();
	}

	[Fact]
	public async Task GetById_Success()
	{
		// Arrange
		var patientId = _fixture.DataSet.PatientList.First().Id;

		// Act
		var patient = await _patientService.GetById(patientId);

		// Assert
		Assert.Equal(patientId, patient.Id);
	}

	[Fact]
	public async Task GetAllPaging_Success()
	{
		// Arrange
		var totalCount = _fixture.DataSet.PatientList.Count;
		var request = new GetAllPagingReq();
		request.PageSize = 3;
		request.RecalcRowCount = true;
		request.GetRowCountOnly = false;

		var lastPgOffset = totalCount / 3;
		if (totalCount % 3 == 0)
			lastPgOffset -= 1; // Adjust if total count is an exact multiple of page size
		var lastPgSize = totalCount - (lastPgOffset * 3);

		// Act
		request.PageOffset = 0;
		var firstPgList = await _patientService.GetAllPaging(request);
		request.PageOffset = lastPgOffset;
		var lastPgList = await _patientService.GetAllPaging(request);

		// Assert
		Assert.True(totalCount == firstPgList.TotalRowCount, $"First total count s/b {totalCount} but was {firstPgList.TotalRowCount}");
		Assert.True(request.PageSize == firstPgList.Items.Count, $"First page size s/b {request.PageSize} but was {firstPgList.Items.Count}");
		Assert.True(totalCount == lastPgList.TotalRowCount, $"Last total count s/b {totalCount} but was {firstPgList.TotalRowCount}");
		Assert.True(lastPgSize == lastPgList.Items.Count, $"Last page size s/b {lastPgSize} but was {lastPgList.Items.Count}");
	}
}
