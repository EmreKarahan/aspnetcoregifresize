using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Hubs;

namespace GifResize.Hubs
{
    [HubName("testHub")]
    public class TestHub : Hub
    {
        public void NewContosoChatMessage(string name, string message)
        {
            Clients.All.addContosoChatMessageToPage(name, message);
        }
    }
}