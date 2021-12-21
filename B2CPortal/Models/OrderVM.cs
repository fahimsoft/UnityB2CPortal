using API_Base.Common;
using B2C_Models.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace B2CPortal.Models
{
    public class OrderVM
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OrderDescription { get; set; }
        public Nullable<int> TotalQuantity { get; set; }
        public Nullable<decimal> TotalPrice { get; set; }
        public int FK_Customer { get; set; }
        public string OrderNo { get; set; }
        public string ShippingAddress { get; set; }
        public string BillingAddress { get; set; }
        public string PhoneNo { get; set; }
        public string Status { get; set; }
        public string EmailId { get; set; }
        public string Gender { get; set; }
        public string PaymentMode { get; set; }
        public string Currency { get; set; }
        public decimal ConversionRate { get; set; }
        public PaymentType paymenttype { get; set; }
        public bool PaymentStatus { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public string CNIC { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public string MasterImageUrl { get; set; }
        public string Date { get; set; }
        public  decimal bTotal { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MMM-yyyy}")]
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MMM-yyyy}")]
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<decimal> Price { get; set; }
        public int FK_OrderMaster { get; set; }
        public int FK_ProductMaster { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public decimal CartSubTotal { get; set; }
        public List<OrderVM> orderVMs { get; set; }
        public List<OrderDetailsViewModel> OrderDetailsViewModels { get; set; }
        public decimal CartSubTotalDiscount { get; internal set; }

        public decimal ShipingAndHostring { get; internal set; }
        public decimal OrderTotal { get; internal set; }
        public decimal? SubTotalPrice { get; internal set; }
        public decimal? DiscountAmount { get; internal set; }
        public  ICollection<OrderDetail> OrderDetails { get; set; }
        public  ShippingDetail shippingdetails { get; set; }
        public string GivenName { get; internal set; }
        public bool IsShipping { get; internal set; }
        public int FK_ShippingDetails { get; internal set; }
    }
    public class OrderDetailsViewModel
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OrderDescription { get; set; }
        public Nullable<int> TotalQuantity { get; set; }
        public Nullable<decimal> TotalPrice { get; set; }
        public int FK_Customer { get; set; }
        public string OrderNo { get; set; }
        public string ShippingAddress { get; set; }
        public string BillingAddress { get; set; }
        public string PhoneNo { get; set; }
        public string Status { get; set; }
        public string EmailId { get; set; }
        public string Gender { get; set; }
        public string PaymentMode { get; set; }
        public string Currency { get; set; }
        public decimal ConversionRate { get; set; }
        public PaymentType paymenttype { get; set; }
        public bool PaymentStatus { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public string CNIC { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public string MasterImageUrl { get; set; }
        public string Date { get; set; }
        public decimal bTotal { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MMM-yyyy}")]
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MMM-yyyy}")]
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<decimal> Price { get; set; }
        public int FK_OrderMaster { get; set; }
        public int FK_ProductMaster { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public decimal CartSubTotal { get; set; }
        public List<OrderVM> orderVMs { get; set; }
        public decimal CartSubTotalDiscount { get; internal set; }

        public decimal ShipingAndHostring { get; internal set; }
        public decimal OrderTotal { get; internal set; }
        public decimal? SubTotalPrice { get; internal set; }
        public decimal? DiscountAmount { get; internal set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }

    }
    public enum PaymentType
    {
        Stripe = 1,
        Paypal = 2,
        HBL = 3,
        JazzCash = 4,
        EasyPaisa = 5,
        COD = 6
    }
    public enum OrderStatus
    {
        Pending = 1,
        Confirmed = 2,
        InProcess = 3,
        Delivered = 4,
        Cancelled = 5,
        Rejected = 6,
    }
}