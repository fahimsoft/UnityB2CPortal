﻿using API_Base.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace B2CPortal.Models
{
    public class PaymentViewModel
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
        public PaymentType paymenttype { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public string CNIC { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public string MasterImageUrl { get; set; }
        public string Date { get; set; }
        public string PaymentMode { get; set; }
        public decimal? ConversionRate { get; set; }
        public string Currency { get; set; }
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
        public decimal CartSubTotalDiscount { get; internal set; }

        public decimal ShipingAndHostring { get; internal set; }
        public decimal OrderTotal { get; internal set; }
        public int VatTax { get; internal set; }
        public CartViewModel CartViewModel { get; set; }
        public List<OrderVM> orderVMs { get; set; }
        public decimal? SubTotalPrice { get; internal set; }
        public decimal? DiscountAmount { get; internal set; }
   
    }
}