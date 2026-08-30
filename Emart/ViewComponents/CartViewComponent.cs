using EMart.Repositories.Interfaces;
using EMart.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EMart.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        private readonly ICartRepository _cartRepository;

        public CartViewComponent(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            int customerId = 1;

            var items = await _cartRepository.GetMiniCart(customerId);

            MiniCartVM model = new MiniCartVM
            {
                Items = items,
                CartCount = items.Sum(x => x.Quantity),
                GrandTotal = items.Sum(x => x.Total)
            };

            return View(model);
        }
    }
}