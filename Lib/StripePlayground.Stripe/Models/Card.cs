using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using Newtonsoft.Json;

namespace StripePlayground.Stripe.Models
{
    public class Card
    {
        /// <summary>
        /// Name on card.
        /// </summary>
        [JsonProperty(PropertyName = "Name", Required = Required.Always)]
        public string Name { get; set; }

        /// <summary>
        /// The card number.
        /// </summary>
        [JsonProperty(PropertyName = "Number", Required = Required.Always)]
        public string Number { get; set; }

        /// <summary>
        /// The card Cvc.
        /// </summary>
        [JsonProperty(PropertyName = "Cvc", Required = Required.Always)]
        public string Cvc { get; set; }

        /// <summary>
        /// Card expiration month.
        /// </summary>
        [JsonProperty(PropertyName = "ExpirationMonth", Required = Required.Always)]
        public long ExpirationMonth { get; set; }

        /// <summary>
        /// Card expiration year.
        /// </summary>
        [JsonProperty(PropertyName = "ExpirationYear", Required = Required.Always)]
        public long ExpirationYear { get; set; }

        /// <summary>
        /// Card description.
        /// </summary>
        [JsonProperty(PropertyName = "Description", Required = Required.Default, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [DefaultValue(null)]
        public string Description { get; set; }
    }
}
