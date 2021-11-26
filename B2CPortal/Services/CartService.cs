using API_Base.Base;
using B2C_Models.Models;
using B2CPortal.Interfaces;
using B2CPortal.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace B2CPortal.Services
{
    public class CartService : DALBase<Cart>, ICart
    {
        public async Task<Cart> CreateCart(Cart cart)
        {
            try
            {
                _dxcontext.Configuration.LazyLoadingEnabled = false;
                Current = await _dxcontext.Carts.FirstOrDefaultAsync(x => x.IsActive == true && x.IsWishlist == false && x.FK_ProductMaster == cart.FK_ProductMaster && x.Guid == cart.Guid);
                if (Current == null)
                {
                    New();
                    Current.CreatedOn = DateTime.Now;
                    Current.Guid = cart.Guid;
                }
                else
                {
                    if ((cart.Quantity + Current.Quantity) > 9)
                    {
                        return null;
                    }
                    PrimaryKeyValue = Current.Id;
                    Current.ModifiedOn = DateTime.Now;
                }
                if (cart.FK_Customer > 0)
                {
                    Current.FK_Customer = cart.FK_Customer;
                }
                Current.Quantity = cart.Quantity + (Current.Quantity == null ? 0 : Current.Quantity);
                Current.TotalPrice = (cart.TotalPrice * Current.Quantity);
                Current.FK_ProductMaster = cart.FK_ProductMaster;
                Current.IsActive = cart.IsActive;
                Current.TotalQuantity = Current.Quantity;
                Current.IsActive = cart.IsActive;
                Current.IsWishlist = false;

                Save();
                return Current;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Cart> UpdateCartFromWishList(Cart cart)
        {
            try
            {
                _dxcontext.Configuration.LazyLoadingEnabled = false;
                //Current = await _dxcontext.Carts.FirstOrDefaultAsync(x => x.FK_ProductMaster == cart.FK_ProductMaster && x.Guid == cart.Guid);
                //Current = await _dxcontext.Carts.Where(x => x.Guid == cart.Guid && x.IsWishlist == true && x.FK_ProductMaster == cart.FK_ProductMaster || x.IsWishlist == true && x.FK_Customer == cart.FK_Customer && x.FK_ProductMaster == cart.FK_ProductMaster).FirstOrDefaultAsync();



                Current = await _dxcontext.Carts.FirstOrDefaultAsync
                (
                x =>
                (x.Guid == cart.Guid
                && x.IsWishlist == true
                && x.FK_ProductMaster == cart.FK_ProductMaster)
                ||
                (x.IsWishlist == true
                && x.FK_Customer == cart.FK_Customer
                && x.FK_ProductMaster == cart.FK_ProductMaster)
                );




                if (Current == null)
                {
                    New();
                    Current.CreatedOn = DateTime.Now;
                    Current.IsWishlist = cart.IsWishlist;





                }
                else
                {
                    PrimaryKeyValue = Current.Id;
                    Current.ModifiedOn = DateTime.Now;
                    Current.IsWishlist = cart.IsWishlist;





                }





                Current.Guid = cart.Guid;
                if (cart.FK_Customer > 0)
                {
                    Current.FK_Customer = cart.FK_Customer;
                }
                Current.FK_ProductMaster = cart.FK_ProductMaster;
                Current.Quantity = cart.Quantity;
                Current.IsActive = cart.IsActive;
                Current.TotalPrice = cart.TotalPrice;
                Current.TotalQuantity = cart.TotalQuantity;



                Save();
                return Current;





            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<Cart>> GetCartProducts(string guid, int customerid)
        {
            List<Cart> cartlist = new List<Cart>();
            _dxcontext.Configuration.LazyLoadingEnabled = false;
            cartlist = await _dxcontext.Carts.Where(x => (x.Guid == guid && x.IsWishlist == false && x.IsActive == true)
            ||
            (x.IsWishlist == false && x.IsActive == true && x.FK_Customer == customerid)).ToListAsync();
            return cartlist;
        }

        public async Task<bool> DeleteCart(long Id)
        {
            try
            {
                Current = await _dxcontext.Carts.FirstOrDefaultAsync(o => o.Id == Id && o.IsWishlist == false && o.IsActive == true);
                if (Current != null)
                    PrimaryKeyValue = Current.Id;
                Delete();

                return true;
            }
            catch (Exception Ex)
            {

                throw Ex;
            }
        }

        public async Task<Cart> GetCartById(long Id)
        {
            Current = await _dxcontext.Carts.Where(x => x.Id == Id && x.IsActive == true && x.IsWishlist == false).FirstOrDefaultAsync();
            return Current;
        }

        public async Task<Cart> GetWishlistById(int Id)
        {
            Current = await _dxcontext.Carts.Where(x => x.Id == Id && x.IsActive == true && x.IsWishlist == true).FirstOrDefaultAsync();
            return Current;
        }

        public async Task<bool> UpdateCart(Cart cart)
        {
            try
            {
                Current = await _dxcontext.Carts.Where(x => x.Id == cart.Id && x.IsActive == true && x.IsWishlist == false).FirstOrDefaultAsync();
                if (Current == null)
                {
                    New();
                    Current.CreatedOn = DateTime.Now;
                    Current.Quantity = cart.Quantity;
                    Current.TotalQuantity = cart.TotalQuantity;
                }
                else
                {
                    PrimaryKeyValue = Current.Id;
                    Current.ModifiedOn = DateTime.Now;
                    Current.FK_Customer = cart.FK_Customer;
                }
                Save();
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        public async Task<bool> UpdateWishlistQuantity(Cart cart)
        {
            try
            {
                Current = await _dxcontext.Carts.Where(x => x.Id == cart.Id && x.IsActive == true && x.IsWishlist == true).FirstOrDefaultAsync();
                if (Current == null)
                {
                    New();
                    Current.CreatedOn = DateTime.Now;
                    Current.Quantity = cart.Quantity;
                    Current.TotalQuantity = cart.TotalQuantity;
                }
                else
                {
                    PrimaryKeyValue = Current.Id;
                    Current.ModifiedOn = DateTime.Now;
                }
                Save();
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        //work by ahsan---------
        public async Task<IEnumerable<Cart>> GetWishListProducts(string guid, int customerId)
        {
            var wishlist = await _dxcontext.Carts.Where(x =>
            (x.Guid == guid && x.IsWishlist == true
            && x.IsActive == true)
            ||
            (x.IsWishlist == true
            && x.IsActive == true
            && x.FK_Customer == customerId)



            ).ToListAsync();// GetAll();
            return wishlist;
        }
        public async Task<Cart> CreateWishList(Cart cart)
        {
            try
            {
                _dxcontext.Configuration.LazyLoadingEnabled = false;
                Current = await _dxcontext.Carts.FirstOrDefaultAsync(x => x.Guid == cart.Guid && x.IsWishlist == true && x.IsActive == true && x.FK_ProductMaster == cart.FK_ProductMaster || x.IsWishlist == true && x.FK_ProductMaster == cart.FK_ProductMaster && x.IsActive == true && x.FK_Customer == cart.FK_Customer);
                //var quan = await _dxcontext.Carts.FirstOrDefaultAsync(x => x.Guid == cart.Guid);



                if (Current == null)
                {
                    New();

                    Current.CreatedOn = DateTime.Now;
                    Current.Quantity = 1;
                    Current.TotalQuantity = 1;
                    Current.TotalPrice = cart.TotalPrice;
                }
                else
                {
                    //if(quan != null)
                    //{

                    //}
                    PrimaryKeyValue = Current.Id;
                    Current.ModifiedOn = DateTime.Now;
                    Current.Quantity++;
                    Current.TotalQuantity++;
                    Current.TotalPrice = (cart.TotalPrice * Current.Quantity);

                }
                Current.Guid = cart.Guid;
                Current.IsWishlist = cart.IsWishlist;
                Current.IsActive = cart.IsActive;
                Current.FK_ProductMaster = cart.FK_ProductMaster;
                Current.FK_Customer = cart.FK_Customer;

                Save();
                return Current;



            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<Cart> GetCartData(string guid, int customerId, int productId)
        {
            var cartData = await _dxcontext.Carts.Where(x =>
            (x.Guid == guid && x.IsWishlist == false
            && x.IsActive == true && x.FK_ProductMaster == productId)
            ||
            (x.IsWishlist == false
            && x.IsActive == true
            && x.FK_Customer == customerId && x.FK_ProductMaster == productId)

            ).FirstOrDefaultAsync();
            return cartData;
        }
        //public async Task<Cart> GetCartData(string guid, int customerId, int productId)
        //{
        //    var cartData = await _dxcontext.Carts.Where(x =>
        //    (x.Guid == guid && x.IsWishlist == false
        //    && x.IsActive == true && x.FK_ProductMaster == productId)
        //    ||
        //    (x.IsWishlist == false
        //    && x.IsActive == true
        //    && x.FK_Customer == customerId && x.FK_ProductMaster == productId)



        //    ).FirstOrDefaultAsync();
        //    return cartData;
        //}

        public async Task<Cart> UpdateToCart(Cart wishlistVM)
        {
            try
            {
                _dxcontext.Configuration.LazyLoadingEnabled = false; if (wishlistVM.IsWishlist == true)
                {
                    Current = await _dxcontext.Carts.Where(x =>
                    (x.Guid == wishlistVM.Guid && x.IsWishlist == true
                    && x.IsActive == true)
                    ||
                    (x.IsWishlist == true
                    && x.IsActive == true
                    && x.FK_Customer == wishlistVM.FK_Customer)).FirstOrDefaultAsync();
                }
                else
                {
                    Current = await _dxcontext.Carts.Where(x =>
                    (x.Guid == wishlistVM.Guid && x.IsWishlist == false
                    && x.IsActive == true && x.FK_ProductMaster == wishlistVM.FK_ProductMaster)
                    ||
                    (x.IsWishlist == false
                    && x.IsActive == true
                    && x.FK_Customer == wishlistVM.FK_Customer && x.FK_ProductMaster == wishlistVM.FK_ProductMaster)).FirstOrDefaultAsync();
                }
                if (Current != null)
                {
                    PrimaryKeyValue = Current.Id;
                    Current.FK_Customer = wishlistVM?.FK_Customer;
                    Current.Guid = wishlistVM?.Guid;
                    Current.ModifiedOn = DateTime.Now;
                    Current.Quantity = wishlistVM.Quantity;
                    Current.TotalQuantity = wishlistVM.TotalQuantity;
                    Current.TotalPrice = (wishlistVM.TotalPrice * Current.TotalQuantity);
                    Current.IsWishlist = wishlistVM.IsWishlist == true ? true : false;
                }
                Save();
                return Current;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Cart> UpdateWishList(int cartId)
        {
            try
            {
                Current = await _dxcontext.Carts.Where(x => x.Id == cartId && x.IsActive == true).FirstOrDefaultAsync();
                if (Current != null)
                {
                    PrimaryKeyValue = Current.Id;
                    Current.ModifiedOn = DateTime.Now;
                    Current.IsActive = false;
                }
                Save();
                return Current;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<bool> DeleteFromCart(int Id)
        {
            try
            {
                Current = await _dxcontext.Carts.FirstOrDefaultAsync(o => o.Id == Id);
                if (Current != null)
                    PrimaryKeyValue = Current.Id;
                Delete();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<bool> DisableCart(int customerId, string guid)
        {
            try
            {
                // return await _dxcontext.Carts.Where(x => x.IsActive == true && x.IsWishlist == false).ToListAsync();
                var db = await _dxcontext.Carts.Where(x => (x.FK_Customer == customerId && x.IsActive == true && x.IsWishlist == false)
                ||
                (x.Guid == guid && x.IsWishlist == false && x.IsActive == true))
                .ToListAsync();
                if (db != null)
                {
                    foreach (var item in db)
                    {
                        PrimaryKeyValue = item.Id;
                        item.IsActive = false;
                        Current = item;
                    }
                    Save();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}