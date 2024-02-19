using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Utility.Utilities
{
    public static class StaticDetails
    {
        #region

        public const int INT_0 = 0;

        #endregion

        #region Roles
        public const string Role_Customer = "Customer";
        public const string Role_Company = "Company";
        public const string Role_Admin = "Admin";
        public const string Role_Employee = "Employee";
        #endregion

        #region Areas
        public const string AREA_ADMIN = "Admin";
        public const string AREA_CUSTOMER = "Customer";
        #endregion

        #region Notifications
        public const string NOTIF_ERROR = "Error";
        public const string NOTIF_SUCCESS = "Success";
        #endregion

        public const string STATUS_PENDING = "Pending";
        public const string STATUS_APPROVED = "Approved";
        public const string STATUS_INPROCESS = "Processing";
        public const string STATUS_SHIPPED = "Shipped";
        public const string STATUS_CANCELLED = "Cancelled";
        public const string STATUS_REFUNDED = "Refunded";

        public const string PAYMENT_PENDING = "Pending";
        public const string PAYMENT_APPROVED = "Approved";
        public const string PAYMENT_DELAYED = "ApprovedForDelayedPayment";
        public const string PAYMENT_REJECTED = "Rejected";
    }
}
