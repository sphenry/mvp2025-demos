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

KernelPlugin plugin = KernelPluginFactory.CreateFromType<MenuPlugin>();
kernel.Plugins.Add(plugin);

var text =
    """
    type: chat_completion_agent
    name: FunctionCallingAgent
    instructions: Use the provided functions to answer questions about the menu.
    description: This agent uses the provided functions to answer questions about the menu.
    model:
        options:
        temperature: 0.4
    tools:
        - id: MenuPlugin.GetSpecials
        type: function
        - id: MenuPlugin.GetItemPrice
        type: function
    """;
var kernelAgentFactory = new ChatCompletionAgentFactory();

var agent = await kernelAgentFactory.CreateAgentFromYamlAsync(text, kernel);

await foreach (ChatMessageContent response in agent!.InvokeAsync(
    new ChatMessageContent(AuthorRole.User, "What is the special soup and how much does it cost?")
    ))
{
    this.WriteAgentChatMessage(response);
}