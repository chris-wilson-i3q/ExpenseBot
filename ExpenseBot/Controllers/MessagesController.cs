using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace ExpenseBot
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
            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                // calculate something for us to return
                int length = (activity.Text ?? string.Empty).Length;


                // return our reply to the user
                //Activity reply = activity.CreateReply($"You sent {activity.Text} which was {length} characters");
                //await connector.Conversations.ReplyToActivityAsync(reply);
                await Conversation.SendAsync(activity, BuildDialog);

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

        private static IDialog<ExpenseCategorySelection> BuildDialog()
        {
            return Chain.From(() => FormDialog.FromForm(BuildForm));
        }

        private static IForm<ExpenseCategorySelection> BuildForm()
        {
            return new FormBuilder<ExpenseCategorySelection>()
                .Message("What type of expense do you want to claim for?")
                .OnCompletion(FinishExpenseSelection)
                .Build();
        }
        public enum ExpenseCategoryTypes
        {
           
            Cartrip,
            Hotel,
            Train,
            Mileage
            
               
        }

        [Serializable]
        public class ExpenseCategorySelection
        {
            public ExpenseCategoryTypes ExpenseType;
            public string ToLocation;
            public string FromLocation;
            public int NoOfMiles;
        }

        private static Task FinishExpenseSelection(IDialogContext context, ExpenseCategorySelection state)
        {
            var message = context.MakeMessage();
            message.Text = $"Your new expense {state.ExpenseType} from {state.FromLocation} to {state.ToLocation} will be recorded. Please login to check and submit";
            //message.SetBotPerUserInConversationData(CarBuilderLabel, false);
            return context.PostAsync(message);
        }

    }
}