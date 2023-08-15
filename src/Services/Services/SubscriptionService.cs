// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using ManagedApplicationScheduler.DataAccess.Contracts;
using ManagedApplicationScheduler.DataAccess.Entities;
using ManagedApplicationScheduler.Services.Models;
using System.Collections.Generic;
using System.Linq;



namespace ManagedApplicationScheduler.Services.Services;

/// <summary>
/// Subscriptions Service.
/// </summary>
public class SubscriptionService
{
    /// <summary>
    /// The subscription repository.
    /// </summary>
    private readonly ISubscriptionsRepository subscriptionRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="SubscriptionService"/> class.
    /// </summary>
    /// <param name="subscriptionRepo">The subscription repo.</param>
    /// <param name="planRepository">The plan repository.</param>
    /// <param name="currentUserId">The current user identifier.</param>
    public SubscriptionService(ISubscriptionsRepository subscriptionRepository)
    {
        this.subscriptionRepository = subscriptionRepository;

    }

    public int SaveSubscription(SubscriptionModel subscription)
    {
        var entity = new Subscription
        {
            PlanId = subscription.PlanId,
            Product = subscription.Product,
            ProvisionState = subscription.ProvisionState,
            ProvisionTime = subscription.ProvisionTime,
            Publisher = subscription.Publisher,
            ResourceUsageId = subscription.ResourceUsageId,
            id = subscription.id,
            PartitionKey = subscription.id,
            SubscriptionStatus = subscription.SubscriptionStatus,
            Version = subscription.Version,
            Dimension = subscription.Dimension
        };

        return this.subscriptionRepository.Save(entity);


    }

    public void UpdateSubscription(SubscriptionModel subscription)
    {

        var entity = this.subscriptionRepository.Get(subscription.id);
        entity.PlanId = subscription.PlanId;
        entity.Product = subscription.Product;
        entity.Publisher = subscription.Publisher;
        entity.Version = subscription.Version;
        entity.Dimension = subscription.Dimension;
        this.subscriptionRepository.Update(entity);


    }

    public void DeleteSubscription(SubscriptionModel subscription)
    {
        var entity = this.subscriptionRepository.Get(subscription.id);
        entity.PlanId = subscription.PlanId;
        entity.Product = subscription.Product;
        entity.ProvisionState = subscription.ProvisionState;
        entity.ProvisionTime = subscription.ProvisionTime;
        entity.Publisher = subscription.Publisher;
        entity.ResourceUsageId = subscription.ResourceUsageId;
        entity.Version = subscription.Version;
        entity.Dimension = subscription.Dimension;
        entity.SubscriptionStatus = "Unsubscribed";
        this.subscriptionRepository.Update(entity);
    }

    public void DeleteSubscription(string id)
    {
        // Check if the subscription has any Tasks
        var entity = this.subscriptionRepository.Get(id);
        this.subscriptionRepository.Remove(entity);

    }

    public void UpdateSubscriptionStatus(string id, string status)
    {
        var entity = this.subscriptionRepository.Get(id);
        entity.SubscriptionStatus = status;
        this.subscriptionRepository.Update(entity);
    }

    public SubscriptionModel GetSubscriptionByID(string id)
    {
        var subscriptionModel = new SubscriptionModel();
        var subscription = this.subscriptionRepository.Get(id);
        subscriptionModel.PlanId = subscription.PlanId;
        subscriptionModel.Product = subscription.Product;
        subscriptionModel.ProvisionState = subscription.ProvisionState;
        subscriptionModel.ProvisionTime = subscription.ProvisionTime;
        subscriptionModel.Publisher = subscription.Publisher;
        subscriptionModel.ResourceUsageId = subscription.ResourceUsageId;
        subscriptionModel.id = subscription.id;
        subscriptionModel.SubscriptionStatus = subscription.SubscriptionStatus;
        subscriptionModel.Version = subscription.Version;
        subscriptionModel.Dimension = subscription.Dimension;

        return subscriptionModel;
    }

    public List<SubscriptionModel> GetSubscriptions()
    {
        var subscriptions = new List<SubscriptionModel>();
        var entities = this.subscriptionRepository.GetAll();
        foreach (Subscription entity in entities)
        {
            var subscriptionModel = new SubscriptionModel
            {
                PlanId = entity.PlanId,
                Product = entity.Product,
                ProvisionState = entity.ProvisionState,
                ProvisionTime = entity.ProvisionTime,
                Publisher = entity.Publisher,
                ResourceUsageId = entity.ResourceUsageId,
                id = entity.id,
                SubscriptionStatus = entity.SubscriptionStatus,
                Version = entity.Version,
                Dimension = entity.Dimension
            };
            subscriptions.Add(subscriptionModel);
        }

        return subscriptions;
    }
    public List<SubscriptionViewModel> GetSubscriptionsView()
    {
        var subscriptions = new List<SubscriptionViewModel>();
        var entities = this.subscriptionRepository.GetAll();
        foreach (var sub in entities)
        {


            subscriptions.Add(new SubscriptionViewModel
            {
                id = sub.id,
                Product = sub.Product,
                AppId = sub.id.Split("|")[8],
                PlanId = sub.PlanId,
                Subscription = sub.id.Split("|")[2],
                ProvisionState = sub.ProvisionState,
                SubscriptionStatus = sub.SubscriptionStatus
            });
        }
        return subscriptions;
    }

    public SubscriptionViewModel GetSubscriptionsViewById(string subscriptionId)
    {

        var sub = this.subscriptionRepository.Get(subscriptionId);

        return new SubscriptionViewModel
        {
            id = sub.id,
            Product = sub.Product,
            AppId = sub.id.Split("|")[8],
            PlanId = sub.PlanId,
            Subscription = sub.id.Split("|")[2],
            ProvisionState = sub.ProvisionState,
            SubscriptionStatus = sub.SubscriptionStatus
        };


    }


    public List<SubscriptionModel> GetActiveSubscriptionsWithMeteredPlan()
    {
        var subscriptions = new List<SubscriptionModel>();
        var entities = this.subscriptionRepository.GetAll().Where(s => s.SubscriptionStatus == "Subscribed" && s.Dimension != null).ToList();

        foreach (Subscription entity in entities)
        {
            var subscriptionModel = new SubscriptionModel
            {
                PlanId = entity.PlanId,
                Product = entity.Product,
                ProvisionState = entity.ProvisionState,
                ProvisionTime = entity.ProvisionTime,
                Publisher = entity.Publisher,
                ResourceUsageId = entity.ResourceUsageId,
                id = entity.id,
                SubscriptionStatus = entity.SubscriptionStatus,
                Version = entity.Version,
                Dimension = entity.Dimension
            };
            subscriptions.Add(subscriptionModel);
        }

        return subscriptions;

    }
}