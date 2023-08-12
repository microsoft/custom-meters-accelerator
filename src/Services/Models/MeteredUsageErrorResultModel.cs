// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System.Text.Json.Serialization;

namespace ManagedApplicationScheduler.Services.Models;


public class MeteredUsageErrorResultModel
{
    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    /// <value>
    /// The status.
    /// </value>
    [JsonPropertyName("additionalInfo")]
    public AdditionalInfo additionalInfo { get; set; }
    [JsonPropertyName("message")]
    public string Message { get; set; }
    [JsonPropertyName("code")]
    public string Code { get; set; }

    public class AdditionalInfo
    {
        public MeteredUsageResultModel acceptedMessage { get; set; }

    }

}