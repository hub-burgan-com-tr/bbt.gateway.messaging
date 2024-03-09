using bbt.gateway.common.Api.Reminder.Model;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbt.gateway.common.Api.Reminder
{
    public interface IReminderApi
    {
        [Get("/notifications/customer/{customerId}/{pageIndex}/{pageSize}")]
        Task<List<NotificationInfo>> GetNotifications(string customerId, int pageIndex, int pageSize);
        [Post("/notifications/customer/setRead/{customerId}")]
        Task SetNotificationRead(string customerId, SetReadRequest setReadRequest);
        [Delete("/notifications/customer/{customerId}/{notificationId}")]
        Task DeleteNotification(string customerId, string notificationId);
        [Delete("/notifications/customer/{customerId}")]
        Task DeleteNotifications(string customerId);
        [Get("/notifications/customer/statistics/{customerId}")]
        Task<NotificationCount> GetNoficationsCount(string customerId);


    }
}
