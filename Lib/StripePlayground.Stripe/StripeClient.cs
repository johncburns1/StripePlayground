using System;
using System.Collections.Generic;
using System.Text;

using Stripe;

namespace StripePlayground.Stripe
{
    public class StripeClient
    {
        // The Stripe API keys
        private const string PUBLISHABLE_KEY = "pk_test_lF36iYnW4BWNCrnBjg6vINsn002sr1hR1e";
        private const string SECRET_KEY      = "sk_test_DlSneVoe5MAQNRTWJT4w6iye003r1BgO4u";

        // class fields
        public OrderClient orderClient;

        /// <summary>
        /// Constructor for Stripe server.
        /// </summary>
        public StripeClient()
        {
            this.orderClient = new OrderClient();
        }

        /// <summary>
        /// Constructor for Stripe server.
        /// </summary>
        public StripeClient(OrderClient orderClient)
        {
            this.orderClient = orderClient;
        }

        /// <summary>
        /// Returns the <see cref="string"/> Stripe publishable api key.
        /// </summary>
        /// 
        /// <returns>The <see cref="string"/> Stripe publishable api key.</returns>
        public static string GetPublishableKey()
        {
            return PUBLISHABLE_KEY;
        }

        /// <summary>
        /// Creates a new Strip <see cref="SetupIntent"/>
        /// and returns the resulting <see cref="SetupIntent"/>.
        /// </summary>
        /// 
        /// <param name="paymentMethodId">The <see cref="string"/> Id of the <see cref="PaymentMethod"/> to use.</param>
        /// <param name="accountId">The <see cref="string"/> orderId.</param>
        /// <param name="orderId">The <see cref="string"/> accountId.</param>
        /// 
        /// <returns>The <see cref="string"/> client secret of the resulting <see cref="SetupIntent"/>.</returns>
        public string CreateSetupIntent(
            string paymentMethodId,
            string orderId, 
            string accountId)
        {
            // SetupIntentCreateOptions allows us to specify options for
            // creating SetupIntents for future payments.
            var options = new SetupIntentCreateOptions
            {
                Usage              = "off_session",
                Confirm            = true,
                PaymentMethodTypes = new List<string> { "card" },
                PaymentMethod      = paymentMethodId,
                Metadata           = new Dictionary<string, string>
                {
                    { "OrderId",   orderId },
                    { "AccountId", accountId }
                },
            };

            // request options for setting API key and IdempotencyKey
            var requestOptions = new RequestOptions
            {
                ApiKey         = SECRET_KEY,
                IdempotencyKey = Guid.NewGuid().ToString()
            };

            // create the SetupIntentService used to generate the
            // SetupIntent
            var service = new SetupIntentService();

            // Create the SetupIntent using the service and the options.
            var setupIntent = service.Create(options: options, requestOptions: requestOptions);

            // update the order at the specified orderId with the
            // new SetupIntentId.
            var order           = orderClient.GetOrderById(orderId);
            order.SetupIntentId = setupIntent.Id;
            
            orderClient.UpdateOrder(orderId, order);

            // returns the SetupIntent
            return setupIntent.ClientSecret;
        }

        /// <summary>
        /// Gets a <see cref="SetupIntent"/> from Stripe.
        /// </summary>
        /// 
        /// <param name="clientSecret">The <see cref="string"/> client secret from the <see cref="SetupIntent"/> we are
        /// trying to retrieve.</param>
        /// <param name="orderId">The <see cref="string"/> orderId of the order associated with the 
        /// <see cref="SetupIntent"/> we are trying to get.</param>
        /// 
        /// <returns>The <see cref="SetupIntent"/> at the specified orderId.</returns>
        public SetupIntent GetSetupIntent(string clientSecret, string orderId)
        {
            var order = orderClient.GetOrderById(orderId);

            var setupIntentGetOptions = new SetupIntentGetOptions
            {
                ClientSecret = clientSecret
            };

            var requestOptions = new RequestOptions
            {
                ApiKey         = GetPublishableKey(),
                IdempotencyKey = Guid.NewGuid().ToString()
            };

            var service = new SetupIntentService();

            var setupIntent = service.Get(
                setupIntentId:  order.SetupIntentId, 
                options:        setupIntentGetOptions, 
                requestOptions: requestOptions);

            return setupIntent;
        } 

        /// <summary>
        /// Creates a payment method from a tokenized <see cref="Card"/>.
        /// </summary>
        /// 
        /// <param name="token">The <see cref="string"/> tokenId of the card <see cref="Token"/>.</param>
        /// <param name="address">The <see cref="AddressOptions"/> of the card billing address.</param>
        /// <param name="name">The <see cref="string"/> name of the card owner.</param>
        /// <param name="phone">The <see cref="string"/> phone number of the card owner.</param>
        /// <param name="email"><see cref="string"/> email.</param>
        /// 
        /// <returns>The <see cref="string"/> id of the resulting <see cref="PaymentMethod"/>.</returns>
        public string CreatePaymentMethod(
            string         token,
            AddressOptions address,
            string         name  = null,
            string         phone = null,
            string         email = null)
        {
            var paymentMethodoptions = new PaymentMethodCreateOptions
            {
                Type = "card",
                Card = new PaymentMethodCardCreateOptions
                {
                    Token = token
                },
                BillingDetails = new BillingDetailsOptions
                {
                    Address = address,
                    Name    = name,
                    Email   = email,
                    Phone   = phone,
                }
            };

            // request options for setting API key and IdempotencyKey
            var requestOptions = new RequestOptions
            {
                ApiKey         = GetPublishableKey(),
                IdempotencyKey = Guid.NewGuid().ToString()
            };

            var service       = new PaymentMethodService();
            var paymentMethod = service.Create(options: paymentMethodoptions, requestOptions: requestOptions);

            return paymentMethod.Id;
        }

        /// <summary>
        /// Confirm a <see cref="SetupIntent"/>.
        /// </summary>
        /// 
        /// <param name="intent">The <see cref="SetupIntent"/> to confirm.</param>
        /// <param name="paymentMethod">The <see cref="PaymentMethod"/> to attach to the <see cref="SetupIntent"/>.</param>
        /// 
        /// <returns>The resulting <see cref="SetupIntent"/>.</returns>
        public static string ConfirmIntent(SetupIntent intent, PaymentMethod paymentMethod)
        {
            var confirmOptions = new SetupIntentConfirmOptions
            {
                ClientSecret  = intent.ClientSecret,
                PaymentMethod = paymentMethod.Id,
            };

            // request options for setting API key and IdempotencyKey
            var requestOptions = new RequestOptions
            {
                ApiKey         = GetPublishableKey(),
                IdempotencyKey = Guid.NewGuid().ToString()
            };

            var metadata = intent.Metadata;

            var setupIntentService = new SetupIntentService();
            
            intent = setupIntentService.Confirm(
                setupIntentId:  intent.Id,
                options:        confirmOptions,
                requestOptions: requestOptions);

            intent.Metadata = metadata;

            return intent.Id;
        }

        /// <summary>
        /// Creates a Stripe credit card <see cref="Token"/>.
        /// </summary>
        /// 
        /// <param name="name">The <see cref="string"/> name on the card.</param>
        /// <param name="number">The <see cref="string"/> card number.</param>
        /// <param name="cvc">The <see cref="string"/> card cvc.</param>
        /// <param name="expMonth">The <see cref="long"/> expiration month.</param>
        /// <param name="expYear">The <see cref="long"/> expiration year.</param>
        /// 
        /// <returns>The <see cref="string"/> Id of the resulting <see cref="Token"/>.</returns>
        public static string CreateCardToken(
            string name,
            string number,
            string cvc,
            long   expMonth,
            long   expYear)
        {
            var tokenOptions = new TokenCreateOptions
            {
                Card = new CreditCardOptions
                {
                    Name     = name,
                    Number   = number,
                    Cvc      = cvc,
                    ExpMonth = expMonth,
                    ExpYear  = expYear
                }
            };

            var requestOptions = new RequestOptions
            {
                ApiKey         = GetPublishableKey(),
                IdempotencyKey = Guid.NewGuid().ToString()
            };

            var service = new TokenService();
            var token   = service.Create(options: tokenOptions, requestOptions: requestOptions);

            return token.Id;
        }
    }
}
