using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.IO;
using System.Net.Http.Headers;
using System.Collections;

namespace Bot_Application1
{
	[BotAuthentication]
	public class MessagesController : ApiController
	{

		/// <summary>
		/// POST: api/Messages
		/// Receive a message from a user and reply to it
		/// </summary>
		public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
		{
			ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
			Activity reply;
			if (activity.Type == ActivityTypes.Message)
			{
				if (activity.Text.ToUpper().Contains("HI") || activity.Text.ToUpper().Contains("HELLO") || activity.Text.ToUpper().Contains("HEY"))
				{					
					reply = activity.CreateReply("Hi, I am Akash, How can I help you?");
				}
				else if (activity.Text.ToUpper().Contains("PAYMENT")) {
					string x = await CallWebAPIAsync("http://akashpc/WebAPIToCall/", "api/values/5");
					reply = activity.CreateReply("Payment amount is " + x);
				}
				else { 
				int length = (activity.Text ?? string.Empty).Length;
				reply = activity.CreateReply($"You entered {length} characters");
				}

				await connector.Conversations.ReplyToActivityAsync(reply);
			}
			else
			{
				HandleSystemMessage(activity);
			}
			var response = Request.CreateResponse(HttpStatusCode.OK);
			return response;
		}

		private Activity HandleSystemMessage(Activity message)
		{
			if (message.Type == ActivityTypes.DeleteUserData)
			{
				// Implement user deletion here
				// If we handle user deletion, return a real message
			}
			else if (message.Type == ActivityTypes.ConversationUpdate)
			{
				// Handle conversation state changes, like members being added and removed
				// Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
				// Not available in all channels
			}
			else if (message.Type == ActivityTypes.ContactRelationUpdate)
			{
				// Handle add/remove from contact lists
				// Activity.From + Activity.Action represent what happened
			}
			else if (message.Type == ActivityTypes.Typing)
			{
				// Handle knowing tha the user is typing
			}
			else if (message.Type == ActivityTypes.Ping)
			{
			}

			return null;
		}

		private async Task<string> CallWebAPIAsync(string baseAd, string apiVal)
		{
			try
			{
				HttpClient client = new HttpClient();
				client.BaseAddress = new Uri(baseAd);

				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				HttpResponseMessage response = client.GetAsync(apiVal).Result;
				if (response.IsSuccessStatusCode)
				{
					return await response.Content.ReadAsStringAsync();
				}
			}
			catch (Exception e)
			{
				return e.Message;
			}

			return "No Data Present!";
		}
	}
}