﻿using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;
using WesternStatesWater.WaDE.Accessors.EntityFramework;
using WesternStatesWater.WaDE.Tests.Helpers;
using WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework;

namespace WesternStatesWater.WaDE.Accessors.Tests
{
    [TestClass]
    public class ApiAggregratedAmountsAccessorTests : DbTestBase
    {
        private readonly ILoggerFactory loggerFactory = new LoggerFactory();

        [TestMethod]
        public async Task GetAggregatedAmountsAsync_Filters_None()
        {
            var configuration = Configuration.GetConfiguration();
            AggregatedAmountsFact aggregatedAmount;
            using (var db = new WaDEContext(configuration))
            {
                aggregatedAmount = await AggregatedAmountsFactBuilder.Load(db);

                aggregatedAmount.AggregatedAmountId.Should().NotBe(0);
            }

            var filters = new AggregatedAmountsFilters();

            var sut = CreateAggregatedAmountsAccessor();
            var result = await sut.GetAggregatedAmountsAsync(filters, 0, int.MaxValue);

            result.TotalAggregatedAmountsCount.Should().Be(1);
            result.Organizations.Should().HaveCount(1);

            var org = result.Organizations.Single();
            org.OrganizationId.Should().Be(aggregatedAmount.OrganizationId);
            org.AggregatedAmounts.Should().HaveCount(1);
            org.AggregatedAmounts[0].AggregatedAmountId.Should().Be(aggregatedAmount.AggregatedAmountId);
        }

        [DataTestMethod]
        [DataRow(null, null, null, true)]
        [DataRow(null, "2022-02-23", null, false)]
        [DataRow(null, null, "2022-02-25", false)]
        [DataRow(null, "2022-02-23", "2022-02-25", false)]
        [DataRow("2022-02-24", null, null, true)]
        [DataRow("2022-02-24", "2022-02-23", null, true)]
        [DataRow("2022-02-24", "2022-02-24", null, true)]
        [DataRow("2022-02-24", "2022-02-25", null, false)]
        [DataRow("2022-02-24", null, "2022-02-23", false)]
        [DataRow("2022-02-24", null, "2022-02-24", true)]
        [DataRow("2022-02-24", null, "2022-02-25", true)]
        [DataRow("2022-02-22", "2022-02-23", "2022-02-25", false)]
        [DataRow("2022-02-23", "2022-02-23", "2022-02-25", true)]
        [DataRow("2022-02-24", "2022-02-23", "2022-02-25", true)]
        [DataRow("2022-02-25", "2022-02-23", "2022-02-25", true)]
        [DataRow("2022-02-26", "2022-02-23", "2022-02-25", false)]
        public async Task GetAggregatedAmountsAsync_Filters_DataPublicationDate(string dataPublicationDate, string startDataPublicationDate, string endDataPublicationDate, bool shouldMatch)
        {
            var configuration = Configuration.GetConfiguration();
            AggregatedAmountsFact aggregatedAmount;
            DateDim publicationDate = null;
            using (var db = new WaDEContext(configuration))
            {
                if (dataPublicationDate != null)
                {
                    publicationDate = await DateDimBuilder.Load(db);
                    publicationDate.Date = DateTime.Parse(dataPublicationDate);
                }

                aggregatedAmount = await AggregatedAmountsFactBuilder.Load(db, new AggregatedAmountsFactBuilderOptions
                {
                    DataPublicationDate = publicationDate
                });
            }

            var filters = new AggregatedAmountsFilters
            {
                StartDataPublicationDate = startDataPublicationDate == null ? null : DateTime.Parse(startDataPublicationDate),
                EndDataPublicationDate = endDataPublicationDate == null ? null : DateTime.Parse(endDataPublicationDate),
            };

            var sut = CreateAggregatedAmountsAccessor();
            var result = await sut.GetAggregatedAmountsAsync(filters, 0, int.MaxValue);

            if (shouldMatch)
            {
                result.TotalAggregatedAmountsCount.Should().Be(1);
                result.Organizations.Should().HaveCount(1);

                var org = result.Organizations.Single();
                org.OrganizationId.Should().Be(aggregatedAmount.OrganizationId);
                org.AggregatedAmounts.Should().HaveCount(1);
                org.AggregatedAmounts[0].AggregatedAmountId.Should().Be(aggregatedAmount.AggregatedAmountId);
            }
            else
            {
                result.TotalAggregatedAmountsCount.Should().Be(0);
                result.Organizations.Should().HaveCount(0);
            }
        }

        [TestMethod]
        public async Task GetAggregatedAmountsAsync_Paging()
        {
            var configuration = Configuration.GetConfiguration();
            using (var db = new WaDEContext(configuration))
            {
                for (var i = 0; i < 15; i++)
                {
                    await AggregatedAmountsFactBuilder.Load(db);
                }
            }

            var filters = new AggregatedAmountsFilters();

            var alreadyRetrieved = new List<long>();

            for (var i = 1; i <= 5; i++)
            {
                var sut = CreateAggregatedAmountsAccessor();
                var pagedResults = await sut.GetAggregatedAmountsAsync(filters, Utility.NthTriangle(i - 1), i);
                pagedResults.TotalAggregatedAmountsCount.Should().Be(15);
                var waterAllocations = pagedResults.Organizations.SelectMany(a => a.AggregatedAmounts).Select(a => a.AggregatedAmountId).ToList();
                waterAllocations.Should().HaveCount(i);
                foreach (var waterAllocation in waterAllocations)
                {
                    alreadyRetrieved.Should().NotContain(waterAllocation);
                    alreadyRetrieved.Add(waterAllocation);
                }
            }

            alreadyRetrieved.Should().OnlyHaveUniqueItems().And.HaveCount(15);
        }

        [TestMethod]
        public async Task GetAggregatedAmountsAsync_Paging_Consistency()
        {
            var configuration = Configuration.GetConfiguration();
            using (var db = new WaDEContext(configuration))
            {
                for (var i = 0; i < 15; i++)
                {
                    await AggregatedAmountsFactBuilder.Load(db);
                }
            }

            var filters = new AggregatedAmountsFilters();

            var sut = CreateAggregatedAmountsAccessor();
            var baseResults = (await sut.GetAggregatedAmountsAsync(filters, 0, 15)).Organizations.SelectMany(a => a.AggregatedAmounts).Select(a => a.AggregatedAmountId).ToList();

            for (var i = 0; i < 14; i++)
            {
                var pagedResults = await sut.GetAggregatedAmountsAsync(filters, i, 2);
                var waterAllocations = pagedResults.Organizations.SelectMany(a => a.AggregatedAmounts).Select(a => a.AggregatedAmountId).ToList();
                waterAllocations.Should().HaveCount(2);
                waterAllocations.Should().BeEquivalentTo(baseResults.Skip(i).Take(2));
            }
        }

        [TestMethod]
        public async Task GetAggregatedAmountsAsync_Paging_RequestMoreThanExists()
        {
            var configuration = Configuration.GetConfiguration();
            using (var db = new WaDEContext(configuration))
            {
                for (var i = 0; i < 3; i++)
                {
                    await AggregatedAmountsFactBuilder.Load(db);
                }
            }

            var filters = new AggregatedAmountsFilters();

            var sut = CreateAggregatedAmountsAccessor();
            var pagedResults = await sut.GetAggregatedAmountsAsync(filters, 1, 10);
            pagedResults.TotalAggregatedAmountsCount.Should().Be(3);
            var waterAllocations = pagedResults.Organizations.SelectMany(a => a.AggregatedAmounts).Select(a => a.AggregatedAmountId).ToList();
            waterAllocations.Should().HaveCount(2);
        }

        [TestMethod]
        public async Task GetAggregatedAmountsAsync_Paging_RequestAfterLast()
        {
            var configuration = Configuration.GetConfiguration();
            using (var db = new WaDEContext(configuration))
            {
                for (var i = 0; i < 3; i++)
                {
                    await AggregatedAmountsFactBuilder.Load(db);
                }
            }

            var filters = new AggregatedAmountsFilters();

            var sut = CreateAggregatedAmountsAccessor();
            var pagedResults = await sut.GetAggregatedAmountsAsync(filters, 4, 10);
            pagedResults.TotalAggregatedAmountsCount.Should().Be(3);
            var waterAllocations = pagedResults.Organizations.SelectMany(a => a.AggregatedAmounts).Select(a => a.AggregatedAmountId).ToList();
            waterAllocations.Should().HaveCount(0);
        }

        private IAggregatedAmountsAccessor CreateAggregatedAmountsAccessor()
        {
            return new AggregratedAmountsAccessor(Configuration.GetConfiguration(), loggerFactory);
        }
    }
}
