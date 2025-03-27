// See https://aka.ms/new-console-template for more information
using System.ComponentModel;
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
            Arguments = new KernelArguments(new PromptExecutionSettings() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() }),

        };

KernelPlugin plugin =  KernelPluginFactory.CreateFromType<MenuPlugin>();
agent.Kernel.Plugins.Add(plugin);

// Create a message and a thread
var input = "What is the special soup and its price?";
ChatMessageContent message = new(AuthorRole.User, input);

AgentThread? thread = null;

// Invoke the agent
await foreach (AgentResponseItem<ChatMessageContent> response in agent.InvokeAsync(message, thread))
{
    Console.WriteLine($"Response: {response.Message}");
    thread = response.Thread;
}

sealed class MenuPlugin
{
    [KernelFunction, Description("Provides a list of specials from the menu.")]
    public string GetSpecials() =>
        """
        Special Soup: Clam Chowder
        Special Salad: Cobb Salad
        Special Drink: Chai Tea
        """;

    [KernelFunction, Description("Provides the price of the requested menu item.")]
    public string GetItemPrice(
        [Description("The name of the menu item.")]
        string menuItem) =>
        "$9.99";
}
