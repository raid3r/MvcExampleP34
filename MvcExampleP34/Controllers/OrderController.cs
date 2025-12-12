using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcExampleP34.Models;
using MvcExampleP34.Models.Dto;
using MvcExampleP34.Models.Forms;
using MvcExampleP34.Models.LiqPay;
using MvcExampleP34.Models.PageModels;
using MvcExampleP34.Models.Services;
using System.Diagnostics;
using System.Security.Claims;

namespace MvcExampleP34.Controllers;

[Authorize]
public class OrderController(
    StoreContext context,
    CartService cartService,
    UserManager<User> userManager
    ) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Init()
    {
        var cart = await cartService.GetCartAsync();
        var user = await userManager.GetUserAsync(User);
        int orderNumber = 1;
        try
        {
            orderNumber = 1 + await context.Orders.MaxAsync(x => x.Number);
        }
        catch
        {
            // skip
        }


        var order = new Order
        {
            Number = orderNumber,
            UniqueId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Status = OrderStatusConstants.Draft,
            User = user!,
            Items = [.. cart.Items.Select(cartItem => new OrderItem
            {
                Product = cartItem.Product,
                Price = cartItem.Product.Price,
                Quantity = cartItem.Quantity
            })],
        };

        // Очистити кошик
        cart.Items.Clear();
        // Записати замовлення в базу даних - статус "Draft"
        context.Orders.Add(order);
        await context.SaveChangesAsync();

        // Відправити на сторінку оформлення замовлення (де буде вводитися додаткова інформація)
        return RedirectToAction("Create", new { guid = order.UniqueId });
    }

    [HttpGet]
    public async Task<IActionResult> Create(Guid guid)
    {
        var order = await GetOrderByGuidAsync(guid);

        if (order.Status != OrderStatusConstants.Draft)
        {
            return RedirectToAction("CreateSuccess", new { guid = order.UniqueId });
        }

        ViewData["Order"] = order;

        var checkout = LiqPayHelper.GetLiqPayModel(order.UniqueId.ToString(), order.Items.Sum(x => x.Price * x.Quantity));
        ViewData["LiqPayCheckout"] = checkout;

        // Показати сторінку оформлення замовлення
        return View(new CreateNewOrderForm());
    }

    private async Task<Order> GetOrderByGuidAsync(Guid guid)
    {
        var order = await context.Orders
            .Include(x => x.Items)
            .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x => x.UniqueId == guid);



        return order!;
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid guid, [FromForm] CreateNewOrderForm form)
    {
        var order = await GetOrderByGuidAsync(guid);

        

        if (!ModelState.IsValid)
        {
            ViewData["Order"] = order;
            return View(form);
        }
        // Обробити форму оформлення замовлення
        order.Comment = form.Comment;
        // Змінити статус замовлення на "New"
        order.Status = OrderStatusConstants.New;
        order.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();

        // Перенаправити на сторінку успішного оформлення замовлення
        return RedirectToAction("CreateSuccess", new { guid = guid });
    }

    public async Task<IActionResult> CreateSuccess(Guid guid)
    {
        // Показати сторінку успішного оформлення замовлення
        return View(await GetOrderByGuidAsync(guid));
    }

    public async Task<IActionResult> List()
    {
        var userId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value);

        var orders = await context.Orders
            .Where(x => x.User.Id == userId)
            .Include(x => x.Items)
            .ThenInclude(x => x.Product)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return View(orders);
    }

    public async Task<IActionResult> Details(Guid guid)
    {
        var userId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value);
        var order = await context.Orders
            .Where(x => x.User.Id == userId && x.UniqueId == guid)
            .Include(x => x.Items)
            .ThenInclude(x => x.Product)
            .ThenInclude(x => x.Images)
            .FirstOrDefaultAsync();
        if (order == null)
        {
            return NotFound();
        }
        return View(order);
    }

    [HttpPost]
    public async Task<IActionResult> LiqPayCallback()
    {
        // TODO handle LiqPay callback (succes payment notification)

        /*
         * {
    "transaction_id": 2763752316,
    "amount_bonus": 0,
    "status": "sandbox",
    "type": "buy",
    "user": {
        "country_code": null,
        "id": null,
        "nick": null,
        "phone": null
    },
    "sender_card_mask2": "424242*42",
    "ip": "145.224.94.149",
    "is_3ds": false,
    "currency": "UAH",
    "show_moment_part": false,
    "amount_credit": 210,
    "action": "pay",
    "create_date": 1765561484791,
    "need_cardholder_name": true,
    "payment_id": 2763752316,
    "language": "uk",
    "version": 3,
    "public_key": "sandbox_i75414272515",
    "currency_credit": "UAH",
    "commission_debit": 0,
    "sender_bonus": 0,
    "notify": {
        "data": "eyJwYXltZW50X2lkIjoyNzYzNzUyMzE2LCJhY3Rpb24iOiJwYXkiLCJzdGF0dXMiOiJzYW5kYm94IiwidmVyc2lvbiI6MywidHlwZSI6ImJ1eSIsInBheXR5cGUiOiJjYXJkIiwicHVibGljX2tleSI6InNhbmRib3hfaTc1NDE0MjcyNTE1IiwiYWNxX2lkIjo0MTQ5NjMsIm9yZGVyX2lkIjoiYjhmYjgwOTMtY2U4Zi00ZWZkLWIzYjUtZmQxMGFlNmU5ZDA3IiwibGlxcGF5X29yZGVyX2lkIjoiRVpDREpWQksxNzY1NTYxNDg0Nzg3NzA3IiwiZGVzY3JpcHRpb24iOiLQntC/0LvQsNGC0LAg0LfQsNC80L7QstC70LXQvdC90Y8gIyBiOGZiODA5My1jZThmLTRlZmQtYjNiNS1mZDEwYWU2ZTlkMDciLCJzZW5kZXJfZmlyc3RfbmFtZSI6InRlc3QiLCJzZW5kZXJfbGFzdF9uYW1lIjoidGVzdCIsInNlbmRlcl9jYXJkX21hc2syIjoiNDI0MjQyKjQyIiwic2VuZGVyX2NhcmRfYmFuayI6IlRlc3QiLCJzZW5kZXJfY2FyZF90eXBlIjoidmlzYSIsInNlbmRlcl9jYXJkX2NvdW50cnkiOjgwNCwiaXAiOiIxNDUuMjI0Ljk0LjE0OSIsImFtb3VudCI6MjEwLjAsImN1cnJlbmN5IjoiVUFIIiwic2VuZGVyX2NvbW1pc3Npb24iOjAuMCwicmVjZWl2ZXJfY29tbWlzc2lvbiI6My4xNSwiYWdlbnRfY29tbWlzc2lvbiI6MC4wLCJhbW91bnRfZGViaXQiOjIxMC4wLCJhbW91bnRfY3JlZGl0IjoyMTAuMCwiY29tbWlzc2lvbl9kZWJpdCI6MC4wLCJjb21taXNzaW9uX2NyZWRpdCI6My4xNSwiY3VycmVuY3lfZGViaXQiOiJVQUgiLCJjdXJyZW5jeV9jcmVkaXQiOiJVQUgiLCJzZW5kZXJfYm9udXMiOjAuMCwiYW1vdW50X2JvbnVzIjowLjAsIm1waV9lY2kiOiI3IiwiaXNfM2RzIjpmYWxzZSwibGFuZ3VhZ2UiOiJ1ayIsImNyZWF0ZV9kYXRlIjoxNzY1NTYxNDg0NzkxLCJlbmRfZGF0ZSI6MTc2NTU2MTQ4NDkwNiwidHJhbnNhY3Rpb25faWQiOjI3NjM3NTIzMTZ9",
        "signature": "BZZqCEtGcEdB58vn/F8+LeaWgZI="
    },
    "sender_card_country": 804,
    "amount_debit": 210,
    "result": "ok",
    "amount": 210,
    "commission_credit": 3.15,
    "currency_debit": "UAH",
    "sender_card_bank": "Test",
    "end_date": 1765561484906,
    "receiver_commission": 3.15,
    "acq_id": 414963,
    "order_id": "b8fb8093-ce8f-4efd-b3b5-fd10ae6e9d07",
    "description": "Оплата замовлення # b8fb8093-ce8f-4efd-b3b5-fd10ae6e9d07",
    "agent_commission": 0,
    "sender_first_name": "test",
    "pay_way": "card,privat24,gpay,apay,qr",
    "sender_last_name": "test",
    "mpi_eci": "7",
    "liqpay_order_id": "EZCDJVBK1765561484787707",
    "sender_card_type": "visa",
    "card_mask": "424242*42",
    "paytype": "card",
    "sender_commission": 0,
    "cmd": "liqpay.callback",
    "data": "eyJwYXltZW50X2lkIjoyNzYzNzUyMzE2LCJhY3Rpb24iOiJwYXkiLCJzdGF0dXMiOiJzYW5kYm94IiwidmVyc2lvbiI6MywidHlwZSI6ImJ1eSIsInBheXR5cGUiOiJjYXJkIiwicHVibGljX2tleSI6InNhbmRib3hfaTc1NDE0MjcyNTE1IiwiYWNxX2lkIjo0MTQ5NjMsIm9yZGVyX2lkIjoiYjhmYjgwOTMtY2U4Zi00ZWZkLWIzYjUtZmQxMGFlNmU5ZDA3IiwibGlxcGF5X29yZGVyX2lkIjoiRVpDREpWQksxNzY1NTYxNDg0Nzg3NzA3IiwiZGVzY3JpcHRpb24iOiLQntC/0LvQsNGC0LAg0LfQsNC80L7QstC70LXQvdC90Y8gIyBiOGZiODA5My1jZThmLTRlZmQtYjNiNS1mZDEwYWU2ZTlkMDciLCJzZW5kZXJfZmlyc3RfbmFtZSI6InRlc3QiLCJzZW5kZXJfbGFzdF9uYW1lIjoidGVzdCIsInNlbmRlcl9jYXJkX21hc2syIjoiNDI0MjQyKjQyIiwic2VuZGVyX2NhcmRfYmFuayI6IlRlc3QiLCJzZW5kZXJfY2FyZF90eXBlIjoidmlzYSIsInNlbmRlcl9jYXJkX2NvdW50cnkiOjgwNCwiaXAiOiIxNDUuMjI0Ljk0LjE0OSIsImFtb3VudCI6MjEwLjAsImN1cnJlbmN5IjoiVUFIIiwic2VuZGVyX2NvbW1pc3Npb24iOjAuMCwicmVjZWl2ZXJfY29tbWlzc2lvbiI6My4xNSwiYWdlbnRfY29tbWlzc2lvbiI6MC4wLCJhbW91bnRfZGViaXQiOjIxMC4wLCJhbW91bnRfY3JlZGl0IjoyMTAuMCwiY29tbWlzc2lvbl9kZWJpdCI6MC4wLCJjb21taXNzaW9uX2NyZWRpdCI6My4xNSwiY3VycmVuY3lfZGViaXQiOiJVQUgiLCJjdXJyZW5jeV9jcmVkaXQiOiJVQUgiLCJzZW5kZXJfYm9udXMiOjAuMCwiYW1vdW50X2JvbnVzIjowLjAsIm1waV9lY2kiOiI3IiwiaXNfM2RzIjpmYWxzZSwibGFuZ3VhZ2UiOiJ1ayIsImNyZWF0ZV9kYXRlIjoxNzY1NTYxNDg0NzkxLCJlbmRfZGF0ZSI6MTc2NTU2MTQ4NDkwNiwidHJhbnNhY3Rpb25faWQiOjI3NjM3NTIzMTZ9",
    "signature": "BZZqCEtGcEdB58vn/F8+LeaWgZI="
}
         * 
         */



        // { Data: "", Signature  }

        return new JsonResult(new  { Ok = true });
    }
}
