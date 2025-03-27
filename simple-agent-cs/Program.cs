using DotNetEnv;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
Env.Load();

// Create the kernel
var builder = Kernel.CreateBuilder();
builder.AddAzureOpenAIChatCompletion(
                Env.GetString("AZURE_OPENAI_DEPLOYMENT"),
                Env.GetString("AZURE_OPENAI_ENDPOINT"),
                Env.GetString("AZURE_OPENAI_API_KEY")
                );
var kernel = builder.Build();   

string AgentName = "MySemanticKernelAgent";
string AgentInstructions = "You are a helpful assistant. Be concise.";

// Create the agent
ChatCompletionAgent agent =
        new()
        {
            Name = AgentName,
            Instructions = AgentInstructions,
            Kernel = kernel,
        };

// Create a message and a thread
var input = "Tell me a haiku about Semantic Kernel";
ChatMessageContent message = new(AuthorRole.User, input);

AgentThread? thread = null;

// Invoke the agent
await foreach (AgentResponseItem<ChatMessageContent> response in agent.InvokeAsync(message, thread))
{
    Console.WriteLine($"Response: {response.Message}");
    thread = response.Thread;
}
