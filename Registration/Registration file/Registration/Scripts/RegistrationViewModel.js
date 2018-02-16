$(document).ready(function (e) {
  
    $('#UserName').focusout(function () {
        if ($('#UserName').val().length > 3) {
            //alert($('#User_Id').val());
            $.ajax({
                type: 'POST',
                url: "/User/isUserNamefound",
                data: { "userName": $('#UserName').val() },
                dataType: 'json',
                success: function (data) {
                    console.log(data);
                    if (!data) {
                        $('#sUser_Id').text("Available");
                    } else if (data) {
                        $('#sUser_Id').text("User ID is taken");
                    } else {
                        $('#sUser_Id').text("........................");
                    }
                },

                error: function (response) {
                    var r = jQuery.parseJSON(response.responseText);
                    alert("Message: " + r.Message);
                    alert("StackTrace: " + r.StackTrace);
                    alert("ExceptionType: " + r.ExceptionType);

                }


            });
        } else {
            $('#sUser_Id').text("User ID must be greater than 3");
        }
    });

    $('#UserName').focusin(function () {
        $('#sUser_Id').text(" ");
    });
    var regx = "/[^A-Za-z]/g";
    $('#FirstName').focusout(function () {
        
        $('#FirstName').val($('#FirstName').val().replace(eval(regx), ''));
       
    })
    $('#MiddleName').focusout(function () {
        
        $('#MiddleName').val($('#MiddleName').val().replace(eval(regx), ''));

    })
    $('#LastName').focusout(function () {
       
        $('#LastName').val($('#LastName').val().replace(eval(regx), ''));
      
    })

    

    /*
    $('#Subbmit').click(function () {
        $('#Subbmit').prop("disabled", true);
    });
    */
});