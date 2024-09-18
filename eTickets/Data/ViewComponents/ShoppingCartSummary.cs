using eTickets.Data.Cart;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eTickets.Data.ViewComponents
{
    public class ShoppingCartSummary:ViewComponent
    {
        private readonly ShoppingCart _shoppingCart;
        // Constructor that receives the ShoppingCart service
        // via dependency injection
        public ShoppingCartSummary(ShoppingCart shoppingCart)
        {
            _shoppingCart = shoppingCart;
        }
        // The Invoke method is used to render the view component
        public IViewComponentResult Invoke()
        {
            // Retrieve the list of items in the shopping cart
            var items = _shoppingCart.GetShoppingCartItems();
            // Pass the count of items to the view component's view
            return View(items.Count);
        }
    }
}
