using System.Text.Json.Serialization;

namespace Naspinski.FoodTruck.Admin.Models.SquareModels
{

    public class SquareWebhookRequest
    {
        [JsonPropertyName("data")]
        public SquareWebhookData Data { get; set; }
    }

    public class SquareWebhookData
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("object")]
        public SquareWebhookObject Object { get; set; }
    }


    public class SquareWebhookObject
    {
        [JsonPropertyName("order_fulfillment_updated")]
        public OrderFullfillmentUpdated OrderFullfillmentUpdated { get; set; }
    }

    public class OrderFullfillmentUpdated
    {
        [JsonPropertyName("order_id")]
        public string OrderId { get; set; }

        [JsonPropertyName("fulfillment_update")]
        public OrderFullfillmentUpdate[] FullfillmentUpdate { get; set; }
    }

    public class OrderFullfillmentUpdate
    {
        [JsonPropertyName("new_state")]
        public string NewState { get; set; }

        [JsonPropertyName("old_state")]
        public string OldState { get; set; }
    }
}
