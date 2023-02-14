using OpenAI_API.Completions;
using OpenAI_API.Models;
using OpenAI_API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Noobsenger.Core.Ultra.DataManager;

namespace Noobsenger.Core.Ultra
{
    internal class GPT3
    {
        private UltraClient Client { get; set; }
        private OpenAIAPI API { get; set; }
        public GPT3(string key, UltraClient client)
        {
            API = new OpenAIAPI(key);
            Client = client;
            Client.ChannelAdded += (_, e) =>
            {
                Client.Channels.FirstOrDefault(x => x.Port == e).ChatRecieved += async (s, ce) =>
                {
                    if(ce.DataType == DataType.Chat)
                    {
                        if (ce.DataType == DataType.Chat)
                        {
                            if (ce.Message.ToLower().StartsWith("\\gpt") || ce.Message.ToLower().StartsWith("/gpt"))
                            {
                                var m = await TextDavinci(ce.Message.Replace("\\gpt", "").Replace("\\GPT", "").Replace("\\Gpt", "").Replace("/gpt", "").Replace("/GPT", "").Replace("/Gpt", ""));
                                await Client.Channels.FirstOrDefault(x => x.Port == e).SendMessage(new Data(Client.UserName, m, Client.Avatar, dataType: DataType.Chat));
                            }
                            if (ce.Message.ToLower().StartsWith("\\codex") || ce.Message.ToLower().StartsWith("/codex"))
                            {
                                var m = await Codex(ce.Message.Replace("\\codex", "").Replace("\\CODEX", "").Replace("\\Codex", "").Replace("/codex", "").Replace("/CODEX", "").Replace("/Codex", ""));
                                await Client.Channels.FirstOrDefault(x => x.Port == e).SendMessage(new Data(Client.UserName, m, Client.Avatar, dataType: DataType.Chat));
                            }
                        }
                    }
                };
                
            };
        }

        public async Task<string> Codex(string input)
        {
            try
            {
                string result = "";
                await API.Completions.StreamCompletionAsync(new CompletionRequest(input, model: Model.DavinciCode, temperature: 0.6), x => result += x.ToString());
                return result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> TextDavinci(string input)
        {
            try
            {
                var propmt = string.Join("\n", new string[]
                {
                "You are a chat bot inside a chat app called Noobsenger.",
                "Your name is GPTNoob.",
                "You were created by a developer called NoobNotFound.",
                "You respond to queries users ask you they are called your input, which could be anything. Your goal is to be pleasant and welcoming.",
                "User input may be multi-line, and you can respond with multiple lines as well. Here are some examples:",
                "Input: Hi!",
                "Your response: Hello! how can I help you?",
                "Input: i don't like you",
                "Also I'm bored",
                "Your response: I like you! I hope I can grow on you",
                "... hi bored, I'm dad!",
                "Input: why is the sky blue?",
                "Your response: As white light passes through our atmosphere, tiny air molecules cause it to 'scatter'. The scattering caused by these tiny air molecules (known as Rayleigh scattering) increases as the wavelength of light decreases. Violet and blue light have the shortest wavelengths and red light has the longest.",
                "Input: " + input,
                "Your response: "
                });
                var result = await API.Completions.CreateCompletionAsync(new CompletionRequest(propmt, model: Model.DavinciText, temperature: 0.5));
                return result.Completions[0].Text;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
