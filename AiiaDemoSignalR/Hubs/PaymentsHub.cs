using AiiaDemoSignalR.Model;
using Microsoft.AspNetCore.SignalR;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AiiaDemoSignalR.Hubs
{
    public class PaymentsHub : Hub
    {
        static IList<User> users = new List<User>();

        public async Task ConnectPaymentUser(string userName, string approverName)
        {
            var paymentUser = users.FirstOrDefault(u => u.Name == userName);
            if (paymentUser == null)
            {
                users.Add(new User
                {
                    Name = userName,
                    ApproverName = approverName,
                    ConnectionId = Context.ConnectionId,
                    Type = UserType.Payment
                });
            }
            else
            {
                paymentUser.ConnectionId = Context.ConnectionId;
            }
        }

        public async Task ConnectApproverUser(string userName)
        {
            var approverUser = users.FirstOrDefault(u => u.Name == userName);
            if (approverUser == null)
            {
                users.Add(new User
                {
                    Name = userName,
                    ApproverName = string.Empty,
                    ConnectionId = Context.ConnectionId,
                    Type = UserType.Approver
                });
            }
            else
            {
                approverUser.ConnectionId = Context.ConnectionId;
            }
        }

        public async Task RequestPayment(string merchant, string product, decimal amount, string currency, string authorizeUrl)
        {
            var currentUser = users.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
            if (currentUser != null)
            {
                var approverUser = users.FirstOrDefault(u => u.Name == currentUser.ApproverName);
                if (approverUser != null)
                {
                    await Clients.Client(approverUser.ConnectionId).SendAsync("RequestPaymentFromParent", currentUser.Name, merchant, product, amount, currency, authorizeUrl);
                }
            }
        }

        public async Task RejectPayment()
        {
            var currentUser = users.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
            if (currentUser != null)
            {
                var paymentUser = users.FirstOrDefault(u => u.ApproverName == currentUser.Name);
                if (paymentUser != null)
                {
                    await Clients.Client(paymentUser.ConnectionId).SendAsync("RejectPaymentToClient");
                }
            }
        }

        public async Task NotifyPaymentAuthorization(string paymentUserName, string authId)
        {
            var paymentUser = users.FirstOrDefault(u => u.Name == paymentUserName);
            if (paymentUser != null)
            {
                await Clients.Client(paymentUser.ConnectionId).SendAsync("NotifyPaymentAuthorizationToClient", authId);
            }
        }
    }
}
