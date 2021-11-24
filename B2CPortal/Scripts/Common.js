
//---------------cart count and hover list----------------
var rating = 0;
$(document).ready(function () {
    ShowCartProducts();
    $("#handlesearch").autocomplete(
        {
            source: function (request, response) {
                $.ajax({
                    url: '/Product/SearchProductList',
                    type: "POST",
                    dataType: "json",
                    data: { Prefix: request.term },
                    success: function (data) {
                        response($.map(data, function (item) {
                            return {
                                item // label: item.MasterImageUrl, value: item.MasterImageUrl, MasterImageUrl: item.Name
                            }
                        }));
                    }
                });
            },
            open: (event) => {
                $('.ui-autocomplete .ui-menu-item div').toArray().forEach((element) => {
                    let imagePath = element.innerHTML;
                    $(element).html('');
                    var inner_html = '<div class="list_item_container"><div class="image"><img src="' +
                        imagePath + '"></div>';
                    $(element).append(inner_html);
                });
            }
        
        });

    //jQuery('#handlesearch').keyup(function () {
    //    var searchtext = $(this).val();
        
    //});
    jQuery('.plus-minus-box').keyup(function () {
        this.value = this.value.replace(/[^0-9\.]/g, '');
    });

    //test
    $(document).on('click', '.qtybuttonquickview', function () {
        var $button = $(this);
        var oldValue = $button.parent().find("input").val();
        oldValue = oldValue == "NaN" || oldValue == "" ? 0 : $button.parent().find("input").val();
        if ($button.text() == "+") {
            var newVal = parseFloat(oldValue) + 1;
        } else {
            // Don't allow decrementing below zero
            if (oldValue > 0) {
                var newVal = parseFloat(oldValue) - 1;
                newVal = newVal == 0 ? 1 : newVal;
            } else {
                newVal = 1;
            }
        }
        var price = parseFloat($('#discoountedprice').text());
        let totalvalue = (price * newVal);
        $('#labalprice').text(totalvalue.toLocaleString());
        $button.parent().find("input").val(newVal);

    });
    $(document).on('click', '.qtybutton', function () {
        //$(".qtybutton").on("click", function () {
        var $button = $(this);
        var oldValue = $button.parent().find("input").val();
        oldValue = oldValue == "NaN" || oldValue == "" ? 0 : $button.parent().find("input").val();
        if ($button.text() == "+") {
            var newVal = parseFloat(oldValue) + 1;
        } else {
            // Don't allow decrementing below zero
            if (oldValue > 0) {
                var newVal = parseFloat(oldValue) - 1;
                newVal = newVal == 0 ? 1 : newVal;
            } else {
                newVal = 1;
            }
        }
        var row = $(this).closest("tr");
        var Discount = parseInt($(row).find('td')[1].textContent);
        var price = parseInt($(row).find('td')[2].textContent);
        let actualTotal = (price * newVal);

        price = price * (1 - (Discount / 100));
        let totalvalue = (price * newVal);

        DiscountAmount = actualTotal - totalvalue;

        $(row).find('td')[4].textContent = actualTotal.toLocaleString();
        $(row).find('td')[5].textContent = '- ' + DiscountAmount.toLocaleString();
        $(row).find('td')[6].textContent = totalvalue.toLocaleString();

        $button.parent().find("input").val(newVal);
    });
});
function removequickviewvalues(id) {

    //$('#quentityvalue').val("");
    document.getElementsByClassName("main-view modal-content").remove();
}
//=====================On hover cart list========================
function ShowCartProducts() {
    $.ajax({
        type: "POST",
        url: "/Product/GetCartCount",
        data: {},
        success: function (data) {
            var html = "";
            let dataobj = JSON.parse(data.data);

            dataobj.cartproducts.map(function (item, index) {
                html += ` <div class="sin-itme clearfix">
                    <i  onclick="RemoveCartProduct(${item.Id})" class="mdi mdi-close removecartbtn"></i>
                    <a class="cart-img" href="cart.html"><img src="${item.MasterImageUrl}" alt="" /></a>
                    <div class="menu-cart-text">
                        <a href="#"><h5>${item.Name}</h5></a>
                        <span>Quantity: ${item.Quantity}</span>
                        <strong>PKR.${item.TotalPrice.toLocaleString()} </strong>
                    </div>
                </div> `;
            });
            html += `
<div class="totalPriceDetails">
<div class="total">
                                <span>total <strong>= PKR. ${dataobj.totalprice.toLocaleString()}</strong></span>
                            </div>
                            <a class="goto" href="/Product/AddToCart"> go to cart</a>
                            <a class="out-menu" href="/Orders/Checkout">Check out</a>
                            </div>

`;
            $('#cartdrop').html(html);
            $('#productaddtocart').html(dataobj.cartproductscount);
            $('#totalprice').html(dataobj.totalprice.toLocaleString());

        }
    });

}

function RemoveCartProduct(id) {

    $.ajax({
        type: "POST",
        url: "/Product/DeleteCart",
        data: {
            id: id,
        },
        success: function (data) {
            ShowCartProducts();
            toastr.success(data.msg);
            var tb = $('#myTable tbody');
            tb.find("tr").each(function (index, element) {
                var trvalue = parseInt($(element).attr('data-id'));
                if (trvalue == parseInt(id)) {
                    $(element).remove();
                }
            });
            return false;
        }
    });
}
function HandleAddtocart(id) {

    var proid = $(id).attr("productIdList");
    var quentity = $('#quentityvalue').val();
    $.ajax({
        type: "POST",
        url: "/Product/AddToCart",
        data: {
            proid: proid,
            quentity: quentity,
        },
        success: function (data) {
            ShowCartProducts();
            toastr.success("Product Add to Cart successfully.");

        }
    });

}
function Handleorderdetail(id) {
    $.ajax({
        url: '/Orders/GetOrderDetailsById/',
        type: "GET",
        data: {
            id: id
        },
        //contentType: "application/json;charset=utf-8",
        //dataType: "json",
        success: function (result) {
            var html = '';
            var htmlProductPriceDetail = '', htmlProductSize = '';
            var data = result.data;
            $(data).each(function (index, item) {
                htmlProductPriceDetail += ` <tr data-id="58">
                                        <td class="td-img text-left">
                                            <a href="/ProductDetails?productId=${item.FK_ProductMaster}"><img src="${item.MasterImageUrl}" alt="Add Product"></a>
                                            <div class="items-dsc">
                                                <h5><a href="/ProductDetails?productId=${item.FK_ProductMaster}"> ${item.Name}
                                                 </a></h5>

                                            </div>
                                        </td>
                            
                                        <td class="pricecol">
                                           ${item.Discount}
                                        </td>
                                        <td>
                                           ${item.Price}
                                        </td>
                                        <td>
                                           ${item.Quantity}
                                        </td>
                                         <td>
                                           ${item.SubTotalPrice}
                                        </td>
                                            <td>
                                           ${item.DiscountAmount}
                                        </td>
                                            <td>
                                           ${item.TotalPrice}
                                        </td>

                                        <td>
${moment(item.Date).format('MMMM Do YYYY')}
                                        </td>
                                    </tr>`

            });




            html = `
<div class="container">
        <div class="row">
            <div class="col-xs-12">
                <div class="d-table">
                    <div class="d-tablecell">
                        <div class="modal-dialog">
                            <div class="main-view modal-content">
                                <div class="modal-footer" data-dismiss="modal" onclick="removequickviewvalues()">
                                    <span>x</span>
                                </div>
                                <div class="row">

                         <table class="wishlist-table text-center" id="myTable">
                        <thead>
                            <tr>
                                <th>Product Name</th>
                                <th>Discount %</th>
                                <th>Price</th>
                                <th>Quantity</th>
                                <th>Sub Total</th>
                                <th>Discounted Amount</th>
                                <th>Total</th>
                                <th>Ordered Date </th>
                            </tr>
                        </thead>
                        <tbody>
            ${htmlProductPriceDetail}
  
                        </tbody>
                    </table>

                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>`;


            $('#quick-view').html(html);
            //SetLocalStorage(elem);
        },
        error: function (errorMessage) {
            alert(errorMessage.responseText);
        }
    });
}
//Plus and minus quentiry------------>
//---------------cart count and hover list----------------]
//---------------cart Wishlist ----------------
function HandleAddtoWishList(id) {
    var proid = $(id).attr("productIdList");
    //var guid = $.cookie("cartguid");
    //  var guid = document.cookie.split('=')[1];
    $.ajax({
        type: "POST",
        url: "/Product/GetDataForWishList",
        data: {
            id: proid,
            // guid: guid
        },
        success: function (data) {
            // document.cookie = "cartguid=" + data.data.Guid;
            toastr.success("Product Add to Wish List successfully.");
        }
    });

}
//---------------cart Wishlist ----------------
//---------------Product management----------------
//Load Product List
function loadProductList() {
    var html = '', htmlProductList = '', htmlProductGrid = '';
    $.ajax({
        url: "/Product/GetProduct",
        type: "GET",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            var data = JSON.parse(result.data);

            $('#lblTotalCount').text('Total Records: ' + data.length);

            $.each(data, function (key, item) {

                //List Html
                $(item.ProductPrices).each(function (productPricesIndex, productPricesValue) {

                    htmlProductList += `<div class="col - xs - 12">
                            <div class="single-list-view">
                                <div class="row">
                                    <div class="col-xs-12 col-md-4">
                                        <div class="list-img">
                                            <div class="product-img">
                                                <div class="pro-type sell">
                                                    <span>${productPricesValue.Discount}%</span>
                                                </div>
                                                <a href="/ProductDetails/Index?productId=${item.Id}" onClick="SetLocalStorage(this)" productId="${item.Id}" productName="${item.Name}" productImg="${item.MasterImageUrl}"><img src="${item.MasterImageUrl}" alt="Product Title" /></a>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-md-8">
                                        <div class="list-text">
                                            <h3>${item.Name}</h3>
                                            <span>${item.ShortDescription}</span>
                                            <div class="ratting floatright">
                                                <p>( 27 Rating )</p>
                                                <i class="mdi mdi-star"></i>
                                                <i class="mdi mdi-star"></i>
                                                <i class="mdi mdi-star"></i>
                                                <i class="mdi mdi-star-half"></i>
                                                <i class="mdi mdi-star-outline"></i>
                                            </div>
                                            <h5><del>${productPricesValue.Price} PKR</del>&nbsp${productPricesValue.Price * (1 - (productPricesValue.Discount / 100))} PKR</h5 >
                                            <p>${item.LongDescription}</p>
                                            <div class="list-btn">
                                                <a onclick="HandleAddtocart(this)" href="javascript:void(0)" productIdList=${item.Id} >add to cart</a>
                                                <a onclick="HandleAddtoWishList(this)" href="javascript:void(0)" productIdList=${item.Id}>wishlist</a>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                             </div >
                           </div >`;
                });

                //Grid Html
                $(item.ProductPrices).each(function (productPricesGIndex, productPricesGValue) {

                    htmlProductGrid += `<div class="col-xs-12 col-sm-6 col-md-4">
                                                <div class="single-product">
                                                    <div class="product-img">
                                                        <div class="pro-type">
                                                      <span>${productPricesGValue.Discount}%</span>
                                                        </div>
                                                        <a href="/ProductDetails/Index?productId=${item.Id}" productId="${item.Id}" productName="${item.Name}" productImg="${item.MasterImageUrl}" onClick="SetLocalStorage(this)"><img src="${item.MasterImageUrl}" alt="Product Title" /></a>
                                                        <div class="actions-btn">
                                                            <a onclick="HandleAddtocart(this)" href="javascript:void(0)" productIdList=${item.Id}><i class="mdi mdi-cart"></i></a>
                                                            <a href="javascript:void(0)" data-toggle="modal" onClick="LoadQuickView(this)" productId="${item.Id}" productName="${item.Name}" productImg="${item.MasterImageUrl}" onClick="LoadQuickView(this)" id="${item.Id}" data-target="#quick-view"><i class="mdi mdi-eye"></i></a>
                                                            <a onclick="HandleAddtoWishList(this)" productIdList=${item.Id} href="javascript:void(0)"><i class="mdi mdi-heart"></i></a>
                                                        </div>
                                                    </div>
                                                    <div class="product-dsc">
                                                        <p><a href="#">${item.Name}</a></p>
                                                        <div class="ratting">
                                                            <i class="mdi mdi-star"></i>
                                                            <i class="mdi mdi-star"></i>
                                                            <i class="mdi mdi-star"></i>
                                                            <i class="mdi mdi-star-half"></i>
                                                            <i class="mdi mdi-star-outline"></i>
                                                        </div>
                                                        <span><del style='color:silver'>${productPricesGValue.Price} PKR</del>&nbsp${productPricesGValue.Price * (1 - (productPricesGValue.Discount / 100))} PKR</span>
                                                    </div>
                                                </div>
                                            </div>`;

                });

                html = `<div class="tab-pane fade in text-center" id="grid">
                                  ${htmlProductGrid}
                                  </div>
                                  <div class="tab-pane fade active in" id="list">
                                      ${htmlProductList}
                                  </div>`;

            });

            $('#htmlListAndGrid').html(html);
        },
        error: function (errorMessage) {
            alert(errorMessage.responseText);
        }
    });
}

//Load Product Quick View
function LoadQuickView(elem) {
    var Id = elem.id;
    $.ajax({
        url: '/Product/GetProductbyId?Id=' + Id + '',
        type: "GET",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            var html = '';
            var index = 1;
            var fade = 1;
            var productPrice = 0;
            var htmlProductDetail = '';
            var htmlProductPriceDetail = '', htmlProductSize = '';
            var item = JSON.parse(result.data);

            $(item.ProductPrices).each(function (PriceIndex, PriceValue) {

                productPrice = PriceValue.Price;
                productDiscount = PriceValue.Discount;
            });

            $(item.ProductDetails).each(function (ProductDetailsIndex, ProductDetailsValue) {

                if (ProductDetailsIndex == 1) {
                    htmlProductDetail += `<li class="active"><a data-toggle="tab"  href="#sin-${index}"> <img  src="${ProductDetailsValue.ImageUrl}" alt="quick view" /> </a></li>`
                }
                else {
                    htmlProductDetail += `<li><a data-toggle="tab"  href="#sin-${index}"> <img  src="${ProductDetailsValue.ImageUrl}" alt="quick view" /> </a></li>`
                }


                index += 1;
            });

            $(item.ProductDetails).each(function (ProductPDIndex, ProductPDValue) {

                if (ProductPDIndex == 0) {
                    htmlProductPriceDetail += `<div class="simpleLens-container tab-pane active fade in" id="sin-${fade}">
                        <div class="pro-type">
                            <span id="discountedvalue">${productDiscount} </span>%
                        </div>
                        <a class="simpleLens-image" data-lens-image="${ProductPDValue.ImageUrl}" href="#"><img src="${ProductPDValue.ImageUrl}" alt="" class="simpleLens-big-image"></a>
                                                        </div>`
                }
                else {
                    htmlProductPriceDetail += `<div class="simpleLens-container tab-pane fade in" id="sin-${fade}">
                        <div class="pro-type">
                            <span>new</span>
                        </div>
                        <a class="simpleLens-image" data-lens-image="${ProductPDValue.ImageUrl}" href="#"><img src="${ProductPDValue.ImageUrl}" alt="" class="simpleLens-big-image"></a>
                                                        </div>`
                }

                fade += 1;
            });

            $(item.ProductPackSize).each(function (ProductDetailsIndex, ProductSizeValue) {

                htmlProductSize += `<option value="${ProductSizeValue.Id}">${ProductSizeValue.UOM}</option>`


            });


            html = `<div class="container">
        <div class="row">
            <div class="col-xs-12">
                <div class="d-table">
                    <div class="d-tablecell">
                        <div class="modal-dialog">
                            <div class="main-view modal-content">
                                <div class="modal-footer" data-dismiss="modal" onclick="removequickviewvalues()">
                                    <span>x</span>
                                </div>
                                <div class="row">
                                    <div class="col-xs-12 col-sm-5 col-md-4">
                                        <div class="quick-image">
                                            <div class="single-quick-image text-center">
                                                <div class="list-img">
                                                    <div class="product-img tab-content">
                                                    ${htmlProductPriceDetail}
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="quick-thumb">
                                                <ul class="product-slider">${htmlProductDetail}

                                              </ul>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-7 col-md-8">
                                        <div class="quick-right">
                                            <div class="list-text">
                                                <h3>${item.Name} ${htmlProductSize}</h3>
                                                <span>${item.ShortDescription}</span>
                                                <div class="ratting floatright">
                                                    <p>( 27 Rating )</p>
                                                    <i class="mdi mdi-star"></i>
                                                    <i class="mdi mdi-star"></i>
                                                    <i class="mdi mdi-star"></i>
                                                    <i class="mdi mdi-star-half"></i>
                                                    <i class="mdi mdi-star-outline"></i>
                                                </div>
                                                <h5> <del>${productPrice} PKR</del><labal style="color:gray">-${productDiscount}% </labal>   <b id="discoountedprice"> ${productPrice * (1 - (productDiscount / 100))} </b> PKR </h5>
                                                <p>${item.LongDescription}</p>
                                                 <div class="plus-minus">
                                                <a class="dec qtybuttonquickview qtybutton">-</a>
                                                <input type="number" value="1" name="qtybutton" id="quentityvalue" class="plus-minus-box">
                                                <a class="inc qtybuttonquickview qtybutton">+</a> 
                                            </div>
                                                PKR. <labal id="labalprice"> ${productPrice * (1 - (productDiscount / 100))}</labal>
                                                </div>
                                                <div class="list-btn">
                                                    <a href="#"   onclick="HandleAddtocart(this)" productIdList=${item.Id} >add to cart</a>
                                                    <a onclick="HandleAddtoWishList(this)" productIdList=${item.Id} href="#">wishlist</a>
                                                    
                                                </div>
                                                <div class="share-tag clearfix">
                                                    <ul class="blog-share floatleft">
                                                        <li><h5>share </h5></li>
                                                        <li><a href="#"><i class="mdi mdi-facebook"></i></a></li>
                                                        <li><a href="#"><i class="mdi mdi-twitter"></i></a></li>
                                                        <li><a href="#"><i class="mdi mdi-linkedin"></i></a></li>
                                                        <li><a href="#"><i class="mdi mdi-vimeo"></i></a></li>
                                                        <li><a href="#"><i class="mdi mdi-dribbble"></i></a></li>
                                                        <li><a href="#"><i class="mdi mdi-instagram"></i></a></li>
                                                    </ul>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>`;


            $('#quick-view').html(html);
            SetLocalStorage(elem);
        },
        error: function (errorMessage) {
            alert(errorMessage.responseText);
        }
    });
}

//Load Product List by Name

function loadProductListByName() {

    var productName = $('#textProductName').val();

    if (productName != null && productName != undefined && productName != '') {

        $.ajax({
            url: "/Product/GetProductbyName",
            data: { productName: productName },
            type: "GET",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (result) {

                var html = '', htmlProductList = '', htmlProductGrid = '';
                var data = JSON.parse(result.data);
                $('#lblTotalCount').text('Total Records: ' + data.length);
                $.each(data, function (key, item) {

                    //List Html
                    $(item.ProductPrices).each(function (productPricesIndex, productPricesValue) {

                        htmlProductList += `<div class="col - xs - 12">
                            <div class="single-list-view">
                                <div class="row">
                                    <div class="col-xs-12 col-md-4">
                                        <div class="list-img">
                                            <div class="product-img">
                                                <div class="pro-type sell">
                                                    <span>${productPricesValue.Discount}%</span>
                                                </div>
                                                <a href="#"><img src="${item.MasterImageUrl}" alt="Product Title" /></a>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-md-8">
                                        <div class="list-text">
                                            <h3>${item.Name}</h3>
                                            <span>${item.ShortDescription}</span>
                                            <div class="ratting floatright">
                                                <p>( 27 Rating )</p>
                                                <i class="mdi mdi-star"></i>
                                                <i class="mdi mdi-star"></i>
                                                <i class="mdi mdi-star"></i>
                                                <i class="mdi mdi-star-half"></i>
                                                <i class="mdi mdi-star-outline"></i>
                                            </div>
                                            <h5><del>${productPricesValue.Price} PKR</del>&nbsp${productPricesValue.Price * (1 - (productPricesValue.Discount / 100))} PKR</h5 >
                                            <p>${item.LongDescription}</p>
                                            <div class="list-btn">
                                                <a onclick="HandleAddtocart(this)" href="javascript:void(0)" productIdList=${item.Id} >add to cart</a>
                                                <a href="#">wishlist</a>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                             </div >
                           </div >`;
                    });

                    //Grid Html
                    $(item.ProductPrices).each(function (productPricesGIndex, productPricesGValue) {

                        htmlProductGrid += `<div class="col-xs-12 col-sm-6 col-md-4">
                                                <div class="single-product">
                                                    <div class="product-img">
                                                        <div class="pro-type">
                                                      <span>${productPricesGValue.Discount}%</span>
                                                        </div>
                                                        <a href="#"><img src="${item.MasterImageUrl}" alt="Product Title" /></a>
                                                        <div class="actions-btn">
                                                            <a onclick="HandleAddtocart(this)" productIdList=${item.Id}><i  class="mdi mdi-cart"></i></a>
                                                            <a href="#" data-toggle="modal" onClick="LoadQuickView(this)" id="${item.Id}" data-target="#quick-view"><i class="mdi mdi-eye"></i></a>
                                                            <a href="#"><i class="mdi mdi-heart"></i></a>
                                                        </div>
                                                    </div>
                                                    <div class="product-dsc">
                                                        <p><a href="#">${item.Name}</a></p>
                                                        <div class="ratting">
                                                            <i class="mdi mdi-star"></i>
                                                            <i class="mdi mdi-star"></i>
                                                            <i class="mdi mdi-star"></i>
                                                            <i class="mdi mdi-star-half"></i>
                                                            <i class="mdi mdi-star-outline"></i>
                                                        </div>
                                                        <span><del style='color:silver'>${productPricesGValue.Price} PKR</del>&nbsp${productPricesGValue.Price * (1 - (productPricesGValue.Discount / 100))} PKR</span>
                                                    </div>
                                                </div>
                                            </div>`;

                    });

                    html = `<div class="tab-pane fade in text-center" id="grid">
                                  ${htmlProductGrid}
                                  </div>
                                  <div class="tab-pane fade active in" id="list">
                                      ${htmlProductList}
                                  </div>`;

                });

                $('#htmlListAndGrid').html(html);
            },
            error: function (errorMessage) {
                alert(errorMessage.responseText);
            }
        });

    } else {
        loadProductList();
    }
}
//Load Product List by Id
function loadProductListById(filterList) {
    var html = '', htmlProductList = '', htmlProductGrid = '';

    if (filterList != null && filterList != undefined && filterList != '') {

        //var postData = {
        //    filterList: filterList
        //};

        $.ajax({
            url: "/Product/GetProductListbySidebar",
            type: "POST",
            data: JSON.stringify(filterList),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            traditional: true,
            success: function (result) {

                var data = JSON.parse(result.data);
                $('#lblTotalCount').text('Total Records: ' + data.length);
                $.each(data, function (key, item) {

                    //List Html
                    $(item.ProductPrices).each(function (productPricesIndex, productPricesValue) {

                        htmlProductList += `<div class="col - xs - 12">
                            <div class="single-list-view">
                                <div class="row">
                                    <div class="col-xs-12 col-md-4">
                                        <div class="list-img">
                                            <div class="product-img">
                                                <div class="pro-type sell">
                                                    <span>${productPricesValue.Discount}%</span>
                                                </div>
                                                <a href="#"><img src="${item.MasterImageUrl}" alt="Product Title" /></a>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-md-8">
                                        <div class="list-text">
                                            <h3>${item.Name}</h3>
                                            <span>${item.ShortDescription}</span>
                                            <div class="ratting floatright">
                                                <p>( 27 Rating )</p>
                                                <i class="mdi mdi-star"></i>
                                                <i class="mdi mdi-star"></i>
                                                <i class="mdi mdi-star"></i>
                                                <i class="mdi mdi-star-half"></i>
                                                <i class="mdi mdi-star-outline"></i>
                                            </div>
                                            <h5><del>${productPricesValue.Price} PKR</del>&nbsp${productPricesValue.Price * (1 - (productPricesValue.Discount / 100))} PKR</h5 >
                                            <p>${item.LongDescription}</p>
                                            <div class="list-btn">
                                                <a onclick="HandleAddtocart(this)" href="javascript:void(0)" productIdList=${item.Id} >add to cart</a>
                                                <a href="#">wishlist</a>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                             </div >
                           </div >`;
                    });

                    //Grid Html
                    $(item.ProductPrices).each(function (productPricesGIndex, productPricesGValue) {

                        htmlProductGrid += `<div class="col-xs-12 col-sm-6 col-md-4">
                                                <div class="single-product">
                                                    <div class="product-img">
                                                        <div class="pro-type">
                                                      <span>${productPricesGValue.Discount}%</span>
                                                        </div>
                                                        <a href="#"><img src="${item.MasterImageUrl}" alt="Product Title" /></a>
                                                        <div class="actions-btn">
                                                            <a href="#" onclick="HandleAddtocart(this)" productIdList=${item.Id}><i class="mdi mdi-cart"></i></a>
                                                            <a href="#" data-toggle="modal" onClick="LoadQuickView(this)" id="${item.Id}" data-target="#quick-view"><i class="mdi mdi-eye"></i></a>
                                                            <a href="#"><i class="mdi mdi-heart"></i></a>
                                                        </div>
                                                    </div>
                                                    <div class="product-dsc">
                                                        <p><a href="#">${item.Name}</a></p>
                                                        <div class="ratting">
                                                            <i class="mdi mdi-star"></i>
                                                            <i class="mdi mdi-star"></i>
                                                            <i class="mdi mdi-star"></i>
                                                            <i class="mdi mdi-star-half"></i>
                                                            <i class="mdi mdi-star-outline"></i>
                                                        </div>
                                                        <span><del style='color:silver'>${productPricesGValue.Price} PKR</del>&nbsp${productPricesGValue.Price * (1 - (productPricesGValue.Discount / 100))} PKR</span>
                                                    </div>
                                                </div>
                                            </div>`;

                    });

                    html = `<div class="tab-pane fade in text-center" id="grid">
                                  ${htmlProductGrid}
                                  </div>
                                  <div class="tab-pane fade active in" id="list">
                                      ${htmlProductList}
                                  </div>`;

                });

                $('#htmlListAndGrid').html(html);
            },
            error: function (errorMessage) {
                alert(errorMessage.responseText);
            }
        });

    } else {
        loadProductList();
    }
}
// filer Change Event

$('.main-filter-container').on("change", ".filter", function () {

    //var filterList = new Array();

    let filterList = [];

    if ($(this).prop("checked")) {

        //var brandId = $(this).attr('brandId');

        var Count = 0;
        $('.filter').each(function (i, v) {


            //if ($(this).attr('brandId') != undefined) {

            //    if ($(this).prop("checked")) {

            //        filterList[Count] = parseInt($(this).attr('brandId'));
            //        Count += 1;
            //    }

            //}

            //if ($(this).attr('Category') != undefined) {

            if ($(this).prop("checked")) {

                //filterList[Count] = parseInt($(this).attr('brandId'));
                //Count += 1;

                filterList.push({ "Name": $(this).attr('myfilter'), "ID": parseInt($(this).attr('categoryId')) });


            }

            //}

            //if ($(this).attr('categoryId') != undefined) {

            //    if ($(this).prop("checked")) {

            //        filterList[Count] = parseInt($(this).attr('categoryId'));
            //        Count += 1;
            //    }
            //}

        });

        loadProductListById(filterList);

    } else {

        var ifPresent = false;

        $('.filter').each(function (i, v) {
            if ($(this).prop("checked")) {
                ifPresent = true;
                return false;
            }
        });

        if (ifPresent) {

            let filterList_Dummy = [];

            var Count = 0;
            $('.filter').each(function (i, v) {

                if ($(this).prop("checked")) {

                    filterList_Dummy.push({ "Name": $(this).attr('myfilter'), "ID": parseInt($(this).attr('categoryId')) });
                }

            });

            loadProductListById(filterList_Dummy);
        }
        else {
            loadProductList();
        }
    }
});

//Load Data function

function loadFeatureProduct() {
    $.ajax({
        url: "/Product/GetFeaturedProduct",
        type: "GET",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            var html = '';
            var data = JSON.parse(result.data);

            $.each(data, function (key, item) {


                $(item.ProductPrices).each(function (productPriceIndex, productPriceValue) {

                    html += `<li>
                    <div class="text-center">
                        <div class="col-xs-12 col-sm-6 col-md-3">
                            <div class="single-product">
                                <div class="product-img">
                                    <div class="pro-type">
                                        <span>${productPriceValue.Discount}%</span>
                                    </div>
                                    <a href="/ProductDetails/Index?productId=${item.Id}" onClick="SetLocalStorage(this)" productId="${item.Id}" productName="${item.Name}" productImg="${item.MasterImageUrl}"><img src="${item.MasterImageUrl}" alt="Product Title" /></a>
                                    <div class="actions-btn">
                                        <a onclick="HandleAddtocart(this)" productIdList=${item.Id}  ><i class="mdi mdi-cart"></i></a>
                                        <a href="#" data-toggle="modal" onClick="LoadQuickView(this)" id="${item.Id}" productId="${item.Id}" productName="${item.Name}" productImg="${item.MasterImageUrl}" data-target="#quick-view"><i class="mdi mdi-eye"></i></a>
                                        <a onclick="HandleAddtoWishList(this)" productIdList=${item.Id}><i class="mdi mdi-heart"></i></a>
                                    </div>
                                </div>
                                <div class="product-dsc">
                                    <p><a href="/ProductDetails/Index?productId=${item.Id}" productId="${item.Id}" productName="${item.Name}" productImg="${item.MasterImageUrl}">${item.Name}</a></p>
                                    <span><del style='color: silver'>${productPriceValue.Price}PKR</del>&nbsp${productPriceValue.Price * (1 - (productPriceValue.Discount / 100))} PKR</span>
                                </div>
                            </div>
                        </div>
                        </div></div>
                </li>`;

                });




            });
            $('#ulLoadFeatureProduct').html(html);
        },
        error: function (errorMessage) {
            alert(errorMessage.responseText);
        }
    });
}
function loadRecentViewProduct() {

    if (localStorage.length > 0) {
        var html = '';
        //var index = 0;
        var id = JSON.parse(localStorage.Id);
        var name = JSON.parse(localStorage.Name);
        var img = JSON.parse(localStorage.Img);

        // $(id).each(function () {
        for (var index = id.length - 1; index >= 0; index--) {


            html += `<li>
                    <div class="text-center">
                        <div class="col-xs-12 col-sm-6 col-md-3">
                            <div class="single-product">
                                <div class="product-img">
                                    <div class="pro-type">
                                        <span></span>
                                    </div>
                                    <a href="/ProductDetails/Index?productId=${id[index]}"><img src="${img[index]}" alt="Product Title" /></a>
                                    <div class="actions-btn">
                                        <a onclick="HandleAddtocart(this)" href="javascript:void(0)" productIdList=${id[index]}><i class="mdi mdi-cart"></i></a>
                                        <a href="#" data-toggle="modal" id="${id[index]}" data-target="#quick-view"> <i class="mdi mdi-eye"></i></a>
                                        <a onclick="HandleAddtoWishList(this)" productIdList=${id[index]} href="#"><i class="mdi mdi-heart"></i></a>

                                    </div>
                                </div>
                                <div class="product-dsc">
                                    <p><a href="/ProductDetails/Index?productId=${id[index]}">${name[index]}</a></p>
                                    
                                </div>
                            </div>
                        </div>
                        </div></div>
                </li>`;

            //index += 1;
        }
        // });

        $('#ulLoadRecentViewProduct').html(html);


    }
}
//---------------Product management----------------
//---------------Toggle----------------------------
function toggle(className, obj) {

    if (obj.checked) $(className).show();
    else $(className).hide();
}

// Comment And Rating Work ------------------



$("#btnSave").click(function (e) {

    // if (validateFileds()) {




    e.preventDefault();



    let urlstr = location.href;
    let url = new URL(urlstr);
    let searchparams = url.searchParams;
    var productId = searchparams.get('productId');



    var commentAndRating = {




        CustomerId: $("#UserId").val(),
        AnonymousName: $('#txtName').val(),
        EmailId: $('#txtEmail').val(),
        Comment: $('#txtMessage').val(),
        FK_ProductMaster: productId,
        rate: rating,



    }



    $.ajax({
        type: "POST",
        url: "/ProductDetails/SaveComment",
        data: JSON.stringify(commentAndRating),
        contentType: 'application/json',
        dataType: 'json',
        success: function (data) {



            if (data.isSuccessful == true) {
                toastr.success("Your Reviews has been Updated.");
                GetProductCommentAndRating();
            }
            else if (data.isSuccessful == false) {
                toastr.error("Found Issue,Please Check your Data.");
            }
        },
        error: function (data) {
            toastr.error("Found Issue, Please Check your Data.");
        },



    });
    // }
});



// validate Comment Section
//function validateFileds() {



// if ($('#txtMessage').val() != null || $('#txtMessage').val() != undefined || $('#txtMessage').val() != '') {
// $('#lblMessage').text('Please Insert a Review.')
// return false;
// }
// else {
// $('#lblMessage').text('');
// return true;
// }



//}



function GetProductCommentAndRating() {



    let urlstr = location.href;
    let url = new URL(urlstr);
    let searchparams = url.searchParams;



    var productId = searchparams.get('productId');



    if (productId != null && productId != undefined) {
        var html = ''; htlmRating = '';

        $.ajax({
            url: '/ProductDetails/GetProductCommentbyId',
            type: "Get",
            data: { id: productId },
            success: function (result) {



                var item = JSON.parse(result.data);

                $(item).each(function (index, value) {



                    htlmRating = '';

                    var rating = item[index].CustomerRate;
                    var remainingRating = 5 - item[index].CustomerRate;



                    for (var i = 0; i < rating; i++) {

                        htlmRating += ` <i class="mdi mdi-star"></i>`;
                    }
                    for (var i = 0; i < remainingRating; i++) {



                        htlmRating += `<i class="mdi mdi-star-outline"></i>`;
                    }





                    html += `<hr/><div class="autohr-text">
<img src="#" alt="" />



<div class="author-des">
<h4><a href="#">${item[index].CustomerName == null ? 'Anonymous' : item[index].CustomerName}</a></h4>
<span class="floatright ratting" id="assign-rating">
${htlmRating}
</span>
<span>${moment(item[index].CommentDate).format('MMMM Do YYYY, h:mm A')}</span>
<p>${item[index].CustomerComment}</p>
</div>

</div>`;
                });

                $('#commentAndRating').html(html);

            },
            error: function (errorMessage) {
                alert(errorMessage.responseText);
            }
        });



    }



}