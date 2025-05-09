﻿using KosmosERP.BusinessLayer.Interfaces;
using KosmosERP.BusinessLayer.MessagePublisher;
using KosmosERP.Models;
using KosmosERP.Models.Interfaces;

namespace KosmosERP.BusinessLayer;

public class MessageFactory
{
    public static IMessagePublisher Create(IMessagePublisherSettings settings)
    {
        // Ensure data consistency. Will throw errors
        ValidateSettings(settings);

        if (settings.account_provider.Equals(MessagePublisherType.AWS, StringComparison.OrdinalIgnoreCase))
        {
            return new AmazonMessagePublisher(settings);
        }
        else if (settings.account_provider.Equals(MessagePublisherType.Azure, StringComparison.OrdinalIgnoreCase))
        {
            return new AzureMessagePublisher(settings);
        }
        else if (settings.account_provider.Equals(MessagePublisherType.Google, StringComparison.OrdinalIgnoreCase))
        {
            return new GoogleMessagePublisher(settings);
        }
        else if (settings.account_provider.Equals(MessagePublisherType.RabbitMq, StringComparison.OrdinalIgnoreCase))
        {
            return new RabbitMqMessagePublisher(settings);
        }
        else if (settings.account_provider.Equals(MessagePublisherType.MOCK, StringComparison.OrdinalIgnoreCase))
        {
            return new MockMessagePublisher(settings);
        }
        else
        {
            throw new ArgumentNullException("Messaging account provider not supported.");
        }
    }

    private static void ValidateSettings(IMessagePublisherSettings settings)
    {
        if (string.IsNullOrEmpty(settings.account_provider))
            throw new ArgumentNullException("Messaging account provider cannot be null or empty.");

        if(string.IsNullOrEmpty(settings.transaction_movement_topic))
            throw new ArgumentNullException($"Messaging topic or queue {RequiredMessageTopics.TransactionMovementTopic} must exist and cannot be null or empty.");
    }
}
