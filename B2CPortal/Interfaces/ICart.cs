using System;
using System.Collections.Generic;
using System.Linq;
using B2C_Models.Models;
using System.Text;
using System.Threading.Tasks;
using B2CPortal.Models;

namespace B2CPortal.Interfaces
{
    public interface ICart
    {
        Task<IEnumerable<Cart>> GetCartProducts(string guid , int customerid);
        Task<Cart> CreateCart(Cart cart);

        Task<Cart> UpdateCartFromWishList(Cart cart);

        Task<Cart> GetCartById(long Id);
        Task<Cart> GetWishlistById(int Id);

        Task<bool> UpdateCart(Cart cart);
        Task<bool> UpdateWishlistQuantity(Cart cart);

        Task<bool> DeleteCart(long Id);
        Task<Cart> CreateWishList(Cart cart);
        Task<IEnumerable<Cart>> GetWishListProducts(string guid, int customerId);
        Task<Cart> GetCartData(string guid, int customerId, int productId);
        Task<Cart> UpdateToCart(Cart wishlistVM);
        Task<Cart> UpdateWishList(int cartId);
        Task<bool> DeleteFromCart(int Id);

    }
}
