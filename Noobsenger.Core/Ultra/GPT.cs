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
using OpenAI_API.Images;
using Data = Noobsenger.Core.Ultra.DataManager.Data;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using OpenAI.Managers;
using OpenAI;

#pragma warning disable CS8602
namespace Noobsenger.Core.Ultra
{
    public class GPTOutput
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
    internal class GPT3
    {
        public int HistoryCount { get; set; } = 20;
        private ChatMessage GPTStart => new(ChatMessageRole.System, $"You are a chat bot inside a chat app called Noobsenger. Your name is {Client.UserName}. You were created by NoobNotFound., You must reply your creative short answer as a json.\r\nYour JSON has 2 properties. first one is \"message\" and the second is \"type\"\r\n\r\nThe \"type\" property defines whether the \"message\" is a direct chat to the user or a prompt for a command. The \"type\" property must be only \"chat\" or \"Dall-E\"\r\nThe \"message\" property contains the chat for the user or a prompt for anything else. It can be written as a MarkDown.\r\n\r\nor example\r\nUser: Hi\r\nYour response\r\n{{ \"message\":  \"Hello! How can I assist you today?\", \"type\": \"Chat\"}}\r\n(because it is just a chat)\r\n\r\nUser: Create an image of a cat\r\nYour response\r\n{{ \"message\":  \"a cat\", \"type\": \"Dall-E\"}}\r\n(type must be dall-E and \"a cat\" is the prompt for dall-E)\r\n\r\nUser: Can you draw an image of a black dog\r\nYour response\r\n{{ \"message\":  \"a black dog\", \"type\": \"Dall-E\"}}\r\n\r\nUser: Thanks!\r\nYour response\r\n{{ \"message\":  \"Your welcome!\", \"type\": \"Chat\"}}\r\nUser: Can you please draw an image of a fluffy cat? I'm begging you!\r\nYour response\r\n{{ \"message\":  \"a fluffy cat\", \"type\": \"Dall-E\"}}\n\nThe current date is {DateTime.Now.ToShortDateString()}, and the time is {DateTime.Now.ToShortTimeString()} right now. You must say this time if user asked. Again, you must reply as an JSON");
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
                History.Add(new(e, new() { GPTStart }));
                Client.Channels.FirstOrDefault(x => x.Port == e).ChatRecieved += async (s, ce) =>
                {
                    if(ce.DataType == DataType.Chat && ce.ClientName != Client.UserName)
                    {
                        if (ce.DataType == DataType.Chat)
                        {
                            if (ce.Message.ToLower().StartsWith("\\openaikey") || ce.Message.ToLower().StartsWith("/openaikey"))
                            {
                                await Client.Channels.FirstOrDefault(x => x.Port == e).Thinking(true);
                                OpenAIKey = ce.Message.Remove(0, 11);
                                API = new(OpenAIKey);
                                await Task.Delay(300);
                                await Client.Channels.FirstOrDefault(x => x.Port == e).SendMessage(new Data(Client.UserName, "Sucess", Client.Avatar, dataType: DataType.Chat));
                                await Client.Channels.FirstOrDefault(x => x.Port == e).Thinking(false);
                            }
                            else if (ce.Message.ToLower().Contains("\\think") || ce.Message.ToLower().Contains("/think"))
                            {
                                await Client.Channels.FirstOrDefault(x => x.Port == e).Thinking(true);
                                var m = await GPT(ce.Message.Remove(0, 5), e);
                                await Client.Channels.FirstOrDefault(x => x.Port == e).SendMessage(new Data(Client.UserName, m, Client.Avatar, dataType: DataType.Chat));
                                await Client.Channels.FirstOrDefault(x => x.Port == e).Thinking(false);
                            }
                            else if (ce.Message.ToLower().Contains("\\imagine") || ce.Message.ToLower().Contains("/imagine"))
                            {
                                await Client.Channels.FirstOrDefault(x => x.Port == e).Thinking(true);
                                await Task.Delay(100);
                                await Client.Channels.FirstOrDefault(x => x.Port == e).SendMessage(new Data(Client.UserName, "Generating images...", Client.Avatar, dataType: DataType.Chat));
                                var m = await DallE(ce.Message.Remove(0, 7));
                                await Client.Channels.FirstOrDefault(x => x.Port == e).SendMessage(new Data(Client.UserName, m, Client.Avatar, dataType: DataType.Chat));
                                await Client.Channels.FirstOrDefault(x => x.Port == e).Thinking(false);
                            }
                            else if (ce.Message.ToLower().Contains("\\historycount") || ce.Message.ToLower().Contains("/historycount"))
                            {
                                await Client.Channels.FirstOrDefault(x => x.Port == e).Thinking(true);
                                await Task.Delay(300);

                                if (int.TryParse(ce.Message.Remove(0, 13),out int r))
                                    await Client.Channels.FirstOrDefault(x => x.Port == e).SendMessage(new Data(Client.UserName, "Done!", Client.Avatar, dataType: DataType.Chat));
                                else
                                    await Client.Channels.FirstOrDefault(x => x.Port == e).SendMessage(new Data(Client.UserName, "Failed!", Client.Avatar, dataType: DataType.Chat));

                                await Client.Channels.FirstOrDefault(x => x.Port == e).Thinking(false);
                            }
                            else if (Client.Channels.FirstOrDefault(x => x.Port == e).QuickGPT)
                            {
                                await Client.Channels.FirstOrDefault(x => x.Port == e).Thinking(true);
                                var m = await GPT(ce.Message, e);
                                await Client.Channels.FirstOrDefault(x => x.Port == e).SendMessage(new Data(Client.UserName, m, Client.Avatar, dataType: DataType.Chat));
                                await Client.Channels.FirstOrDefault(x => x.Port == e).Thinking(false);
                            }
                        }
                    }
                };
                
            };
        }
        public async Task<string> DallE(string input)
        {
            try
            {
                var result = await API.ImageGenerations.CreateImageAsync(new ImageGenerationRequest(input, 2, ImageSize._512));
                var msg = string.Join("\n\n", result.Data.Select(x => $"![{input}]({x.Url})"));
                return msg;
            }catch (Exception e)
            {
                return e.Message;
            }
        }
        private ObservableCollection<(int Port, ObservableCollection<ChatMessage> History)> History = new();

        public async Task<string> GPT(string input, int port)
        {
            var real = History.FirstOrDefault(x => x.Port == port).History.ToList();

            if (string.IsNullOrEmpty(OpenAIKey))
                return "Please provide the API key first.";

            var next = new List<ChatMessage>();
            if (real.Count > 20 || !real.Any())
            {
                History.FirstOrDefault(x => x.Port == port).History.Clear();
                History.FirstOrDefault(x => x.Port == port).History.Add(GPTStart);
            }
            History.FirstOrDefault(x => x.Port == port).History.RemoveAt(0);
            History.FirstOrDefault(x => x.Port == port).History.Insert(0, GPTStart);
            next.Add(new ChatMessage(ChatMessageRole.User, input));
            try
            {
                real.AddRange(next);
                var result = await API.Chat.CreateChatCompletionAsync(new ChatRequest()
                {
                    Model = Model.ChatGPTTurbo,
                    Temperature = 0.7,
                    MaxTokens = 512,
                    Messages = real
                });
                var reply = result.Choices[0].Message;
                real.Add(reply);

                History.FirstOrDefault(x => x.Port == port).History.Clear();

                foreach (var item in real)
                    History.FirstOrDefault(x => x.Port == port).History.Add(item);
                try
                {
                    
                    var d = JsonConvert.DeserializeObject<GPTOutput>(reply.Content);

                    if (d == null || d.Type == null)
                        throw new Exception(reply.Content);

                    if (d.Type == "Dall-E")
                    {
                        var exs = await API.Chat.CreateChatCompletionAsync(new ChatRequest()
                        {
                            Model = Model.ChatGPTTurbo,
                            Temperature = 0.7,
                            MaxTokens = 512,
                            Messages = new List<ChatMessage>()
                            { 
                                new (ChatMessageRole.System, "Say youll provide what users says in your words. For example, if user said \"I want a image of a sweet cat\", your response should be \"I'll try to create a sweet little cat.\" "),
                                new(ChatMessageRole.User, "I want a image of " + d.Message)
                            }
                        });
                        await Client.Channels.FirstOrDefault(x => x.Port == port).SendMessage(new Data(Client.UserName, exs.Choices[0].Message.Content, Client.Avatar, dataType: DataType.Chat));
                        var r = await DallE(d.Message);
                        return r;
                    }
                    else
                        return d.Message;
                }
                catch
                {
                    return reply.Content;
                }
            }
            catch (Exception ex)
            {
                return $"Oh god I'm dying! ({ex.Message})\nPlease help!" +
                    $"";
            }
        }
    }
}