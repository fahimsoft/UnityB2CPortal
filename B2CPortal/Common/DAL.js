

function ajaxGet(url, callback) {
  
    $.ajax({
        url: url,
        type: "GET",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (response) {
            ;
            callback(response);
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}


function ajaxPost(url, body, callback) {
   
    $.ajax({
        url: url,
        type: "POST",
        contentType: "application/json;charset=utf-8",
        data: JSON.stringify(body),
        dataType: "json",
        success: function (response) {
            ;
            callback(response);
        },
        error: function (errormessage) {
            ;
            alert(errormessage.responseText);
        }
    });
}


function ajaxDelete(url, Id, callback) {
    
    $.ajax({
        url: url + Id,
        type: "POST",
        contentType: "application/json;charset=utf-8",
       // dataType: "json",
        success: function (response) {
            callback(response);
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}

function ajaxGetByID(url, Id,callback) {
  
    $.ajax({
        url: url+Id,
        type: "GET",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (response) {
            ;
            callback(response);
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}


