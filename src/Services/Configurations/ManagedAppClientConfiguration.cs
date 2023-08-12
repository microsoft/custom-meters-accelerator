// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.
namespace ManagedApplicationScheduler.Services.Configurations;

/// <summary>
/// Fulfillment Client Configuration.
/// </summary>
public class ManagedAppClientConfiguration
{
    /// <summary>
    /// Gets or sets the type of the grant.
    /// </summary>
    /// <value>
    /// The type of the grant.
    /// </value>
    public string GrantType { get; set; }

    /// <summary>
    /// Gets or sets the client identifier.
    /// </summary>
    /// <value>
    /// The client identifier.
    /// </value>
    public string ClientId { get; set; }

    /// <summary>
    /// Gets or sets the client secret.
    /// </summary>
    /// <value>
    /// The client secret.
    /// </value>
    public string ClientSecret { get; set; }

    /// <summary>
    /// Gets or sets the AAD Client ID resource.
    /// </summary>
    /// <value>
    /// The resource.
    /// </value>

    /// <summary>
    /// Gets or sets the signed out redirect URI.
    /// </summary>
    /// <value>
    /// The signed out redirect URI.
    /// </value>
    public string SignedOutRedirectUri { get; set; }

    /// <summary>
    /// Gets or sets the tenant identifier.
    /// </summary>
    /// <value>
    /// The tenant identifier.
    /// </value>
    public string TenantId { get; set; }

    public string Scope { get; set; }
    /// <summary>
    /// Gets or sets the API version.
    /// </summary>
    /// <value>
    /// The API version.
    /// </value>
    public string AdAuthenticationEndPoint { get; set; }

    public string DataBaseName { get; set; }

    public string PC_TenantId { get; set; }
    public string PC_ClientSecret { get; set; }
    public string PC_ClientID { get; set; }
    public string PC_Scope { get; set; }

    public string Marketplace_Uri { get; set; }

    public string Signature { get; set; }
}