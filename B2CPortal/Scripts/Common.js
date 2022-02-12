var rating = 0;
var totalComment = 0;
var totalProductList = 0;
var PrevClickValue = 0;
var NextClickValue = 10;
var PrevProdClickValue = 0;
var NextProdClickValue = 10;
var pricesymbol = "pricesymbol";
var userid = "userid";
var UserName = "username";
var UserEmail = "useremail";

window.onload = function () {
 /*   var divToHide = document.getElementById('quick-view');*/
    document.onclick = function (e) {
        $(e.target).find('.modal-content').remove();
        //$('body').addClass('body-pading-right');
        //$('.quick-view .modal-dialog .modal-content').find('.row').remove();
    };
    $.ajax({
        type: "GET",
        url: "/Home/GetCityList",
        data: {
        },
        success: function (data) {
            var html = '';
            data.data.map(function (item, index) {
                if (item.Name == "Karachi") {
                    html += `<option ${item.Selected == true ? "Selected": ""} value="${item.Id}">${item.Name}</option>`;
                }
            });
            $('#handlecity').html(html);
        }
    });
};
$(document).ready(function () {
    ShowCartProducts();
    let customerid = getCookie(userid);
    var accounthtml = '';
    if (parseInt(customerid) > 0) {
        var username = getCookie(UserName);
        $('#logedin').html();
        accounthtml += `     
                    <i style="font-size: 1.73em; " class="mdi mdi-account"></i> <br>
                    <b style="font-size:12px">Hi, ${username}</b>
                    <ul>
                        <li><a href="/Account/MyAccount">My Account</a></li>
                        <li><a href="/Account/LogOut">Logout</a></li>

                        </ul>
                    `;
        $('#logedin').html(accounthtml);
    } else {
        $('#register').html();

        accounthtml += `    
                                            <a href="#"><i style="font-size: 1.73em;" class="mdi mdi-account"></i></a>

                                            <ul class="log">
                                                <li><a href="/Account/Login">Login</a></li>
                                                <li><a href="/Account/Login">Register</a></li>
                                            </ul>
                                       `;
        $('#register').html(accounthtml);

    }

    var username = getCookie(UserName);

    $.fn.digits = function () {
        return this.each(function () {
            var num = $(this).text();
            var symbolvalue = GetCookieByName(pricesymbol);
            if (symbolvalue.toLowerCase() == "pkr") {
                var amountnumber = parseFloat(num).toFixed(2);
                $(this).text(amountnumber.replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1,"));
            } else {
                //$(this).text($(this).text().replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$,"));
            }
        })
    }

    //---------------------------handle plus minus change ---------------
    $(document).on('keyup', '.plus-minus-box', function () {
        if (this.value > 11) {
            this.value = 10;
            return false
        }
        this.value = this.value.replace(/[^0-9\.]/g, '');
    });
    $(document).on('change', 'input[name=qtybuttonquickview]', function () {
        var $button = $(this);
        var oldValue = $button.parent().find("input").val();
        newVal = oldValue == "0" || parseFloat(oldValue) <= 0 || oldValue == "" ? 1 : $button.parent().find("input").val();
        if (parseFloat(newVal) <= 11) {
            var price = parseFloat($('#discoountedprice').text());
            let totalvalue = (price * newVal);
            $('#labalprice').text(totalvalue.toLocaleString());
            $button.parent().find("input").val(newVal);
        } else {
            return false;
        }
    });
    $(document).on('change', 'input[name=qtybutton]', function () {
        var $button = $(this);
        var oldValue = $(this).val();
        newVal = oldValue == "0" || parseFloat(oldValue) <= 0 || oldValue == "" ? 1 : $button.parent().find("input").val();
        var row = $(this).closest("tr");
        var Discount = parseFloat($(row).find('td')[1]?.textContent);
        if (!isNaN(Discount)) {

            var price = parseFloat($(row).find('td')[2].textContent);
            let actualTotal = (price * newVal);

            price = price * (1 - (Discount / 100));
            let totalvalue = (price * newVal);

            DiscountAmount = actualTotal - totalvalue;

            $(row).find('td')[4].textContent = actualTotal.toLocaleString();
            $(row).find('td')[5].textContent = DiscountAmount.toLocaleString();
            $(row).find('td')[6].textContent = totalvalue.toLocaleString();
            $button.parent().find("input").val(newVal);
        } else {
            $button.parent().find("input").val(newVal);
        }
    });

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
        if (parseFloat(newVal) <= 10) {
            var price = parseFloat($('#discoountedprice').text());
            let totalvalue = (price * newVal);
            $('#labalprice').text(totalvalue.toLocaleString());
            $button.parent().find("input").val(newVal);
        } else {
            return false;
        }
    });
    $(document).on('click', '.qtybutton', function () {
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
        var Discount = parseFloat($(row).find('td')[1]?.textContent);
        if (parseFloat(newVal) <= 10 && !isNaN(Discount)) {
            var price = parseFloat($(row).find('td')[2].textContent);
            let actualTotal = (price * newVal);

            price = price * (1 - (Discount / 100));
            let totalvalue = (price * newVal);

            DiscountAmount = actualTotal - totalvalue;

            $(row).find('td')[4].textContent = actualTotal.toLocaleString();
            $(row).find('td')[5].textContent = DiscountAmount.toLocaleString();
            $(row).find('td')[6].textContent = totalvalue.toLocaleString();

            $button.parent().find("input").val(newVal);
        } else {
            return false;
        }
    });
    //---------------------------handle plus minus change ---------------
    $('.nav-view').on('click', '.my-list-grid-btn', function () {
        localStorage.removeItem('my-list-grid-btn');
        localStorage.setItem("my-list-grid-btn", $(this).attr("mylistgridbtn"));
    });
    //----------------------on change city --------------------

    $(document).on('change', '#handlecity', function () {
        var id = $("#handlecity").val();
        if (id == 0) {
            window.location.reload();
        }
        $.ajax({
            type: "POST",
            url: "/Home/ChangeCity",
            data: {
                id: id
            },
            success: function (data) {
                toastr.success(data.msg);
                window.location.reload();
            }
        });
    });

    //========================emil subscription========================
    $(document).on('click', '#handlingsubscribe', function () {
        var emailid = $("#subemail").val();
        debugger
        if (emailid) {
            $.ajax({
                type: "POST",
                url: "/Account/EmailSubscription",
                data: {
                    emailid: emailid
                },
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.msg);
                       $("#subemail").val("");

                    } else {
                        toastr.error(data.msg);
                    }
                }
            });
        }

    });
});
///========================search with autocomplete========================================
function autocomplete(inp, arr) {
    var currentFocus;
    inp.addEventListener("input", function (e) {
        var a, b, i, val = this.value;
        /*close any already open lists of autocompleted values*/
        closeAllLists();
        if (val.length <= 2) { return false; }
        currentFocus = -1;
        /*create a DIV element that will contain the items (values):*/
        a = document.createElement("DIV");
        a.setAttribute("id", this.id + "autocomplete-list");
        a.setAttribute("class", "autocomplete-items");
        /*append the DIV element as a child of the autocomplete container:*/
        this.parentNode.appendChild(a);
        //***********************************************************
        $.ajax({
            url: '/Product/SearchProductList',
            type: "POST",
            dataType: "json",
            data: { Prefix: val },
            success: function (data) {
                if (data.length <= 0) {
                    $('#handlesearchautocomplete-list').remove();
                    return false;
                }
                $(data).map((index, item) => {
                    b = document.createElement("DIV");
                    b.innerHTML = "<img class='searchimg' src='" + item.MasterImageUrl + "' />";
                    b.innerHTML += "<strong class='searchnametxt'>" + item.Name + "</strong>";
                    //b.innerHTML += "| <strong style='color:red'>" + item.Price + " <strong class='pricesymbol'> </strong> </strong>";
                    b.innerHTML += "<input  type='hidden' value='" + item.Id + "'>";
                    b.addEventListener("click", function (e) {

                        var id = $('input[type = hidden]').val();
                        var name = $('.searchnametxt').text();
                        inp.value = $(this).find('.searchnametxt').text();
                        window.location.href = '/ProductDetails?productId=' + id + ''
                        closeAllLists();
                    });
                    a.appendChild(b);
                    //}
                    //}
                });
            }
        });
        //***********************************************************
    });
    inp.addEventListener("keydown", function (e) {
        var x = document.getElementById(this.id + "autocomplete-list");
        if (x) x = x.getElementsByTagName("div");
        if (e.keyCode == 40) {
            /*If the arrow DOWN key is pressed,
            increase the currentFocus variable:*/
            currentFocus++;
            /*and and make the current item more visible:*/
            addActive(x);
        } else if (e.keyCode == 38) { //up
            /*If the arrow UP key is pressed,
            decrease the currentFocus variable:*/
            currentFocus--;
            /*and and make the current item more visible:*/
            addActive(x);
        } else if (e.keyCode == 13) {
            /*If the ENTER key is pressed, prevent the form from being submitted,*/
            e.preventDefault();
            if (currentFocus > -1) {
                /*and simulate a click on the "active" item:*/
                if (x) x[currentFocus].click();
            }
        }
    });
    function addActive(x) {
        /*a function to classify an item as "active":*/
        if (!x) return false;
        /*start by removing the "active" class on all items:*/
        removeActive(x);
        if (currentFocus >= x.length) currentFocus = 0;
        if (currentFocus < 0) currentFocus = (x.length - 1);
        /*add class "autocomplete-active":*/
        x[currentFocus].classList.add("autocomplete-active");
    }
    function removeActive(x) {
        /*a function to remove the "active" class from all autocomplete items:*/
        for (var i = 0; i < x.length; i++) {
            x[i].classList.remove("autocomplete-active");
        }
    }
    function closeAllLists(elmnt) {
        /*close all autocomplete lists in the document,
            except the one passed as an argument:*/
        var x = document.getElementsByClassName("autocomplete-items");
        for (var i = 0; i < x.length; i++) {
            if (elmnt != x[i] && elmnt != inp) {
                x[i].parentNode.removeChild(x[i]);
            }
        }
    }
    /*execute a function when someone clicks in the document:*/
    document.addEventListener("click", function (e) {
        closeAllLists(e.target);
    });
}
autocomplete(document.getElementById("handlesearch"), "");
///========================search with autocomplete=========================================
//=====================On hover cart list========================
function removequickviewvalues(id) {
    $('body').addClass('body-pading-right');
    $('.quick-view .modal-dialog .modal-content').find('.row').remove();
}
//=====================On hover cart list========================
function ShowCartProducts() {
    document.getElementsByClassName("loader-container")[0].style.display = "block";
    $.ajax({
        type: "POST",
        url: "/Product/GetCartCount",
        data: {},
        success: function (data) {
            var html = "<div class='cartdrop-sin-container'>";
            let dataobj = JSON.parse(data.data);
            dataobj.cartproducts.map(function (item, index) {
                html += ` <div class="sin-itme clearfix">
<i onclick="RemoveCartProduct(${item.Id})" class="mdi mdi-close removecartbtn"></i>
<a href="/ProductDetails?productId=${item.FK_ProductMaster}" class="cart-img" href="cart.html"><img src="${item.MasterImageUrl}" alt="" /></a>
<div class="menu-cart-text">
<a href="/ProductDetails?productId=${item.FK_ProductMaster}"><h5>${item.Name} ${item.Packsize}</h5></a>
<span>Quantity: ${item.Quantity}</span>
<strong class="pricesymbol_numbers"> <strong class="pricesymbol"> </strong> <span class="numbers"> ${item.TotalPrice} </span></strong>
</div>
</div> `;
            });
            html += `</div>
<div class="totalPriceDetails">
<div class="total">
<span>total <strong>= <strong class="pricesymbol"> </strong ><span class="numbers"> ${dataobj.totalprice}</span></strong></span>
</div>
<a class="goto" href="/Product/AddToCart"> go to cart</a>
<a class="out-menu" href="/Orders/Checkout">Check out</a>
</div>
`;
            $('#cartdrop').html(html);
            $(".numbers").digits();
            $('#productaddtocart').html(dataobj.cartproductscount);
            $('#totalprice').html(dataobj.totalprice.toLocaleString("en-US"));
            var symbolvalue = GetCookieByName(pricesymbol);
            $('.pricesymbol').text(symbolvalue);
            document.getElementsByClassName("loader-container")[0].style.display = "none";
            
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
            if (data.success == true) {
                ShowCartProducts();
                toastr.success(data.msg);
                var tb = $('#myTable tbody');
                if (tb.find("tr").length == 1) {
                    $('#cartmanagebtn').replaceWith(`
                    <div class="text-right">
                            <a class="btn btn-primary" href="/Product/ProductList">Shop Now</a>
                    </div>
                `);
                }
                tb.find("tr").each(function (index, element) {
                    var trvalue = parseInt($(element).attr('data-id'));
                    if (trvalue == parseInt(id)) {
                        $(element).remove();
                    }
                });
                //if checkout remove cart products-----
                if (window.location.href.indexOf("Orders/Checkout") > -1) {
                    window.location.reload();
                }
            } else {
                toastr.error(data.msg);
            }

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
            if (data.success) {
                ShowCartProducts();
                toastr.success(data.msg);
            } else {
                toastr.error(data.msg);
            }

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
        success: function (result) {
            var html = '';
            var htmlProductPriceDetail = '', htmlProductSize = '';
            var data = result.data;
            $(data).each(function (index, item) {
                htmlProductPriceDetail += ` <tr data-id="58">
                                        <td class="td-img text-left">
                                            <a href="/ProductDetails?productId=${item.FK_ProductMaster}">
<img src="${item.MasterImageUrl}" alt="Add Product"/>
</a>
                                            <div class="items-dsc">
                                                <h5>
                                                         ${item.Name}
                                                
                                                </h5>

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
                                            ${moment(item.Date).format('DD-MMM-yyyy')}
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
                                <th> (<strong class="pricesymbol"> </strong>)Price</th>
                                <th>Quantity</th>
                                <th> (<strong class="pricesymbol"></strong>)Sub Total</th>
                                <th> (<strong class="pricesymbol"> </strong>)Discounted Amount</th>
                                <th>(<strong class="pricesymbol"> </strong>)Total</th>
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
            var symbolvalue = GetCookieByName(pricesymbol);
            $('.pricesymbol').text(symbolvalue);
        },
        error: function (errorMessage) {
            alert(errorMessage.responseText);
        }
    });
}
//Plus and minus quentiry------------>
//---------------cart count and hover list----------------
//---------------cart Wishlist ----------------
function HandleAddtoWishList(id) {
    var proid = $(id).attr("productIdList");
    var quentity = $('#quentityvalue').val();
    //var guid = $.cookie("cartguid");
    //  var guid = document.cookie.split('=')[1];
    $.ajax({
        type: "POST",
        url: "/Product/AddWishlistProduct",
        data: {
            id: proid,
            quentity: quentity,
            // guid: guid
        },
        success: function (data) {
            if (data.success) {
                ShowCartProducts();
                toastr.success(data.msg);
            } else {
                toastr.error(data.msg);
            }
        }
    });

}
//---------------cart Wishlist ----------------
//---------------Product management----------------
//Load Product List
function loadProductList() {
    alert();
    var htmldata = '', htmlProductList = '', htmlProductGrid = '';
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
<h5><del class="numbers">${productPricesValue.Price} <strong class="pricesymbol"> </strong></del>&nbsp<span class="numbers">${productPricesValue.Price * (1 - (productPricesValue.Discount / 100))} </span><strong class="pricesymbol"> </strong></h5 >
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





                    htmlProductGrid += `<div class="col-xs-12 col-sm-6 col-md-4">
<div class="single-product">
<div class="product-img">
<div class="pro-type">
<span>${productPricesValue.Discount}%</span>
</div>
<a href="/ProductDetails/Index?productId=${item.Id}" productId="${item.Id}" productName="${item.Name}" productImg="${item.MasterImageUrl}" onClick="SetLocalStorage(this)"><img src="${item.MasterImageUrl}" alt="Product Title" /></a>
<div class="actions-btn">
<a onclick="HandleAddtocart(this)" href="javascript:void(0)" productIdList=${item.Id}><i class="mdi mdi-cart"></i></a>
<a href="javascript:void(0)" data-toggle="modal" onClick="LoadQuickViewWithRating(this)" productId="${item.Id}" productName="${item.Name}" productImg="${item.MasterImageUrl}" onClick="LoadQuickViewWithRating(this)" id="${item.Id}" data-target="#quick-view"><i class="mdi mdi-eye"></i></a>
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
<span>
${item.Discount > 0 ?
                        `<del style='color:silver'>
                        <span class="numbers">${productPricesValue.Price}</span> <strong class="pricesymbol"> </strong>
                        </del>`: ""}

&nbsp<span class="numbers">${productPricesValue.Price * (1 - (productPricesValue.Discount / 100))}</span> <strong class="pricesymbol"> </strong></span>
</div>
</div>
</div>`;
                });





            });



            var gl1 = ""; var gl2 = "";





            var activeclassfor = localStorage.getItem("my-list-grid-btn");
            switch (activeclassfor) {
                case "list":
                    gl1 = "active";
                    gl2 = "";
                    break;
                case "grid":
                    gl1 = "";
                    gl2 = "active";
                    break;
                default:
                    gl1 = "";
                    gl2 = "active";
                    break;
            }





            htmldata = `<div class="tab-pane fade in text-center ${gl2}" id="grid">
${htmlProductGrid}
</div>
<div class="tab-pane fade in ${gl1}" id="list">
${htmlProductList}
</div>`;





            $('#htmlListAndGrid').html(htmldata);
            $(".numbers").digits();
            var symbolvalue = GetCookieByName(pricesymbol);
            $('.pricesymbol').text(symbolvalue);
            //document.getElementsByClassName("pricesymbol").innerHTML = pricesymbol.symbol;
        },
        error: function (errorMessage) {
            alert(errorMessage.responseText);
        }
    });
}
function LoadQuickViewWithRating(elem) {
    var Id = elem.id;
    $.ajax({
        url: '/Product/GetProductbyIdWithRating?Id=' + Id + '',
        type: "GET",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {

            var html = '';
            var index = 1;
            var fade = 2;
            var productPrice = 0;
            var productDiscount = 0;
            var htmlProductDetail = '';
            var htmlProductPriceDetail = '';
            var htmlProductSize = '';

            var item = JSON.parse(result.data);

            $(item).each(function (i, v) {

                productPrice = item[i].Price;
                Discount = item[i].Discount;
                productDiscount = item[i].DiscountedAmount;
            });

            for (i = 0; i <= item.length; i++) {
                
                if (item[0].ImageUrl2[i] != undefined) {
                    if (i == 1) {
                        htmlProductDetail += `<li class="detail-image active"><a data-toggle="tab" href="#sin-${index}"> <img src="${item[0].ImageUrl2[i]}" alt="quick view" /> </a></li>`;
                    } else {
                        htmlProductDetail += `<li class="detail-image"><a data-toggle="tab" href="#sin-${index}"> <img src="${item[0].ImageUrl2[i]}" alt="quick view" /> </a></li>`;
                    }
                    index += 1;
                }

            }
            $(item).each(function (i, v) {

                if (i == 1) {
                    htmlProductDetail += `<li class="active"><a data-toggle="tab" href="#sin-${index}"> <img src="${item[i].ImageUrl}" alt="quick view" /> </a></li>`
                }
                else {
                    htmlProductDetail += `<li><a data-toggle="tab" href="#sin-${index}"> <img src="${item[i].ImageUrl}" alt="quick view" /> </a></li>`
                }
                index += 1;

            });

            $(item).each(function (i, v) {





                if (i == 0) {
                    htmlProductPriceDetail += `<div class="simpleLens-container tab-pane active fade in" id="sin-${fade}">
<div class="pro-type">
<span id="discountedvalue">${Discount} </span>%
</div>
<a class="simpleLens-image" data-lens-image="${item[i].ImageUrl}" href="#"><img src="${item[i].ImageUrl}" alt="" class="simpleLens-big-image"></a>
</div>`
                }
                else {
                    htmlProductPriceDetail += `<div class="simpleLens-container tab-pane fade in" id="sin-${fade}">
<div class="pro-type">
<span>${productDiscount}%</span>
</div>
<a class="simpleLens-image" data-lens-image="${item[i].ImageUrl}" href="#"><img src="${item[i].ImageUrl}" alt="" class="simpleLens-big-image"></a>
</div>`
                }





                fade += 1;
            });

            $(item).each(function (i, v) {


                htmlProductSize += `<option value="${item[i].Id}"></option><h3>${item[i].Name}&nbsp;${item[i].UOM} </h3>
<span>${item[i].ShortDescription}</span>
<div class="ratting floatright">
<p>( ${item[i].TotalRating} Rating )</p>
${GetProductRating(item[i].AvgRating)}`;
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
${htmlProductSize}
</div>
<h5>
${Discount > 0 ?
                `<del>
<strong class="pricesymbol"> </strong> ${productPrice == undefined ? 0 : productPrice}
</del>`: ""}


<labal style="color:gray"> ${Discount == undefined ? 0 : Discount}% </labal> <strong class="pricesymbol"> </strong> <b id="discoountedprice"> ${productDiscount} </b>  </h5>
<p>${item[0].LongDescription}</p>
<div class="plus-minus">
<a class="dec qtybuttonquickview qtybutton">-</a>
<input type="text" value="1" name="qtybuttonquickview" id="quentityvalue" class="plus-minus-box">
<a class="inc qtybuttonquickview qtybutton">+</a>
</div>
<strong style="font-size:18px"> <strong class="pricesymbol"> </strong>. <labal id="labalprice"><span class="numbers"> ${productDiscount}</span></labal></strong>
</div>
<div class="list-btn">
<a onclick="HandleAddtocart(this)" productIdList=${item[0].Id} >add to cart</a>
<a onclick="HandleAddtoWishList(this)" productIdList=${item[0].Id} >wishlist</a>



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
            $(".numbers").digits();



            SetLocalStorage(elem);
            var symbolvalue = GetCookieByName(pricesymbol);
            $('.pricesymbol').text(symbolvalue);
        },
        error: function (errorMessage) {
            alert(errorMessage.responseText);
        }
    });
}
//Single Function Using ..on Load ,Filtering, Paggination,Search
function loadProductListById(filterList, search, nextPage = 10, prevPage = 0) {
    var htmldata = '';
    var htmlProductList = '';
    var countBtn = 1;
    var htmlPrevButton = '';
    var htmlNextButton = '';
    var htmlbtn = '';
    var htmlProductGrid = '';

    var postData = {
        filterList: filterList,
        search: search,
        nextPage: nextPage,
        prevPage: prevPage
    };

    $.ajax({

        url: "/Product/GetProductListbySidebar",
        type: "POST",
        data: JSON.stringify(postData),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        traditional: true,
        success: function (result) {
            var data = JSON.parse(result.data);
            if (data.length <= 0)
                totalProductList = 0;
            else
                totalProductList = data[0].totalProduct;

            $('#lblTotalCount').text('Total Records: ' + totalProductList);
            $.each(data, function (key, item) {

                htmlProductList += `<div class="col - xs - 12">
<div class="single-list-view">
<div class="row">
<div class="col-xs-12 col-md-4">
<div class="listinge list-img">
<div class="product-img">
<div class="pro-type sell">
<span>${item.Discount}%</span>
</div>
<a href="/ProductDetails/Index?productId=${item.Id}" onClick="SetLocalStorage(this)" productId="${item.Id}" productName="${item.Name}" productImg="${item.MasterImageUrl}"><img src="${item.MasterImageUrl}" alt="Product Title" /></a>
</div>
</div>
</div>
<div class="col-xs-12 col-md-8">
<div class="listingtext list-text">
<h3>${item.Name}&nbsp;${item.UOM}</h3>
<span>${item.ShortDescription}</span>
<div class="ratting floatright">
<p>( ${item.TotalRating} Rating )</p>
${GetProductRating(item.AvgRating)}
</div>
<h5>
${item.Discount > 0 ?
                    `<del>
<strong class="pricesymbol"> </strong> <span class="numbers">${item.Price}</span>
</del>`: ""}

&nbsp <strong class="pricesymbol"></strong> <span class="numbers"> ${item.DiscountedAmount}</span> </h5>
<p></p>
<div class="list-btn">
<a onclick="HandleAddtocart(this)" href="javascript:void(0)" productIdList=${item.Id} >add to cart</a>
<a onclick="HandleAddtoWishList(this)" href="javascript:void(0)" productIdList=${item.Id}>wishlist</a>
</div>
</div>
</div>
</div>
</div >
</div >`;



                htmlProductGrid += `<div class="col-xs-12 col-sm-6 col-md-3">
<div class="single-product">
<div class="product-img">
<div class="pro-type">
<span>${item.Discount}%</span>
</div>
<a href="/ProductDetails/Index?productId=${item.Id}" productName="${item.Name}" onClick="SetLocalStorage(this)" productId="${item.Id}" productImg="${item.MasterImageUrl}"><img src="${item.MasterImageUrl}" alt="Product Title" /></a>
<div class="actions-btn">
<a onclick="HandleAddtocart(this)" href="javascript:void(0)" productIdList=${item.Id}><i class="mdi mdi-cart"></i></a>
<a href="javascript:void(0)" data-toggle="modal" onClick="LoadQuickViewWithRating(this)" productId="${item.Id}" productName="${item.Name}" productImg="${item.MasterImageUrl}" onClick="LoadQuickViewWithRating(this)" id="${item.Id}" data-target="#quick-view"><i class="mdi mdi-eye"></i></a>
<a onclick="HandleAddtoWishList(this)" productIdList=${item.Id} href="javascript:void(0)"><i class="mdi mdi-heart"></i></a>
</div>
</div>
<div class="product-dsc">
<p><a href="/ProductDetails/Index?productId=${item.Id}" productName="${item.Name}" onClick="SetLocalStorage(this)" productId="${item.Id}" productImg="${item.MasterImageUrl}">${item.Name}&nbsp;${item.UOM}</a></p>
<div class="ratting">
${GetProductRating(item.AvgRating)}
</div>
<span>
${item.Discount > 0?
                    `<del style='color:silver'>
                        <strong class="pricesymbol"></strong><span class="numbers">${item.Price}</span>
                    </del>`:""}
 &nbsp </span><strong class="pricesymbol"></strong><span class="numbers">${item.DiscountedAmount} </span>
</div>
</div>
</div>`;
            });


            if (totalProductList <= 10 || totalProductList == undefined) {


                countBtn = 1;
            } else {



                countBtn = totalProductList / 10;
                countBtn = Math.ceil(countBtn);
            }



            htmlPrevButton = `<span> <input type="button" Class="btn-Prod-PrevAndNext" value="Previous" class="btn-Prod-Paggination" /></span>`;
            htmlNextButton = `<span> <input type="button" Class="btn-Prod-PrevAndNext" value="Next" class="btn-Prod-Paggination" /></span>`;



            let prevCount = 0;
            let oprevPage = prevPage;
            for (i = 1; i <= countBtn; i++) {



                prevPage = parseInt(prevCount) * 10;
                htmlbtn += `<span> <input type="button" value="${i}" class="btn-Prod-Paggination" next-page="${nextPage}" prev-page="${prevPage}" /></span>`;
                prevCount += 1;



            }



            var gl1 = ""; var gl2 = "";
            var activeclassfor = localStorage.getItem("my-list-grid-btn");



            switch (activeclassfor) {
                case "list":
                    gl1 = "active";
                    gl2 = "";
                    break;
                case "grid":
                    gl1 = "";
                    gl2 = "active";
                    break;
                default:
                    gl1 = "";
                    gl2 = "active";
                    break;
            }



            htmldata = `<div class="tab-pane fade in text-center ${gl2}" id="grid">
${htmlProductGrid}
</div>
<div class="tab-pane fade in ${gl1}" id="list">
${htmlProductList}
</div>`;



            let htmlPaggination = htmlPrevButton + htmlbtn + htmlNextButton;



            $('#htmlListAndGrid').html(htmldata);
            $('#htmlProdListPaggination').html(htmlPaggination);




            $('#htmlProdListPaggination').find('.btn-Prod-Paggination').eq(((oprevPage / nextPage))).addClass('btn-Prod-Paggination-active');



            var symbolvalue = GetCookieByName(pricesymbol);
            $('.pricesymbol').text(symbolvalue);
            if (filterList != null && filterList != undefined && filterList != '[[]]' && filterList.length > 0) {



                $.each(filterList, function (index, value) {



                    var filterListslides = document.getElementsByClassName("filter");



                    for (var i = 0; i < filterListslides.length; i++) {



                        if ($(filterListslides.item(i)).attr("myfilter") == value.Name && $(filterListslides.item(i)).attr("categoryid") == value.ID) {
                            $(filterListslides.item(i)).prop("checked", true);
                        }



                    }



                });
            }
        },
        error: function (errorMessage) {
            alert(errorMessage.responseText);
        }
    });



}
//updated 1-Dec-2021

$('.main-filter-container').on("change", ".filter", function () {


    var filterCategoryAndBrand = [];
    GetfilterListDummy();

    var filterSearchByName = localStorage.getItem("filterSearchByName") == undefined ? "" : localStorage.getItem("filterSearchByName");
    var filterPrevpage = localStorage.getItem("filterPrevpage") == undefined ? 0 : localStorage.getItem("filterPrevpage");
    var filterNextPage = localStorage.getItem("filterNextPage") == undefined ? 10 : localStorage.getItem("filterNextPage");

    filterCategoryAndBrand = localStorage.getItem("categoryAndBrand");

    //Merge to . 4-Dec-2021
    var filterCategoryAndBrandarray;

    if (filterCategoryAndBrand != '[[]]' && filterCategoryAndBrand.length > 0) {

        // if (typeof (filterList)=='string')
        var filterCategoryAndBrandList = filterCategoryAndBrand.substring(1, filterCategoryAndBrand.length - 1);
        filterCategoryAndBrandarray = JSON.parse(filterCategoryAndBrandList);

    }

    loadProductListById(filterCategoryAndBrandarray, filterSearchByName, 10, 0);


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
                html += `<li>
<div class="text-center">
<div class="col-xs-12 col-sm-6 col-md-3">
<div class="single-product">
<div class="product-img">
<div class="pro-type">
<span>${item.Discount}%</span>
</div>
<a href="/ProductDetails/Index?productId=${item.Id}" onClick="SetLocalStorage(this)" productId="${item.Id}" productName="${item.Name}" productImg="${item.MasterImageUrl}"><img src="${item.MasterImageUrl}" alt="Product Title" /></a>
<div class="actions-btn">
<a onclick="HandleAddtocart(this)" productIdList=${item.Id} ><i class="mdi mdi-cart"></i></a>
<a href="#" data-toggle="modal" onClick="LoadQuickViewWithRating(this)" id="${item.Id}" productId="${item.Id}" productName="${item.Name}" productImg="${item.MasterImageUrl}" data-target="#quick-view"><i class="mdi mdi-eye"></i></a>
<a onclick="HandleAddtoWishList(this)" productIdList=${item.Id}><i class="mdi mdi-heart"></i></a>
</div>
</div>
<div class="product-dsc">
<p><a href="/ProductDetails/Index?productId=${item.Id}" productId="${item.Id}" productName="${item.Name}" productImg="${item.MasterImageUrl}">${item.Name}</a></p>
${item.Discount > 0 ?
                    `<del>
<strong class="pricesymbol"></strong><span class="numbers">${item.Price}</span>
</del>`: ""}


&nbsp </span><strong class="pricesymbol"></strong><span class="numbers">${item.DiscountedAmount} </span>

</div>
</div>
</div>
</div></div>
</li>`;

            });
//<strong class="pricesymbol"></strong>  ${item.Price}
//</del >& nbsp < strong class="pricesymbol" > </strong > ${ item.DiscountedAmount }

            $('#ulLoadFeatureProduct').html(html);
            $("span .numbers").digits();
            var symbolvalue = GetCookieByName(pricesymbol);
            $('.pricesymbol').text(symbolvalue);// document.getElementsByClassName("pricesymbol").innerHTML = dd;
        },
        error: function (errorMessage) {
            alert(errorMessage.responseText);
        }
    });

}
function loadnewarrivalproducts() {
    $.ajax({
        url: "/Product/LoadNewArrivalProducts",
        type: "GET",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            var html = '';
            var data = JSON.parse(result.data);

            $.each(data, function (key, item) {
                html += `<li>
<div class="text-center">
<div class="col-xs-12 col-sm-6 col-md-3">
<div class="single-product">
<div class="product-img">
<div class="pro-type">
<span>${item.Discount}%</span>
</div>
<a href="/ProductDetails/Index?productId=${item.Id}" onClick="SetLocalStorage(this)" productId="${item.Id}" productName="${item.Name}" productImg="${item.MasterImageUrl}"><img src="${item.MasterImageUrl}" alt="Product Title" /></a>
<div class="actions-btn">
<a onclick="HandleAddtocart(this)" productIdList=${item.Id} ><i class="mdi mdi-cart"></i></a>
<a href="#" data-toggle="modal" onClick="LoadQuickViewWithRating(this)" id="${item.Id}" productId="${item.Id}" productName="${item.Name}" productImg="${item.MasterImageUrl}" data-target="#quick-view"><i class="mdi mdi-eye"></i></a>
<a onclick="HandleAddtoWishList(this)" productIdList=${item.Id}><i class="mdi mdi-heart"></i></a>
</div>
</div>
<div class="product-dsc">
<p><a href="/ProductDetails/Index?productId=${item.Id}" productId="${item.Id}" productName="${item.Name}" productImg="${item.MasterImageUrl}">${item.Name}</a></p>
${item.Discount > 0 ?
                    ` <del>
<strong class="pricesymbol"></strong><span class="numbers">${item.Price}</span>
</del>`: ""}


&nbsp </span><strong class="pricesymbol"></strong><span class="numbers">${item.DiscountedAmount} </span>

</div>
</div>
</div>
</div></div>
</li>`;

            });
//<strong class="pricesymbol"></strong>  ${item.Price}
//</del >& nbsp < strong class="pricesymbol" > </strong > ${ item.DiscountedAmount }

            $('#ulLoadNewArrivalProducts').html(html);
            $("span .numbers").digits();
            var symbolvalue = GetCookieByName(pricesymbol);
            $('.pricesymbol').text(symbolvalue);// document.getElementsByClassName("pricesymbol").innerHTML = dd;
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
        var id = JSON.parse(localStorage?.Id);
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
                                    <div class=" actioneye actions-btn">
                                       
<a href="javascript:void(0)" data-toggle="modal" onClick="LoadQuickViewWithRating(this)" productId="${id[index]}" productName="${name[index]}" productImg="${img[index]}" onClick="LoadQuickViewWithRating(this)" id="${id[index]}" data-target="#quick-view"><i class=" mdi mdi-eye"></i></a>

                                      

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
        var symbolvalue = GetCookieByName(pricesymbol);
        $('.pricesymbol').text(symbolvalue);
        //document.getElementsByClassName("pricesymbol").innerHTML = pricesymbol.symbol;

    }
}
//---------------Product management----------------
//---------------Toggle----------------------------
function toggle(className, obj) {

    if (obj.checked) $(className).show();
    else $(className).hide();
}
// Comment And Rating Work ------------------
//------------------------------------------------------------------------
//------------------------------------------------------------------------
// Wasiq Code Start
$("#btnSave").click(function (e) {
    e.preventDefault();
    rating = rating == 0 ? 5 : rating;
    //alert(rating);
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

    var validate = CommentValidate(commentAndRating.AnonymousName, commentAndRating.EmailId, commentAndRating.Comment);
    if (validate) {

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

    }

});

function SetLocalStorageCommentSection(nextPage, prevPage) {

    let localvalues = [];
    let ProductId = [];

    localvalues.push(nextPage);
    localvalues.push(prevPage);
    ProductId.push(GetProductIdFromURL());

    SetLocalStorageInSingleKey("CurrentProductId", ProductId);
    SetLocalStorageInSingleKey("CommentSection", localvalues);
}

// Get Product Comment And Rating Default Record 0 to 10
function GetProductCommentAndRating() {

    

    let urlstr = location.href;
    let url = new URL(urlstr);
    let searchparams = url.searchParams;



    var productId = searchparams.get('productId');



    if (productId != null && productId != undefined) {

        var html = '';
        var htlmRating = '';
        var htmlbtn = '';
        var countBtn = 1;
        var htmlPrevButton = '';
        var htmlNextButton = '';

        $.ajax({
            url: '/ProductDetails/GetProductCommentbyId',
            type: "Get",
            data: { id: productId },
            success: function (result) {



                var item = JSON.parse(result.data);
                totalComment = item.length;

                $(item).each(function (index, value) {

                    var nextPage = 10;
                    var prevPage = 0;

                    htmlbtn = '';
                    htlmRating = '';

                    var rating = item[index].CustomerRate;
                    var remainingRating = 5 - item[index].CustomerRate;

                    for (var i = 0; i < rating; i++) {

                        htlmRating += ` <i class="mdi mdi-star"></i>`;
                    }
                    for (var i = 0; i < remainingRating; i++) {

                        htlmRating += `<i class="mdi mdi-star-outline"></i>`;
                    }


                    totalComment = item[index].totalComment;
                    if (totalComment <= 10 || totalComment == undefined) {
                        countBtn = 1;
                    } else {
                        countBtn = totalComment / 10;
                        countBtn = Math.ceil(countBtn);
                    }

                    htmlPrevButton = `<span> <input type="button" Class="btn-PrevAndNext" value="Previous" geclass="btn-paggination" /></span>`;
                    htmlNextButton = `<span> <input type="button" Class="btn-PrevAndNext" value="Next" class="btn-paggination" /></span>`;
                    let prevCount = 0;
                    for (var i = 1; i <= countBtn; i++) {

                        prevPage = parseInt(prevCount) * 10;
                        if (i == 1)
                            htmlbtn += `<span> <input type="button"  value="${i}" class="btn-paggination applyActiveBtn" next-page="${nextPage}" prev-page="${prevPage}" /></span>`;
                        else
                            htmlbtn += `<span> <input type="button" value="${i}" class="btn-paggination" next-page="${nextPage}" prev-page="${prevPage}" /></span>`;

                        prevCount += 1;
                    }

                    html += `<hr/><div class="autohr-text">
                        <img src="#" alt="" />

                        <div class="author-des">
                        <h4><a href="javascript:void(0)">${item[index].CustomerName == null ? 'Anonymous' : item[index].CustomerName}</a></h4>
                        <span class="floatright ratting" id="assign-rating">
                        ${htlmRating}
                        </span>
                        <span>${moment(item[index].CommentDate).format('MMMM Do YYYY, h:mm A')}</span>
                        <p>${item[index].CustomerComment}</p>
                        </div>

                        </div>`;
                });

                var htmlPaggination = htmlPrevButton + htmlbtn + htmlNextButton;

                $('#commentAndRating').html(html);
                $('#div-paggination').html(htmlPaggination);

            },
            error: function (errorMessage) {
                alert(errorMessage.responseText);
            }
        });



    }



}

// Get Product Comment And Rating with Paggination
function GetProductCommentWithPaggination(nextPage, prevPage) {


    var htmlPrevButton = '';
    var htmlNextButton = '';
    let urlstr = location.href;
    let url = new URL(urlstr);
    let searchparams = url.searchParams;

    var productId = searchparams.get('productId');



    if (productId != null && productId != undefined) {

        var html = '';
        var htlmRating = '';

        $.ajax({
            url: '/ProductDetails/GetProductCommentWithPaggination',
            type: "POST",
            data: { id: productId, nextPage: nextPage, prevPage: prevPage },
            success: function (result) {


                var item = JSON.parse(result.data);

                $(item).each(function (index, value) {


                    htmlbtn = '';
                    htlmRating = '';
                    var nextPage = 10;
                    var prevPage = 0;


                    var rating = item[index].CustomerRate;
                    var remainingRating = 5 - item[index].CustomerRate;

                    for (var i = 0; i < rating; i++) {

                        htlmRating += ` <i class="mdi mdi-star"></i>`;
                    }
                    for (var i = 0; i < remainingRating; i++) {

                        htlmRating += `<i class="mdi mdi-star-outline"></i>`;
                    }


                    totalComment = item[index].totalComment;
                    if (totalComment <= 10 || totalComment == undefined) {

                        countBtn = 1;
                    } else {

                        countBtn = totalComment / 10;
                        countBtn = Math.ceil(countBtn);
                    }

                    htmlPrevButton = `<span> <input type="button" Class="btn-PrevAndNext" value="Previous" geclass="btn-paggination" /></span>`;
                    htmlNextButton = `<span> <input type="button" Class="btn-PrevAndNext" value="Next" class="btn-paggination" /></span>`;
                    let prevCount = 0;
                    for (i = 1; i <= countBtn; i++) {

                        prevPage = parseInt(prevCount) * 10;
                        htmlbtn += `<span> <input type="button" value="${i}" class="btn-paggination" next-page="${nextPage}" prev-page="${prevPage}" /></span>`;
                        //prevPage += 1;
                        prevCount += 1;

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
                var htmlPaggination = htmlPrevButton + htmlbtn + htmlNextButton;
                $('#commentAndRating').html(html);
                $('#div-paggination').html(htmlPaggination);
                $('#div-paggination').find('.btn-paggination').eq(((currentPrevPage / nextPage))).addClass('btn-Prod-Paggination-active');

            },
            error: function (errorMessage) {
                alert(errorMessage.responseText);
            }
        });



    }



}

// Set Local Storage (Array) Generalize Fn
function SetLocalStorageInArray(key, value) {
    
    alert();
    var keyValue = JSON.parse(localStorage.getItem(key)) || [];
    var ParamValue = JSON.parse(localStorage.getItem(value)) || [];

    if (keyValue != undefined) {

        for (var i = 0; i < keyValue.length; i++) {

            if (keyValue[i] == key && ParamValue[i] == value) {

                //Duplicate Error
            } else {

                keyValue[i].push(key);
                Paramvalue[i].push(value);
            }


        }
    } else {
        keyValue.push(key);
        Paramvalue.push(value);
    }
}

// Set Local Storage--- Store Multiple values on Single Key ( Generalize Fn)
function SetLocalStorageInSingleKey(key, value) {

    if (key != undefined) {
        var pushValue = [];
        for (var i = 0; i < value.length; i++) {

            pushValue.push(value[i]);

        }
        localStorage.setItem(key, pushValue);
        //localStorage.setItem(key, JSON.stringify(pushValue));
    }

}


//Get Product Id From URL
function GetProductIdFromURL() {

    let urlstr = location.href;
    let url = new URL(urlstr);
    let searchparams = url.searchParams;

    var productId = searchparams.get('productId');

    return productId;
}

function GetProductByIdWithRating() { //updated 30-Nov-2021
    var productId = GetProductIdFromURL();
    if (productId != null && productId != undefined) {
        $.ajax({
            url: "/Product/GetProductbyIdWithRating",
            type: "Get",
            data: {
                Id: productId
            },
            success: function (result) {

                var html = '';
                var index = 1;
                var fade = 1;
                var productPrice = 0;
                var productDiscount = 0;
                var Discount = 0;
                var htmlProductDetail = '';
                var htmlProductPriceDetail = '';
                var htmlProductSize = '';

                var item = JSON.parse(result.data);
                var rating = item[0].AvgRating;

                var htlmRating = GetProductRating(rating);

                $(item).each(function (i, v) {
                    productPrice = item[i].Price;
                    Discount = item[i].Discount;
                    productDiscount = item[i].DiscountedAmount;
                });

                //$(item).each(function (i, v) {
                //    if (i == 1) {
                //        htmlProductDetail += `<li class="active"><a data-toggle="tab" href="#sin-${index}"> <img src="${item[i].ImageUrl}" alt="quick view" /> </a></li>`
                //    } else {
                //        htmlProductDetail += `<li><a data-toggle="tab" href="#sin-${index}"> <img src="${item[i].ImageUrl}" alt="quick view" /> </a></li>`
                //    }
                //    index += 1;
                //});
                for (i = 0; i <= item.length; i++) {
                    if (i == 1) {
                        htmlProductDetail += `<li class="active"><a data-toggle="tab" href="#sin-${index}"> <img src="${item[0].ImageUrl2[0]}" alt="quick view" /> </a></li>`;
                    } else {
                        htmlProductDetail += `<li><a data-toggle="tab" href="#sin-${index}"> <img src="${item[0].ImageUrl2[i]}" alt="quick view" /> </a></li>`;
                    }
                    index += 1;
                }
                $(item).each(function (i, v) {
                    if (i == 0) {
                        htmlProductPriceDetail += `<div class="simpleLens-container tab-pane active fade in" id="sin-${fade}">
                        <div class="pro-type">
                        <span>new</span>
                        </div>
                        <a class="simpleLens-image" data-lens-image="${item[i].ImageUrl}" href="#"><img src="${item[i].ImageUrl}" alt="" class="simpleLens-big-image"></a>
                        </div>`
                    } else {
                        htmlProductPriceDetail += `<div class="simpleLens-container tab-pane fade in" id="sin-${fade}">
                        <div class="pro-type">
                        <span>new</span>
                        </div>
                        <a class="simpleLens-image" data-lens-image="${item[i].ImageUrl}" href="#"><img src="${item[i].ImageUrl}" alt="" class="simpleLens-big-image"></a>
                        </div>`
                    }
                    fade += 1;
                });

                $(item).each(function (i, v) {
                    htmlProductSize += `${item[i].UOM}`
                });



                html = `
                       <div class="col-xs-12">
                       <div class="d-table">
                       <div class="d-tablecell">
                       <div class="">
                       <div class="main-view">
                       <div class="modal-footer" data-dismiss="modal">
                       <span></span>
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
                       <ul class="product-slider">${htmlProductDetail} </ul>
                       </div>
                       </div>
                       </div>
                       <div class="col-xs-12 col-sm-7 col-md-8">
                       <div class="quick-right">
                       <div class="list-text">
                       <h3>${item[0].Name} ${htmlProductSize}</h3>
                       <span>${item[0].ShortDescription}</span>
                       <div class="ratting floatright">
                       <p>( ${item[0].TotalRating} Rating )</p>

                        ${htlmRating}
                       </div>
                       <h5>
${Discount > 0 ?
                    `<del>
<strong class="pricesymbol"></strong> ${productPrice}
</del>`: ""}

<labal style="color:#999"> ${Discount}% </labal> <strong class="pricesymbol"></strong> <b id="discoountedprice">${productDiscount} </h5>
                       <div class="all-choose">
                       </div>
                                   <div class="plus-minus">
                                   <a class="dec qtybuttonquickview qtybutton">-</a>
                                   <input type="test" value="1" name="qtybuttonquickview" id="quentityvalue" class="plus-minus-box">
                                   <a class="inc qtybuttonquickview qtybutton">+</a>
                                   </div>
                                   <strong style="font-size:18px">  <strong class="pricesymbol"> </strong> <labal id="labalprice"> ${productDiscount} </labal> </strong>

                       <div class="list-btn">
                       <a onclick="HandleAddtocart(this)" productIdList=${item[0].Id} >add to cart</a>
                       <a onclick="HandleAddtoWishList(this)" productIdList=${item[0].Id}>wishlist</a>
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
                       `;
                $('#quick-view').html(html);
                var symbolvalue = GetCookieByName(pricesymbol);
                $('.pricesymbol').text(symbolvalue);
            },
            error: function (errorMessage) {
                alert(errorMessage.responseText);
            }
        });
    }
}


// Validation
var rating = 0;
$('.product-rating').click(function () {

    rating = $(this).attr('rating');
});

function CommentValidate(name, email, message) {
    $('#lblName').text('');
    $('#lblEmail').text('');
    $('#lblMessage').text('');
    $('#lblrating').text('');

    const emailValidate = /^(([^<>()[\]\.,;:\s@\"]+(\.[^<>()[\]\.,;:\s@\"]+)*)|(\".+\"))@(([^<>()[\]\.,;:\s@\"]+\.)+[^<>()[\]\.,;:\s@\"]{2,})$/i;
    var isEmailValid = emailValidate.test(email);

    if (name.length == 0) {
        $('#lblName').text("Please insert a Name.");
        return false;

    } else if (isEmailValid == false) {
        $('#lblEmail').text("Please insert a valid Email.");
        return false;

    } else if (message.length == 0) {
        $('#lblMessage').text("Please insert Reviews");
        return false;




    } else if (rating == 0) {
        $('#lblrating').text("Please Mark Product Rating.");
        return false;



    } else {
        rating = 0;
        return true;
    }



}
function GetProductRating(rating) {



    var htlmRating = '';
    var remainingRating = 5 - rating;



    var isDecimalRatingAssign = false;
    var isDecimalRating = rating % 1;



    if (isDecimalRating != 0) {
        isDecimalRating = true;



        rating = Math.ceil(rating);
        remainingRating = Math.floor(remainingRating);
    }
    else {
        isDecimalRating = false;
    }



    for (var i = 1; i <= rating; i++) {
        if (isDecimalRating) { //true
            if (i == rating) { // 4==4
                //half star
                htlmRating += `<i class="mdi mdi-star-half"></i>`;
                isDecimalRatingAssign = true;
            }
            else {
                //full star
                htlmRating += `<i class="mdi mdi-star"></i>`;
            }
        } else {
            //full star
            htlmRating += `<i class="mdi mdi-star"></i>`;
        }
    }
    if (isDecimalRatingAssign) { //true
        for (var i = 0; i < remainingRating; i++) {
            //half star
            htlmRating += `<i class="mdi mdi-star-outline"></i>`;
        }
    }
    if (!isDecimalRating) { //false
        for (var i = 0; i < remainingRating; i++) {
            // Empty Star
            htlmRating += `<i class="mdi mdi-star-outline"></i>`;
        }
    }



    return htlmRating;
}

function GetfilterListDummy() {
    
    //Updated 9-Dec-
    var slides = document.getElementsByClassName("filter");
    let filterList_Dummy = [];

    for (var i = 0; i < slides.length; i++) {

        if ($(slides.item(i)).prop("checked")) {
            ifPresent = true;
            if ($(slides.item(i)).attr('myfilter') != undefined)
                filterList_Dummy.push({ "Name": $(slides.item(i)).attr('myfilter'), "ID": parseInt($(slides.item(i)).attr('categoryId')) });
        }
    }
    //Pack Size Work
    var packSize = [];
    var priceLimit = $('#amount').val()
    packSize = priceLimit.split("-");

    for (x = 0; x < packSize.length; x++) {
        if ($('#Chk-Amount').prop("checked")) {
            if (packSize[x] != undefined && packSize[x] != NaN)
                filterList_Dummy.push({ "Name": "PackSize", "ID": parseInt(packSize[x]) });
        }
    }

    let categoryAndBrand = [];
    localStorage.removeItem("categoryAndBrand");
    categoryAndBrand.push(filterList_Dummy);

    localStorage.setItem("categoryAndBrand", JSON.stringify(categoryAndBrand));

    return filterList_Dummy;
}
function SetLocalStorageForFilter(FilterCateAndBrand, filterSearchByName, filterNextPage, filterPrevpage) {



    localStorage.setItem("filterSearchByName", filterSearchByName);
    localStorage.setItem("filterPrevpage", filterPrevpage);
    localStorage.setItem("filterNextPage", filterNextPage);
    // localStorage.setItem("categoryAndBrand", FilterCateAndBrand);



}
function ScrollTop() {
    $('html, body').animate({ scrollTop: 0 }, 'slow');
}
// Wasiq Code End
//*******************************************************************
// static object status
var paymenttype = {
    Stripe: "Stripe",
    HBL: "HBL",
    JazzCash: "JazzCash",
    EasyPaisa: "EasyPaisa",
};
//*******************************************************************

//----------------------------for special charctar--------------------------
$(document).ready(function () {
    $(document).on('keypress', '.removespecialchar', function (event) {
        var regex = RegExp("^[a-zA-Z0-9]+$");
        var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
        if (!regex.test(key)) {
            return false;
        }
    })
});
//------------------get user cookie data ---------------------
function getCookie(cName) {
    const name = cName + "=";
    const cDecoded = decodeURIComponent(document.cookie); //to be careful
    const cArr = cDecoded.split('; ');
    let res;
    cArr.forEach(val => {
        if (val.indexOf(name) === 0) res = val.substring(name.length);
    })
    return res
}

