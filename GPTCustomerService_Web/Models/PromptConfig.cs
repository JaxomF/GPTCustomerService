using Azure.AI.OpenAI;

namespace GPTCustomerService_Web.Models
{
    public class PromptConfig
    {
        public int schema { get; set; }
        public string type { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public Completion completion { get; set; }
        public Input input { get; set; }
    }

    public class Completion
    {
        public int max_tokens { get; set; }
        public float temperature { get; set; }
        public float top_p { get; set; }
        public float presence_penalty { get; set; }
        public float frequency_penalty { get; set; }
    }

    public class Input
    {
        public Parameter[] parameters { get; set; }
    }

    public class Parameter
    {
        public string name { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public string defaultValue { get; set; } = string.Empty;
    }
}
