using System;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Radzen;

using FISTNESSGYM.Data;

namespace FISTNESSGYM
{
    public partial class databaseService
    {
        databaseContext Context
        {
           get
           {
             return this.context;
           }
        }

        private readonly databaseContext context;
        private readonly NavigationManager navigationManager;

        public databaseService(databaseContext context, NavigationManager navigationManager)
        {
            this.context = context;
            this.navigationManager = navigationManager;
        }

        public void Reset() => Context.ChangeTracker.Entries().Where(e => e.Entity != null).ToList().ForEach(e => e.State = EntityState.Detached);

        public void ApplyQuery<T>(ref IQueryable<T> items, Query query = null)
        {
            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }
        }


        public async Task ExportEventsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/events/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/events/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportEventsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/events/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/events/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnEventsRead(ref IQueryable<FISTNESSGYM.Models.database.Event> items);

        public async Task<IQueryable<FISTNESSGYM.Models.database.Event>> GetEvents(Query query = null)
        {
            var items = Context.Events.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnEventsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnEventGet(FISTNESSGYM.Models.database.Event item);
        partial void OnGetEventById(ref IQueryable<FISTNESSGYM.Models.database.Event> items);


        public async Task<FISTNESSGYM.Models.database.Event> GetEventById(int id)
        {
            var items = Context.Events
                              .AsNoTracking()
                              .Where(i => i.Id == id);

 
            OnGetEventById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnEventGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnEventCreated(FISTNESSGYM.Models.database.Event item);
        partial void OnAfterEventCreated(FISTNESSGYM.Models.database.Event item);

        public async Task<FISTNESSGYM.Models.database.Event> CreateEvent(FISTNESSGYM.Models.database.Event _event)
        {
            OnEventCreated(_event);

            var existingItem = Context.Events
                              .Where(i => i.Id == _event.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Events.Add(_event);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(_event).State = EntityState.Detached;
                throw;
            }

            OnAfterEventCreated(_event);

            return _event;
        }

        public async Task<FISTNESSGYM.Models.database.Event> CancelEventChanges(FISTNESSGYM.Models.database.Event item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnEventUpdated(FISTNESSGYM.Models.database.Event item);
        partial void OnAfterEventUpdated(FISTNESSGYM.Models.database.Event item);

        public async Task<FISTNESSGYM.Models.database.Event> UpdateEvent(int id, FISTNESSGYM.Models.database.Event _event)
        {
            OnEventUpdated(_event);

            var itemToUpdate = Context.Events
                              .Where(i => i.Id == _event.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(_event);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterEventUpdated(_event);

            return _event;
        }

        partial void OnEventDeleted(FISTNESSGYM.Models.database.Event item);
        partial void OnAfterEventDeleted(FISTNESSGYM.Models.database.Event item);

        public async Task<FISTNESSGYM.Models.database.Event> DeleteEvent(int id)
        {
            var itemToDelete = Context.Events
                              .Where(i => i.Id == id)
                              .Include(i => i.Reservations)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnEventDeleted(itemToDelete);


            Context.Events.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterEventDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportOrdersToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/orders/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/orders/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportOrdersToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/orders/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/orders/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnOrdersRead(ref IQueryable<FISTNESSGYM.Models.database.Order> items);

        public async Task<IQueryable<FISTNESSGYM.Models.database.Order>> GetOrders(Query query = null)
        {
            var items = Context.Orders.AsQueryable();

            items = items.Include(i => i.OrderStatus);
            items = items.Include(i => i.AspNetUser);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnOrdersRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnOrderGet(FISTNESSGYM.Models.database.Order item);
        partial void OnGetOrderById(ref IQueryable<FISTNESSGYM.Models.database.Order> items);


        public async Task<FISTNESSGYM.Models.database.Order> GetOrderById(int id)
        {
            var items = Context.Orders
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            items = items.Include(i => i.OrderStatus);
            items = items.Include(i => i.AspNetUser);
 
            OnGetOrderById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnOrderGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnOrderCreated(FISTNESSGYM.Models.database.Order item);
        partial void OnAfterOrderCreated(FISTNESSGYM.Models.database.Order item);

        public async Task<FISTNESSGYM.Models.database.Order> CreateOrder(FISTNESSGYM.Models.database.Order order)
        {
            OnOrderCreated(order);

            var existingItem = Context.Orders
                              .Where(i => i.Id == order.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Orders.Add(order);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(order).State = EntityState.Detached;
                throw;
            }

            OnAfterOrderCreated(order);

            return order;
        }

        public async Task<FISTNESSGYM.Models.database.Order> CancelOrderChanges(FISTNESSGYM.Models.database.Order item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnOrderUpdated(FISTNESSGYM.Models.database.Order item);
        partial void OnAfterOrderUpdated(FISTNESSGYM.Models.database.Order item);

        public async Task<FISTNESSGYM.Models.database.Order> UpdateOrder(int id, FISTNESSGYM.Models.database.Order order)
        {
            OnOrderUpdated(order);

            var itemToUpdate = Context.Orders
                              .Where(i => i.Id == order.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(order);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterOrderUpdated(order);

            return order;
        }

        partial void OnOrderDeleted(FISTNESSGYM.Models.database.Order item);
        partial void OnAfterOrderDeleted(FISTNESSGYM.Models.database.Order item);

        public async Task<FISTNESSGYM.Models.database.Order> DeleteOrder(int id)
        {
            var itemToDelete = Context.Orders
                              .Where(i => i.Id == id)
                              .Include(i => i.OrderItems)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnOrderDeleted(itemToDelete);


            Context.Orders.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterOrderDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportOrderItemsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/orderitems/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/orderitems/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportOrderItemsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/orderitems/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/orderitems/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnOrderItemsRead(ref IQueryable<FISTNESSGYM.Models.database.OrderItem> items);

        public async Task<IQueryable<FISTNESSGYM.Models.database.OrderItem>> GetOrderItems(Query query = null)
        {
            var items = Context.OrderItems.AsQueryable();

            items = items.Include(i => i.Order);
            items = items.Include(i => i.Product);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnOrderItemsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnOrderItemGet(FISTNESSGYM.Models.database.OrderItem item);
        partial void OnGetOrderItemById(ref IQueryable<FISTNESSGYM.Models.database.OrderItem> items);


        public async Task<FISTNESSGYM.Models.database.OrderItem> GetOrderItemById(int id)
        {
            var items = Context.OrderItems
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            items = items.Include(i => i.Order);
            items = items.Include(i => i.Product);
 
            OnGetOrderItemById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnOrderItemGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnOrderItemCreated(FISTNESSGYM.Models.database.OrderItem item);
        partial void OnAfterOrderItemCreated(FISTNESSGYM.Models.database.OrderItem item);

        public async Task<FISTNESSGYM.Models.database.OrderItem> CreateOrderItem(FISTNESSGYM.Models.database.OrderItem orderitem)
        {
            OnOrderItemCreated(orderitem);

            var existingItem = Context.OrderItems
                              .Where(i => i.Id == orderitem.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.OrderItems.Add(orderitem);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(orderitem).State = EntityState.Detached;
                throw;
            }

            OnAfterOrderItemCreated(orderitem);

            return orderitem;
        }

        public async Task<FISTNESSGYM.Models.database.OrderItem> CancelOrderItemChanges(FISTNESSGYM.Models.database.OrderItem item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnOrderItemUpdated(FISTNESSGYM.Models.database.OrderItem item);
        partial void OnAfterOrderItemUpdated(FISTNESSGYM.Models.database.OrderItem item);

        public async Task<FISTNESSGYM.Models.database.OrderItem> UpdateOrderItem(int id, FISTNESSGYM.Models.database.OrderItem orderitem)
        {
            OnOrderItemUpdated(orderitem);

            var itemToUpdate = Context.OrderItems
                              .Where(i => i.Id == orderitem.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(orderitem);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterOrderItemUpdated(orderitem);

            return orderitem;
        }

        partial void OnOrderItemDeleted(FISTNESSGYM.Models.database.OrderItem item);
        partial void OnAfterOrderItemDeleted(FISTNESSGYM.Models.database.OrderItem item);

        public async Task<FISTNESSGYM.Models.database.OrderItem> DeleteOrderItem(int id)
        {
            var itemToDelete = Context.OrderItems
                              .Where(i => i.Id == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnOrderItemDeleted(itemToDelete);


            Context.OrderItems.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterOrderItemDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportOrderStatusesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/orderstatuses/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/orderstatuses/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportOrderStatusesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/orderstatuses/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/orderstatuses/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnOrderStatusesRead(ref IQueryable<FISTNESSGYM.Models.database.OrderStatus> items);

        public async Task<IQueryable<FISTNESSGYM.Models.database.OrderStatus>> GetOrderStatuses(Query query = null)
        {
            var items = Context.OrderStatuses.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnOrderStatusesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnOrderStatusGet(FISTNESSGYM.Models.database.OrderStatus item);
        partial void OnGetOrderStatusById(ref IQueryable<FISTNESSGYM.Models.database.OrderStatus> items);


        public async Task<FISTNESSGYM.Models.database.OrderStatus> GetOrderStatusById(int id)
        {
            var items = Context.OrderStatuses
                              .AsNoTracking()
                              .Where(i => i.Id == id);

 
            OnGetOrderStatusById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnOrderStatusGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnOrderStatusCreated(FISTNESSGYM.Models.database.OrderStatus item);
        partial void OnAfterOrderStatusCreated(FISTNESSGYM.Models.database.OrderStatus item);

        public async Task<FISTNESSGYM.Models.database.OrderStatus> CreateOrderStatus(FISTNESSGYM.Models.database.OrderStatus orderstatus)
        {
            OnOrderStatusCreated(orderstatus);

            var existingItem = Context.OrderStatuses
                              .Where(i => i.Id == orderstatus.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.OrderStatuses.Add(orderstatus);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(orderstatus).State = EntityState.Detached;
                throw;
            }

            OnAfterOrderStatusCreated(orderstatus);

            return orderstatus;
        }

        public async Task<FISTNESSGYM.Models.database.OrderStatus> CancelOrderStatusChanges(FISTNESSGYM.Models.database.OrderStatus item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnOrderStatusUpdated(FISTNESSGYM.Models.database.OrderStatus item);
        partial void OnAfterOrderStatusUpdated(FISTNESSGYM.Models.database.OrderStatus item);

        public async Task<FISTNESSGYM.Models.database.OrderStatus> UpdateOrderStatus(int id, FISTNESSGYM.Models.database.OrderStatus orderstatus)
        {
            OnOrderStatusUpdated(orderstatus);

            var itemToUpdate = Context.OrderStatuses
                              .Where(i => i.Id == orderstatus.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(orderstatus);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterOrderStatusUpdated(orderstatus);

            return orderstatus;
        }

        partial void OnOrderStatusDeleted(FISTNESSGYM.Models.database.OrderStatus item);
        partial void OnAfterOrderStatusDeleted(FISTNESSGYM.Models.database.OrderStatus item);

        public async Task<FISTNESSGYM.Models.database.OrderStatus> DeleteOrderStatus(int id)
        {
            var itemToDelete = Context.OrderStatuses
                              .Where(i => i.Id == id)
                              .Include(i => i.Orders)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnOrderStatusDeleted(itemToDelete);


            Context.OrderStatuses.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterOrderStatusDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportProductsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/products/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/products/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportProductsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/products/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/products/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnProductsRead(ref IQueryable<FISTNESSGYM.Models.database.Product> items);

        public async Task<IQueryable<FISTNESSGYM.Models.database.Product>> GetProducts(Query query = null)
        {
            var items = Context.Products.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnProductsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnProductGet(FISTNESSGYM.Models.database.Product item);
        partial void OnGetProductById(ref IQueryable<FISTNESSGYM.Models.database.Product> items);


        public async Task<FISTNESSGYM.Models.database.Product> GetProductById(int id)
        {
            var items = Context.Products
                              .AsNoTracking()
                              .Where(i => i.Id == id);

 
            OnGetProductById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnProductGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnProductCreated(FISTNESSGYM.Models.database.Product item);
        partial void OnAfterProductCreated(FISTNESSGYM.Models.database.Product item);

        public async Task<FISTNESSGYM.Models.database.Product> CreateProduct(FISTNESSGYM.Models.database.Product product)
        {
            OnProductCreated(product);

            var existingItem = Context.Products
                              .Where(i => i.Id == product.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Products.Add(product);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(product).State = EntityState.Detached;
                throw;
            }

            OnAfterProductCreated(product);

            return product;
        }

        public async Task<FISTNESSGYM.Models.database.Product> CancelProductChanges(FISTNESSGYM.Models.database.Product item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnProductUpdated(FISTNESSGYM.Models.database.Product item);
        partial void OnAfterProductUpdated(FISTNESSGYM.Models.database.Product item);

        public async Task<FISTNESSGYM.Models.database.Product> UpdateProduct(int id, FISTNESSGYM.Models.database.Product product)
        {
            OnProductUpdated(product);

            var itemToUpdate = Context.Products
                              .Where(i => i.Id == product.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(product);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterProductUpdated(product);

            return product;
        }

        partial void OnProductDeleted(FISTNESSGYM.Models.database.Product item);
        partial void OnAfterProductDeleted(FISTNESSGYM.Models.database.Product item);

        public async Task<FISTNESSGYM.Models.database.Product> DeleteProduct(int id)
        {
            var itemToDelete = Context.Products
                              .Where(i => i.Id == id)
                              .Include(i => i.OrderItems)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnProductDeleted(itemToDelete);


            Context.Products.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterProductDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportReservationsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/reservations/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/reservations/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportReservationsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/reservations/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/reservations/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnReservationsRead(ref IQueryable<FISTNESSGYM.Models.database.Reservation> items);

        public async Task<IQueryable<FISTNESSGYM.Models.database.Reservation>> GetReservations(Query query = null)
        {
            var items = Context.Reservations.AsQueryable();

            items = items.Include(i => i.Event);
            items = items.Include(i => i.AspNetUser);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnReservationsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnReservationGet(FISTNESSGYM.Models.database.Reservation item);
        partial void OnGetReservationById(ref IQueryable<FISTNESSGYM.Models.database.Reservation> items);


        public async Task<FISTNESSGYM.Models.database.Reservation> GetReservationById(int id)
        {
            var items = Context.Reservations
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            items = items.Include(i => i.Event);
            items = items.Include(i => i.AspNetUser);
 
            OnGetReservationById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnReservationGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnReservationCreated(FISTNESSGYM.Models.database.Reservation item);
        partial void OnAfterReservationCreated(FISTNESSGYM.Models.database.Reservation item);

        public async Task<FISTNESSGYM.Models.database.Reservation> CreateReservation(FISTNESSGYM.Models.database.Reservation reservation)
        {
            OnReservationCreated(reservation);

            var existingItem = Context.Reservations
                              .Where(i => i.Id == reservation.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Reservations.Add(reservation);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(reservation).State = EntityState.Detached;
                throw;
            }

            OnAfterReservationCreated(reservation);

            return reservation;
        }

        public async Task<FISTNESSGYM.Models.database.Reservation> CancelReservationChanges(FISTNESSGYM.Models.database.Reservation item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnReservationUpdated(FISTNESSGYM.Models.database.Reservation item);
        partial void OnAfterReservationUpdated(FISTNESSGYM.Models.database.Reservation item);

        public async Task<FISTNESSGYM.Models.database.Reservation> UpdateReservation(int id, FISTNESSGYM.Models.database.Reservation reservation)
        {
            OnReservationUpdated(reservation);

            var itemToUpdate = Context.Reservations
                              .Where(i => i.Id == reservation.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(reservation);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterReservationUpdated(reservation);

            return reservation;
        }

        partial void OnReservationDeleted(FISTNESSGYM.Models.database.Reservation item);
        partial void OnAfterReservationDeleted(FISTNESSGYM.Models.database.Reservation item);

        public async Task<FISTNESSGYM.Models.database.Reservation> DeleteReservation(int id)
        {
            var itemToDelete = Context.Reservations
                              .Where(i => i.Id == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnReservationDeleted(itemToDelete);


            Context.Reservations.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterReservationDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportSubscriptionsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/subscriptions/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/subscriptions/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportSubscriptionsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/subscriptions/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/subscriptions/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnSubscriptionsRead(ref IQueryable<FISTNESSGYM.Models.database.Subscription> items);

        public async Task<IQueryable<FISTNESSGYM.Models.database.Subscription>> GetSubscriptions(Query query = null)
        {
            var items = Context.Subscriptions.AsQueryable();

            items = items.Include(i => i.SubscriptionStatus);
            items = items.Include(i => i.AspNetUser);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnSubscriptionsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnSubscriptionGet(FISTNESSGYM.Models.database.Subscription item);
        partial void OnGetSubscriptionById(ref IQueryable<FISTNESSGYM.Models.database.Subscription> items);


        public async Task<FISTNESSGYM.Models.database.Subscription> GetSubscriptionById(int id)
        {
            var items = Context.Subscriptions
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            items = items.Include(i => i.SubscriptionStatus);
            items = items.Include(i => i.AspNetUser);
 
            OnGetSubscriptionById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnSubscriptionGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnSubscriptionCreated(FISTNESSGYM.Models.database.Subscription item);
        partial void OnAfterSubscriptionCreated(FISTNESSGYM.Models.database.Subscription item);

        public async Task<FISTNESSGYM.Models.database.Subscription> CreateSubscription(FISTNESSGYM.Models.database.Subscription subscription)
        {
            OnSubscriptionCreated(subscription);

            var existingItem = Context.Subscriptions
                              .Where(i => i.Id == subscription.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Subscriptions.Add(subscription);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(subscription).State = EntityState.Detached;
                throw;
            }

            OnAfterSubscriptionCreated(subscription);

            return subscription;
        }

        public async Task<FISTNESSGYM.Models.database.Subscription> CancelSubscriptionChanges(FISTNESSGYM.Models.database.Subscription item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnSubscriptionUpdated(FISTNESSGYM.Models.database.Subscription item);
        partial void OnAfterSubscriptionUpdated(FISTNESSGYM.Models.database.Subscription item);

        public async Task<FISTNESSGYM.Models.database.Subscription> UpdateSubscription(int id, FISTNESSGYM.Models.database.Subscription subscription)
        {
            OnSubscriptionUpdated(subscription);

            var itemToUpdate = Context.Subscriptions
                              .Where(i => i.Id == subscription.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(subscription);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterSubscriptionUpdated(subscription);

            return subscription;
        }

        partial void OnSubscriptionDeleted(FISTNESSGYM.Models.database.Subscription item);
        partial void OnAfterSubscriptionDeleted(FISTNESSGYM.Models.database.Subscription item);

        public async Task<FISTNESSGYM.Models.database.Subscription> DeleteSubscription(int id)
        {
            var itemToDelete = Context.Subscriptions
                              .Where(i => i.Id == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnSubscriptionDeleted(itemToDelete);


            Context.Subscriptions.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterSubscriptionDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportSubscriptionStatusesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/subscriptionstatuses/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/subscriptionstatuses/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportSubscriptionStatusesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/subscriptionstatuses/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/subscriptionstatuses/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnSubscriptionStatusesRead(ref IQueryable<FISTNESSGYM.Models.database.SubscriptionStatus> items);

        public async Task<IQueryable<FISTNESSGYM.Models.database.SubscriptionStatus>> GetSubscriptionStatuses(Query query = null)
        {
            var items = Context.SubscriptionStatuses.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnSubscriptionStatusesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnSubscriptionStatusGet(FISTNESSGYM.Models.database.SubscriptionStatus item);
        partial void OnGetSubscriptionStatusById(ref IQueryable<FISTNESSGYM.Models.database.SubscriptionStatus> items);


        public async Task<FISTNESSGYM.Models.database.SubscriptionStatus> GetSubscriptionStatusById(int id)
        {
            var items = Context.SubscriptionStatuses
                              .AsNoTracking()
                              .Where(i => i.Id == id);

 
            OnGetSubscriptionStatusById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnSubscriptionStatusGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnSubscriptionStatusCreated(FISTNESSGYM.Models.database.SubscriptionStatus item);
        partial void OnAfterSubscriptionStatusCreated(FISTNESSGYM.Models.database.SubscriptionStatus item);

        public async Task<FISTNESSGYM.Models.database.SubscriptionStatus> CreateSubscriptionStatus(FISTNESSGYM.Models.database.SubscriptionStatus subscriptionstatus)
        {
            OnSubscriptionStatusCreated(subscriptionstatus);

            var existingItem = Context.SubscriptionStatuses
                              .Where(i => i.Id == subscriptionstatus.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.SubscriptionStatuses.Add(subscriptionstatus);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(subscriptionstatus).State = EntityState.Detached;
                throw;
            }

            OnAfterSubscriptionStatusCreated(subscriptionstatus);

            return subscriptionstatus;
        }

        public async Task<FISTNESSGYM.Models.database.SubscriptionStatus> CancelSubscriptionStatusChanges(FISTNESSGYM.Models.database.SubscriptionStatus item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnSubscriptionStatusUpdated(FISTNESSGYM.Models.database.SubscriptionStatus item);
        partial void OnAfterSubscriptionStatusUpdated(FISTNESSGYM.Models.database.SubscriptionStatus item);

        public async Task<FISTNESSGYM.Models.database.SubscriptionStatus> UpdateSubscriptionStatus(int id, FISTNESSGYM.Models.database.SubscriptionStatus subscriptionstatus)
        {
            OnSubscriptionStatusUpdated(subscriptionstatus);

            var itemToUpdate = Context.SubscriptionStatuses
                              .Where(i => i.Id == subscriptionstatus.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(subscriptionstatus);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterSubscriptionStatusUpdated(subscriptionstatus);

            return subscriptionstatus;
        }

        partial void OnSubscriptionStatusDeleted(FISTNESSGYM.Models.database.SubscriptionStatus item);
        partial void OnAfterSubscriptionStatusDeleted(FISTNESSGYM.Models.database.SubscriptionStatus item);

        public async Task<FISTNESSGYM.Models.database.SubscriptionStatus> DeleteSubscriptionStatus(int id)
        {
            var itemToDelete = Context.SubscriptionStatuses
                              .Where(i => i.Id == id)
                              .Include(i => i.Subscriptions)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnSubscriptionStatusDeleted(itemToDelete);


            Context.SubscriptionStatuses.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterSubscriptionStatusDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportAspNetRoleClaimsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/aspnetroleclaims/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/aspnetroleclaims/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAspNetRoleClaimsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/aspnetroleclaims/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/aspnetroleclaims/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAspNetRoleClaimsRead(ref IQueryable<FISTNESSGYM.Models.database.AspNetRoleClaim> items);

        public async Task<IQueryable<FISTNESSGYM.Models.database.AspNetRoleClaim>> GetAspNetRoleClaims(Query query = null)
        {
            var items = Context.AspNetRoleClaims.AsQueryable();

            items = items.Include(i => i.AspNetRole);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnAspNetRoleClaimsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAspNetRoleClaimGet(FISTNESSGYM.Models.database.AspNetRoleClaim item);
        partial void OnGetAspNetRoleClaimById(ref IQueryable<FISTNESSGYM.Models.database.AspNetRoleClaim> items);


        public async Task<FISTNESSGYM.Models.database.AspNetRoleClaim> GetAspNetRoleClaimById(int id)
        {
            var items = Context.AspNetRoleClaims
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            items = items.Include(i => i.AspNetRole);
 
            OnGetAspNetRoleClaimById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnAspNetRoleClaimGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAspNetRoleClaimCreated(FISTNESSGYM.Models.database.AspNetRoleClaim item);
        partial void OnAfterAspNetRoleClaimCreated(FISTNESSGYM.Models.database.AspNetRoleClaim item);

        public async Task<FISTNESSGYM.Models.database.AspNetRoleClaim> CreateAspNetRoleClaim(FISTNESSGYM.Models.database.AspNetRoleClaim aspnetroleclaim)
        {
            OnAspNetRoleClaimCreated(aspnetroleclaim);

            var existingItem = Context.AspNetRoleClaims
                              .Where(i => i.Id == aspnetroleclaim.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.AspNetRoleClaims.Add(aspnetroleclaim);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(aspnetroleclaim).State = EntityState.Detached;
                throw;
            }

            OnAfterAspNetRoleClaimCreated(aspnetroleclaim);

            return aspnetroleclaim;
        }

        public async Task<FISTNESSGYM.Models.database.AspNetRoleClaim> CancelAspNetRoleClaimChanges(FISTNESSGYM.Models.database.AspNetRoleClaim item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAspNetRoleClaimUpdated(FISTNESSGYM.Models.database.AspNetRoleClaim item);
        partial void OnAfterAspNetRoleClaimUpdated(FISTNESSGYM.Models.database.AspNetRoleClaim item);

        public async Task<FISTNESSGYM.Models.database.AspNetRoleClaim> UpdateAspNetRoleClaim(int id, FISTNESSGYM.Models.database.AspNetRoleClaim aspnetroleclaim)
        {
            OnAspNetRoleClaimUpdated(aspnetroleclaim);

            var itemToUpdate = Context.AspNetRoleClaims
                              .Where(i => i.Id == aspnetroleclaim.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(aspnetroleclaim);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAspNetRoleClaimUpdated(aspnetroleclaim);

            return aspnetroleclaim;
        }

        partial void OnAspNetRoleClaimDeleted(FISTNESSGYM.Models.database.AspNetRoleClaim item);
        partial void OnAfterAspNetRoleClaimDeleted(FISTNESSGYM.Models.database.AspNetRoleClaim item);

        public async Task<FISTNESSGYM.Models.database.AspNetRoleClaim> DeleteAspNetRoleClaim(int id)
        {
            var itemToDelete = Context.AspNetRoleClaims
                              .Where(i => i.Id == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnAspNetRoleClaimDeleted(itemToDelete);


            Context.AspNetRoleClaims.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAspNetRoleClaimDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportAspNetRolesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/aspnetroles/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/aspnetroles/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAspNetRolesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/aspnetroles/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/aspnetroles/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAspNetRolesRead(ref IQueryable<FISTNESSGYM.Models.database.AspNetRole> items);

        public async Task<IQueryable<FISTNESSGYM.Models.database.AspNetRole>> GetAspNetRoles(Query query = null)
        {
            var items = Context.AspNetRoles.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnAspNetRolesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAspNetRoleGet(FISTNESSGYM.Models.database.AspNetRole item);
        partial void OnGetAspNetRoleById(ref IQueryable<FISTNESSGYM.Models.database.AspNetRole> items);


        public async Task<FISTNESSGYM.Models.database.AspNetRole> GetAspNetRoleById(string id)
        {
            var items = Context.AspNetRoles
                              .AsNoTracking()
                              .Where(i => i.Id == id);

 
            OnGetAspNetRoleById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnAspNetRoleGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAspNetRoleCreated(FISTNESSGYM.Models.database.AspNetRole item);
        partial void OnAfterAspNetRoleCreated(FISTNESSGYM.Models.database.AspNetRole item);

        public async Task<FISTNESSGYM.Models.database.AspNetRole> CreateAspNetRole(FISTNESSGYM.Models.database.AspNetRole aspnetrole)
        {
            OnAspNetRoleCreated(aspnetrole);

            var existingItem = Context.AspNetRoles
                              .Where(i => i.Id == aspnetrole.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.AspNetRoles.Add(aspnetrole);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(aspnetrole).State = EntityState.Detached;
                throw;
            }

            OnAfterAspNetRoleCreated(aspnetrole);

            return aspnetrole;
        }

        public async Task<FISTNESSGYM.Models.database.AspNetRole> CancelAspNetRoleChanges(FISTNESSGYM.Models.database.AspNetRole item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAspNetRoleUpdated(FISTNESSGYM.Models.database.AspNetRole item);
        partial void OnAfterAspNetRoleUpdated(FISTNESSGYM.Models.database.AspNetRole item);

        public async Task<FISTNESSGYM.Models.database.AspNetRole> UpdateAspNetRole(string id, FISTNESSGYM.Models.database.AspNetRole aspnetrole)
        {
            OnAspNetRoleUpdated(aspnetrole);

            var itemToUpdate = Context.AspNetRoles
                              .Where(i => i.Id == aspnetrole.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(aspnetrole);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAspNetRoleUpdated(aspnetrole);

            return aspnetrole;
        }

        partial void OnAspNetRoleDeleted(FISTNESSGYM.Models.database.AspNetRole item);
        partial void OnAfterAspNetRoleDeleted(FISTNESSGYM.Models.database.AspNetRole item);

        public async Task<FISTNESSGYM.Models.database.AspNetRole> DeleteAspNetRole(string id)
        {
            var itemToDelete = Context.AspNetRoles
                              .Where(i => i.Id == id)
                              .Include(i => i.AspNetRoleClaims)
                              .Include(i => i.AspNetUserRoles)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnAspNetRoleDeleted(itemToDelete);


            Context.AspNetRoles.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAspNetRoleDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportAspNetUserClaimsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/aspnetuserclaims/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/aspnetuserclaims/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAspNetUserClaimsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/aspnetuserclaims/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/aspnetuserclaims/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAspNetUserClaimsRead(ref IQueryable<FISTNESSGYM.Models.database.AspNetUserClaim> items);

        public async Task<IQueryable<FISTNESSGYM.Models.database.AspNetUserClaim>> GetAspNetUserClaims(Query query = null)
        {
            var items = Context.AspNetUserClaims.AsQueryable();

            items = items.Include(i => i.AspNetUser);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnAspNetUserClaimsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAspNetUserClaimGet(FISTNESSGYM.Models.database.AspNetUserClaim item);
        partial void OnGetAspNetUserClaimById(ref IQueryable<FISTNESSGYM.Models.database.AspNetUserClaim> items);


        public async Task<FISTNESSGYM.Models.database.AspNetUserClaim> GetAspNetUserClaimById(int id)
        {
            var items = Context.AspNetUserClaims
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            items = items.Include(i => i.AspNetUser);
 
            OnGetAspNetUserClaimById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnAspNetUserClaimGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAspNetUserClaimCreated(FISTNESSGYM.Models.database.AspNetUserClaim item);
        partial void OnAfterAspNetUserClaimCreated(FISTNESSGYM.Models.database.AspNetUserClaim item);

        public async Task<FISTNESSGYM.Models.database.AspNetUserClaim> CreateAspNetUserClaim(FISTNESSGYM.Models.database.AspNetUserClaim aspnetuserclaim)
        {
            OnAspNetUserClaimCreated(aspnetuserclaim);

            var existingItem = Context.AspNetUserClaims
                              .Where(i => i.Id == aspnetuserclaim.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.AspNetUserClaims.Add(aspnetuserclaim);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(aspnetuserclaim).State = EntityState.Detached;
                throw;
            }

            OnAfterAspNetUserClaimCreated(aspnetuserclaim);

            return aspnetuserclaim;
        }

        public async Task<FISTNESSGYM.Models.database.AspNetUserClaim> CancelAspNetUserClaimChanges(FISTNESSGYM.Models.database.AspNetUserClaim item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAspNetUserClaimUpdated(FISTNESSGYM.Models.database.AspNetUserClaim item);
        partial void OnAfterAspNetUserClaimUpdated(FISTNESSGYM.Models.database.AspNetUserClaim item);

        public async Task<FISTNESSGYM.Models.database.AspNetUserClaim> UpdateAspNetUserClaim(int id, FISTNESSGYM.Models.database.AspNetUserClaim aspnetuserclaim)
        {
            OnAspNetUserClaimUpdated(aspnetuserclaim);

            var itemToUpdate = Context.AspNetUserClaims
                              .Where(i => i.Id == aspnetuserclaim.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(aspnetuserclaim);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAspNetUserClaimUpdated(aspnetuserclaim);

            return aspnetuserclaim;
        }

        partial void OnAspNetUserClaimDeleted(FISTNESSGYM.Models.database.AspNetUserClaim item);
        partial void OnAfterAspNetUserClaimDeleted(FISTNESSGYM.Models.database.AspNetUserClaim item);

        public async Task<FISTNESSGYM.Models.database.AspNetUserClaim> DeleteAspNetUserClaim(int id)
        {
            var itemToDelete = Context.AspNetUserClaims
                              .Where(i => i.Id == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnAspNetUserClaimDeleted(itemToDelete);


            Context.AspNetUserClaims.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAspNetUserClaimDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportAspNetUserLoginsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/aspnetuserlogins/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/aspnetuserlogins/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAspNetUserLoginsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/aspnetuserlogins/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/aspnetuserlogins/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAspNetUserLoginsRead(ref IQueryable<FISTNESSGYM.Models.database.AspNetUserLogin> items);

        public async Task<IQueryable<FISTNESSGYM.Models.database.AspNetUserLogin>> GetAspNetUserLogins(Query query = null)
        {
            var items = Context.AspNetUserLogins.AsQueryable();

            items = items.Include(i => i.AspNetUser);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnAspNetUserLoginsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAspNetUserLoginGet(FISTNESSGYM.Models.database.AspNetUserLogin item);
        partial void OnGetAspNetUserLoginByLoginProviderAndProviderKey(ref IQueryable<FISTNESSGYM.Models.database.AspNetUserLogin> items);


        public async Task<FISTNESSGYM.Models.database.AspNetUserLogin> GetAspNetUserLoginByLoginProviderAndProviderKey(string loginprovider, string providerkey)
        {
            var items = Context.AspNetUserLogins
                              .AsNoTracking()
                              .Where(i => i.LoginProvider == loginprovider && i.ProviderKey == providerkey);

            items = items.Include(i => i.AspNetUser);
 
            OnGetAspNetUserLoginByLoginProviderAndProviderKey(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnAspNetUserLoginGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAspNetUserLoginCreated(FISTNESSGYM.Models.database.AspNetUserLogin item);
        partial void OnAfterAspNetUserLoginCreated(FISTNESSGYM.Models.database.AspNetUserLogin item);

        public async Task<FISTNESSGYM.Models.database.AspNetUserLogin> CreateAspNetUserLogin(FISTNESSGYM.Models.database.AspNetUserLogin aspnetuserlogin)
        {
            OnAspNetUserLoginCreated(aspnetuserlogin);

            var existingItem = Context.AspNetUserLogins
                              .Where(i => i.LoginProvider == aspnetuserlogin.LoginProvider && i.ProviderKey == aspnetuserlogin.ProviderKey)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.AspNetUserLogins.Add(aspnetuserlogin);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(aspnetuserlogin).State = EntityState.Detached;
                throw;
            }

            OnAfterAspNetUserLoginCreated(aspnetuserlogin);

            return aspnetuserlogin;
        }

        public async Task<FISTNESSGYM.Models.database.AspNetUserLogin> CancelAspNetUserLoginChanges(FISTNESSGYM.Models.database.AspNetUserLogin item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAspNetUserLoginUpdated(FISTNESSGYM.Models.database.AspNetUserLogin item);
        partial void OnAfterAspNetUserLoginUpdated(FISTNESSGYM.Models.database.AspNetUserLogin item);

        public async Task<FISTNESSGYM.Models.database.AspNetUserLogin> UpdateAspNetUserLogin(string loginprovider, string providerkey, FISTNESSGYM.Models.database.AspNetUserLogin aspnetuserlogin)
        {
            OnAspNetUserLoginUpdated(aspnetuserlogin);

            var itemToUpdate = Context.AspNetUserLogins
                              .Where(i => i.LoginProvider == aspnetuserlogin.LoginProvider && i.ProviderKey == aspnetuserlogin.ProviderKey)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(aspnetuserlogin);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAspNetUserLoginUpdated(aspnetuserlogin);

            return aspnetuserlogin;
        }

        partial void OnAspNetUserLoginDeleted(FISTNESSGYM.Models.database.AspNetUserLogin item);
        partial void OnAfterAspNetUserLoginDeleted(FISTNESSGYM.Models.database.AspNetUserLogin item);

        public async Task<FISTNESSGYM.Models.database.AspNetUserLogin> DeleteAspNetUserLogin(string loginprovider, string providerkey)
        {
            var itemToDelete = Context.AspNetUserLogins
                              .Where(i => i.LoginProvider == loginprovider && i.ProviderKey == providerkey)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnAspNetUserLoginDeleted(itemToDelete);


            Context.AspNetUserLogins.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAspNetUserLoginDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportAspNetUserRolesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/aspnetuserroles/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/aspnetuserroles/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAspNetUserRolesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/aspnetuserroles/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/aspnetuserroles/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAspNetUserRolesRead(ref IQueryable<FISTNESSGYM.Models.database.AspNetUserRole> items);

        public async Task<IQueryable<FISTNESSGYM.Models.database.AspNetUserRole>> GetAspNetUserRoles(Query query = null)
        {
            var items = Context.AspNetUserRoles.AsQueryable();

            items = items.Include(i => i.AspNetRole);
            items = items.Include(i => i.AspNetUser);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnAspNetUserRolesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAspNetUserRoleGet(FISTNESSGYM.Models.database.AspNetUserRole item);
        partial void OnGetAspNetUserRoleByUserIdAndRoleId(ref IQueryable<FISTNESSGYM.Models.database.AspNetUserRole> items);


        public async Task<FISTNESSGYM.Models.database.AspNetUserRole> GetAspNetUserRoleByUserIdAndRoleId(string userid, string roleid)
        {
            var items = Context.AspNetUserRoles
                              .AsNoTracking()
                              .Where(i => i.UserId == userid && i.RoleId == roleid);

            items = items.Include(i => i.AspNetRole);
            items = items.Include(i => i.AspNetUser);
 
            OnGetAspNetUserRoleByUserIdAndRoleId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnAspNetUserRoleGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAspNetUserRoleCreated(FISTNESSGYM.Models.database.AspNetUserRole item);
        partial void OnAfterAspNetUserRoleCreated(FISTNESSGYM.Models.database.AspNetUserRole item);

        public async Task<FISTNESSGYM.Models.database.AspNetUserRole> CreateAspNetUserRole(FISTNESSGYM.Models.database.AspNetUserRole aspnetuserrole)
        {
            OnAspNetUserRoleCreated(aspnetuserrole);

            var existingItem = Context.AspNetUserRoles
                              .Where(i => i.UserId == aspnetuserrole.UserId && i.RoleId == aspnetuserrole.RoleId)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.AspNetUserRoles.Add(aspnetuserrole);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(aspnetuserrole).State = EntityState.Detached;
                throw;
            }

            OnAfterAspNetUserRoleCreated(aspnetuserrole);

            return aspnetuserrole;
        }

        public async Task<FISTNESSGYM.Models.database.AspNetUserRole> CancelAspNetUserRoleChanges(FISTNESSGYM.Models.database.AspNetUserRole item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAspNetUserRoleUpdated(FISTNESSGYM.Models.database.AspNetUserRole item);
        partial void OnAfterAspNetUserRoleUpdated(FISTNESSGYM.Models.database.AspNetUserRole item);

        public async Task<FISTNESSGYM.Models.database.AspNetUserRole> UpdateAspNetUserRole(string userid, string roleid, FISTNESSGYM.Models.database.AspNetUserRole aspnetuserrole)
        {
            OnAspNetUserRoleUpdated(aspnetuserrole);

            var itemToUpdate = Context.AspNetUserRoles
                              .Where(i => i.UserId == aspnetuserrole.UserId && i.RoleId == aspnetuserrole.RoleId)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(aspnetuserrole);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAspNetUserRoleUpdated(aspnetuserrole);

            return aspnetuserrole;
        }

        partial void OnAspNetUserRoleDeleted(FISTNESSGYM.Models.database.AspNetUserRole item);
        partial void OnAfterAspNetUserRoleDeleted(FISTNESSGYM.Models.database.AspNetUserRole item);

        public async Task<FISTNESSGYM.Models.database.AspNetUserRole> DeleteAspNetUserRole(string userid, string roleid)
        {
            var itemToDelete = Context.AspNetUserRoles
                              .Where(i => i.UserId == userid && i.RoleId == roleid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnAspNetUserRoleDeleted(itemToDelete);


            Context.AspNetUserRoles.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAspNetUserRoleDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportAspNetUsersToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/aspnetusers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/aspnetusers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAspNetUsersToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/aspnetusers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/aspnetusers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAspNetUsersRead(ref IQueryable<FISTNESSGYM.Models.database.AspNetUser> items);

        public async Task<IQueryable<FISTNESSGYM.Models.database.AspNetUser>> GetAspNetUsers(Query query = null)
        {
            var items = Context.AspNetUsers.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnAspNetUsersRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAspNetUserGet(FISTNESSGYM.Models.database.AspNetUser item);
        partial void OnGetAspNetUserById(ref IQueryable<FISTNESSGYM.Models.database.AspNetUser> items);


        public async Task<FISTNESSGYM.Models.database.AspNetUser> GetAspNetUserById(string id)
        {
            var items = Context.AspNetUsers
                              .AsNoTracking()
                              .Where(i => i.Id == id);

 
            OnGetAspNetUserById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnAspNetUserGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAspNetUserCreated(FISTNESSGYM.Models.database.AspNetUser item);
        partial void OnAfterAspNetUserCreated(FISTNESSGYM.Models.database.AspNetUser item);

        public async Task<FISTNESSGYM.Models.database.AspNetUser> CreateAspNetUser(FISTNESSGYM.Models.database.AspNetUser aspnetuser)
        {
            OnAspNetUserCreated(aspnetuser);

            var existingItem = Context.AspNetUsers
                              .Where(i => i.Id == aspnetuser.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.AspNetUsers.Add(aspnetuser);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(aspnetuser).State = EntityState.Detached;
                throw;
            }

            OnAfterAspNetUserCreated(aspnetuser);

            return aspnetuser;
        }

        public async Task<FISTNESSGYM.Models.database.AspNetUser> CancelAspNetUserChanges(FISTNESSGYM.Models.database.AspNetUser item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAspNetUserUpdated(FISTNESSGYM.Models.database.AspNetUser item);
        partial void OnAfterAspNetUserUpdated(FISTNESSGYM.Models.database.AspNetUser item);

        public async Task<FISTNESSGYM.Models.database.AspNetUser> UpdateAspNetUser(string id, FISTNESSGYM.Models.database.AspNetUser aspnetuser)
        {
            OnAspNetUserUpdated(aspnetuser);

            var itemToUpdate = Context.AspNetUsers
                              .Where(i => i.Id == aspnetuser.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(aspnetuser);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAspNetUserUpdated(aspnetuser);

            return aspnetuser;
        }

        partial void OnAspNetUserDeleted(FISTNESSGYM.Models.database.AspNetUser item);
        partial void OnAfterAspNetUserDeleted(FISTNESSGYM.Models.database.AspNetUser item);

        public async Task<FISTNESSGYM.Models.database.AspNetUser> DeleteAspNetUser(string id)
        {
            var itemToDelete = Context.AspNetUsers
                              .Where(i => i.Id == id)
                              .Include(i => i.AspNetUserClaims)
                              .Include(i => i.AspNetUserLogins)
                              .Include(i => i.AspNetUserRoles)
                              .Include(i => i.AspNetUserTokens)
                              .Include(i => i.Orders)
                              .Include(i => i.Reservations)
                              .Include(i => i.Subscriptions)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnAspNetUserDeleted(itemToDelete);


            Context.AspNetUsers.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAspNetUserDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportAspNetUserTokensToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/aspnetusertokens/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/aspnetusertokens/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAspNetUserTokensToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/aspnetusertokens/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/aspnetusertokens/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAspNetUserTokensRead(ref IQueryable<FISTNESSGYM.Models.database.AspNetUserToken> items);

        public async Task<IQueryable<FISTNESSGYM.Models.database.AspNetUserToken>> GetAspNetUserTokens(Query query = null)
        {
            var items = Context.AspNetUserTokens.AsQueryable();

            items = items.Include(i => i.AspNetUser);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnAspNetUserTokensRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAspNetUserTokenGet(FISTNESSGYM.Models.database.AspNetUserToken item);
        partial void OnGetAspNetUserTokenByUserIdAndLoginProviderAndName(ref IQueryable<FISTNESSGYM.Models.database.AspNetUserToken> items);


        public async Task<FISTNESSGYM.Models.database.AspNetUserToken> GetAspNetUserTokenByUserIdAndLoginProviderAndName(string userid, string loginprovider, string name)
        {
            var items = Context.AspNetUserTokens
                              .AsNoTracking()
                              .Where(i => i.UserId == userid && i.LoginProvider == loginprovider && i.Name == name);

            items = items.Include(i => i.AspNetUser);
 
            OnGetAspNetUserTokenByUserIdAndLoginProviderAndName(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnAspNetUserTokenGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAspNetUserTokenCreated(FISTNESSGYM.Models.database.AspNetUserToken item);
        partial void OnAfterAspNetUserTokenCreated(FISTNESSGYM.Models.database.AspNetUserToken item);

        public async Task<FISTNESSGYM.Models.database.AspNetUserToken> CreateAspNetUserToken(FISTNESSGYM.Models.database.AspNetUserToken aspnetusertoken)
        {
            OnAspNetUserTokenCreated(aspnetusertoken);

            var existingItem = Context.AspNetUserTokens
                              .Where(i => i.UserId == aspnetusertoken.UserId && i.LoginProvider == aspnetusertoken.LoginProvider && i.Name == aspnetusertoken.Name)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.AspNetUserTokens.Add(aspnetusertoken);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(aspnetusertoken).State = EntityState.Detached;
                throw;
            }

            OnAfterAspNetUserTokenCreated(aspnetusertoken);

            return aspnetusertoken;
        }

        public async Task<FISTNESSGYM.Models.database.AspNetUserToken> CancelAspNetUserTokenChanges(FISTNESSGYM.Models.database.AspNetUserToken item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAspNetUserTokenUpdated(FISTNESSGYM.Models.database.AspNetUserToken item);
        partial void OnAfterAspNetUserTokenUpdated(FISTNESSGYM.Models.database.AspNetUserToken item);

        public async Task<FISTNESSGYM.Models.database.AspNetUserToken> UpdateAspNetUserToken(string userid, string loginprovider, string name, FISTNESSGYM.Models.database.AspNetUserToken aspnetusertoken)
        {
            OnAspNetUserTokenUpdated(aspnetusertoken);

            var itemToUpdate = Context.AspNetUserTokens
                              .Where(i => i.UserId == aspnetusertoken.UserId && i.LoginProvider == aspnetusertoken.LoginProvider && i.Name == aspnetusertoken.Name)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(aspnetusertoken);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAspNetUserTokenUpdated(aspnetusertoken);

            return aspnetusertoken;
        }

        partial void OnAspNetUserTokenDeleted(FISTNESSGYM.Models.database.AspNetUserToken item);
        partial void OnAfterAspNetUserTokenDeleted(FISTNESSGYM.Models.database.AspNetUserToken item);

        public async Task<FISTNESSGYM.Models.database.AspNetUserToken> DeleteAspNetUserToken(string userid, string loginprovider, string name)
        {
            var itemToDelete = Context.AspNetUserTokens
                              .Where(i => i.UserId == userid && i.LoginProvider == loginprovider && i.Name == name)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnAspNetUserTokenDeleted(itemToDelete);


            Context.AspNetUserTokens.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAspNetUserTokenDeleted(itemToDelete);

            return itemToDelete;
        }
        }
}