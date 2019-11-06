using System;
using System.Collections.Generic;

using Stripe;

using StripePlayground.Stripe;
using StripePlayground.Stripe.Models;

using Xunit;

namespace Test.Stripe
{
    public class Unit
    {
        StripePlayground.Stripe.StripeClient stripeClient;
        OrderClient orderClient;

        public Unit()
        {
            this.orderClient  = new OrderClient();
            this.stripeClient = new StripePlayground.Stripe.StripeClient(orderClient);
        }

        //[Fact]
        //public List<SetupIntent> ConfirmIntents() 
        //{
        //    var setupIntents = new List<SetupIntent> 
        //    { 
        //        CreateSetupIntent(),
        //        CreateSetupIntent(),
        //        CreateSetupIntent(),
        //        CreateSetupIntent()
        //    };

        //    Assert.NotNull(setupIntents);
        //    Assert.Equal(4, setupIntents.Count);

        //    var paymentMethods = CreatePaymentMethods();

        //    Assert.NotNull(paymentMethods);
        //    Assert.Equal(4, paymentMethods.Count);

        //    var confirmedIntents = new List<SetupIntent>();
        //    var counter          = 0;
            
        //    foreach (var intent in setupIntents)
        //    {
        //        Assert.Equal("requires_payment_method", intent.Status);
                
        //        confirmedIntents.Add(client.ConfirmIntent(intent, paymentMethods[counter]));
        //        counter++;
        //    }

        //    foreach (var intent in confirmedIntents)
        //    {
        //        Assert.NotNull(intent);
        //        Assert.NotNull(intent.PaymentMethodId);
        //        Assert.Equal("my-order", intent.Metadata["OrderId"]);
        //        Assert.Equal("my-account", intent.Metadata["AccountId"]);
        //        Assert.Equal("off_session", intent.Usage);
        //        Assert.Single<string>(intent.PaymentMethodTypes);
        //        Assert.Equal("card", intent.PaymentMethodTypes[0]);
        //        Assert.NotEqual("requires_confirmation", intent.Status);
        //        Assert.NotNull(intent.ClientSecret);
        //        Assert.NotEmpty(intent.ClientSecret);
        //    }

        //    return confirmedIntents;
        //}

        [Fact]
        public SetupIntent CreateSetupIntent()
        {
            var paymentMethods = CreatePaymentMethods();
            
            foreach (var paymentMethod in paymentMethods)
            {
                var intentSecretClient = stripeClient.CreateSetupIntent(paymentMethod, "my-order", "my-account");

                Assert.Equal("my-order", intent.Metadata["OrderId"]);
                Assert.Equal("my-account", intent.Metadata["AccountId"]);
                Assert.Equal("off_session", intent.Usage);
                Assert.Single<string>(intent.PaymentMethodTypes);
                Assert.Equal("card", intent.PaymentMethodTypes[0]);
                Assert.Equal("requires_payment_method", intent.Status);
                Assert.NotNull(intent.ClientSecret);
                Assert.NotEmpty(intent.ClientSecret);
                Assert.Null(intent.PaymentMethod);
            }

            return intent;
        }

        [Fact]
        public void CreateCardTokens()
        {
            var cards = CreateCards();
            
            List<string> tokens = new List<string>();

            foreach (var card in cards)
            {
                tokens.Add(CreateCardToken(card));
            }

            foreach(var token in tokens)
            {
                Assert.NotNull(token);
                Assert.NotEmpty(token);
            }
        }

        public string CreatePaymentMethod(
            string         token, 
            string         name,
            AddressOptions address)
        {
            var paymentMethodId = stripeClient.CreatePaymentMethod(
                token:   token,
                address: address,
                name:    name
            );

            return paymentMethodId;
        }

        public List<AddressOptions> CreateAddresses()
        {
            var addresses = new List<AddressOptions>
            {
                new AddressOptions
                {
                    PostalCode = "98122",
                    State      = "WA",
                },
                new AddressOptions
                {
                    PostalCode = "98104",
                    State      = "WA",
                },
                new AddressOptions
                {
                    PostalCode = "63130",
                    State      = "MO",
                },
                new AddressOptions
                {
                    PostalCode = "63105",
                    State      = "MO",
                }
            };

            return addresses;
        }

        public string CreateCardToken(StripePlayground.Stripe.Models.Card card)
        {
            var token = StripePlayground.Stripe.StripeClient.CreateCardToken(
                name:     card.Name,
                number:   card.Number,
                cvc:      card.Cvc,
                expMonth: card.ExpirationMonth,
                expYear:  card.ExpirationYear
            );

            return token;
        }

        public List<StripePlayground.Stripe.Models.Card> CreateCards()
        {
            List<StripePlayground.Stripe.Models.Card> cards = new List<StripePlayground.Stripe.Models.Card>();

            cards.Add(new StripePlayground.Stripe.Models.Card
            {
                Name            = "Jack C Burns",
                Number          = "4000002500003155",
                Cvc             = "618",
                ExpirationMonth = (long)05,
                ExpirationYear  = (long)2021,
                Description     = "This test card requires authentication for one-time payments. " +
                    "However, if you set up this card using the Setup Intents API " +
                    "and use the saved card for subsequent payments, " +
                    "no further authentication is needed."
            });

            cards.Add(new StripePlayground.Stripe.Models.Card
            {
                Name            = "Jill B Johns",
                Number          = "4000002760003184",
                Cvc             = "210",
                ExpirationMonth = (long)12,
                ExpirationYear  = (long)2025,
                Description     = "This test card requires authentication on all transactions."
            });

            cards.Add(new StripePlayground.Stripe.Models.Card
            {
                Name            = "Zima C Blu",
                Number          = "4000008260003178",
                Cvc             = "216",
                ExpirationMonth = (long)10,
                ExpirationYear  = (long)2021,
                Description     = "This test card requires authentication, " +
                    "but payments will be declined with an insufficient_funds " +
                    "failure code after successful authentication."
            });

            cards.Add(new StripePlayground.Stripe.Models.Card
            {
                Name            = "Yadier D Molina",
                Number          = "4000000000003055",
                Cvc             = "417",
                ExpirationMonth = (long)01,
                ExpirationYear  = (long)2020,
                Description     = "This test card supports authentication via 3D Secure 2, " +
                    "but does not require it. Payments using this card do not require additional " +
                    "authentication in test mode unless your test mode Radar rules request authentication."
            });

            return cards;
        }
    }
}
