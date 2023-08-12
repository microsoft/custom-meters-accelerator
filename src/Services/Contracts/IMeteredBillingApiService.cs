// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using ManagedApplicationScheduler.Services.Models;
using System.Threading.Tasks;

namespace ManagedApplicationScheduler.Services.Contracts;

/// <summary>
/// Metered ApiClient Interface.
/// </summary>
public interface IMeteredBillingApiService
{
    /// <summary>
    /// Emits the usage event asynchronous.
    /// </summary>
    /// <param name="usageEventRequest">The usage event request.</param>
    /// <returns>Event usage.</returns>
    Task<MeteredUsageResultModel> EmitUsageEventAsync(MeteredUsageRequestModel usageEventRequest);


}