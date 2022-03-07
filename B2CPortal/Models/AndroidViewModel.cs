using B2C_Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace B2CPortal.Models
{
    public class AndroidViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal? Price { get; set; }
        public decimal? Discount { get; set; }
        public string ImageUrl { get; set; }
        public string MasterImageUrl { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string UOM { get; set; }
        public decimal? TotalRating { get; set; }
        public decimal? AvgRating { get; set; }
        public int? UnitInNumeric { get; set; }
        public bool IsFeatured { get; internal set; }
        public bool? IsNewArrival { get; internal set; }
    }
    public class AndroidAuthenticationVM
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string PhoneNo { get; set; }
        public string EmailId { get; set; }
        public string Password { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public string Guid { get; set; }
    }
    public class CartDataList
    {
        public int ProductId { get; set; }
        public decimal ProductPrice { get; set; }
        public decimal ProductDiscount { get; set; }
        public int ProductQuantity { get; set; }
    }
    public class CartDataResponseList
    {
        public int ProductId { get; set; }
        public decimal ProductPrice { get; set; }
        public string Name { get; set; }
        public decimal ProductDiscount { get; internal set; }
        public string MasterImageUrl { get; internal set; }
        public int ProductQuantity { get; internal set; }
    }

    public class AndroidCheckoutVM
    {
        public List<CartDataList> cartData { get; set; }
        public ShippingDetail shippingdetails { get; set; }
        public string Usercity { get; set; }
        public string userid { get; set; }
        public string guid { get; set; }
        //-----for prevent extra call db----------
        public string username { get; set; }
        public string useremail { get; set; }
        public int Id { get; set; }
        public string OrderDescription { get; set; }
        public Nullable<int> TotalQuantity { get; set; }
        public Nullable<decimal> TotalPrice { get; set; }
        public int FK_Customer { get; set; }
        public string OrderNo { get; set; }
        public string ShippingAddress { get; set; }
        public string BillingAddress { get; set; }
        public string PhoneNo { get; set; }
        public string EmailId { get; set; }
        public string Country { get; set; }
        public string Status { get; set; }
        public string Currency { get; set; }
        public Nullable<decimal> ConversionRate { get; set; }
        public string PaymentMode { get; set; }
        public Nullable<bool> PaymentStatus { get; set; }
        public bool IsShipping { get; set; }
        public Nullable<int> FK_ShippingDetails { get; set; }
        public Nullable<int> FK_CityId { get; set; }
        public string CityName { get; internal set; }
    }
    public class AndroidOrderDetailsVM
    {
        public string Name { get; internal set; }
        public decimal? Price { get; internal set; }
        public decimal? Discount { get; internal set; }
        public decimal? SubTotalPrice { get; internal set; }
        public int? Quantity { get; internal set; }
        public decimal? DiscountedPrice { get; internal set; }
        public decimal? TotalPrice { get; internal set; }
        public string MasterImageUrl { get; internal set; }
        public string Date { get; internal set; }
        public int FK_ProductMaster { get; internal set; }
    }
    public static class ResultStatus
    {
        public static string Error = "something went wrong";
        public static string failed = "something went wrong.Please try again";
        public static string unauthorized = "Incorrect Email or password. Please try again.";
        public static string notfound = "Page Not Found";
        public static string Loginsuccess = "You are successfully logged in";
        public static string RegisterSuccess = "You are successfully Registered.";
        public static string AlreadyExist = "You are Already Registered.";
        public static string success = "Success";

        public static string InsertOrder = "Order placed successfully";
        public static string Insert = "Successfully Added";
        public static string Update = "Record Updated Successfully";
        public static string Delete = "Record Deleted Successfully";
        public static string EmptyFillData = "Please Fill All Fields Correctly. Please try again.";

        public static string UserNotExist = "User Not Exist. Please try again.";
        public static string PriceChanged = "Price has been changed.";

    }
public class AndroidOrderListVM
    {
        //public ShippingDetail shippingdetails { get; set; }
        public List<AndroidOrderDetailsListVM> AndroidOrderDetailsListVMs { get; set; }
        //public string Usercity { get; set; }
        //public string userid { get; set; }
        //public string guid { get; set; }
        //-----for prevent extra call db----------
        //public string username { get; set; }
        //public string useremail { get; set; }
        public int Id { get; set; }
        public string OrderDescription { get; set; }
        public Nullable<int> TotalQuantity { get; set; }
        public Nullable<decimal> TotalPrice { get; set; }
        //public int FK_Customer { get; set; }
        public string OrderNo { get; set; }
        public string ShippingAddress { get; set; }
        public string BillingAddress { get; set; }
        public string PhoneNo { get; set; }
        public string EmailId { get; set; }
        public string Country { get; set; }
        public string Status { get; set; }
        public string Currency { get; set; }
        public Nullable<decimal> ConversionRate { get; set; }
        public string PaymentMode { get; set; }
        public Nullable<bool> PaymentStatus { get; set; }
        //public bool IsShipping { get; set; }
        //public Nullable<int> FK_ShippingDetails { get; set; }
        //public Nullable<int> FK_CityId { get; set; }
        public string City { get; internal set; }
        public string CreatedOnDate { get; internal set; }
    }

    public class AndroidOrderDetailsListVM
    {
        public int Id { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<decimal> Price { get; set; }
        //public int FK_OrderMaster { get; set; }
        //public int FK_ProductMaster { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        //public Nullable<int> CreatedBy { get; set; }
        //public Nullable<System.DateTime> ModifiedOn { get; set; }
        //public Nullable<int> ModifiedBy { get; set; }
        //public Nullable<bool> IsActive { get; set; }
        //public Nullable<int> FK_Customer { get; set; }
        public Nullable<decimal> DiscountedPrice { get; set; }
        public Nullable<decimal> TotalPrice { get; set; }
        public string Currency { get; set; }
        public Nullable<decimal> ConversionRate { get; set; }
        public string ImageURL { get; internal set; }
        public string Name { get; internal set; }
        public string PackSize { get; internal set; }
        public string CreatedOnDate { get; internal set; }
    }


}