using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

using FISTNESSGYM.Data;

namespace FISTNESSGYM.Controllers
{
    public partial class ExportdatabaseController : ExportController
    {
        private readonly databaseContext context;
        private readonly databaseService service;

        public ExportdatabaseController(databaseContext context, databaseService service)
        {
            this.service = service;
            this.context = context;
        }

        [HttpGet("/export/database/aspnetroleclaims/csv")]
        [HttpGet("/export/database/aspnetroleclaims/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspNetRoleClaimsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAspNetRoleClaims(), Request.Query, false), fileName);
        }

        [HttpGet("/export/database/aspnetroleclaims/excel")]
        [HttpGet("/export/database/aspnetroleclaims/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspNetRoleClaimsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAspNetRoleClaims(), Request.Query, false), fileName);
        }

        [HttpGet("/export/database/aspnetroles/csv")]
        [HttpGet("/export/database/aspnetroles/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspNetRolesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAspNetRoles(), Request.Query, false), fileName);
        }

        [HttpGet("/export/database/aspnetroles/excel")]
        [HttpGet("/export/database/aspnetroles/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspNetRolesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAspNetRoles(), Request.Query, false), fileName);
        }

        [HttpGet("/export/database/aspnetuserclaims/csv")]
        [HttpGet("/export/database/aspnetuserclaims/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspNetUserClaimsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAspNetUserClaims(), Request.Query, false), fileName);
        }

        [HttpGet("/export/database/aspnetuserclaims/excel")]
        [HttpGet("/export/database/aspnetuserclaims/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspNetUserClaimsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAspNetUserClaims(), Request.Query, false), fileName);
        }

        [HttpGet("/export/database/aspnetuserlogins/csv")]
        [HttpGet("/export/database/aspnetuserlogins/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspNetUserLoginsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAspNetUserLogins(), Request.Query, false), fileName);
        }

        [HttpGet("/export/database/aspnetuserlogins/excel")]
        [HttpGet("/export/database/aspnetuserlogins/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspNetUserLoginsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAspNetUserLogins(), Request.Query, false), fileName);
        }

        [HttpGet("/export/database/aspnetuserroles/csv")]
        [HttpGet("/export/database/aspnetuserroles/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspNetUserRolesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAspNetUserRoles(), Request.Query, false), fileName);
        }

        [HttpGet("/export/database/aspnetuserroles/excel")]
        [HttpGet("/export/database/aspnetuserroles/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspNetUserRolesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAspNetUserRoles(), Request.Query, false), fileName);
        }

        [HttpGet("/export/database/aspnetusers/csv")]
        [HttpGet("/export/database/aspnetusers/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspNetUsersToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAspNetUsers(), Request.Query, false), fileName);
        }

        [HttpGet("/export/database/aspnetusers/excel")]
        [HttpGet("/export/database/aspnetusers/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspNetUsersToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAspNetUsers(), Request.Query, false), fileName);
        }

        [HttpGet("/export/database/aspnetusertokens/csv")]
        [HttpGet("/export/database/aspnetusertokens/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspNetUserTokensToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAspNetUserTokens(), Request.Query, false), fileName);
        }

        [HttpGet("/export/database/aspnetusertokens/excel")]
        [HttpGet("/export/database/aspnetusertokens/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspNetUserTokensToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAspNetUserTokens(), Request.Query, false), fileName);
        }

        [HttpGet("/export/database/events/csv")]
        [HttpGet("/export/database/events/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportEventsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetEvents(), Request.Query, false), fileName);
        }

        [HttpGet("/export/database/events/excel")]
        [HttpGet("/export/database/events/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportEventsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetEvents(), Request.Query, false), fileName);
        }

        [HttpGet("/export/database/orders/csv")]
        [HttpGet("/export/database/orders/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportOrdersToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetOrders(), Request.Query, false), fileName);
        }

        [HttpGet("/export/database/orders/excel")]
        [HttpGet("/export/database/orders/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportOrdersToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetOrders(), Request.Query, false), fileName);
        }

        [HttpGet("/export/database/orderitems/csv")]
        [HttpGet("/export/database/orderitems/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportOrderItemsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetOrderItems(), Request.Query, false), fileName);
        }

        [HttpGet("/export/database/orderitems/excel")]
        [HttpGet("/export/database/orderitems/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportOrderItemsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetOrderItems(), Request.Query, false), fileName);
        }

        [HttpGet("/export/database/orderstatuses/csv")]
        [HttpGet("/export/database/orderstatuses/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportOrderStatusesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetOrderStatuses(), Request.Query, false), fileName);
        }

        [HttpGet("/export/database/orderstatuses/excel")]
        [HttpGet("/export/database/orderstatuses/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportOrderStatusesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetOrderStatuses(), Request.Query, false), fileName);
        }

        [HttpGet("/export/database/products/csv")]
        [HttpGet("/export/database/products/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportProductsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetProducts(), Request.Query, false), fileName);
        }

        [HttpGet("/export/database/products/excel")]
        [HttpGet("/export/database/products/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportProductsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetProducts(), Request.Query, false), fileName);
        }

        [HttpGet("/export/database/reservations/csv")]
        [HttpGet("/export/database/reservations/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportReservationsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetReservations(), Request.Query, false), fileName);
        }

        [HttpGet("/export/database/reservations/excel")]
        [HttpGet("/export/database/reservations/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportReservationsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetReservations(), Request.Query, false), fileName);
        }

        [HttpGet("/export/database/subscriptions/csv")]
        [HttpGet("/export/database/subscriptions/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportSubscriptionsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetSubscriptions(), Request.Query, false), fileName);
        }

        [HttpGet("/export/database/subscriptions/excel")]
        [HttpGet("/export/database/subscriptions/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportSubscriptionsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetSubscriptions(), Request.Query, false), fileName);
        }

        [HttpGet("/export/database/subscriptionstatuses/csv")]
        [HttpGet("/export/database/subscriptionstatuses/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportSubscriptionStatusesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetSubscriptionStatuses(), Request.Query, false), fileName);
        }

        [HttpGet("/export/database/subscriptionstatuses/excel")]
        [HttpGet("/export/database/subscriptionstatuses/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportSubscriptionStatusesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetSubscriptionStatuses(), Request.Query, false), fileName);
        }

        [HttpGet("/export/database/productcategories/csv")]
        [HttpGet("/export/database/productcategories/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportProductCategoriesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetProductCategories(), Request.Query, false), fileName);
        }

        [HttpGet("/export/database/productcategories/excel")]
        [HttpGet("/export/database/productcategories/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportProductCategoriesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetProductCategories(), Request.Query, false), fileName);
        }

        [HttpGet("/export/database/cartitems/csv")]
        [HttpGet("/export/database/cartitems/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCartItemsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetCartItems(), Request.Query, false), fileName);
        }

        [HttpGet("/export/database/cartitems/excel")]
        [HttpGet("/export/database/cartitems/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCartItemsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetCartItems(), Request.Query, false), fileName);
        }
    }
}
