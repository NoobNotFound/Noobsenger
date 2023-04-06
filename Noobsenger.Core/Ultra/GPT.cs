using OpenAI_API.Completions;
using OpenAI_API.Models;
using OpenAI_API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Noobsenger.Core.Ultra.DataManager;
using OpenAI_API.Chat;

namespace Noobsenger.Core.Ultra
{
    internal class GPT3
    {
        private ChatMessage GPTStart => new(ChatMessageRole.System, $"You are a chat bot inside a chat app called Noobsenger. Your name is GPTNoob. You were created by a developer called NoobNotFound. Answer friendly, creatively and shortly if posiible.");
        private UltraClient Client { get; set; }
        private OpenAIAPI API { get; set; }
        private string OpenAIKey { get; set; }
        public GPT3(UltraClient client)
        {
            API = new OpenAIAPI();
            Client = client;
            Client.ChannelRemoved += (_, e) => History.Remove(History.FirstOrDefault(x => x.Port == e));
            Client.ChannelAdded += (_, e) =>
            {
                History.Add(new(e, new List<ChatMessage>() { GPTStart }));
                Client.Channels.FirstOrDefault(x => x.Port == e).ChatRecieved += async (s, ce) =>
                {
                    if(ce.DataType == DataType.Chat && ce.ClientName != "GPTNoob")
                    {
                        if (ce.DataType == DataType.Chat)
                        {
                            if (ce.Message.ToLower().StartsWith("\\openaikey") || ce.Message.ToLower().StartsWith("/openaikey"))
                            {
                                OpenAIKey = ce.Message.Remove(0, 11);
                                API = new(OpenAIKey);
                                await Client.Channels.FirstOrDefault(x => x.Port == e).SendMessage(new Data(Client.UserName, "Sucess", Client.Avatar, dataType: DataType.Chat));
                            }
                            else if (ce.Message.ToLower().StartsWith("\\gpt") || ce.Message.ToLower().StartsWith("/gpt"))
                            {
                                var m = await GPT(ce.Message.Replace("\\gpt", "").Replace("\\GPT", "").Replace("\\Gpt", "").Replace("/gpt", "").Replace("/GPT", "").Replace("/Gpt", ""), e);
                                await Client.Channels.FirstOrDefault(x => x.Port == e).SendMessage(new Data(Client.UserName, m, Client.Avatar, dataType: DataType.Chat));
                            }
                            else if (Client.Channels.FirstOrDefault(x => x.Port == e).QuickGPT)
                            {
                                var m = await GPT(ce.Message, e);
                                await Client.Channels.FirstOrDefault(x => x.Port == e).SendMessage(new Data(Client.UserName, m, Client.Avatar, dataType: DataType.Chat));
                            }
                        }
                    }
                };
                
            };
        }

        private List<(int Port, List<ChatMessage> History)> History = new();

        public async Task<string> GPT(string input, int port)
        {
            if (string.IsNullOrEmpty(OpenAIKey))
                return "Please provide the API key first.";

            var history = new List<ChatMessage>();

            if (History.FirstOrDefault(x => x.Port == port).History.Count > 15)
            {
                History.FirstOrDefault(x => x.Port == port).History.Clear();
                history.Add(GPTStart);
            }

            history.Add(new ChatMessage(ChatMessageRole.System, $"The current date is {DateTime.Now.ToShortDateString()}, and the time is {DateTime.Now.ToShortDateString()} right now. You must say this time if user asked."));
            history.Add(new ChatMessage(ChatMessageRole.User, input));
            try
            {
                var real = History.FirstOrDefault(x => x.Port == port).History;
                real.AddRange(history);
                var result = await API.Chat.CreateChatCompletionAsync(new ChatRequest()
                {
                    Model = Model.ChatGPTTurbo,
                    Temperature = 0.6,
                    MaxTokens = 512,
                    Messages = real
                });
                var reply = result.Choices[0].Message;
                history.Add(reply);
                History.FirstOrDefault(x => x.Port == port).History.AddRange(history);
                return reply.Content;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
