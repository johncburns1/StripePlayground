using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using Newtonsoft.Json;

namespace StripePlayground.Stripe.Models
{
    public class Order
    {
        /// <summary>
        /// Id of the order.
        /// </summary>
        [JsonProperty(PropertyName = "Id", Required = Required.Always)]
        public string Id { get; set; }

        /// <summary>
        /// Id of the setup intent.
        /// </summary>
        [JsonProperty(PropertyName = "SetupIntentId", Required = Required.Always)]
        public string SetupIntentId { get; set; }

        /// <summary>
        /// Id of the payment intent.
        /// </summary>
        [JsonProperty(PropertyName = "PaymentIntentId", Required = Required.Always)]
        public string PaymentIntentId { get; set; }
    }
}
