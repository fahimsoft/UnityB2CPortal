
//get cookie by name 
function GetCookieByName(name) {
    var sym = unescape((document.cookie.match(name + '=([^;].+?)(;|$)') || [])[1] || '');
    sym = sym == "PKR" ? "PKR" : "$";
    return sym;
}

//*******************************************************************
var paymenttype = {
    Stripe: "Stripe",
    HBL: "HBL",
    JazzCash: "JazzCash",
    EasyPaisa: "EasyPaisa",
};
//*******************************************************************